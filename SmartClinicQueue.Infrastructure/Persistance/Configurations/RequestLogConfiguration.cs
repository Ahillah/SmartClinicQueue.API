using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartClinicQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Infrastructure.Persistance.Configurations
{
  
        public class RequestLogConfiguration : IEntityTypeConfiguration<RequestLog>
        {
            public void Configure(EntityTypeBuilder<RequestLog> builder)
            {
                builder.ToTable("RequestLogs");

                builder.Property(r => r.HttpMethod)
                    .IsRequired()
                    .HasMaxLength(10);

                builder.Property(r => r.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                builder.Property(r => r.Headers)
                    .HasMaxLength(2000);

                builder.Property(r => r.IpAddress)
                    .HasMaxLength(45); 

                builder.HasIndex(r => r.CreatedAt);

                builder.HasIndex(r => new { r.UserId, r.CreatedAt });

                builder.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            }
        }
    }
