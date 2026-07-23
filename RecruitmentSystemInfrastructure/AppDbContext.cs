using Microsoft.EntityFrameworkCore;
using RecruitmentSystemDomain.Models;
namespace RecruitmentSystemInfrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<CandidateProject> CandidateProjects => Set<CandidateProject>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
        public DbSet<AttributeOption> AttributeOptions => Set<AttributeOption>();
        public DbSet<CandidateAttributeValue> CandidateAttributeValues => Set<CandidateAttributeValue>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<PositionAttribute> PositionAttributes => Set<PositionAttribute>();
        public DbSet<PositionAccessRule> PositionAccessRules => Set<PositionAccessRule>();
        public DbSet<CV> CVs => Set<CV>();
        public DbSet<DiscussionPost> DiscussionPosts => Set<DiscussionPost>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<AttributeCategory> AttributeCategories => Set<AttributeCategory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("ix_user_email");

                entity.Property(u => u.UserRole)
                    .IsRequired()
                    .HasConversion<string>();

                entity.HasOne(u => u.UserProfile)
                    .WithOne()
                    .HasForeignKey<UserProfile>(p => p.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.CVs)
                    .WithOne()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Likes)
                    .WithOne()
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Version)
                    .IsConcurrencyToken();

                entity.HasMany(p => p.CandidateProjects)
                    .WithOne(cp => cp.UserProfile)
                    .HasForeignKey("UserProfileId")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.UserId).IsUnique();

                entity.HasMany(p => p.CandidateAttributeValues)
                    .WithOne(v => v.UserProfile) 
                    .HasForeignKey(v => v.UserProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeCategory>().HasData(
                    new AttributeCategory { Id = Guid.Parse("4259a6c1-2112-432f-8499-5a52687b0ff9"), Name = "Certification" },
                    new AttributeCategory { Id = Guid.Parse("2f6e4b7d-5260-447a-a78a-7b3401ffbc9d"), Name = "Domain Knowledge" },
                    new AttributeCategory { Id = Guid.Parse("06e94f80-f673-4a97-986f-36278ac51ac8"), Name = "Personal Information" },
                    new AttributeCategory { Id = Guid.Parse("cb188329-0a8a-4def-aa72-23847feeb4cd"), Name = "Soft Skills" } //probably I should add more here 
                );


            modelBuilder.Entity<CandidateProject>(entity =>
            {
                entity.HasKey(cp => cp.Id);

                entity.Property(cp => cp.Description)
                    .HasColumnType("text"); 

                entity.HasMany(cp => cp.Tags)
                    .WithMany()
                    .UsingEntity(j => j.ToTable("CandidateProjectTags"));
            });


            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasIndex(t => t.Name)
                    .IsUnique()
                    .HasDatabaseName("ix_tag_name");
            });


            modelBuilder.Entity<AttributeDefinition>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(a => a.Name)
                    .IsUnique()
                    .HasDatabaseName("ix_attribute_name");

                entity.HasOne(a => a.AttributeCategory)
                    .WithMany(a => a.AttributeDefinitions)
                    .HasForeignKey(a => a.AttributeCategoryId);

                entity.Property(a => a.DataType)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(32);

                entity.Property(a => a.Version)
                    .IsConcurrencyToken();

                entity.HasMany(a => a.AttributeOptions)
                    .WithOne()
                    .HasForeignKey(o => o.AttributeDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<AttributeOption>(entity =>
            {
                entity.HasKey(o => o.Id);
            });

            modelBuilder.Entity<CandidateAttributeValue>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Value)
                    .HasColumnType("text");

                entity.Property(v => v.Version)
                    .IsConcurrencyToken();

                entity.HasIndex(v => new { v.UserProfileId, v.AttributeId })
                    .IsUnique();

                entity.HasOne<AttributeDefinition>()
                    .WithMany()
                    .HasForeignKey(v => v.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Description)
                    .HasColumnType("text");

                entity.Property(p => p.Version)
                    .IsConcurrencyToken();

                entity.HasIndex(p => p.CreatedAt); 
                entity.HasIndex(p => p.IsPublic);

                entity.HasMany(p => p.CVs)
                    .WithOne()
                    .HasForeignKey(c => c.PositionId)
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.HasMany(p => p.PositionAttributes)
                    .WithOne()
                    .HasForeignKey(pa => pa.PositionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(a => a.AccessRules)
                    .WithOne()
                    .HasForeignKey(a => a.PositionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.DiscussionPosts)
                    .WithOne(dp => dp.Position)
                    .HasForeignKey(dp => dp.PositionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Tags)
                    .WithMany()
                    .UsingEntity(j => j.ToTable("PositionTags"));
            });


            modelBuilder.Entity<PositionAttribute>(entity =>
            {
                entity.HasKey(pa => pa.Id);

                entity.HasIndex(pa => new { pa.PositionId, pa.AttributeDefinitionId })
                    .IsUnique();

                entity.HasOne<AttributeDefinition>()
                    .WithMany()
                    .HasForeignKey(pa => pa.AttributeDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PositionAccessRule>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Operator)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(r => r.Value)
                    .IsRequired();

                entity.Property(r => r.SecondValue);

                entity.HasOne<AttributeDefinition>()
                    .WithMany()
                    .HasForeignKey(r => r.AttributeDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<CV>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.CVState)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(32);

                entity.Property(c => c.Version)
                    .IsConcurrencyToken();

                entity.HasIndex(c => new { c.UserId, c.PositionId })
                    .IsUnique();

                entity.HasIndex(c => c.CVState);
            });

            modelBuilder.Entity<DiscussionPost>(entity =>
            {
                entity.HasKey(dp => dp.Id);

                entity.Property(dp => dp.Content)
                    .HasColumnType("text");

                entity.Property(dp => dp.CreatedAt)
                    .IsRequired();

                entity.HasOne(dp => dp.Author)
                    .WithMany()
                    .HasForeignKey(dp => dp.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.HasIndex(dp => new { dp.PositionId, dp.CreatedAt });
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.HasIndex(l => new { l.UserId, l.CVId })
                    .IsUnique();

                entity.HasOne<CV>()
                    .WithMany()
                    .HasForeignKey(l => l.CVId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
 