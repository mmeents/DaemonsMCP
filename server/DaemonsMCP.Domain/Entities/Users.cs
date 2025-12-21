using System;
using System.Collections.Generic;

namespace DaemonsMCP.Domain.Entities;

public class User {
  public int Id { get; private set; }

  // Basic auth fields
  public string Email { get; private set; } = string.Empty;
  public string? PasswordHash { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;

  // OAuth provider fields
  public string? GoogleId { get; private set; }
  public string? GitHubId { get; private set; }

  // Metadata
  public DateTime CreatedAt { get; private set; }
  public DateTime? LastLoginAt { get; private set; }
  public bool IsActive { get; private set; }

  // EF Core constructor
  private User() { }

  // Factory method for email/password users
  public static User CreateWithPassword(string email, string passwordHash, string displayName) {
    return new User {
      Email = email.ToLowerInvariant(),
      PasswordHash = passwordHash,
      DisplayName = displayName,
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };
  }

  // Factory method for OAuth users
  public static User CreateWithOAuth(string email, string displayName, string? googleId = null, string? gitHubId = null) {
    return new User {
      Email = email.ToLowerInvariant(),
      DisplayName = displayName,
      GoogleId = googleId,
      GitHubId = gitHubId,
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };
  }

  // Methods
  public void UpdatePassword(string newPasswordHash) {
    PasswordHash = newPasswordHash;
  }

  public void LinkGoogleAccount(string googleId) {
    GoogleId = googleId;
  }

  public void LinkGitHubAccount(string gitHubId) {
    GitHubId = gitHubId;
  }

  public void UpdateDisplayName(string displayName) {
    DisplayName = displayName;
  }

  public void RecordLogin() {
    LastLoginAt = DateTime.UtcNow;
  }

  public void Deactivate() {
    IsActive = false;
  }

  public void Activate() {
    IsActive = true;
  }
}
