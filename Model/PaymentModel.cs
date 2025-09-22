using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Model
{
    public class PaymentModel
    {

        public string? PaymentId { get; set; }
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? PaymentType { get; set;}
        public string? PlanId { get; set; }
        public string? PlanName { get; set; }
        public Decimal? AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
