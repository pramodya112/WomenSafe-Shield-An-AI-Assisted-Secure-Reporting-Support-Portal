using System;
using Microsoft.EntityFrameworkCore;
using WomenEmpower.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WomenEmpower.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Evidence> Evidences { get; set; }
        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        protected override void OnModelCreating (ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Evidence>()
                .HasOne(e => e.Report)
                .WithMany(r => r.Evidence)
                .HasForeignKey(e => e.ReportId);

            modelbuilder.Entity<AIAnalysis>()
                .HasOne(a => a.Report)
                .WithOne(r => r.Analysis)
                .HasForeignKey<AIAnalysis>(a => a.ReportId);
        }

    }

}

