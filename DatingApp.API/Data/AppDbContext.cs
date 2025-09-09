using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data;

public class AppDbContext : DbContext
{
	private readonly IConfiguration _configuration;

	public DbSet<AppUser> Users { get; set; }
	public DbSet<Member> Members { get; set; }
	public DbSet<Photo> Photos { get; set; }

	public AppDbContext(IConfiguration configuration)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
		optionsBuilder.UseSqlite(connectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
