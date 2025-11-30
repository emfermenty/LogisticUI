using LogisticWPF.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticWPF.DTO
{
    public class ShippingQuote
    {
        public ShippingType Type { get; set; }
        public double Cost { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> Requirements { get; set; }
        public double MaxWeight { get; set; }
        public double MaxVolume { get; set; }
    }
}
