using check_in_out.Common.Utils;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace check_in_out.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int EmployeeId { get; set; }
        public DateTime Workday { get; set; }
        public int MinutesWorked { get; set; }

        public static ConsolidatedEntity CreateConsolidatedEntityFromCheckInOutEntity(CheckInOutEntity checkInOutEntity, int totalMinutesWorked)
        {
            return new ConsolidatedEntity
            {
                ETag = "*",
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = checkInOutEntity.EmployeeId,
                Workday = DateUtils.GetDateTimeMidnightInUniversalTime(checkInOutEntity.DateCheck),
                MinutesWorked = totalMinutesWorked
            };
        }
    }
}
