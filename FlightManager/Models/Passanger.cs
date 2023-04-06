using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightManager.Models
{
	public class Passanger
	{
		public int PassangerId { get; set; }

		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		[StringLength(10)]
		public string EGN { get; set; }
		[StringLength(10)]
		public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public ICollection<PassangerReservation> Reservations { get; set; }=new 
			List<PassangerReservation>();
    }
}
