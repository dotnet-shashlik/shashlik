using System;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class DateTimeTests
    {
        [Fact]
        public void Tests()
        {
            var time = new DateTime(2020, 8, 1, 12, 0, 0);
            time.ToStringyyyyMMddHHmm().ShouldBe(time.ToString("yyyy-MM-dd HH:mm"));
            time.ToStringyyyyMMddHHmmss().ShouldBe(time.ToString("yyyy-MM-dd HH:mm:ss"));
            time.GetWeekFirstDay().Date.ShouldBe(new DateTime(2020, 7, 27));
            time.GetWeekFirstDay(DayOfWeek.Sunday).Date.ShouldBe(new DateTime(2020, 7, 26));
            time.GetWeekLastDay().Date.ShouldBe(new DateTime(2020, 8, 2));
            time.GetWeekLastDay(DayOfWeek.Sunday).Date.ShouldBe(new DateTime(2020, 8, 1));
            time.GetNextWeekFirstDay().Date.ShouldBe(new DateTime(2020, 8, 3));
            time.GetNextWeekFirstDay(DayOfWeek.Sunday).Date.ShouldBe(new DateTime(2020, 8, 2));
            time.GetNextWeekLastDay().Date.ShouldBe(new DateTime(2020, 8, 9));
            time.GetNextWeekLastDay(DayOfWeek.Sunday).Date.ShouldBe(new DateTime(2020, 8, 8));
            time.GetMonthFirstDay().ShouldBe(new DateTime(2020, 8 ,1));
            time.GetMonthLastDay().ShouldBe(new DateTime(2020, 8 ,31));
            time.GetNextMonthFirstDay().ShouldBe(new DateTime(2020, 9 ,1));
            time.GetNextMonthLastDay().ShouldBe(new DateTime(2020, 9 ,30));
            time.GetNextSpecificDayOfWeek(DayOfWeek.Sunday).ShouldBe(new DateTime(2020, 8, 2));
            new DateTime(2020, 7, 30).GetNextSpecificDayOfWeek(DayOfWeek.Saturday).ShouldBe(new DateTime(2020, 8, 1));
            time.GetNextSpecificDayOfWeek(DayOfWeek.Wednesday).ShouldBe(new DateTime(2020, 8, 5));
            time.GetNextSpecificDayOfWeek(DayOfWeek.Saturday).ShouldBe(new DateTime(2020, 8, 8));
            time.GetCurrentOrNextSpecificDayOfWeek(DayOfWeek.Sunday).ShouldBe(new DateTime(2020, 8, 2));
            time.GetCurrentOrNextSpecificDayOfWeek(DayOfWeek.Wednesday).ShouldBe(new DateTime(2020, 8, 5));
            time.GetCurrentOrNextSpecificDayOfWeek(DayOfWeek.Saturday).ShouldBe(new DateTime(2020, 8, 1));
            time.GetNextSpecificDayOfMonth(12).ShouldBe(new DateTime(2020, 9, 12));
            Should.Throw<Exception>(() => time.GetNextSpecificDayOfMonth(32));
            Should.Throw<Exception>(() => time.GetNextSpecificDayOfMonth(31));
            Should.Throw<Exception>(() => new DateTime(2020, 1, 2).GetNextSpecificDayOfMonth(30));
            time.GetCurrentOrNextSpecificDayOfMonth(1).ShouldBe(new DateTime(2020, 8, 1));
            time.GetCurrentOrNextSpecificDayOfMonth(12).ShouldBe(new DateTime(2020, 9, 12));

            time.IsSameDay(new DateTime(2020, 8, 1, 13, 1, 2)).ShouldBeTrue();
            time.IsSameDay(new DateTime(2020, 8, 2, 13, 1, 2)).ShouldBeFalse();
            
            time.GetLongDate().LongToDateTime().ShouldBe(time);
            var longTime = new DateTime(2048, 8, 1, 12, 0, 0);
            longTime.GetLongDate().LongToDateTime().ShouldBe(longTime);
            
            new DateTime(2000, 1, 1).GetAge().ShouldBe(DateTime.Now.Year - 2000);
            // TODO: birthday Test
            var now = DateTime.Now;
            now.AddYears(1).GetAge().ShouldBe(0);
            if (now.Month != 12 && now.Day != 31)
            {
                new DateTime(2000, 12, 31).GetAge().ShouldBe(DateTime.Now.Year - 2000 - 1);
            }
            time.GetQuarter().ShouldBe(3);
            time.GetQuarterFirstDay().ShouldBe(new DateTime(2020, 7, 1));
            time.GetQuarterLastDay().ShouldBe(new DateTime(2020, 9, 30));
        }
    }
}