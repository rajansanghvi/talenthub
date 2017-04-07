using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using static TalentHub.AppCode.Models.Structs;

namespace TalentHub.AppCode.Utilities {
  public class DateUtility {
    
    public const string DB_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public static DateTime DefaultDate = XmlConvert.ToDateTime ("1900-01-01", "yyyy-MM-dd");
    public static DateTime MaxDate = new DateTime (2100, 1, 1);

    private static readonly string className = System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType.ToString ();

    public static DateRange AllDays {
      get {
        return new DateRange (DateTime.Today.AddDays (-1000), DateTime.Today);
      }
    }

    public static string GetCurrentDateTimeForDisplay {
      get {
        return FormatDateTimeForDisplay (DateTime.Now);
      }
    }

    public static bool IsBankWorkingTime {
      get {
        DayOfWeek day = DateTime.Today.DayOfWeek;
        if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) {
          return false;
        }
        return TimeBetween (10, 17);
      }
    }

    public static DateRange Last14Days {
      get {
        return new DateRange (DateTime.Today.AddDays (-13), DateTime.Today);
      }
    }

    public static DateRange Last30Days {
      get {
        return new DateRange (DateTime.Today.AddDays (-29), DateTime.Today);
      }
    }

    public static DateRange Last360Days {
      get {
        return new DateRange (DateTime.Today.AddDays (-359), DateTime.Today);
      }
    }

    public static DateRange Last7Days {
      get {
        return new DateRange (DateTime.Today.AddDays (-6), DateTime.Today);
      }
    }

    public static DateRange LastMonth {
      get {
        DateRange lastMonth = DateUtility.ThisMonth;
        lastMonth.EndDate = lastMonth.StartDate.AddDays (-1);
        lastMonth.StartDate = GetMonthStart (lastMonth.EndDate);

        return lastMonth;
      }
    }

    public static DateRange LastWeek {
      get {
        DateRange lastWeek = DateUtility.ThisWeek;
        lastWeek.EndDate = lastWeek.StartDate.AddDays (-1);
        lastWeek.StartDate = lastWeek.EndDate.AddDays (-6);

        return lastWeek;
      }
    }

    public static DateRange Next7Days {
      get {
        return new DateRange (DateTime.Today, DateTime.Today.AddDays (6));
      }
    }

    public static DateRange Previous30Days {
      get {
        return new DateRange (DateTime.Today.AddDays (-59), DateTime.Today.AddDays (-29));
      }
    }

    public static DateRange Previous7Days {
      get {
        return new DateRange (DateTime.Today.AddDays (-13), DateTime.Today.AddDays (-7));
      }
    }

    public static DateRange PreviousMonth {
      get {
        DateRange previousMonth = DateUtility.LastMonth;
        previousMonth.EndDate = previousMonth.StartDate.AddDays (-1);
        previousMonth.StartDate = GetMonthStart (previousMonth.EndDate);

        return previousMonth;
      }
    }

    public static DateRange ThisFortnight {
      get {
        // WTD
        CultureInfo info = Thread.CurrentThread.CurrentCulture;
        DayOfWeek firstday = DayOfWeek.Monday;
        DateTime today = DateTime.Today;
        DayOfWeek todayDow = info.Calendar.GetDayOfWeek (today);

        int diff = todayDow - firstday;

        if (diff == -1) {
          diff = 6;
        }

        // week start
        DateTime startDate = today.AddDays (-diff);

        // fortnight start
        startDate = startDate.AddDays (-7);

        return new DateRange (startDate, today);
      }
    }

    public static DateRange ThisMonth {
      get {
        DateTime today = DateTime.Today;
        return new DateRange (GetMonthStart (today), today);
      }
    }

    public static DateRange ThisWeek {
      get {
        // WTD
        CultureInfo info = Thread.CurrentThread.CurrentCulture;
        DayOfWeek firstday = DayOfWeek.Monday;
        DateTime today = DateTime.Today;
        DayOfWeek todayDow = info.Calendar.GetDayOfWeek (today);

        int diff = todayDow - firstday;

        if (diff == -1) {
          diff = 6;
        }
        DateTime startDate = today.AddDays (-diff);

        return new DateRange (startDate, today);
      }
    }

    public static DateRange ThisYear {
      get {
        DateTime today = DateTime.Today;
        DateTime endDate = today.AddDays (1);
        return new DateRange (GetYearStart (today), endDate);
      }
    }

