using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using System.Xml;

namespace TalentHub.AppCode.Utilities {
	public class Utility {
		private static readonly string className = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();

		public const string SYSTEM_USER_ID = "system";

		public static string ConnectionString {
			get {
				return ConfigurationManager.ConnectionStrings["TalentHubDbContext"].ConnectionString;
			}
		}

		internal static string GetMd5Hash(string input) {
			using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create()) {
				// Convert the input string to a byte array and compute the hash.
				byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

				// Create a new Stringbuilder to collect the bytes
				// and create a string.
				StringBuilder sBuilder = new StringBuilder();

				// Loop through each byte of the hashed data
				// and format each one as a hexadecimal string.
				for (int i = 0; i < data.Length; i++) {
					sBuilder.Append(data[i].ToString("x2"));
				}

				// Return the hexadecimal string.
				return sBuilder.ToString();
			}
		}

		public static string GetUniqueKey(int maxSize, bool useSpecialCharacters, bool onlyNumbers) {
			string characterSet = "abcdefghijklmnopqrstuvwxyz1234567890";
			if (useSpecialCharacters) {
				characterSet += "-[]@#!()";
			}
			if (onlyNumbers) {
				characterSet = "1234567890";
			}

			char[] chars = characterSet.ToCharArray();

			List<byte> byteList = new List<byte>();
			byteList.AddRange(Guid.NewGuid().ToByteArray());
			byteList.AddRange(Guid.NewGuid().ToByteArray());
			byteList.AddRange(Guid.NewGuid().ToByteArray());

			StringBuilder result = new StringBuilder(maxSize);
			for (int i = 0; i < maxSize; i++) {
				byte b = byteList[i];
				result.Append(chars[b % (chars.Length - 1)]);
			}
			return result.ToString();
		}

		internal static bool IsProduction {
			get {
				return (Utility.GetConfigValueFromRegistry("Environment") == "PROD");
			}
		}

		public static string GetConfigValueFromRegistry(string keyName) {
			string keyValue = "";

			try {
				RegistryKey key = Registry.LocalMachine.OpenSubKey("Software");
				RegistryKey subKey = key.OpenSubKey("TalentHub");
				keyValue = (string) subKey.GetValue(keyName);

				subKey.Close();
				key.Close();
			}
			catch {
			}

			return keyValue;
		}

		public static string GetConfigValue(string keyName) {
			string keyValue = "";

			keyValue = ConfigurationManager.AppSettings[keyName];
			if (keyValue == null) {
				keyValue = "";
			}

			return keyValue;
		}

		public static string AddWildCard(string input) {
			if (string.IsNullOrWhiteSpace(input)) {
				return string.Empty;
			}
			const string WC = "%{0}%";

			return string.Format(WC, input);
		}

		public static string TalentHubUrlInternal {
			get {
				return Utility.GetConfigValueFromRegistry("TalentHubServerUrl");
			}
		}

		public static string CurrentUserId {
			get {
				try {
					return Thread.CurrentPrincipal.Identity.Name.ToLower();
				}
				catch (Exception) {
					return Utility.SYSTEM_USER_ID;
				}
			}
		}

		public static string Environment {
			get {
				return Utility.GetConfigValueFromRegistry("Environment");
			}
		}

		internal static bool IsDevelopment {
			get {
				return (Utility.GetConfigValueFromRegistry("Environment") == "DEV");
			}
		}

		internal static bool IsTest {
			get {
				return (Utility.GetConfigValueFromRegistry("Environment") == "TEST");
			}
		}

		internal static string ServerUrl {
			get {
				return Utility.GetConfigValue(Utility.IsProduction ? "URL_PROD" : "URL_TEST");
			}
		}

		public static string GetBlankStringForNull(string s) {
			return (string.IsNullOrWhiteSpace(s) ? string.Empty : s);
		}

		public static string GetConnString(string connStringName) {
			return Utility.GetConfigValueFromRegistry(connStringName);
		}

		public static Decimal GetDecimalValueFromQueryString(NameValueCollection qs, string paramName) {
			Decimal returnValue = 0;

			if (qs[paramName] != null) {
				try {
					returnValue = XmlConvert.ToDecimal(qs[paramName]);
				}
				catch {
				}
			}
			return returnValue;
		}

		public static int GetIntConfigValue(string keyName) {
			string keyValue = GetConfigValue(keyName);

			if (string.IsNullOrWhiteSpace(keyValue)) {
				return -1;
			}
			return Convert.ToInt32(keyValue);
		}
		
		public static string GetValueFromQueryString(NameValueCollection qs, string paramName) {
			string returnValue = qs[paramName];

			if (returnValue == null) {
				returnValue = "";
			}

			return returnValue;
		}

		public static Decimal GetXmlDecimalValueFromDom(XmlDocument dom, string xPath) {
			Decimal returnValue = 0;
			XmlNode selectedNode = dom.SelectSingleNode(xPath);

			if (selectedNode != null) {
				try {
					returnValue = XmlConvert.ToDecimal(selectedNode.InnerText);
				}
				catch {
				}
			}
			return returnValue;
		}

