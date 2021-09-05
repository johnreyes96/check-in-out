using check_in_out.Common.Utils;
using System;

namespace check_in_out.Common.Models
{
    public class CheckInOutType
    {
        public static CheckInOutType Instance { get; } = new CheckInOutType();
        public int Code { get; }
        public string Description { get; }

        public CheckInOutType(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public CheckInOutType()
        {
        }

        public string GetDescription(int code)
        {
            foreach (CheckInOutType checkInOutType in CheckInOutTypeFactory.Instance.CheckInOutTypeList)
            {
                if (checkInOutType.Code == code)
                {
                    return checkInOutType.Description;
                }
            }
            throw new ArgumentException($"There is not type with code {code}.");
        }
    }
}
