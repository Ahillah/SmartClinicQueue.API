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
     public class QueueTicketConfiguration : IEntityTypeConfiguration<QueueTicket>
        {
            public void Configure(EntityTypeBuilder<QueueTicket> builder)
            {
                builder.ToTable("QueueTickets");

                builder.Property(t => t.Priority)
                    .HasConversion<int>()
                    .IsRequired();

                builder.Property(t => t.Status)
                    .HasConversion<int>()
                    .IsRequired();

              
                builder.HasIndex(t => new { t.DoctorId, t.Status });

              
                builder.HasIndex(t => new { t.PatientId, t.Status });

        
                builder.HasIndex(t => new { t.DoctorId, t.TicketNumber });
            }
        }
    }

