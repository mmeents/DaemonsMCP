using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DaemonsMCP.Domain.Entities;

namespace DaemonsMCP.Infrastructure.Persistence.Configurations;

public class SettingConfiguration : IEntityTypeConfiguration<Setting> {
  public void Configure(EntityTypeBuilder<Setting> builder) {
    builder.HasKey(s => s.Id);

    builder.Property(s => s.Key)
        .IsRequired()
        .HasMaxLength(200);

    builder.HasIndex(s => s.Key).IsUnique();

    builder.Property(s => s.Value)
        .IsRequired()
        .HasMaxLength(4000);

    builder.Property(s => s.Description)
        .HasMaxLength(1000);

    // Seed default values
    var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    builder.HasData(
        new {
          Id = 1,
          Key = "FileSystem.BlockedFolders",
          Value = "bin,obj,node_modules,.git,.vs,.vscode,packages,Debug,Release,TestResults,.idea",
          Description = "Comma-separated list of folder names to exclude from sync",
          CreatedAt = seedDate,
          ModifiedAt = seedDate
        },
        new {
          Id = 2,
          Key = "FileSystem.BlockedExtensions",
          Value = ".dll,.exe,.pdb,.cache,.suo,.user,.tmp,.temp,.log,.bak",
          Description = "Comma-separated list of file extensions to exclude from sync",
          CreatedAt = seedDate,
          ModifiedAt = seedDate
        },
        new {
          Id = 3,
          Key = "FileSystem.AllowedExtensions",
          Value = ".cs,.csproj,.sln,.json,.xml,.md,.txt,.config,.yml,.yaml",
          Description = "If set, only these extensions are synced (leave empty to allow all non-blocked)",
          CreatedAt = seedDate,
          ModifiedAt = seedDate
        },
        new {
          Id = 4,
          Key = "FileSystem.BlockedFiles",
          Value = ".DS_Store,Thumbs.db,desktop.ini",
          Description = "Comma-separated list of specific filenames to exclude",
          CreatedAt = seedDate,
          ModifiedAt = seedDate
        }
    );
  }
}
