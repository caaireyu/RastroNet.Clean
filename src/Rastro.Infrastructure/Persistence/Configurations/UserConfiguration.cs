using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rastro.Domain.Users;

namespace Rastro.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100);
        
        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(u => u.Password)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired(false);
        
        //Indices
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_users_email");
        
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("idx_users_created_at");

        builder.Ignore(x => x.GetDomainEvents());
        
        builder.Property(x => x.Email)
            .HasConversion(
                email => email.ToLowerInvariant(),
                email => email
                );

    }
}