using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManager.ViewModels
{
	public class Flight
	{
		public int Id { get; set; }
		[Required]
		public string From { get; set; }
		[Required]
		public string To { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime DepartureDateTime { get; set; }
		[Required]
		[DataType(DataType.DateTime)]
		public DateTime ArrivalDateTime { get; set; }
		[Required]
		public string AircraftType { get; set; }
		[Required]
		public string FlightNumber { get; set; }
		[Required]
		public string PilotName { get; set; }
		[Required]
		public int PassengerCapacity { get; set; }
		[Required]
		public int BusinessCapacity { get; set; }
		public List<Reservation> Reservations { get; set; }
	}
}