		public static int GetXmlIntValueFromDom(XmlDocument dom, string xPath) {
			int returnValue = 0;
			XmlNode selectedNode = dom.SelectSingleNode(xPath);

			if (selectedNode != null) {
				try {
					returnValue = XmlConvert.ToInt32(selectedNode.InnerText);
				}
				catch {
				}
			}
			return returnValue;
		}

		public static string GetXmlValueFromDom(XmlDocument dom, string xPath) {
			string returnValue = "";
			XmlNode selectedNode = dom.SelectSingleNode(xPath);

			if (selectedNode != null) {
				returnValue = selectedNode.InnerText;
			}
			return returnValue;
		}

		public static string LongRunningConnectionString(string connstring) {
			MySqlConnectionStringBuilder connSb = new MySqlConnectionStringBuilder(connstring);
			connSb.DefaultCommandTimeout = 120;
			return connSb.ToString();
		}

		public static string NumberToText(double n) {
			n = Math.Floor(n);
			if (n < 0)
				return "Minus " + NumberToText(-n);
			else if (n == 0)
				return "";
			else if (n <= 19)
				return new string[] {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight",
				 "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
				 "Seventeen", "Eighteen", "Nineteen"}[Convert.ToInt32(n) - 1] + " ";
			else if (n <= 99)
				return new string[] {"Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy",
				 "Eighty", "Ninety"}[Convert.ToInt32(n) / 10 - 2] + " " + NumberToText(n % 10);
			else if (n <= 199)
				return "One Hundred " + NumberToText(n % 100);
			else if (n <= 999)
				return NumberToText(n / 100) + "Hundred " + NumberToText(n % 100);
			else if (n <= 1999)
				return "One Thousand " + NumberToText(n % 1000);
			else if (n <= 999999)
				return NumberToText(n / 1000) + "Thousand " + NumberToText(n % 1000);
			else if (n <= 1999999)
				return "One Million " + NumberToText(n % 1000000);
			else if (n <= 999999999)
				return NumberToText(n / 1000000) + "Millions " + NumberToText(n % 1000000);
			else if (n <= 1999999999)
				return "One Billion " + NumberToText(n % 1000000000);
			else
				return NumberToText(n / 1000000000) + "Billions " + NumberToText(n % 1000000000);
		}

		public static T ParseEnum<T>(string value) {
			return (T) Enum.Parse(typeof(T), value, true);
		}

		public static double Percentile(double[] sequence, double percentile) {
			if (sequence.Length == 0) {
				return 0;
			}

			Array.Sort(sequence);

			//return sequence[Convert.ToInt32(Math.Round(sequence.Length * percentile, 0))];
			int N = sequence.Length;
			double n = (N - 1) * percentile + 1;

			if (n == 1d) {
				return sequence[0];
			}
			else if (n == N) {
				return sequence[N - 1];
			}
			else {
				int k = (int) n;
				double d = n - k;
				return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
			}
		}

		public static void SetConfigValue(string keyName, string keyValue) {
			Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

			webConfigApp.AppSettings.Settings[keyName].Value = keyValue;
			webConfigApp.Save();
		}

		internal static StringBuilder AppendForCsvExport(StringBuilder sb, object value) {
			return sb.Append("\"").Append(value.ToString()).Append("\",");
		}

		internal static StringBuilder AppendRowForCsvExport(StringBuilder sb, params object[] values) {
			foreach (Object item in values) {
				sb.Append("\"").Append(item.ToString()).Append("\",");
			}
			sb.Remove(sb.Length - 1, 1);
			sb.AppendLine("");

			return sb;
		}

		internal static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		internal static string BuildCsv(string s) {
			s = RemoveFakes(s);

			return (String.IsNullOrEmpty(s)) ? "" : s + ", ";
		}

		internal static string RemoveFakes(string s) {
			List<String> fakes = new List<string>();
			fakes.Add("*");

			return (fakes.Contains(s)) ? "" : s;
		}

		internal static string CalculatePercentage(decimal numerator, decimal denominator) {
			return Utility.CalculatePercentageNumber(numerator, denominator).ToString() + " %";
		}

		internal static decimal CalculatePercentageNumber(decimal numerator, decimal denominator) {
			if (denominator == 0) {
				return 999;
			}
			return Math.Round((numerator / denominator) * 100, 0);
		}

		internal static decimal CalculatePercentageIncreaseDecimal(decimal currentValue, decimal previousValue) {
			if (previousValue == currentValue) {
				return 0;
			}
			if (currentValue == 0) {
				return -999;
			}
			if (previousValue == 0) {
				return 999;
			}
			return Math.Round(((currentValue - previousValue) / previousValue) * 100, 0);
		}

		internal static bool CompareStrings(string s1, string s2) {
			return (string.Compare(s1, s2, true) == 0);
		}

		internal static string FormatMoneyForDisplay(float f) {
			return f.ToString("C0");
		}

