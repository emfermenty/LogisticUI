using LogisticWPF.Models.Enums;
using System;
using LogisticWPF.Models;

namespace LogisticWPF
{
    public class Shipping
    {
        public Guid Id { get; set; }
        public ShippingStatus Status { get; set; }
        public DateTime? StartShipping { get; set; }
        public string TrackingNumber { get; set; } = null;
        public double Distance { get; set; }
        public double Weight { get; set; }
        public double Volume { get; set; }
        public ShippingType ShippingType { get; set; }
        public double Cost { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public string TypeDescription { get; set; } = null;
    }
}
