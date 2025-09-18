using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYM_CLIENT.Model
{
    public class EquipmentModel
    {

        public string? EquipmentId { get; set; }
        public string? Name { get; set; }
        public string? Quantity { get; set; }
        public string? ImageUrl { get; set; }

        public bool? IsAvailable { get; set; }

        public DateTime? Created { get; set; }

    }
}
