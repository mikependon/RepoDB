using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Extensions
{
    public static class DateTimeExtension
    {
        public static bool ShouldBe(this DateTime date1, DateTime date2, int tolerance)
        {
            var difference = Math.Abs((date1 - date2).TotalMilliseconds);
            return difference >  tolerance;
        }

        public static bool ShouldBeEx(this DateTime? date1, DateTime? date2)
        {
            //TODO: review this tolerance if needed
            var tolerance = 5;

            var difference = Math.Abs((date1.Value - date2.Value).TotalMilliseconds);
            return difference > tolerance;
        }
    }
}
