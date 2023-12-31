﻿using System;

namespace Matrix.Util
{
    /*
 * Twitter_Snowflake
 * SnowFlake的结构如下(每部分用-分开):
 * 0 - 0000000000 0000000000 0000000000 0000000000 0 - 00000 - 00000 - 000000000000
 * 1位标识，由于long基本类型在Java中是带符号的，最高位是符号位，正数是0，负数是1，所以id一般是正数，最高位是0
 * 41位时间截(毫秒级)，注意，41位时间截不是存储当前时间的时间截，而是存储时间截的差值（当前时间截 - 开始时间截)
 * 得到的值），这里的的开始时间截，一般是我们的id生成器开始使用的时间，由我们程序来指定的（如下下面程序IdWorker类的startTime属性）。41位的时间截，可以使用69年，年T = (1L << 41) / (1000L * 60 * 60 * 24 * 365) = 69
 * 10位的数据机器位，可以部署在1024个节点，包括5位datacenterId和5位workerId
 * 12位序列，毫秒内的计数，12位的计数顺序号支持每个节点每毫秒(同一机器，同一时间截)产生4096个ID序号
 * 加起来刚好64位，为一个Long型。
 * SnowFlake的优点是，整体上按照时间自增排序，并且整个分布式系统内不会产生ID碰撞(由数据中心ID和机器ID作区分)，并且效率较高，经测试，SnowFlake每秒能够产生26万ID左右。
 */
    public class SnowflakeIdWorker
    {
        /// <summary>
        /// 开始时间截 (2015-01-01)
        /// </summary>
        private const long Twepoch = 1420041600000L;

        /// <summary>
        /// 机器id所占的位数
        /// </summary>
        private static int WorkerIdBits => 5;

        /// <summary>
        /// 数据标识id所占的位数
        /// </summary>
        private static int DatacenterIdBits => 5;

        /// <summary>
        /// 支持的最大机器id，结果是31 (这个移位算法可以很快的计算出几位二进制数所能表示的最大十进制数)
        /// </summary>
        private readonly long _maxWorkerId = -1L ^ (-1L << WorkerIdBits);

        /// <summary>
        /// 支持的最大数据标识id，结果是31
        /// </summary>
        private readonly long _maxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        /// <summary>
        /// 序列在id中占的位数
        /// </summary>
        private static int SequenceBits => 12;

        /// <summary>
        /// 机器ID向左移12位
        /// </summary>
        private readonly int _workerIdShift = SequenceBits;

        /// <summary>
        /// 数据标识id向左移17位(12+5)
        /// </summary>
        private readonly int _datacenterIdShift = SequenceBits + WorkerIdBits;

        /// <summary>
        /// 时间截向左移22位(5+5+12)
        /// </summary>
        private readonly int _timestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        /// <summary>
        /// 生成序列的掩码，这里为4095 (0b111111111111=0xfff=4095)
        /// </summary>
        private readonly long _sequenceMask = -1L ^ (-1L << SequenceBits);

        /// <summary>
        /// 毫秒内序列(0~4095)
        /// </summary>
        private long _sequence;

        /// <summary>
        /// 上次生成ID的时间截
        /// </summary>
        private long _lastTimestamp = -1L;
        /// <summary>
        /// 机器ID
        /// </summary>
        internal long WorkerId { get; }
        /// <summary>
        /// 数据中心ID
        /// </summary>
        private long DataCenterId { get; }

        private static readonly object LockObj = new object();

        /// <summary>
        /// 雪花ID
        /// </summary>
        /// <param name="workerId">机器ID</param>
        /// <param name="datacenterId">数据中心ID</param>
        public SnowflakeIdWorker(long workerId = 0, long datacenterId = 0)
        {
            if (workerId > _maxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {_maxWorkerId} or less than 0");
            }
            if (datacenterId > _maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"datacenter Id can't be greater than {_maxDatacenterId} or less than 0");
            }

            WorkerId = workerId;
            DataCenterId = datacenterId;
        }

        ///// <summary>
        ///// 获取一个新的ID
        ///// </summary>
        ///// <returns></returns>
        //public static string NewId()
        //{
        //    if (_sigle == null)
        //    {
        //        _sigle = new SnowFlake(4L);//此处4L应该从配置文件里读取当前机器配置
        //    }

        //    var id = _sigle.NextId();
        //    var time = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    return $"{time}{id}";
        //}

        /// <summary>
        /// 获得下一个ID (该方法是线程安全的)
        /// </summary>
        /// <returns></returns>
        public string Next(string delimiter = null)
        {
            lock (LockObj)
            {
                string id = Base36Converter.Encode(GenerateId());
                if (!string.IsNullOrEmpty(delimiter))
                {
                    id = id.Insert(4, delimiter);
                    id = id.Insert(9, delimiter);
                }
                return id;
            }
        }

        /// <summary>
        /// 获得下一个ID (该方法是线程安全的)
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (LockObj)
            {
                return GenerateId();
            }
        }

        /// <summary>
        /// 获得下一个ID
        /// </summary>
        /// <returns></returns>
        private long GenerateId()
        {
            var timestamp = TimeGen();

            //如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过这个时候应当抛出异常
            if (timestamp < _lastTimestamp)
            {
                throw new InvalidSystemClock($"Clock moved backwards.  Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
            }

            //如果是同一时间生成的，则进行毫秒内序列
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & _sequenceMask;
                //毫秒内序列溢出
                if (_sequence == 0)
                {
                    //阻塞到下一个毫秒,获得新的时间戳
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            //时间戳改变，毫秒内序列重置
            else
            {
                _sequence = 0L;
            }

            //上次生成ID的时间截
            _lastTimestamp = timestamp;

            //移位并通过或运算拼到一起组成64位的ID
            return ((timestamp - Twepoch) << _timestampLeftShift)
                   | (DataCenterId << _datacenterIdShift)
                   | (WorkerId << _workerIdShift)
                   | _sequence;
        }

        /// <summary>
        /// 返回以毫秒为单位的当前时间
        /// </summary>
        /// <returns></returns>
        private long TimeGen()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        /// <summary>
        /// 阻塞到下一个毫秒，直到获得新的时间戳
        /// </summary>
        /// <param name="lastTimes">上次生成ID的时间截</param>
        /// <returns>当前时间戳</returns>
        private long TilNextMillis(long lastTimes)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimes)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }
    }
}
