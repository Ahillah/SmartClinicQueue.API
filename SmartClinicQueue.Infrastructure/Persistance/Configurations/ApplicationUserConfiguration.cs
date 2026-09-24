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
   
        public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> builder)
            {
                builder.ToTable("Users");

                builder.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.HasOne(u => u.Doctor)
                    .WithOne(d => d.User)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                
                builder.HasMany(u => u.QueueTickets)
                    .WithOne(t => t.Patient)
                    .HasForeignKey(t => t.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);
        


        }

       
    }
    }
