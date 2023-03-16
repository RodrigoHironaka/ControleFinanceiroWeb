using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Extensions.Enums
{
    public static class DateTimeExtensions
    {
        public static DateTime PrimeiroDiaMes(this DateTime val)
        {
            return new DateTime(val.Year, val.Month, 1);
        }
        public static DateTime UltimoDiaMes(this DateTime val)
        {
            return new DateTime(val.Year, val.Month, DateTime.DaysInMonth(val.Year, val.Month));
        }

        public static DateTime FinalDia(this DateTime val)
        {
            return new DateTime(val.Year, val.Month, val.Day, 23, 59, 59);
        }

    }
}
