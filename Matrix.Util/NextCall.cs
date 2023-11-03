using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Matrix.Util
{
    /// <summary>
    /// 执行某接口集合，直到成功就停止
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NextCall<T>
    {
        private readonly T[] _items;
        private T[] _do_items;
        private int _index;

        public PolicyBuilder DefaultPolicyBuilder { get; set; }
        /// <summary>
        /// 执行接口集合
        /// </summary>
        /// <param name="instances">某接口实列</param>
        public NextCall(IEnumerable<T> instances)
        {
            _items = _do_items = instances.ToArray();
            this.DefaultPolicyBuilder = Polly.Policy.Handle<Exception>();
        }

        /// <summary>
        /// 过滤不需要的执行的对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> Filter(Func<T, bool> predicate)
        {
            _do_items = _items.Where(predicate).ToArray();
            return _do_items;
        }

        private Context InitForExcute()
        {
            Context ctx = new Context();
            ctx["index"] = 0;
            return ctx;
        }

        private int Excuting(Context ctx)
        {
            int.TryParse(ctx["index"].ToString(), out int currentIndex);
            ctx["index"] = currentIndex + 1;
            return currentIndex;
        }

        public void Excute(Action<T> action, Func<PolicyBuilder, PolicyBuilder> policyBuilderFunc)
        {
            Context ctx = InitForExcute();
            policyBuilderFunc(this.DefaultPolicyBuilder).Retry(_do_items.Length).ExecuteAndCapture((ctx) =>
            {
                int currentIndex = Excuting(ctx);
                action(_items[currentIndex]);
            }, ctx);
        }

        public void Excute(Action<T> action)
        {
            this.Excute(action, p => p);
        }

        public R Excute<R>(Func<T, R> func, Func<PolicyBuilder, PolicyBuilder<R>> policyBuilderFunc)
        {
            var ctx = InitForExcute();
            return policyBuilderFunc(this.DefaultPolicyBuilder).Retry(_do_items.Length).ExecuteAndCapture((ctx) =>
            {
                int currentIndex = Excuting(ctx);
                var result = func(_items[currentIndex]);
                return result;
            }, ctx).Result;
        }

        public R Excute<R>(Func<T, R> func)
        {
            return Excute<R>(func, x => x.OrResult<R>(r => false));
        }

        public async Task ExcuteAsync(Func<T, Task> action)
        {
            await this.ExcuteAsync(action, pb => pb);
        }

        public async Task ExcuteAsync(Func<T, Task> action, Func<PolicyBuilder, PolicyBuilder> policyBuilderFunc)
        {
            Context ctx = InitForExcute();
            var policy = policyBuilderFunc(this.DefaultPolicyBuilder).RetryAsync(_do_items.Length);
            await policy.ExecuteAndCaptureAsync(async (ctx) =>
            {
                int currentIndex = Excuting(ctx);
                await action(_items[currentIndex]);
            }, ctx);
        }

        public Task<R> ExcuteAsync<R>(Func<T, Task<R>> func)
        {
            return this.ExcuteAsync<R>(func, x => x.OrResult<R>(r => false));
        }

        public async Task<R> ExcuteAsync<R>(Func<T, Task<R>> func, Func<PolicyBuilder, PolicyBuilder<R>> policyBuilderFunc)
        {
            var ctx = InitForExcute();

            var policy = policyBuilderFunc(this.DefaultPolicyBuilder).RetryAsync<R>(_do_items.Length);
            var result = await policy.ExecuteAndCaptureAsync(async (ctx) =>
             {
                 int currentIndex = Excuting(ctx);
                 var funcResult = await func(_items[currentIndex]);
                 return funcResult;
             }, ctx);
            return result.Result;
        }
    }
}
