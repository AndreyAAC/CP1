using Microsoft.EntityFrameworkCore;

namespace CP1.Data.Models;

public partial class Cp1Context : DbContext
{
    public Cp1Context(DbContextOptions<Cp1Context> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(x => x.TaskId);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.CreatedDate).HasColumnType("datetime2(0)");
        });
    }
}