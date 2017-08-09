using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TalentHub.AppCode.Models {
	public class UserDetails {
		public int Id { get; set; }
		public string EmailId { get; set; }
		public string Username { get; set; }
		public string DeviceId { get; set; }
		public string AuthKey { get; set; }
		public bool Deleted { get; set; }

		public UserDetails() {
			Id = 0;
			EmailId = string.Empty;
			Username = string.Empty;
			DeviceId = string.Empty;
			AuthKey = string.Empty;
			Deleted = false;
		}
	}
}