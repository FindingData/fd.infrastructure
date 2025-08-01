using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Extensions
{
    public static  class DateTimeExtensions
    {
        /// <summary>
        /// 获取当前日期所在季度的起止日期，可传入偏移量获取上/下季度
        /// offset = 0 → 当前季度
        /// offset = -1 → 上季度
        /// offset = 1 → 下季度
        /// </summary>
        public static (DateTime Start, DateTime End) GetQuarterRange(this DateTime date, int offset = 0)
        {
            var now = DateTime.Now;
            int currentQuarter = (now.Month - 1) / 3 + 1;

            // 偏移后的季度编号
            int targetQuarter = currentQuarter + offset;
            int year = now.Year;

            // 处理跨年（负数或大于4）
            while (targetQuarter <= 0)
            {
                targetQuarter += 4;
                year--;
            }
            while (targetQuarter > 4)
            {
                targetQuarter -= 4;
                year++;
            }

            int startMonth = (targetQuarter - 1) * 3 + 1;
            DateTime start = new DateTime(year, startMonth, 1);
            DateTime end = start.AddMonths(3).AddDays(-1);

            return (start, end);
        }

    }
}
