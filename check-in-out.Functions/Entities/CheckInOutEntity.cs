using check_in_out.Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace check_in_out.Functions.Entities
{
    public class CheckInOutEntity : TableEntity
    {
        public static CheckInOutEntity Instance { get; } = new CheckInOutEntity();
        public string EmployeeId { get; set; }
        public DateTime DateCheck { get; set; }
        public short Type { get; set; }
        public bool IsConsolidated { get; set; }

        public CheckInOutEntity CreateCheckInOutEntityFromCheckInOut(CheckInOut checkInOut)
        {
            return new CheckInOutEntity {
                ETag = "*",
                PartitionKey = "CheckInOut",
                RowKey = Guid.NewGuid().ToString(),
                IsConsolidated = false,
                EmployeeId = checkInOut.EmployeeId,
                DateCheck = checkInOut.DateCheck.ToUniversalTime(),
                Type = checkInOut.Type
            };
        }
    }
}