		internal static string FormatMoneyForDisplay(double f) {
			return f.ToString("C0");
		}

		internal static string FormatMoneyForDisplay(decimal f) {
			return f.ToString("C0");
		}

		internal static string CalculatePercentageIncrease(decimal currentValue, decimal previousValue) {
			return Utility.CalculatePercentageIncreaseDecimal(currentValue, previousValue).ToString() + " %";
		}

		internal static string GeneratePin(int numberOfChars, bool alphaNumeric) {
			const string methodName = "GeneratePin";
			// LoggerBL.LogMethodStart (className, methodName);
			const int MIN_SIZE = 1;
			int maxSize = 1;

			if (alphaNumeric) {
				numberOfChars--;
			}

			for (int i = 1; i < numberOfChars; i++) {
				maxSize *= 10;
			}

			string returnValue = (new Random()).Next(MIN_SIZE, maxSize).ToString().PadLeft(numberOfChars, '0');

			if (alphaNumeric) {
				const string ALPHABETS = "ABCDEFGHJKLMNPQRSTUVWXYZ";
				returnValue = ALPHABETS.Substring(new Random().Next(0, 23), 1) + returnValue;
			}
			Thread.Sleep(1);

			// LoggerBL.LogMethodEnd (className, methodName);
			return returnValue;
		}

		internal static string Get10DigitPhoneNumber(string phoneNumber) {
			if (string.IsNullOrWhiteSpace(phoneNumber)) {
				return string.Empty;
			}
			if (phoneNumber.Length <= 10) {
				return phoneNumber;
			}
			return phoneNumber.Substring(phoneNumber.Length - 10);
		}

		internal static string Get12DigitPhoneNumber(string phoneNumber) {
			if (string.IsNullOrWhiteSpace(phoneNumber)) {
				return string.Empty;
			}
			return (phoneNumber.Length == 10) ? "91" + phoneNumber : phoneNumber;
		}

		// return InitCap version of each word of the parameter
		internal static string InitCap(string value) {
			try {
				value = value.Trim();
				value = value.ToLower();
				char[] array = value.ToCharArray();

				// Handle the first letter in the string.
				if (array.Length >= 1) {
					array[0] = char.ToUpper(array[0]);
				}

				// Scan through the letters, checking for spaces.
				// ... Uppercase the lowercase letters following spaces.
				for (int i = 1; i < array.Length; i++) {
					if (array[i - 1] == ' ') {
						array[i] = char.ToUpper(array[i]);
					}
				}
				return new string(array);
			}
			catch {
				return value;
			}
		}

		internal static List<string> InitCap(List<string> value) {
			List<string> returnValue = new List<string>();

			foreach (string s in value) {
				returnValue.Add(Utility.InitCap(s));
			}

			return returnValue;
		}

		internal static string AlphaNumericsOnly(string s) {
			Regex rgx = new Regex("[^a-zA-Z0-9]");
			return rgx.Replace(s, "");
		}

		internal static string ValidatePhoneNumber(string phoneNumber) {
			string returnMessage = string.Empty;
			const string DIGITS = "0123456789";

			// check length
			if (Utility.Get12DigitPhoneNumber(phoneNumber).Length != 12) {
				returnMessage = "Incomplete phone number";
			}

			// check characters
			bool allNumbers = true;
			if (string.IsNullOrWhiteSpace(returnMessage)) {
				for (int i = 0; i < phoneNumber.Length; i++) {
					string s = phoneNumber.Substring(i, 1);

					if (DIGITS.IndexOf(s) == -1) {
						allNumbers = false;
						break;
					}
				}

				if (!allNumbers) {
					returnMessage = "Phone Number contains invalid characters";
				}
			}

			return returnMessage;
		}

		internal static List<string> SplitStringByLength(string s, int len) {
			List<string> retStrings = new List<string>();

			do {
				if (s.Length > len) {
					retStrings.Add(s.Substring(0, len));
					s = s.Substring(len);
				}
				else {
					retStrings.Add(s);
					return retStrings;
				}
			} while (true);
		}

		internal static Stopwatch LogTime(string message, Stopwatch stop, bool startAgain) {
			stop.Stop();
			const string MSG_FORMAT = "Timing: {0}. {1} ms";

			// LoggerBL.LogInfo(string.Empty, string.Empty, MSG_FORMAT, message, stop.ElapsedMilliseconds);

			if (startAgain) {
				return Stopwatch.StartNew();
			}
			return stop;
		}

		internal static bool IsValidPhoneNumber(string phoneNumber) {
			// does not start with 556677 and is not less than 10 chars
			return !(phoneNumber.StartsWith("556677") || phoneNumber.Length < 10);
		}

		internal static bool IsInternalNumber(string phoneNumber) {
			string[] internalNumbers = Utility.GetConfigValue("INTERNAL_PHONE_NUMBERS").Split(',');

			return internalNumbers.Contains(phoneNumber) || phoneNumber.StartsWith("556677");
		}
	}
}