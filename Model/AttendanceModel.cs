using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Model
{
    public class AttendanceModel
    {

        public string? ClientId { get; set; }
        public string? FullName { get; set; }
        public DateTime? CheckInTime { get; set; }
    }
}
