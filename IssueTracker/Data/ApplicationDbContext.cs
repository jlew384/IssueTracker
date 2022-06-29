using IssueTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            //builder.Entity<ApplicationUserProject>().HasKey("ApplicationUserId", "ProjectId");


            //var cascadeFKs = builder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetForeignKeys())
            //    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            //foreach (var fk in cascadeFKs)
            //    fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(builder);   



        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Issue> Issues { get; set; }
    }
}