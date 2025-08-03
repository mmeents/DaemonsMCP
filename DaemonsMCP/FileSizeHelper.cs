using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DaemonsMCP {
  public static class FileSizeHelper {
    private static readonly Dictionary<string, long> SizeMultipliers = new(StringComparer.OrdinalIgnoreCase)
    {
            { "B", 1L },
            { "KB", 1024L },
            { "MB", 1024L * 1024L },
            { "GB", 1024L * 1024L * 1024L },
            { "TB", 1024L * 1024L * 1024L * 1024L }
        };

    private static readonly Regex SizeRegex = new(@"^\s*(\d+(?:\.\d+)?)\s*([KMGT]?B?)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parse a human-readable file size string into bytes
    /// Supports: "10", "10B", "10KB", "1.5MB", "2GB", etc.
    /// </summary>
    /// <param name="sizeString">Size string like "10MB"</param>
    /// <returns>Size in bytes</returns>
    /// <exception cref="ArgumentException">If the format is invalid</exception>
    public static long ParseFileSize(string sizeString) {
      if (string.IsNullOrWhiteSpace(sizeString))
        throw new ArgumentException("Size string cannot be null or empty", nameof(sizeString));

      var match = SizeRegex.Match(sizeString.Trim());
      if (!match.Success)
        throw new ArgumentException($"Invalid size format: '{sizeString}'. Expected format like '10MB', '1.5GB', etc.", nameof(sizeString));

      if (!double.TryParse(match.Groups[1].Value, out var number))
        throw new ArgumentException($"Invalid number in size string: '{sizeString}'", nameof(sizeString));

      var unit = match.Groups[2].Value;
      if (string.IsNullOrEmpty(unit))
        unit = "B"; // Default to bytes

      // Handle common variations
      unit = NormalizeUnit(unit);

      if (!SizeMultipliers.TryGetValue(unit, out var multiplier))
        throw new ArgumentException($"Unknown size unit: '{unit}'. Supported units: B, KB, MB, GB, TB", nameof(sizeString));

      var result = (long)(number * multiplier);
      if (result < 0)
        throw new ArgumentException($"Size cannot be negative: '{sizeString}'", nameof(sizeString));

      return result;
    }

    /// <summary>
    /// Try to parse a file size string, returning false if invalid
    /// </summary>
    /// <param name="sizeString">Size string to parse</param>
    /// <param name="sizeInBytes">Parsed size in bytes</param>
    /// <returns>True if parsing succeeded</returns>
    public static bool TryParseFileSize(string sizeString, out long sizeInBytes) {
      sizeInBytes = 0;
      try {
        sizeInBytes = ParseFileSize(sizeString);
        return true;
      } catch {
        return false;
      }
    }

    /// <summary>
    /// Format bytes into a human-readable string
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Formatted string like "1.5 MB"</returns>
    public static string FormatFileSize(long bytes) {
      if (bytes == 0) return "0 B";
      if (bytes < 0) return "-" + FormatFileSize(-bytes);

      var units = new[] { "B", "KB", "MB", "GB", "TB" };
      var size = (double)bytes;
      var unitIndex = 0;

      while (size >= 1024 && unitIndex < units.Length - 1) {
        size /= 1024;
        unitIndex++;
      }

      return unitIndex == 0
          ? $"{size:F0} {units[unitIndex]}"
          : $"{size:F1} {units[unitIndex]}";
    }

    private static string NormalizeUnit(string unit) {
      return unit.ToUpperInvariant() switch {
        "K" => "KB",
        "M" => "MB",
        "G" => "GB",
        "T" => "TB",
        "" => "B",
        _ => unit.ToUpperInvariant()
      };
    }
  }
}
