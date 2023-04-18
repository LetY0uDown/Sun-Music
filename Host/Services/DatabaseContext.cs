using Microsoft.EntityFrameworkCore;
using Models.Database;

namespace Host.Services;

public sealed class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext (IConfiguration configuration)
    {
        _configuration = configuration;

        Database.EnsureCreated();
    }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MusicTrack> MusicTracks { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
    public DbSet<TrackLike> TrackLikes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) {
            //optionsBuilder.UseMySql(_configuration["ConnectionStrings:Home:MySql"], ServerVersion.AutoDetect(_configuration["ConnectionStrings:Home:MySql"]));
            //optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:Colledge:SqlServer"]);
            optionsBuilder.UseSqlite(_configuration["ConnectionStrings:SqlLite"]);
        }
    }
    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity => {
            entity.ToTable("chats");

            entity.HasIndex(e => e.CreatorID, "IX_Chats_CreatorID");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.CreatorID).HasColumnName("CreatorID");
        });

        modelBuilder.Entity<ChatMember>(entity => {
            entity.ToTable("chatmembers");

            entity.HasIndex(e => e.ChatId, "IX_ChatMembers_ChatID");

            entity.HasIndex(e => e.UserId, "IX_ChatMembers_UserID");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.ChatId).HasColumnName("ChatID");

            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Chat)
                .WithMany(p => p.Chatmembers)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("FK_ChatMembers_Chats_ChatID");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Chatmembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ChatMembers_Users_UserID")
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Message>(entity => {
            entity.ToTable("messages");

            entity.HasIndex(e => e.ChatID, "IX_Messages_ChatID");

            entity.HasIndex(e => e.SenderID, "IX_Messages_SenderID");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.ChatID).HasColumnName("ChatID");

            entity.Property(e => e.SenderID).HasColumnName("SenderID");

            entity.Property(e => e.TimeSended).HasColumnName("TimeSended").HasColumnType("datetime");

            entity.HasOne(d => d.Chat)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatID)
                .HasConstraintName("FK_Messages_Chats_ChatID");

            entity.HasOne(d => d.Sender)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderID)
                .HasConstraintName("FK_Messages_Users_SenderID")
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<MusicTrack>(entity => {
            entity.ToTable("musictracks");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.ReleaseDate).HasMaxLength(6);
        });

        modelBuilder.Entity<Playlist>(entity => {
            entity.ToTable("playlists");

            entity.HasIndex(e => e.UserID, "IX_Playlists_UserID");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.UserID).HasColumnName("UserID");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK_Playlists_Users_UserID");
        });

        modelBuilder.Entity<PlaylistTrack>(entity => {
            entity.ToTable("playlisttracks");

            entity.HasIndex(e => e.PlaylistID, "IX_PlaylistTracks_PlaylistID");

            entity.HasIndex(e => e.TrackID, "IX_PlaylistTracks_TrackID");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.PlaylistID).HasColumnName("PlaylistID");

            entity.Property(e => e.TrackID).HasColumnName("TrackID");

            entity.HasOne(d => d.Playlist)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(d => d.PlaylistID)
                .HasConstraintName("FK_PlaylistTracks_Playlists_PlaylistID");

            entity.HasOne(d => d.Track)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(d => d.TrackID)
                .HasConstraintName("FK_PlaylistTracks_MusicTracks_TrackID");
        });

        modelBuilder.Entity<TrackLike>(entity => {
            entity.ToTable("tracklikes");

            entity.HasIndex(e => e.TrackID, "IX_TrackLikes_TrackID");

            entity.HasIndex(e => e.UserID, "IX_TrackLikes_UserID");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.TrackID).HasColumnName("TrackID");

            entity.Property(e => e.UserID).HasColumnName("UserID");

            entity.HasOne(d => d.Track)
                .WithMany(p => p.TrackLikes)
                .HasForeignKey(d => d.TrackID)
                .HasConstraintName("FK_TrackLikes_MusicTracks_TrackID");

            entity.HasOne(d => d.User)
                .WithMany(p => p.TrackLikes)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK_TrackLikes_Users_UserID");
        });

        modelBuilder.Entity<User>(entity => {
            entity.ToTable("users");

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.HasData(new User {
                ID = Guid.Empty.ToString(),
                ImageBytes = Array.Empty<byte>(),
                Username = string.Empty,
                Password = string.Empty
            });
        });
    }
}