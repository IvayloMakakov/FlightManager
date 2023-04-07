
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManager.ViewModels.Reservation
{
	public class ReservationViewModel
	{
		public int ReservationId { get; set; }

		public string Email { get; set; }
		public bool IsConfirmed { get; set; }
		public int FlightId { get; set; }
		public Flight Flight { get; set; }
		public List<PassangerReservation> Passengers { get; set; } = new List<PassangerReservation>();
		public int PassengerCount { get; set; }
	}
}
