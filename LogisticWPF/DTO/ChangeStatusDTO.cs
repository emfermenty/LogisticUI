using LogisticWPF.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticWPF.DTO
{
    public class ChangeStatusDTO
    {
        public string TrackingNumber { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
    }
}
