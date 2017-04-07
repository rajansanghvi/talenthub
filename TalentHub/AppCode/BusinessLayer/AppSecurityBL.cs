using System.Collections.Generic;
using TalentHub.AppCode.DataLayer;
using TalentHub.AppCode.Utilities;

namespace TalentHub.AppCode.BusinessLayer {
	internal class AppSecurityBL {

		internal static KeyValuePair<string, string> GenerateUniqueAppKeys() {
			//Fetch all existing deviceIds and authKeys
			KeyValuePair<HashSet<string>, HashSet<string>> appKeys = AppSecurityDL.FetchAllAppKeys();

			string deviceId = string.Empty;
			string authKey = string.Empty;

			while (true) {
				deviceId = Utility.GetUniqueKey(25, false, false);
				authKey = Utility.GetUniqueKey(40, true, false);

				if (!appKeys.Key.Contains(deviceId) && !appKeys.Value.Contains(authKey)) {
					break;
				}
				else {
					continue;
				}
			}

			return new KeyValuePair<string, string>(deviceId, authKey);
		}
	}
}