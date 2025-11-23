using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> messageBuilder)
    {
        messageBuilder.ToTable("Messages");

        messageBuilder.HasKey(message => message.Id);
        messageBuilder.Property(message => message.Id)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_GUID_LENGTH)
            .ValueGeneratedNever();

        messageBuilder.Property(message => message.Content)
            .HasColumnType(Constants.STRING_COLUMN_TYPE)
            .HasMaxLength(Rules.MAX_MESSAGE_LENGTH)
            .IsRequired();

        messageBuilder.Property(message => message.ReadAt)
            .HasColumnType(Constants.DATETIME_COLUMN_TYPE)
            .IsRequired(false);
        messageBuilder.Property(message => message.SentAt)
            .HasColumnType(Constants.DATETIME_COLUMN_TYPE)
            .IsRequired();

        // relationships

        // one-to-many (sender -> messages)
        messageBuilder.HasOne(message => message.Sender)
            .WithMany(sender => sender.MessageSent)
            .HasForeignKey(message => message.SenderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);          /* prevent deleting messages if the member is deleted (business logic) */

        // one-to-many (recipient -> messages)
        messageBuilder.HasOne(message => message.Recipient)
            .WithMany(recipient => recipient.MessageReceived)
            .HasForeignKey(message => message.RecipientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);          /* prevent deleting messages if the member is deleted (business logic) */
    }
}
