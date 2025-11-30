using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticWPF.DTO
{
    public class ShippingRequest
    {
        public double Distance { get; set; }
        public double Weight { get; set; }
        public double Volume { get; set; }
    }
}
