using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Data.Configurations;

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> connectionBuilder)
    {
        connectionBuilder.ToTable("Connections");

        connectionBuilder.HasKey(c => c.ConnectionId);
        connectionBuilder.Property(c => c.ConnectionId)
            .HasColumnType(Constants.DEFAULT_STRING_COLUMN_TYPE)
            .ValueGeneratedNever();

        connectionBuilder.Property(c => c.UserId)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_GUID_LENGTH);

        connectionBuilder.HasOne(c => c.Group)
            .WithMany(g => g.Connections)
            .HasForeignKey(c => c.GroupId)
            .IsRequired();
    }
}
