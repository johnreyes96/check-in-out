using check_in_out.Common.Models;
using System;
using System.Collections.Generic;

namespace check_in_out.Common.Utils
{
    public class CheckInOutTypeFactory
    {
        public static CheckInOutTypeFactory Instance { get; } = new CheckInOutTypeFactory();
        public readonly List<CheckInOutType> CheckInOutTypeList = new List<CheckInOutType>();

        private CheckInOutTypeFactory()
        {
            if (CheckInOutTypeList != null || CheckInOutTypeList.Count == 0)
            {
                CheckInOutTypeList.Add(CheckIn);
                CheckInOutTypeList.Add(CheckOut);
            }
        }

        public CheckInOutType CheckIn = new CheckInOutType(0, "check in");
        public CheckInOutType CheckOut = new CheckInOutType(1, "check out");
    }
}
