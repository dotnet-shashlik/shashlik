//using Guc.Utils.Extensions;
//using NPOI.SS.Formula.Eval;
//using Shouldly;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using Xunit;

//namespace Guc.Utils.Test
//{
//    public class EvalTests
//    {

//        [Fact]
//        public void Test1()
//        {
//            string ex = @"
//if(a > 50000) return ""单笔提现不得超过5万元"";
//else if(a + b <= 101000 ) return a;
//else if(a + b > 250000) return ""每月最大提现金额为25万元"";
//else if(b <= 101000 && a + b > 101000) return a * 1.011M * 1.1M * 0.01M;
//else if(b >= 101000 && a + b > 101000) return b * 1.011M * 1.1M * 0.01M;
//else return ""增值税无法计算"";
//";

//            // a本次提现,b:本月累计提现(不算本次),c:本季度累计提现(不算本次),d:本年度累计提现(不算本次)

//            // 累计9万提6万
//            {
//                var evaluator = new Evaluator(ex) { ["a"] = 60000, ["b"] = 90000, ["c"] = 0, ["d"] = 0 };

//                var r3 = evaluator.Eval();
//                r3.ShouldBe("单笔提现不得超过5万元");
//            }

//            // 累计9万提100
//            {
//                var evaluator = new Evaluator(ex) { ["a"] = 100, ["b"] = 90000, ["c"] = 0, ["d"] = 0 };
//                var r3 = evaluator.Eval();
//                r3.ShouldBe(100);
//            }

//            // 累计30万提2万
//            {
//                var evaluator = new Evaluator(ex) { ["a"] = 20000, ["b"] = 300000, ["c"] = 0, ["d"] = 0 };
//                var r3 = evaluator.Eval();
//                r3.ShouldBe("每月最大提现金额为25万元");
//            }

//            // 累计9万提2万
//            {
//                var evaluator = new Evaluator(ex) { ["a"] = 20000, ["b"] = 90000, ["c"] = 0, ["d"] = 0 };
//                var r3 = evaluator.Eval();
//                r3.ShouldBe(20000M * 1.011M * 1.1M * 0.01M);
//            }

//            // 累计12万提2万
//            {
//                var evaluator = new Evaluator(ex) { ["a"] = 20000, ["b"] = 120000, ["c"] = 0, ["d"] = 0 };
//                var r3 = evaluator.Eval();
//                r3.ShouldBe(120000M * 1.011M * 1.1M * 0.01M);
//            }
//        }

//        [Fact]
//        public void Test2()
//        {

//            string ex = @"
//if(a > 90000) return ""超过最大提现金额!"";
//else return a * 1.5M * 0.01M;
//";

//            // a本次提现,b:本月累计提现,c:本季度累计提现,d:本年度累计提现
//            //var r3 = Eval.Execute<object>(ex, new { a = 100, b = 0, c = 0, d = 0 }); ;


//        }

//        [Fact]
//        public void Test3()
//        {

//            string ex = @"
//return ""超过最大提现金额!"";
//";

//            // a本次提现,b:本月累计提现,c:本季度累计提现,d:本年度累计提现
//            var evaluator = new Evaluator(ex);
//            var r3 = evaluator.Eval();


//        }


//        [Fact]
//        public object T1()
//        {
//            decimal a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0;

//            if (f + c >= 250000)
//            {
//                return "单月累计最高提现金额为25万元";
//            }

//            if (f > 101000)
//            {
//                return c / 1.01M * 0.015M;
//            }

//            if (f + c > 101000)
//            {
//                return (f + c) / 1.01M * 0.015M;
//            }

//            return 0;
//        }
//    }
//}
