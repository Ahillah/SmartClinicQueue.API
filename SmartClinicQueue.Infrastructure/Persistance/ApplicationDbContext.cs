using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartClinicQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Infrastructure.Persistance
{
    
        public class ApplicationDbContext
            : IdentityDbContext<ApplicationUser, ApplicationRole, int>
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Doctor> Doctors => Set<Doctor>();
            public DbSet<Clinic> Clinics => Set<Clinic>();
            public DbSet<QueueTicket> QueueTickets => Set<QueueTicket>();

        public DbSet<RequestLog> RequestLogs => Set<RequestLog>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Apply all configurations from this assembly
                modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            }

           
        }
    }
