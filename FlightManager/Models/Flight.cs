using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightManager.Models
{
	public class Flight
	{
        public int FlightId { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public DateTime PlaneTakingOff
        { get; set; }
		public DateTime PlaneLanding
		{ get; set; }
        public string PlaneType { get; set; }
        public string PlaneUniqueNumber { get; set; }

        public string PilotName { get; set; }
		public int PassengerCapacity { get; set; }
		public int BusinessCapacity { get; set; }
		public ICollection<Reservation> Reservations { get; set; }
	}
}
