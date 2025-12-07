using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Entities;

namespace DatingApp.API.Data.Configurations;

public class MemberLikeConfiguration : IEntityTypeConfiguration<MemberLike>
{
    public void Configure(EntityTypeBuilder<MemberLike> memberLikeBuilder)
    {
        memberLikeBuilder.ToTable("Likes");
        memberLikeBuilder
            .HasKey(memberLike => new { memberLike.SourceMemberId, memberLike.TargetMemberId });

        // many-to-many relationships
        memberLikeBuilder
            .HasOne(memberLike => memberLike.SourceMember)
            .WithMany(member => member.LikedMembers)
            .HasForeignKey(memberLike => memberLike.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        memberLikeBuilder
            .HasOne(memberLike => memberLike.TargetMember)
            .WithMany(member => member.LikedByMembers)
            .HasForeignKey(memberLike => memberLike.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
