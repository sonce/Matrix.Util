using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix.Util
{
    /// <summary>
    /// Id获取帮助类
    /// </summary>
    public static class IdHelper
    {
        internal static SnowflakeIdWorker IdWorker { get; set; }

        /// <summary>
        /// 当前WorkerId,范围:1~1023
        /// </summary>
        public static long WorkerId { get => IdWorker.WorkerId; }

        /// <summary>
        /// 获取String型雪花Id
        /// </summary>
        /// <returns></returns>
        static public string GetId()
        {
            return GetLongId().ToString();
        }

        /// <summary>
        /// 获取long型雪花Id
        /// </summary>
        /// <returns></returns>
        static public long GetLongId()
        {
            Init();
            return IdWorker.NextId();
        }

        /// <summary>
        /// 获取一个新的ID
        /// </summary>
        /// <returns></returns>
        public static string NewId()
        {
            var id = GetLongId();
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{time}{id}";
        }

        static public SnowflakeIdWorker Init(long workId = 0, long dataCenterId = 0)
        {
            if (IdWorker != null)
                return IdWorker;
            IdWorker = new SnowflakeIdWorker(workId, dataCenterId);
            return IdWorker;
        }
    }
}
