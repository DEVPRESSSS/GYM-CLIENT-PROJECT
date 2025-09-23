using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Model
{
    public class PlanModel
    {

        public string? PlanId { get; set; }
        public string? PlanName { get; set; }
        public decimal? Price { get; set; }

        public int Duration { get; set; }
        public DateOnly? SOD { get; set; }
        public DateOnly? SOE { get; set; }
        public DateOnly? Created { get; set; }
    }
}
