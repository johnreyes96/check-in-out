using System;

namespace check_in_out.Common.Models
{
    public class Consolidated
    {
        public int EmployeeId { get; set; }
        public DateTime DateCheck { get; set; }
        public int MinutesWorked { get; set; }
    }
}
