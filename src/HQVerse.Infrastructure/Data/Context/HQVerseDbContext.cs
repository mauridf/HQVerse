using HQVerse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HQVerse.Infrastructure.Data.Context;

public class HQVerseDbContext : DbContext
{
    public HQVerseDbContext(DbContextOptions<HQVerseDbContext> options) : base(options) { }

    // DbSets para todas as entidades
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Universe> Universes => Set<Universe>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Creator> Creators => Set<Creator>();
    public DbSet<CreatorRole> CreatorRoles => Set<CreatorRole>();
    public DbSet<ComicSeries> ComicSeries => Set<ComicSeries>();
    public DbSet<ComicIssue> ComicIssues => Set<ComicIssue>();
    public DbSet<StoryArc> StoryArcs => Set<StoryArc>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<ScanGroup> ScanGroups => Set<ScanGroup>();
    public DbSet<ScanLink> ScanLinks => Set<ScanLink>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserCollection> UserCollections => Set<UserCollection>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<ExternalSource> ExternalSources => Set<ExternalSource>();
    public DbSet<ExternalMapping> ExternalMappings => Set<ExternalMapping>();
    public DbSet<Media> Medias => Set<Media>();
    public DbSet<ReadingProgress> ReadingProgresses => Set<ReadingProgress>();

