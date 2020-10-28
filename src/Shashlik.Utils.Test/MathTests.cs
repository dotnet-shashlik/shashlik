using System;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class MathTests
    {
        [Fact]
        public void Tests()
        {
            var decimal1 = 2.123M;
            var decimal2 = 2.623M;
            var float1 = 3.654F;
            var float2 = 3.254F;
            var double1 = 4.869;
            var double2 = 4.369;
            decimal1.RoundCeil(2).ShouldBe(2.13M);
            decimal1.RoundCeil(1).ShouldBe(2.2M);
            decimal2.RoundCeil(2).ShouldBe(2.63M);
            decimal2.RoundCeil(1).ShouldBe(2.7M);
            
            decimal1.RoundFloor(2).ShouldBe(2.12M);
            decimal1.RoundFloor(1).ShouldBe(2.1M);
            decimal2.RoundFloor(2).ShouldBe(2.62M);
            decimal2.RoundFloor(1).ShouldBe(2.6M);
            
            float1.RoundCeil(2).ShouldBe(3.66F);
            float1.RoundCeil(1).ShouldBe(3.7F);
            float2.RoundCeil(2).ShouldBe(3.26F);
            float2.RoundCeil(1).ShouldBe(3.3F);
            
            float1.RoundFloor(2).ShouldBe(3.65F);
            float1.RoundFloor(1).ShouldBe(3.6F);
            float2.RoundFloor(2).ShouldBe(3.25F);
            float2.RoundFloor(1).ShouldBe(3.2F);
            
            double1.RoundCeil(2).ShouldBe(4.87);
            double1.RoundCeil(1).ShouldBe(4.9);
            double2.RoundCeil(2).ShouldBe(4.37);
            double2.RoundCeil(1).ShouldBe(4.4);
            
            double1.RoundFloor(2).ShouldBe(4.86);
            double1.RoundFloor(1).ShouldBe(4.8);
            double2.RoundFloor(2).ShouldBe(4.36);
            double2.RoundFloor(1).ShouldBe(4.3);
            
            // sample from https://docs.microsoft.com/en-us/dotnet/api/system.midpointrounding?view=netcore-3.1
            
            2.5M.MathRound(0, MidpointRounding.AwayFromZero).ShouldBe(3);
            2.5M.MathRound(0, MidpointRounding.ToEven).ShouldBe(2);
            2.5F.MathRound(0, MidpointRounding.AwayFromZero).ShouldBe(3);
            2.5F.MathRound(0, MidpointRounding.ToEven).ShouldBe(2);
            2.5.MathRound(0, MidpointRounding.AwayFromZero).ShouldBe(3);
            2.5.MathRound(0, MidpointRounding.ToEven).ShouldBe(2);
            3.5M.MathRound(0, MidpointRounding.AwayFromZero).ShouldBe(4);
            3.5M.MathRound(0, MidpointRounding.ToEven).ShouldBe(4);
            3.5F.MathRound(0, MidpointRounding.AwayFromZero).ShouldBe(4);
            3.5F.MathRound(0, MidpointRounding.ToEven).ShouldBe(4);
            3.5.MathRound(0, MidpointRounding.AwayFromZero).ShouldBe(4);
            3.5.MathRound(0, MidpointRounding.ToEven).ShouldBe(4);
            
            2.8M.MathRound(0, MidpointRounding.ToNegativeInfinity).ShouldBe(2);
            2.8M.MathRound(0, MidpointRounding.ToPositiveInfinity).ShouldBe(3);
            2.8M.MathRound(0, MidpointRounding.ToZero).ShouldBe(2);
            2.8F.MathRound(0, MidpointRounding.ToNegativeInfinity).ShouldBe(2);
            2.8F.MathRound(0, MidpointRounding.ToPositiveInfinity).ShouldBe(3);
            2.8F.MathRound(0, MidpointRounding.ToZero).ShouldBe(2);
            2.8.MathRound(0, MidpointRounding.ToNegativeInfinity).ShouldBe(2);
            2.8.MathRound(0, MidpointRounding.ToPositiveInfinity).ShouldBe(3);
            2.8.MathRound(0, MidpointRounding.ToZero).ShouldBe(2);
            
            (-2.8M).MathRound(0, MidpointRounding.ToNegativeInfinity).ShouldBe(-3);
            (-2.8M).MathRound(0, MidpointRounding.ToPositiveInfinity).ShouldBe(-2);
            (-2.8M).MathRound(0, MidpointRounding.ToZero).ShouldBe(-2);
            (-2.8M).MathRound(0, MidpointRounding.ToNegativeInfinity).ShouldBe(-3);
            (-2.8M).MathRound(0, MidpointRounding.ToPositiveInfinity).ShouldBe(-2);
            (-2.8M).MathRound(0, MidpointRounding.ToZero).ShouldBe(-2);
            (-2.8M).MathRound(0, MidpointRounding.ToNegativeInfinity).ShouldBe(-3);
            (-2.8M).MathRound(0, MidpointRounding.ToPositiveInfinity).ShouldBe(-2);
            (-2.8M).MathRound(0, MidpointRounding.ToZero).ShouldBe(-2);
        }
    }
}