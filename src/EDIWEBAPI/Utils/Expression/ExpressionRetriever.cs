using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils.Expression
{
    public static class ExpressionRetriever
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        public static System.Linq.Expressions.Expression GetExpression<T>(ParameterExpression param, ExpressionFilter filter)
        {
            MemberExpression member = System.Linq.Expressions.Expression.Property(param, filter.PropertyName);
            ConstantExpression constant = System.Linq.Expressions.Expression.Constant(filter.Value);
            switch (filter.Comparison)
            {
                case Comparison.Equal:
                    return System.Linq.Expressions.Expression.Equal(member, constant);
                case Comparison.GreaterThan:
                    return System.Linq.Expressions.Expression.GreaterThan(member, constant);
                case Comparison.GreaterThanOrEqual:
                    return System.Linq.Expressions.Expression.GreaterThanOrEqual(member, constant);
                case Comparison.LessThan:
                    return System.Linq.Expressions.Expression.LessThan(member, constant);
                case Comparison.LessThanOrEqual:
                    return System.Linq.Expressions.Expression.LessThanOrEqual(member, constant);
                case Comparison.NotEqual:
                    return System.Linq.Expressions.Expression.NotEqual(member, constant);
                case Comparison.Contains:
                    return System.Linq.Expressions.Expression.Call(member, containsMethod, constant);
                case Comparison.StartsWith:
                    return System.Linq.Expressions.Expression.Call(member, startsWithMethod, constant);
                case Comparison.EndsWith:
                    return System.Linq.Expressions.Expression.Call(member, endsWithMethod, constant);
                default:
                    return null;
            }
        }

        public static Expression<Func<T, bool>> Contruct<T>(List<ExpressionFilter> filters)
        {
            if (filters.Count == 0)
                return null;

            var param = System.Linq.Expressions.Expression.Parameter(typeof(T), "t");
            System.Linq.Expressions.Expression exp = null;

            if (filters.Count == 1)
            {
                exp = ExpressionRetriever.GetExpression<T>(param, filters[0]);
            }
            else
            {
                exp = ExpressionRetriever.GetExpression<T>(param, filters[0]);
                for (int i = 1; i < filters.Count; i++)
                {
                    exp = System.Linq.Expressions.Expression.And(exp, ExpressionRetriever.GetExpression<T>(param, filters[i]));
                }
            }

            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(exp, param);
        }
    }
}
