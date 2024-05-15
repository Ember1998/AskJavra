using AskJavra.Models;
using AskJavra.Models.Employee;
using AskJavra.Models.Post;
using AskJavra.Models.Root;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
    }
}
