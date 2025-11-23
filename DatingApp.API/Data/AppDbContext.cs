using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using DatingApp.API.Entities;

namespace DatingApp.API.Data;

public class AppDbContext : DbContext
{
	public DbSet<AppUser> Users { get; set; }
	public DbSet<Member> Members { get; set; }
	public DbSet<Photo> Photos { get; set; }
	public DbSet<MemberLike> MemberLikes { get; set; }
	public DbSet<Message> Messages { get; set; }

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
		var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
			v => v.HasValue ? v.Value.ToUniversalTime() : null,                         /* runs when writing to the database. */
			v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null    /* runs when reading from the database. */
		);

		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			foreach (var property in entityType.GetProperties())
			{
				if (property.ClrType == typeof(DateTime))
				{
					property.SetValueConverter(dateTimeConverter);
				}
				else if (property.ClrType == typeof(DateTime?))
				{
					property.SetValueConverter(nullableDateTimeConverter);
				}
			}
		}

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}
