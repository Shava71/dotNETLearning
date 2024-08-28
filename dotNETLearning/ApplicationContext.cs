using Microsoft.EntityFrameworkCore;

namespace dotNETLearning
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected void OnModelCrating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User{Id = 1, Name = "Tom", Age = 25},
                new User { Id = 1, Name = "Tomas", Age = 55 },
                new User { Id = 1, Name = "Tomik", Age = 75 });
        }
    }
}
