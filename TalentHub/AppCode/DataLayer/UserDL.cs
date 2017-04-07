using TalentHub.AppCode.Utilities;

namespace TalentHub.AppCode.DataLayer {
	internal class UserDL {

		internal static int AddUserDetails(string emailId, string username, string hashedPassword, string deviceId, string authKey) {
			const string sql = @"insert into app_users
													(email_id, username, hashed_password, device_id, auth_key, deleted, created_by, created_date, modified_by, modified_date)
													values
													(?emailId, ?username, ?hashedPassword, ?deviceId, ?authKey, 0, ?username, now(), null, null);
													select last_insert_id();";

			GlobalDL dl = new GlobalDL();
			dl.AddParameter("emailId", emailId);
			dl.AddParameter("username", username);
			dl.AddParameter("hashedPassword", hashedPassword);
			dl.AddParameter("deviceId", deviceId);
			dl.AddParameter("authKey", authKey);

			return dl.ExecuteSqlReturnScalar<int>(Utility.ConnectionString, sql);
		}

		internal static bool CheckEmailIdOrUsernameExists(string emailId, string username) {
			const string sql = @"select    1
														from      app_users
														where     lower(email_id) = LOWER(?emailId)
														or        lower(username) = LOWER(?username);";

			GlobalDL dl = new GlobalDL();
			dl.AddParameter("emailId", emailId);
			dl.AddParameter("username", username);

			return dl.ExecuteSqlReturnScalar<int>(Utility.ConnectionString, sql) == 1 ? true : false;
		}
	}
}