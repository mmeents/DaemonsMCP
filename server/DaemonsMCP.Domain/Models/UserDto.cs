using System;

namespace DaemonsMCP.Domain.Models { 

  public class UserDto {
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool HasPassword { get; set; }
    public bool HasGoogleAuth { get; set; }
    public bool HasGitHubAuth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
  }

  public static class  UserDtoExt {
    public static UserDto ToDto(this Entities.User user) {
      return new UserDto {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
        HasGoogleAuth = user.GoogleId != null,
        HasGitHubAuth = user.GitHubId != null,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt,
        IsActive = user.IsActive
      };
    }
  }


}