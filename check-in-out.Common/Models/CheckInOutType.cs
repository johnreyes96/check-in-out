using System;
using System.Collections.Generic;

namespace check_in_out.Common.Models
{
    public class CheckInOutType
    {
        public static CheckInOutType Instance { get; } = new CheckInOutType();
        private readonly List<CheckInOutType> checkInOutTypeList = new List<CheckInOutType>();
        public int Code { get; }
        public string Description { get; }

        private CheckInOutType()
        {
            if (checkInOutTypeList != null || checkInOutTypeList.Count == 0)
            {
                checkInOutTypeList.Add(CheckIn);
                checkInOutTypeList.Add(CheckOut);
            }
        }

        private CheckInOutType(int code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public static CheckInOutType CheckIn = new CheckInOutType(0, "check in");
        public static CheckInOutType CheckOut = new CheckInOutType(1, "check out");

        public string GetDescription(int code)
        {
            foreach (CheckInOutType checkInOutType in checkInOutTypeList)
            {
                if (checkInOutType.Code == code)
                {
                    return checkInOutType.Description;
                }
            }
            throw new ArgumentException($"There is not type with code {code}");
        }
    }
}
