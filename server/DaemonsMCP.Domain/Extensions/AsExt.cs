using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Extensions {

  public enum AsType {
    Unknown = 0,
    Null = 1,
    Boolean = 2,
    Int32 = 3,
    Int64 = 4,
    Decimal = 5,
    DateTime = 6,
    Guid = 7,
    String = 8,
    Bytes = 9
  }

  public static class AsExt {

    public static int AsInt32(this string value) {
      return int.Parse(value);
    }
    public static long AsInt64(this string value) {
      return long.Parse(value);
    }
    public static byte[] FromStringAsBase64Bytes(this string value) {
      return Convert.FromBase64String(value);
    }

    public static string FromBase64BytesAsString(this byte[] bytes) {
      return Convert.ToBase64String(bytes);
    }
    public static bool AsBoolean(this string value) {
      return Convert.ToBoolean(value);
    }
    public static DateTime AsDateTime(this string value) {
      return DateTime.Parse(value);
    }
    public static Decimal AsDecimal(this string value) {
      return Decimal.Parse(value);
    }
    public static byte[] FromStringAsBytes(this string text) {
      return Encoding.UTF8.GetBytes(text);
    }
    public static string FromBytesAsString(this byte[] bytes) {
      return Encoding.UTF8.GetString(bytes);
    }
    public static string AsString(this int value) {
      return value.ToString();
    }


    public static int AsInt32(this Object value) {
      return Convert.ToInt32(value);
    }
    public static long AsInt64(this Object value) {
      return Convert.ToInt64(value);
    }
    public static byte[] AsBytes(this Object value) {
      return (byte[])value;
    }
    public static bool AsBoolean(this Object value) {
      return Convert.ToBoolean(value);
    }
    public static DateTime AsDateTime(this Object value) {
      return Convert.ToDateTime(value);
    }
    public static Decimal AsDecimal(this Object value) {
      return Convert.ToDecimal(value);
    }
    public static string AsString(this Object value) {
      return value?.ToString() ?? "";
    }


    #region Date to string 
    /// <summary> Day to string Sortable yyyy-MM-dd</summary>
    /// <returns> string </returns>
    public static string AsStrDate(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", x);
    }
    /// <summary> DateTime to string yyyy-MM-dd hh:mm:ss.FFF tt </summary>
    /// <returns> string </returns>
    public static string AsStrDateTime12H(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd hh:mm:ss.FFF tt}", x);
    }
    /// <summary> DateTime to string yyyy-MM-dd HH:mm:ss.FFF</summary>
    /// <returns> string </returns>
    public static string AsStrDateTime24H(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.FFF}", x);
    }
    /// <summary> DateTime to string DateTime h:mm:ss tt</summary>
    /// <returns> string </returns>
    public static string AsStrTime(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:h:mm:ss tt}", x);
    }
    /// <summary> DateTime to string Day Time MM/dd/yyyy hh:mm</summary>
    /// <returns> string </returns>
    public static string AsStrDayHHMM(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:MM/dd/yyyy hh:mm}", x);
    }
    /// <summary> DateTime to string Day MM/dd/yyyy</summary>
    /// <returns> string </returns>
    public static string AsStrDay(this DateTime x) {
      return String.Format(CultureInfo.InvariantCulture, "{0:MM/dd/yyyy}", x);
    }

    #endregion


    public static string[] Parse(this string content, string delims) {
      return content.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public static string ParseFirst(this string content, string delims) {
      string[] sr = content.Parse(delims);
      if (sr.Length > 0) {
        return sr[0];
      }
      return "";
    }
    public static string ParseLast(this string content, string delims) {
      string[] sr = content.Parse(delims);
      if (sr.Length > 0) {
        return sr[^1];
      }
      return "";
    }

  }
}
