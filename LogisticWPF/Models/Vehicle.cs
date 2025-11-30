using LogisticWPF.Models.Enums;
using System;
using System.Collections.Generic;

namespace LogisticWPF.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public double MaxWeight { get; set; }
        public double MaxVolume { get; set; }
        public double Speed { get; set; }
        public double FuelConsumption { get; set; }
        public VehicleType VehicleType { get; set; }
        public List<Shipping> Shippings { get; set; } = new List<Shipping>();
    }
}
