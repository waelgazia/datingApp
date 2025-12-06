using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Entities;
using DatingApp.API.Globals;

namespace DatingApp.API.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> groupBuilder)
    {
        groupBuilder.ToTable("Groups");

        groupBuilder.HasKey(g => g.Name);
        groupBuilder.Property(g => g.Name)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .ValueGeneratedNever();
    }
}