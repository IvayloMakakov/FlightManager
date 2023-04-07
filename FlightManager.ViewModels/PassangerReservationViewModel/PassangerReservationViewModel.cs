using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManager.ViewModels.PassangerReservationViewModel
{
	public class PassangerReservationViewModel
	{
		public int ReservationId { get; set; }
		public int PassengerCapacity { get; set; }
		public int CurrentCount { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		[StringLength(10)]
		public string EGN { get; set; }
		[StringLength(10)]
		public string PhoneNumber { get; set; }
		public string Nationality { get; set; }
		public int TicketType { get; set; }
	}
}
