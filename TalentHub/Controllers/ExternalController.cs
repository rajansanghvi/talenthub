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
					return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Data validation errors. Please rectify the registration data and try again. Thank you!"));
				case enRegistrationResponse.EmailIdOrUsernameExists:
					return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "The username or email id sent in the request is already associated with a different user. Please use another username or email id to continue registration. Thank you!"));
				default:
					return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created));
			}
		}

		public IHttpActionResult Login(dynamic data) {
			string username = data.Username;
			string password = data.Password;
			bool rememberMe = data.RememberMe;
		}
	}
}