    public static DateRange Today {
      get {
        return new DateRange (DateTime.Today, DateTime.Today);
      }
    }

    public static DateRange Yesterday {
      get {
        return new DateRange (DateTime.Today.AddDays (-1), DateTime.Today.AddDays (-1));
      }
    }

    internal static bool IsBackupTime {
      get {
        if (Utility.IsProduction) {
          string [] backupTimings = Utility.GetConfigValue ("BackupTimings").Split (',');
          DateTime backupTime;
          DateTime currentTime = DateTime.Now;
          int backupDuration = 5;

          foreach (string s in backupTimings) {
            backupTime = new DateTime (currentTime.Year, currentTime.Month, currentTime.Day, Convert.ToInt32 (s.Trim ().Split (':') [0]), Convert.ToInt32 (s.Trim ().Split (':') [1]), 0);
            if (currentTime >= backupTime.AddMinutes (-1) && currentTime <= backupTime.AddMinutes (backupDuration)) {
              return true;
            }
          }

          return false;
        }
        return false;
      }
    }
    
    public static string FormatDateForDisplay (DateTime? dt) {
      if (dt.HasValue) {
        return dt.Value.ToString ("dd/MM/yyyy", CultureInfo.InstalledUICulture);
      }
      else {
        return string.Empty;
      }
    }

    public static string GetDateForSqlParam (DateTime dt) {
      return dt.ToString (DB_DATE_FORMAT, CultureInfo.InstalledUICulture);
    }

    public static DateTime GetDateValueFromQueryString (NameValueCollection qs, string paramName) {
      DateTime returnValue = Convert.ToDateTime ("1900-01-01");

      if (qs [paramName] != null) {
        try {
          returnValue = XmlConvert.ToDateTime (qs [paramName], "yyyy-MM-dd HH:mm:ss");
        }
        catch {
        }
      }
      return returnValue;
    }

    public static DateRange GetMonthDateRange (int year, int month) {
      DateTime startDate = new DateTime (year, month, 1);
      DateTime endDate = startDate.AddMonths (1).AddDays (-1);

      return new DateRange (startDate, endDate);
    }
    
    public static Int64 GetUnixTimeStamp (DateTime dt) {
      return Convert.ToInt64 (Math.Round ((dt - new DateTime (1970, 1, 1).ToLocalTime ()).TotalSeconds, 0));
    }

    public static Int64 GetUnixTimeStampInMs (DateTime dt) {
      return Convert.ToInt64 (Math.Round ((dt - new DateTime (1970, 1, 1).ToLocalTime ()).TotalMilliseconds, 0));
    }

    public static DateTime GetXmlDateValueFromDom (XmlDocument dom, string xPath) {
      DateTime returnValue = Convert.ToDateTime ("1900-01-01");
      XmlNode selectedNode = dom.SelectSingleNode (xPath);

      if (selectedNode != null) {
        try {
          returnValue = XmlConvert.ToDateTime (selectedNode.InnerText, "yyyy-MM-dd HH:mm:ss");
        }
        catch {
        }
      }
      return returnValue;
    }

    public static Decimal GetXmlDecimalValueFromDom (XmlDocument dom, string xPath) {
      Decimal returnValue = 0;
      XmlNode selectedNode = dom.SelectSingleNode (xPath);

      if (selectedNode != null) {
        try {
          returnValue = XmlConvert.ToDecimal (selectedNode.InnerText);
        }
        catch {
        }
      }
      return returnValue;
    }

    internal static string FormatCurrentDateForDisplay () {
      return DateUtility.FormatDateForDisplay (DateTime.Today);
    }

    internal static string FormatDateTimeForDisplay (DateTime? dt) {
      if (dt.HasValue) {
        return Convert.ToDateTime (dt).ToString ("dd/MM/yyyy HH:mm");
      }
      else {
        return "";
      }
    }

    internal static string FormatTimeForDisplay (double time) {
      return DateUtility.FormatTimeForDisplay (Convert.ToDecimal (time));
    }

    internal static string FormatTimeForDisplay (decimal time) {
      TimeSpan ts = new TimeSpan (0, 0, Convert.ToInt32 (time));

      if (ts.Hours == 0) {
        return ts.ToString (@"mm\:ss");
      }
      else {
        return ts.ToString (@"hh\:mm\:ss");
      }
    }

