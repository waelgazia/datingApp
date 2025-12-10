using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> memberBuilder)
    {
        memberBuilder.ToTable("Members");

        memberBuilder.HasKey(member => member.Id);
        memberBuilder.Property(member => member.Id)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_ASP_IDENTITY_ID_LENGTH)
            .ValueGeneratedNever();

        memberBuilder.Property(user => user.DisplayName)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_NAME_LENGTH)
            .IsRequired();

        memberBuilder.Property(user => user.ImageUrl)
            .HasColumnType(Constants.DEFAULT_STRING_COLUMN_TYPE);

        memberBuilder.Property(user => user.Created)
            .HasColumnType(Constants.DATETIME_COLUMN_TYPE);
        memberBuilder.Property(user => user.LastActive)
            .HasColumnType(Constants.DATETIME_COLUMN_TYPE);

        memberBuilder.Property(user => user.Gender)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_GENDER_LENGTH)
            .IsRequired();

        memberBuilder.Property(user => user.Description)
            .HasColumnType(Constants.MAX_STRING_COLUMN_TYPE);

        memberBuilder.Property(user => user.City)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_NAME_LENGTH)
            .IsRequired();

        memberBuilder.Property(user => user.Country)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_NAME_LENGTH)
            .IsRequired();

        // relationships
        memberBuilder.HasOne(member => member.User)         /* one-to-one */
            .WithOne(user => user.Member)
            .HasForeignKey<Member>(member => member.Id)
            .IsRequired();
    }
}
