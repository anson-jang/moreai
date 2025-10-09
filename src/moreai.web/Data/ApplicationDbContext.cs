using Microsoft.EntityFrameworkCore;
using moreai.web.Models;

namespace moreai.web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<MemberGroup> MemberGroups { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Username)
            .IsUnique();
            
        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Email)
            .IsUnique();

        modelBuilder.Entity<MemberGroup>()
            .HasKey(mg => new { mg.MemberId, mg.GroupId });

        modelBuilder.Entity<MemberGroup>()
            .HasOne(mg => mg.Member)
            .WithMany(m => m.Groups)
            .HasForeignKey(mg => mg.MemberId);

        modelBuilder.Entity<MemberGroup>()
            .HasOne(mg => mg.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(mg => mg.GroupId);
            
        base.OnModelCreating(modelBuilder);
    }
}