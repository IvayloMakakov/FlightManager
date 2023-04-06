using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightManager.Models
{
	public class Reservation
	{
		public int ReservationId { get; set; }

		public string Email { get; set; }
		public bool IsConfirmed { get; set; }
		public int FlightId { get; set; }
		public Flight Flight { get; set; }
		public List<PassangerReservation> Passengers { get; set; } = new List<PassangerReservation>();

	}
}
