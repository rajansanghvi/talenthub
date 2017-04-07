using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TalentHub.AppCode.Utilities;

namespace TalentHub.AppCode.DataLayer {
	internal class AppSecurityDL {

		internal static KeyValuePair<HashSet<string>, HashSet<string>> FetchAllAppKeys() {
			const string sql = @"select    device_id as device_id
																		, auth_key as auth_key
													from      app_users;";

			GlobalDL dl = new GlobalDL();

			HashSet<string> deviceIds = new HashSet<string>();
			HashSet<string> authKeys = new HashSet<string>();

			using (MySqlDataReader dr = dl.ExecuteSqlReturnReader(Utility.ConnectionString, sql)) {
				while (dr.Read()) {
					deviceIds.Add(dr.GetString("device_id"));
					authKeys.Add(dr.GetString("authKey"));
				}
			}

			return new KeyValuePair<HashSet<string>, HashSet<string>>(deviceIds, authKeys);
		}
	}
}