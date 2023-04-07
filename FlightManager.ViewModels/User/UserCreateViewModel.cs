﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlightManager.ViewModels.User
{
	public class UserCreateViewModel
	{
		public string Email { get; set; }

		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

	
		public string PersonalNo { get; set; }


		public string Address { get; set; }
	
		public string Phone { get; set; }

		public int Role { get; set; }
	}
}
