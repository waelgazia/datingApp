using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using DatingApp.API.Entities;

namespace DatingApp.API.Data;

public class AppDbContext : DbContext
{
	public DbSet<AppUser> Users { get; set; }
	public DbSet<Member> Members { get; set; }
	public DbSet<Photo> Photos { get; set; }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
			v => v.ToUniversalTime(),                         /* runs when writing to the database. */
			v => DateTime.SpecifyKind(v, DateTimeKind.Utc)    /* runs when reading from the database. */
		);

		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			foreach (var property in entityType.GetProperties())
			{
				if (property.ClrType == typeof(DateTime))
				{
					property.SetValueConverter(dateTimeConverter);
				}
			}
		}

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}
