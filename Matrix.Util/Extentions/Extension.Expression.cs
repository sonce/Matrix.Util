using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Matrix.Util.Extentions
{
    /// <summary>
    /// LambdaExpressionExtensions
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Expression? Parser(ParameterExpression parameter, Expression expression)
        {
            if (expression == null) return null;
            switch (expression.NodeType)
            {
                //一元运算符
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    {
                        var unary = expression as UnaryExpression;
                        var exp = Parser(parameter, unary.Operand);
                        return Expression.MakeUnary(expression.NodeType, exp, unary.Type, unary.Method);
                    }
                //二元运算符
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    {
                        var binary = expression as BinaryExpression;
                        var left = Parser(parameter, binary.Left);
                        var right = Parser(parameter, binary.Right);
                        var conversion = Parser(parameter, binary.Conversion);
                        if (binary.NodeType == ExpressionType.Coalesce && binary.Conversion != null)
                        {
                            return Expression.Coalesce(left, right, conversion as LambdaExpression);
                        }
                        return Expression.MakeBinary(expression.NodeType, left, right, binary.IsLiftedToNull, binary.Method);
                    }
                //其他
                case ExpressionType.Call:
                    {
                        var call = expression as MethodCallExpression;
                        List<Expression> arguments = new List<Expression>();
                        foreach (var argument in call.Arguments)
                        {
                            arguments.Add(Parser(parameter, argument));
                        }
                        var instance = Parser(parameter, call.Object);
                        call = Expression.Call(instance, call.Method, arguments);
                        return call;
                    }
                case ExpressionType.Lambda:
                    {
                        var Lambda = expression as LambdaExpression;
                        return Parser(parameter, Lambda.Body);
                    }
                case ExpressionType.MemberAccess:
                    {
                        var memberAccess = expression as MemberExpression;
                        if (memberAccess.Expression == null)
                        {
                            memberAccess = Expression.MakeMemberAccess(null, memberAccess.Member);
                        }
                        else
                        {
                            var exp = Parser(parameter, memberAccess.Expression);
                            var member = exp.Type.GetMember(memberAccess.Member.Name).FirstOrDefault();
                            memberAccess = Expression.MakeMemberAccess(exp, member);
                        }
                        return memberAccess;
                    }
                case ExpressionType.Parameter:
                    return parameter;
                case ExpressionType.Constant:
                    return expression;
                case ExpressionType.TypeIs:
                    {
                        var typeis = expression as TypeBinaryExpression;
                        var exp = Parser(parameter, typeis.Expression);
                        return Expression.TypeIs(exp, typeis.TypeOperand);
                    }
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
            }
        }
        /// <summary>
        /// Expression扩展方法
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <typeparam name="TTarget">目标数据类型</typeparam>
        /// <typeparam name="Result">返回结果数据类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static Expression<Func<TTarget, Result>>? Cast<TSource, TTarget, Result>(this Expression<Func<TSource, Result>> expression)
        {
            if(expression == null)
            {
                return null;
            }
            var param = Expression.Parameter(typeof(TTarget), "param");
            var exp = Parser(param, expression);
            return Expression.Lambda<Func<TTarget, Result>>(exp, param);
        }

        /// <summary>
        /// 创建一个值恒为 <c>true</c> 的表达式。
        /// </summary>
        /// <typeparam name="T">表达式方法类型</typeparam>
        /// <returns>一个值恒为 <c>true</c> 的表达式。</returns>
        public static Expression<Func<T, bool>> True<T>() { return p => true; }

        /// <summary>
        /// 创建一个值恒为 <c>false</c> 的表达式。
        /// </summary>
        /// <typeparam name="T">表达式方法类型</typeparam>
        /// <returns>一个值恒为 <c>false</c> 的表达式。</returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// 使用 Expression.OrElse 的方式拼接两个 System.Linq.Expression。
        /// </summary>
        /// <typeparam name="T">表达式方法类型</typeparam>
        /// <param name="left">左边的 System.Linq.Expression 。</param>
        /// <param name="right">右边的 System.Linq.Expression。</param>
        /// <returns>拼接完成的 System.Linq.Expression。</returns>
        public static Expression<T> Or<T>(this Expression<T> left, Expression<T> right)
        {
            return MakeBinary(left, right, Expression.OrElse);
        }

        /// <summary>
        /// 使用 Expression.AndAlso 的方式拼接两个 System.Linq.Expression。
        /// </summary>
        /// <typeparam name="T">表达式方法类型</typeparam>
        /// <param name="left">左边的 System.Linq.Expression 。</param>
        /// <param name="right">右边的 System.Linq.Expression。</param>
        /// <returns>拼接完成的 System.Linq.Expression。</returns>
        public static Expression<T> And<T>(this Expression<T> left, Expression<T> right)
        {
            return MakeBinary(left, right, Expression.AndAlso);
        }

        /// <summary>
        /// 使用 Expression.AndAlso 的方式拼接两个 System.Linq.Expression。
        /// </summary>
        /// <typeparam name="T">表达式方法类型</typeparam>
        /// <param name="left">左边的 System.Linq.Expression 。</param>
        /// <param name="condtion">条件 。</param>
        /// <param name="right">右边的 System.Linq.Expression。</param>
        /// <returns>拼接完成的 System.Linq.Expression。</returns>
        public static Expression<T> AndIf<T>(this Expression<T> left, bool condtion, Expression<T> right)
        {
            if (condtion)
                return MakeBinary(left, right, Expression.AndAlso);
            return left;
        }

        /// <summary>
        /// 使用自定义的方式拼接两个 System.Linq.Expression。
        /// </summary>
        /// <typeparam name="T">表达式方法类型</typeparam>
        /// <param name="left">左边的 System.Linq.Expression 。</param>
        /// <param name="right">右边的 System.Linq.Expression。</param>
        /// <param name="func"> </param>
        /// <returns>拼接完成的 System.Linq.Expression。</returns>
        private static Expression<T> MakeBinary<T>(this Expression<T> left, Expression<T> right, Func<Expression, Expression, Expression> func)
        {
            return MakeBinary((LambdaExpression)left, right, func) as Expression<T>;
        }

        /// <summary>
        /// 拼接两个 <paramref>
        ///        <name>System.Linq.Expression</name>
        ///      </paramref>  ，两个 <paramref>
        ///                         <name>System.Linq.Expression</name>
        ///                       </paramref>  的参数必须完全相同。
        /// </summary>
        /// <param name="left">左边的 <paramref>
        ///                          <name>System.Linq.Expression</name>
        ///                        </paramref> </param>
        /// <param name="right">右边的 <paramref>
        ///                           <name>System.Linq.Expression</name>
        ///                         </paramref> </param>
        /// <param name="func">表达式拼接的具体逻辑</param>
        /// <returns>拼接完成的 <paramref>
        ///                  <name>System.Linq.Expression</name>
        ///                </paramref> </returns>
        private static LambdaExpression MakeBinary(this LambdaExpression left, LambdaExpression right, Func<Expression, Expression, Expression> func)
        {
            var data = Combinate(right.Parameters, left.Parameters).ToArray();

            right = ParameterReplace.Replace(right, data) as LambdaExpression;

            return Expression.Lambda(func(left.Body, right.Body), left.Parameters.ToArray());
        }

        /// <summary>
        /// 合并参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<T, T>> Combinate<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            var a = left.GetEnumerator();
            var b = right.GetEnumerator();
            while (a.MoveNext() && b.MoveNext())
                yield return new KeyValuePair<T, T>(a.Current, b.Current);
        }
    }


    #region class: ParameterReplace
    internal sealed class ParameterReplace : ExpressionVisitor
    {
        public static Expression Replace(Expression e, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            var item = new ParameterReplace(paramList);
            return item.Visit(e);
        }

        private readonly Dictionary<ParameterExpression, ParameterExpression> _parameters;

        public ParameterReplace(IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            _parameters = paramList.ToDictionary(p => p.Key, p => p.Value, new ParameterEquality());
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression result;
            if (_parameters.TryGetValue(p, out result))
                return result;
            return base.VisitParameter(p);
        }

        #region class: ParameterEquality
        private class ParameterEquality : IEqualityComparer<ParameterExpression>
        {
            public bool Equals(ParameterExpression x, ParameterExpression y)
            {
                if (x == null || y == null)
                    return false;

                return x.Type == y.Type;
            }

            public int GetHashCode(ParameterExpression obj)
            {
                if (obj == null)
                    return 0;

                return obj.Type.GetHashCode();
            }
        }
        #endregion
    }
    #endregion
}
