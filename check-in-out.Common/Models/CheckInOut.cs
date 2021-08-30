using System;

namespace check_in_out.Common.Models
{
    public class CheckInOut
    {
        public int EmployeeId { get; set; }
        public DateTime DateCheck { get; set; }
        public int Type { get; set; }
        public bool IsConsolidated { get; set; }

        public string ValidateRequiredFields()
        {
            string message = string.Empty;
            message = ValidateEmptyFields(message);
            message = ValidateType(message);
            return message;
        }

        private string ValidateEmptyFields(string message)
        {
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
    }
}
