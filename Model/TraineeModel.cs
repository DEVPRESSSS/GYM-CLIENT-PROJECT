using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Model
{
    public class TraineeModel
    {

        public string? TraineerId { get; set; }
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public DateOnly? Created { get; set; }
    }
}
