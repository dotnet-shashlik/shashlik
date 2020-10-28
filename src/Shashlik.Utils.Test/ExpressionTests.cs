using System;
using System.Linq.Expressions;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class ExpressionTests
    {
        private class TestClass
        {
            public string Str { get; set; }
            public int Int { get; set; }
        }
        
        [Fact]
        public void Tests()
        {
            Expression<Func<TestClass, string>> e1 = c => c.Str;
            e1.GetPropertyName().ShouldBe(nameof(TestClass.Str));
            Expression<Func<TestClass, long>> e2 = c => (long)c.Int;
            e2.GetPropertyName().ShouldBe(nameof(TestClass.Int));
            
            Expression<Func<TestClass, string>> e3 = c => c.Str + c.Int;
            Should.Throw<Exception>(() => e3.GetPropertyName());
            Should.Throw<Exception>(() => Expression.Add(Expression.Constant(1), Expression.Constant(3)).GetPropertyName());
        }
    }
}