using System;
using System.Linq.Expressions;

// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault

namespace Shashlik.Utils.Extensions
{
    public static class ExpressionExtensions
    {
        private static string GetPropertyInner(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return expression.NodeType switch
            {
                ExpressionType.MemberAccess => ((MemberExpression) expression).Member.Name,
                ExpressionType.Convert => GetPropertyInner(((UnaryExpression) expression).Operand),
                _ => throw new NotSupportedException(expression.NodeType.ToString())
            };
        }

        /// <summary>
        /// 获取lambda选择的属性名称
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName(this Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression is LambdaExpression lambda)
            {
                return GetPropertyInner(lambda.Body);
            }

            throw new NotSupportedException(expression.NodeType.ToString());
        }
    }
}