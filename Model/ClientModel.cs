using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Model
{
    public class ClientModel
    {

            public  string?  ClientId { get; set; }
            public  string ? FullName { get; set; }
            public  string? Contact { get; set; }

            public  string? TraineeId { get; set; }           
            public  string? TraineeName { get; set; }
            public string? PlanId { get; set; }

            public string?  PlanName { get; set; }
            public Decimal?  Price { get; set; }
            public bool?  hasAccessCard { get; set; }

            //AccessCard
            public string? CardNumber { get; set; }
            public DateTime? IssuedDate { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public int? DaysLeft { get; set; }
    }
}
