using DaemonsMCP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Infrastructure.Persistance.Configurations { 
  public enum IdentifierTypeEnum {
    Namespace = 1,
    Interface = 2,
    Class = 3,
    Method = 4,
    Property = 5,
    Field = 6,
    Event = 7,
    MethodParameter = 8
  }

  public class IdentifierTypeConfiguration : IEntityTypeConfiguration<IdentifierType> {
    public void Configure(EntityTypeBuilder<IdentifierType> builder) {
      builder.HasKey(it => it.Id);
      builder.Property(it => it.Name)
          .IsRequired()
          .HasMaxLength(255);
      builder.HasIndex(it => it.Name).IsUnique();

      builder.HasData(
        new {
          Id = (int)IdentifierTypeEnum.Namespace,
          Name = "namespace"
        },
        new {
          Id = (int)IdentifierTypeEnum.Interface,
          Name = "interface"
        },
        new {
          Id = (int)IdentifierTypeEnum.Class,
          Name = "class"
        },
        new {
          Id = (int)IdentifierTypeEnum.Method,
          Name = "method"
        },
        new {
          Id = (int)IdentifierTypeEnum.Property,
          Name = "property"
        },
        new {
          Id = (int)IdentifierTypeEnum.Field,
          Name = "field"
        },
        new {
          Id = (int)IdentifierTypeEnum.Event,
          Name = "event"
        },
        new {
          Id = (int)IdentifierTypeEnum.MethodParameter,
          Name = "methodParameter"
        }
      );
    }
  }

}



