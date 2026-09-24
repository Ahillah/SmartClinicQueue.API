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
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
        {
            public void Configure(EntityTypeBuilder<Clinic> builder)
            {
                builder.ToTable("Clinics");

                builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                builder.Property(c => c.Description)
                    .HasMaxLength(500);

                builder.Property(c => c.IsActive)
                    .HasDefaultValue(true);

                builder.HasMany(c => c.Doctors)
                    .WithOne(d => d.Clinic)
                    .HasForeignKey(d => d.ClinicId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(c => c.QueueTickets)
                    .WithOne(t => t.Clinic)
                    .HasForeignKey(t => t.ClinicId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
