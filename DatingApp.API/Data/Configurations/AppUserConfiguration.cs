using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> userBuilder)
    {
        userBuilder.ToTable("Users");

        userBuilder.HasKey(user => user.Id);
        userBuilder.Property(user => user.Id)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_GUID_LENGTH)
            .ValueGeneratedNever();

        userBuilder.Property(user => user.DisplayName)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_NAME_LENGTH)
            .IsRequired();

        userBuilder.Property(user => user.Email)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_EMAIL_LENGTH)
            .IsRequired();
        userBuilder.HasIndex(user => user.Email).IsUnique();

    }
}
