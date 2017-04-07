﻿using System.Collections.Generic;
using TalentHub.AppCode.DataLayer;
using TalentHub.AppCode.Models;
using TalentHub.AppCode.Utilities;

namespace TalentHub.AppCode.BusinessLayer {
	internal class UserBL {

		private static enRegistrationResponse ValidateRegistrationData(string emailId, string username, string password, string confirmedPassword) {
			if (string.IsNullOrWhiteSpace(emailId) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmedPassword) || string.Compare(password, confirmedPassword, false) != 0) {
				return enRegistrationResponse.BadData;
			}
			else {
				bool emailIdOrUsernameExists = UserDL.CheckEmailIdOrUsernameExists(emailId, username);
				if (emailIdOrUsernameExists) {
					return enRegistrationResponse.EmailIdOrUsernameExists;
				}
			}
			return enRegistrationResponse.Ok;
		}

		internal static enRegistrationResponse RegisterUser(string emailId, string username, string password, string confirmedPassword) {
			enRegistrationResponse validRegistrationData = ValidateRegistrationData(emailId, username, password, confirmedPassword);
			if (validRegistrationData != enRegistrationResponse.Ok) {
				return validRegistrationData;
			}

			string hashedPassword = Utility.GetMd5Hash(password);
			KeyValuePair<string, string> appKeys = AppSecurityBL.GenerateUniqueAppKeys();

			int id = UserDL.AddUserDetails(emailId, username, hashedPassword, appKeys.Key, appKeys.Value);

			return enRegistrationResponse.Ok;
		}
	}
}