using Microsoft.EntityFrameworkCore;

public class SafeValueDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
        public SafeValueDbContext(DbContextOptions<SafeValueDbContext> options) : base(options)
    {

    }
}
