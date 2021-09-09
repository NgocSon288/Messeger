using App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    { 
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("Chats");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.GroupName)
                .IsRequired(true)
                .HasMaxLength(200);

            builder.Property(x => x.ChatType)
                .IsRequired(true);
        }
    }
}
