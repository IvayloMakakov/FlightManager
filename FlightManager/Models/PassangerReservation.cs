namespace FlightManager.Models
{
	public enum TicketsType
	{
		Normal,
		Business
	}
	public class PassangerReservation
	{
		public int PassangerId { get; set; }
        public Passanger Passanger { get; set; }
        public int ReservationId { get; set; }
        public Reservation  Reservation { get; set; }
        public TicketsType TicketsType { get; set; }
    }
}