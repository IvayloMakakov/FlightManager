using Microsoft.AspNetCore.Http;
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
        public PassangerReservationViewModel()
        {
            
        }
		public PassangerReservationViewModel(IFormCollection form):this()
		{
			FirstName = form.FirstOrDefault(x => x.Key == "FirstName").Value;
			LastName = form.FirstOrDefault(x => x.Key == "LastName").Value;
			MiddleName = form.FirstOrDefault(x => x.Key == "MiddleName").Value;
			Nationality = form.FirstOrDefault(x => x.Key == "Nationality").Value;
			EGN = form.FirstOrDefault(x => x.Key == "EGN").Value;
			PhoneNumber = form.FirstOrDefault(x => x.Key == "PhoneNumber").Value;
			TicketType = int.Parse(form.FirstOrDefault(x => x.Key == "TicketType").Value);
			CurrentCount = int.Parse(form.FirstOrDefault(x => x.Key == "CurrentCount").Value);
			PassengerCapacity = int.Parse(form.FirstOrDefault(x => x.Key == "PassengerCapacity").Value);
			ReservationId = int.Parse(form.FirstOrDefault(x => x.Key == "ReservationId").Value);
		}
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
