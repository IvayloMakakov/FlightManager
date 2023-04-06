using System.ComponentModel.DataAnnotations;

namespace FlightManager.Models
{
	public class User
	{
		public int UserId { get; set; }
        public string Password { get; set; }
		public string Email { get; set; }
        public string FirstName { get; set; }
		public string LastName { get; set; }
        [StringLength(10)]
        public string EGN { get; set; }
        public string Address { get; set; }
		[StringLength(10)]
		public string PhoneNumber { get; set; }
    }
}
