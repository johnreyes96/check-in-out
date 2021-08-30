using System;

namespace check_in_out.Common.Models
{
    public class CheckInOut
    {
        public Int32 EmployeeId { get; set; }
        public DateTime DateCheck { get; set; }
        public Int32 Type { get; set; }
        public bool IsConsolidated { get; set; }

        public string ValidateRequiredFields()
        {
            string message = ValidateEmptyFields();
            message = ValidateType(message);
            return message;
        }

        public string ValidateEmptyFields()
        {
            string message = string.Empty;
            if (EmployeeId == 0)
            {
                message += "The request must have a EmployeeId.";
            }
            if (DateTime.MinValue.Equals(DateCheck))
            {
                message += " The request must have a DateCheck.";
            }
            return message;
        }

        public string ValidateType(string message)
        {
            if (Type < 0 || Type > 1)
            {
                message += " The field Type is a invalid value.";
            }
            return message;
        }

        public string GetTypeDescription()
        {
            return Type switch
            {
                0 => "check in",
                1 => "check out",
                _ => "The field Type is a invalid value.",
            };
        }
    }
}