    // Relacionamentos muitos-para-muitos
    public DbSet<CharacterTeam> CharacterTeams => Set<CharacterTeam>();
    public DbSet<IssueCharacter> IssueCharacters => Set<IssueCharacter>();
    public DbSet<IssueTeam> IssueTeams => Set<IssueTeam>();
    public DbSet<IssueCreator> IssueCreators => Set<IssueCreator>();
    public DbSet<StoryArcIssue> StoryArcIssues => Set<StoryArcIssue>();
    public DbSet<CollectionIssue> CollectionIssues => Set<CollectionIssue>();
    public DbSet<ReviewLike> ReviewLikes => Set<ReviewLike>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyConfigurations(modelBuilder);
    }

    private void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        // Publisher
        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
        });

        // Universe
        modelBuilder.Entity<Universe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
        });

        // Character
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.PublisherId);
            entity.HasIndex(e => e.UniverseId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.RealName).HasMaxLength(255);
            entity.Property(e => e.FirstAppearance).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.HasOne(e => e.Publisher).WithMany(p => p.Characters).HasForeignKey(e => e.PublisherId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Universe).WithMany(u => u.Characters).HasForeignKey(e => e.UniverseId).OnDelete(DeleteBehavior.SetNull);
        });

        // Team
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.Publisher).WithMany(p => p.Teams).HasForeignKey(e => e.PublisherId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Universe).WithMany(u => u.Teams).HasForeignKey(e => e.UniverseId).OnDelete(DeleteBehavior.SetNull);
        });

        // Creator
        modelBuilder.Entity<Creator>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
        });

        // CreatorRole
        modelBuilder.Entity<CreatorRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // ComicSeries
        modelBuilder.Entity<ComicSeries>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.PublisherId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
            entity.HasOne(e => e.Publisher).WithMany(p => p.ComicSeries).HasForeignKey(e => e.PublisherId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Universe).WithMany(u => u.ComicSeries).HasForeignKey(e => e.UniverseId).OnDelete(DeleteBehavior.SetNull);
        });

        // ComicIssue
        modelBuilder.Entity<ComicIssue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.SeriesId);
            entity.HasIndex(e => e.IssueNumber);
            entity.HasIndex(e => new { e.SeriesId, e.IssueNumber }).IsUnique();
            entity.Property(e => e.IssueNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.CoverUrl).HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.HasOne(e => e.Series).WithMany(s => s.Issues).HasForeignKey(e => e.SeriesId).OnDelete(DeleteBehavior.Cascade);
        });

        // StoryArc
        modelBuilder.Entity<StoryArc>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ComicVineId).IsUnique().HasFilter("\"ComicVineId\" IS NOT NULL");
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.Publisher).WithMany(p => p.StoryArcs).HasForeignKey(e => e.PublisherId).OnDelete(DeleteBehavior.SetNull);
        });

        // Scan
        modelBuilder.Entity<Scan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IssueId);
            entity.HasIndex(e => e.ScanGroupId);
            entity.Property(e => e.Language).IsRequired().HasMaxLength(10);
            entity.HasOne(e => e.Issue).WithMany(i => i.Scans).HasForeignKey(e => e.IssueId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ScanGroup).WithMany(sg => sg.Scans).HasForeignKey(e => e.ScanGroupId).OnDelete(DeleteBehavior.SetNull);
        });

        // ScanGroup
        modelBuilder.Entity<ScanGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Website).HasMaxLength(500);
        });

        // ScanLink
        modelBuilder.Entity<ScanLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ScanId);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(1000);
            entity.HasOne(e => e.Scan).WithMany(s => s.Links).HasForeignKey(e => e.ScanId).OnDelete(DeleteBehavior.Cascade);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
        });

        // UserCollection
        modelBuilder.Entity<UserCollection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.User).WithMany(u => u.Collections).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IssueId);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.User).WithMany(u => u.Reviews).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Issue).WithMany(i => i.Reviews).HasForeignKey(e => e.IssueId).OnDelete(DeleteBehavior.Cascade);
        });

        // Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReviewId);
            entity.Property(e => e.Content).IsRequired();
            entity.HasOne(e => e.User).WithMany(u => u.Comments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Review).WithMany(r => r.Comments).HasForeignKey(e => e.ReviewId).OnDelete(DeleteBehavior.Cascade);
        });

        // ExternalSource
        modelBuilder.Entity<ExternalSource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BaseUrl).HasMaxLength(500);
        });

        // ExternalMapping
        modelBuilder.Entity<ExternalMapping>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SourceId, e.ExternalId, e.EntityType }).IsUnique();
            entity.HasIndex(e => new { e.InternalId, e.EntityType });
            entity.HasOne(e => e.Source).WithMany(s => s.Mappings).HasForeignKey(e => e.SourceId).OnDelete(DeleteBehavior.Cascade);
        });

        // Media
        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(1000);
        });

        // ReadingProgress
        modelBuilder.Entity<ReadingProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.IssueId }).IsUnique();
            entity.HasOne(e => e.User).WithMany(u => u.ReadingProgresses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Issue).WithMany().HasForeignKey(e => e.IssueId).OnDelete(DeleteBehavior.Cascade);
        });

        // Relacionamentos muitos-para-muitos
        modelBuilder.Entity<CharacterTeam>(entity =>
        {
            entity.HasKey(e => new { e.CharacterId, e.TeamId });
            entity.HasOne(e => e.Character).WithMany(c => c.CharacterTeams).HasForeignKey(e => e.CharacterId);
            entity.HasOne(e => e.Team).WithMany(t => t.CharacterTeams).HasForeignKey(e => e.TeamId);
        });

        modelBuilder.Entity<IssueCharacter>(entity =>
        {
            entity.HasKey(e => new { e.IssueId, e.CharacterId });
            entity.HasOne(e => e.Issue).WithMany(i => i.IssueCharacters).HasForeignKey(e => e.IssueId);
            entity.HasOne(e => e.Character).WithMany(c => c.IssueCharacters).HasForeignKey(e => e.CharacterId);
        });

        modelBuilder.Entity<IssueTeam>(entity =>
        {
            entity.HasKey(e => new { e.IssueId, e.TeamId });
            entity.HasOne(e => e.Issue).WithMany(i => i.IssueTeams).HasForeignKey(e => e.IssueId);
            entity.HasOne(e => e.Team).WithMany(t => t.IssueTeams).HasForeignKey(e => e.TeamId);
        });

        modelBuilder.Entity<IssueCreator>(entity =>
        {
            entity.HasKey(e => new { e.IssueId, e.CreatorId, e.CreatorRoleId });
            entity.HasOne(e => e.Issue).WithMany(i => i.IssueCreators).HasForeignKey(e => e.IssueId);
            entity.HasOne(e => e.Creator).WithMany(c => c.IssueCreators).HasForeignKey(e => e.CreatorId);
            entity.HasOne(e => e.CreatorRole).WithMany(cr => cr.IssueCreators).HasForeignKey(e => e.CreatorRoleId);
        });

        modelBuilder.Entity<StoryArcIssue>(entity =>
        {
            entity.HasKey(e => new { e.StoryArcId, e.IssueId });
            entity.HasOne(e => e.StoryArc).WithMany(sa => sa.StoryArcIssues).HasForeignKey(e => e.StoryArcId);
            entity.HasOne(e => e.Issue).WithMany(i => i.StoryArcIssues).HasForeignKey(e => e.IssueId);
        });

        modelBuilder.Entity<CollectionIssue>(entity =>
        {
            entity.HasKey(e => new { e.CollectionId, e.IssueId });
            entity.HasIndex(e => e.CollectionId);
            entity.HasIndex(e => e.IssueId);
            entity.HasOne(e => e.Collection).WithMany(c => c.CollectionIssues).HasForeignKey(e => e.CollectionId);
            entity.HasOne(e => e.Issue).WithMany(i => i.CollectionIssues).HasForeignKey(e => e.IssueId);
        });

        modelBuilder.Entity<ReviewLike>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ReviewId });
            entity.HasOne(e => e.User).WithMany(u => u.ReviewLikes).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Review).WithMany(r => r.Likes).HasForeignKey(e => e.ReviewId);
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.EntityType, e.EntityId });
            entity.HasIndex(e => e.UserId);
            entity.HasOne(e => e.User).WithMany(u => u.Favorites).HasForeignKey(e => e.UserId);
        });
    }
}