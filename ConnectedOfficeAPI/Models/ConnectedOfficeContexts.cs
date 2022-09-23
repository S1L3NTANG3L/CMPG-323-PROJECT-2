using Microsoft.EntityFrameworkCore;

namespace ConnectedOfficeAPI.Models
{
    public partial class ConnectedOfficeContexts : DbContext
    {
        public ConnectedOfficeContexts()
        {
        }

        public ConnectedOfficeContexts(DbContextOptions<ConnectedOfficeContexts> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Device> Devices { get; set; } = null!;
        public virtual DbSet<Zone> Zones { get; set; } = null!;
        public virtual DbSet<User> User { get; set; } = null!;
        public virtual DbSet<Role> Role { get; set; } = null!;
        public virtual DbSet<Refreshtoken> Refreshtoken { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("CategoryID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.DeviceId)
                    .ValueGeneratedNever()
                    .HasColumnName("DeviceID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            });
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("Zone");

                entity.Property(e => e.ZoneId)
                    .ValueGeneratedNever()
                    .HasColumnName("ZoneID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("Role");

                entity.Property(e => e.RoleId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("RoleId");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("User");
                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UserId");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Password");

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Refreshtoken>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.ToTable("RefreshToken");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UserId");

                entity.Property(e => e.TokenId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TokenId");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("RefreshToken");

                entity.Property(e => e.IsActive)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IsActive");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