    internal static string FormatTimeForDisplay (DateTime dt) {
      return dt.ToString ("HH:mm");
    }

    internal static string FormatTimeForDisplayNonMilitary (DateTime dt) {
      return dt.ToString ("h:mm tt");
    }

    internal static MySqlParameter GetDateParam (string paramName, DateTime date) {
      MySqlParameter returnValue = new MySqlParameter (paramName, MySqlDbType.DateTime);

      if (date != DateTime.MinValue) {
        returnValue.Value = date;
      }

      return returnValue;
    }

    internal static string GetDateTimeForDisplayFromUnixTimeStamp (object secs) {
      try {
        double dblSecs = double.Parse ((string) secs);
        return DateUtility.GetDateTimeFromUnixTimeStamp (dblSecs).ToString ("dd/MM/yyyy HH:mm:ss");
      }
      catch (Exception) { }
      return "0";
    }

    internal static DateTime GetDateTimeFromUnixTimeStamp (double secs) {
      return (new DateTime (1970, 1, 1).AddSeconds (secs)).ToLocalTime ();
    }

    internal static DateTime GetMonthEnd (DateTime dt) {
      dt = new DateTime (dt.Year, dt.Month, 1);
      dt = dt.AddMonths (1);
      return dt.AddDays (-1);
    }

    internal static DateTime GetMonthStart (DateTime dt) {
      return new DateTime (dt.Year, dt.Month, 1);
    }

    /// get 9AM today if before 9, else 9AM tomorrow.
    internal static DateTime GetNext9Am () {
      if (DateTime.Now.Hour < 9) {
        return DateTime.Today.Date.AddHours (9);
      }
      else {
        return DateTime.Today.Date.AddDays (1).AddHours (9);
      }
    }

    internal static DateRange GetPreviousHalfMonthDate () {
      DateRange returnValue = DateUtility.GetPreviousHalfDate (DateTime.Now);
      returnValue.EndDate = returnValue.EndDate.AddDays (-1);

      return DateUtility.GetPreviousHalfDate (returnValue.EndDate);
    }

    internal static DateRange GetRecentHalfMonthDate () {
      return DateUtility.GetPreviousHalfDate (DateTime.Now);
    }

    internal static DateTime GetTimeFromString (string time) {
      return DateTime.ParseExact (time, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
    }

    internal static DateTime GetWeekEnd (DateTime dt) {
      return GetWeekStart (dt).AddDays (6);
    }

    internal static DateTime GetWeekEnd (DateTime dt, DayOfWeek firstDay) {
      return GetWeekStart (dt, firstDay).AddDays (6);
    }

    internal static DateTime GetWeekStart (DateTime dt) {
      return GetWeekStart (dt, DayOfWeek.Monday);
    }

    internal static DateTime GetWeekStart (DateTime dt, DayOfWeek firstDay) {
      CultureInfo info = Thread.CurrentThread.CurrentCulture;
      DayOfWeek firstday = DayOfWeek.Monday;
      DayOfWeek todayDow = info.Calendar.GetDayOfWeek (dt);

      int diff = todayDow - firstday;

      if (diff == -1) {
        diff = 6;
      }
      return dt.AddDays (-diff);
    }

    internal static DateTime GetYearEnd (DateTime dt) {
      dt = new DateTime (dt.Year, 1, 1);
      dt = dt.AddYears (1);
      return dt.AddDays (-1);
    }

    internal static DateTime GetYearStart (DateTime dt) {
      return new DateTime (dt.Year, 1, 1);
    }

    internal static bool TimeBetween (double startHour, double endHour) {
      double minsSinceStartOfDay = (DateTime.Now - DateTime.Today).TotalMinutes;

      return (minsSinceStartOfDay > (startHour * 60) && minsSinceStartOfDay < (endHour * 60));
    }
    
    /*
      when current day is:
      <= 15: end-date will be end of the previous month
      > 15: end-date will be 15th of current month
      start-date will be 15 days before end-date
    */
    private static DateRange GetPreviousHalfDate (DateTime dt) {
      DateTime returnValue;

      if (dt.Day <= 15) {
        // get the first of the month
        returnValue = new DateTime (dt.Year, dt.Month, 1);

        // get last day of previous month
        returnValue = returnValue.AddDays (-1);
      }
      else {
        returnValue = new DateTime (dt.Year, dt.Month, 15);
      }

      return new DateRange (returnValue.AddDays (-15), returnValue);
    }
  }
}