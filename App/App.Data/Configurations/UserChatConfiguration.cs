using App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.Configurations
{
    public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
    {
        public void Configure(EntityTypeBuilder<UserChat> builder)
        {
            builder.ToTable("UserChats");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserChatType)
                .IsRequired(true);

            builder.HasOne(uc => uc.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uc => uc.AppUser)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uc => uc.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
