using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DaemonsMCP.Core.Extensions {
  public static class Sx {
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

    public static string ResolvePath(this string path) {
      // Handle relative paths
      if (!Path.IsPathRooted(path)) {
        // Relative to config file directory or current directory
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
      }

      // Handle cross-platform path separators
      return Path.GetFullPath(path);
    }

    public static string CopyToBackup(this ProjectModel project, string filePath) {

      if (project == null) throw new ArgumentNullException(nameof(project));
      if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

      string resolvedFilePath = Path.GetFullPath(filePath);
      if (!resolvedFilePath.StartsWith(project.Path, StringComparison.OrdinalIgnoreCase)) {
        throw new ArgumentException("File path is not within the project directory", nameof(filePath));
      }

      string relativePath = Path.GetRelativePath(project.Path, resolvedFilePath);
      string backupDir = Path.Combine(project.BackupPath, Path.GetDirectoryName(relativePath) ?? "");
      Directory.CreateDirectory(backupDir);
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string fileName = Path.GetFileName(resolvedFilePath);
      string extension = Path.GetExtension(fileName);
      string backupFileName = $"{fileName}.bakup{timestamp}.{extension}";
      string backupFilePath = Path.Combine(backupDir, backupFileName);
      File.Copy(resolvedFilePath, backupFilePath, true);
      return backupFilePath;
    }

    #region File Size Parsing and Formatting
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
    public static long ParseFileSize(this string sizeString) {
      if (string.IsNullOrEmpty(sizeString))
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

    public static readonly JsonSerializerOptions DefaultJsonOptions = new() {
      PropertyNameCaseInsensitive = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
      };

    #endregion
  }
}
