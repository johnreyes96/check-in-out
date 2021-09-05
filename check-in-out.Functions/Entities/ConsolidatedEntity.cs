using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace check_in_out.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public static ConsolidatedEntity Instance { get; } = new ConsolidatedEntity();
        public int EmployeeId { get; set; }
        public DateTime Workday { get; set; }
        public int MinutesWorked { get; set; }

        public ConsolidatedEntity CreateConsolidatedEntityFromCheckInOutEntity(CheckInOutEntity checkInOutEntity, int totalMinutesWorked)
        {
            return new ConsolidatedEntity
            {
                ETag = "*",
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = checkInOutEntity.EmployeeId,
                Workday = checkInOutEntity.DateCheck.Subtract(checkInOutEntity.DateCheck.TimeOfDay).AddHours(-5).ToUniversalTime(),
                MinutesWorked = totalMinutesWorked
            };
        }
    }
}
