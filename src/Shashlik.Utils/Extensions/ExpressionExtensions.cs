using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Shashlik.Utils.Extensions
{
    public static class ExpressionExtensions
    {
        private static string GetPropertyInner(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member.Name;
                case ExpressionType.Convert:
                    return GetPropertyInner(((UnaryExpression)expression).Operand);
                default:
                    throw new NotSupportedException(expression.NodeType.ToString());
            }
        }

        /// <summary>
        /// 获取lambda选择的属性名称
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName(this Expression expression)
        {
            if (expression is LambdaExpression lambda)
            {
                return GetPropertyInner(lambda.Body);
            }
            throw new NotSupportedException(expression.NodeType.ToString());
        }
    }
}
