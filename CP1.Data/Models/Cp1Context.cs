using Microsoft.EntityFrameworkCore;

namespace CP1.Data.Models;

public partial class Cp1Context : DbContext
{
    public Cp1Context(DbContextOptions<Cp1Context> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Description).HasColumnType("nvarchar(max)");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DueDate).HasColumnType("datetime");
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.Property(x => x.Approved).HasColumnType("bit");
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.Username).HasMaxLength(50).IsRequired();
            e.Property(x => x.Email).HasMaxLength(255).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(100);
            e.Property(x => x.Password).HasMaxLength(255).IsRequired();
            e.Property(x => x.CreatedAt).HasColumnType("datetime");
            e.Property(x => x.LastLogin).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.RoleId);
            e.Property(x => x.RoleName).HasMaxLength(50).IsRequired();
            e.Property(x => x.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles");
            e.HasKey(x => new { x.UserId, x.RoleId });
            e.Property(x => x.Description).HasMaxLength(100);

            e.HasOne(x => x.User)
             .WithMany(u => u.UserRoles)
             .HasForeignKey(x => x.UserId);

            e.HasOne(x => x.Role)
             .WithMany(r => r.UserRoles)
             .HasForeignKey(x => x.RoleId);
        });
    }
}