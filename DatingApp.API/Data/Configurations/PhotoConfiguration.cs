using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Data.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> photoBuilder)
    {
        photoBuilder.ToTable("Photos");

        photoBuilder.HasKey(photo => photo.Id);

        photoBuilder.Property(photo => photo.Url)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .IsRequired();

        photoBuilder.Property(photo => photo.PublicId)
            .HasColumnType(Constants.STRING_COLUMN_TYPE);

        // relationships
        photoBuilder.HasOne(photo => photo.Member)      /* one-to-many (required) */
            .WithMany(member => member.Photos)
            .HasForeignKey(photo => photo.MemberId)
            .IsRequired();

        photoBuilder.HasQueryFilter(p => p.IsApproved);
    }
}
