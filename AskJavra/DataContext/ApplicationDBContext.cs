using AskJavra.Models;
using AskJavra.Models.Contribution;
using AskJavra.Models.Employee;
using AskJavra.Models.Post;
using AskJavra.Models.Root;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace AskJavra.DataContext
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        public DbSet<Demo> Demos { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PostThread> PostThreads { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PostUpVote> PostUpVotes { get; set; }
        public DbSet<ThreadUpVote> ThreadUpVotes { get; set; }
        public DbSet<ContributionRank> ContributionRanks { get; set; }
        public DbSet<ContributionPointType> ContributionPointTypes { get; set; }
        public DbSet<ContributionPoint> ContributionPoints { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasIndex(p => new { p.LMSEmployeeId})
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
