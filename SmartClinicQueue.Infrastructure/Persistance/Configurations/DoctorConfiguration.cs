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
   
        public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
        {
            public void Configure(EntityTypeBuilder<Doctor> builder)
            {
                builder.ToTable("Doctors");

                builder.Property(d => d.Specialization)
                    .IsRequired()
                    .HasMaxLength(150);

                builder.Property(d => d.IsActive)
                    .HasDefaultValue(true);

              
                builder.HasIndex(d => d.UserId)
                    .IsUnique();

               
                builder.HasMany(d => d.QueueTickets)
                    .WithOne(t => t.Doctor)
                    .HasForeignKey(t => t.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
