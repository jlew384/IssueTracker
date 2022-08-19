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
            base.OnModelCreating(builder);   
        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Issue> Issues { get; set; }

        public virtual DbSet<IssueHistory> IssuesHistory { get; set; }

        public virtual DbSet<IssueComment> IssueComments { get; set; }
    }
}