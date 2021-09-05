using System;
using System.Collections.Generic;

namespace check_in_out.Common.Utils
{
    public class DateUtils
    {

        public static DateTime GetDateTimeMidnight(DateTime dateTime)
        {
            return dateTime.Subtract(dateTime.TimeOfDay);
        }

        public static DateTime GetDateTimeMidnightInUniversalTime(DateTime dateTime)
        {
            return GetDateTimeMidnight(dateTime).AddHours(-5).ToUniversalTime();
        }

        public static int GetTotalMinutesBetweenTwoDateTime(DateTime startDate, DateTime endDate)
        {
            return (int)(endDate - startDate).TotalMinutes;
        }

        public static int[] GetTotalMinutesForEachDayBetweenTwoDateTime(DateTime startDate, DateTime endDate)
        {
            List<int> totalMinutesForEachDay = new List<int>();
            DateTime startDateMidnight = GetDateTimeMidnight(startDate);
            DateTime endDateMidnight = GetDateTimeMidnight(endDate);

            switch (startDateMidnight.CompareTo(endDateMidnight))
            {
                case 0:
                    int totalMinutesWorked = GetTotalMinutesBetweenTwoDateTime(startDate, endDate);
                    totalMinutesForEachDay.Add(totalMinutesWorked);
                    break;
                case -1:
                    int totalMinutesWorkedStartDate = GetTotalMinutesBetweenTwoDateTime(startDate, endDateMidnight);
                    totalMinutesForEachDay.Add(totalMinutesWorkedStartDate);
                    if (endDateMidnight.CompareTo(endDate) == -1)
                    {
                        int totalMinutesWorkedEndDate = GetTotalMinutesBetweenTwoDateTime(endDateMidnight, endDate);
                        totalMinutesForEachDay.Add(totalMinutesWorkedEndDate);
                    }
                    break;
                default:
                    throw new Exception("The check in date is greater than check out date.");
            }
            return totalMinutesForEachDay.ToArray();
        }
    }
}
