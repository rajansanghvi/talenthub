using System.Net;
using System.Net.Http;
using System.Web.Http;
using TalentHub.AppCode.BusinessLayer;
using TalentHub.AppCode.Models;

namespace TalentHub.Controllers {
	public class ExternalController: ApiController {

		[HttpPost]
		public IHttpActionResult RegisterUser(dynamic data) {
			string emailId = data.EmailId;
			string username = data.Username;
			string password = data.Password;
			string confirmedPassword = data.ConfirmedPassword;

			enRegistrationResponse response = UserBL.RegisterUser(emailId, username, password, confirmedPassword);

			HttpResponseMessage responseMsg = new HttpResponseMessage();

			switch (response) {
				case enRegistrationResponse.BadData:
					responseMsg.StatusCode = HttpStatusCode.BadRequest;
					break;

				case enRegistrationResponse.EmailIdOrUsernameExists:
					responseMsg.StatusCode = HttpStatusCode.Conflict;
					break;

				default:
					responseMsg.StatusCode = HttpStatusCode.Created;
					break;
			}

			return ResponseMessage(responseMsg);
		}
	}
}