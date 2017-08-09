using MySql.Data.MySqlClient;
using TalentHub.AppCode.Models;
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

		internal static UserDetails GetUserDetails(string username, string password) {
			const string sql = @"select    id as id
																			, email_id as email_id
																			, username as username
																			, device_id as devuce_id
																			, auth_key as auth_key
																			, deleted as deleted
														from      app_users
														where     lower(username) = LOWER(?username)
														and       hashed_password = ?hashedPassword;";

			GlobalDL dl = new GlobalDL();
			dl.AddParameter("username", username);
			dl.AddParameter("hashedPassword", password);

			using (MySqlDataReader dr = dl.ExecuteSqlReturnReader(Utility.ConnectionString, sql)) {
				if(dr.Read()) {
					return new UserDetails() {
						Id = dr.GetInt32("id"),
						EmailId = dr.GetString("email_id"),
						Username = dr.GetString("username"),
						DeviceId = dr.GetString("device_id"),
						AuthKey = dr.GetString("auth_key"),
						Deleted = dr.GetBoolean("deleted")
					};
				}
			}

			return null;
		}
	}
}