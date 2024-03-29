﻿using check_in_out.Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace check_in_out.Functions.Entities
{
    public class CheckInOutEntity : TableEntity
    {
        public static CheckInOutEntity Instance { get; } = new CheckInOutEntity();
        public int EmployeeId { get; set; }
        public DateTime DateCheck { get; set; }
        public int Type { get; set; }
        public bool IsConsolidated { get; set; }

        public CheckInOutEntity CreateCheckInOutEntityFromCheckInOut(CheckInOut checkInOut)
        {
            return new CheckInOutEntity
            {
                ETag = "*",
                PartitionKey = "CHECKINOUT",
                RowKey = Guid.NewGuid().ToString(),
                IsConsolidated = false,
                EmployeeId = checkInOut.EmployeeId,
                DateCheck = checkInOut.DateCheck.ToUniversalTime(),
                Type = checkInOut.Type
            };
        }

        internal void UpdateCheckInOutEntityForConsolidated()
        {
            DateCheck = DateCheck.ToLocalTime();
            IsConsolidated = true;
        }

        internal void PrepareCheckInOutEntityToUpdate(CheckInOut checkInOut)
        {
            Type = checkInOut.Type;

            if (checkInOut.EmployeeId != 0)
            {
                EmployeeId = checkInOut.EmployeeId;
            }

            if (!DateTime.MinValue.Equals(checkInOut.DateCheck))
            {
                DateCheck = checkInOut.DateCheck.ToUniversalTime();
            }
        }
    }
}