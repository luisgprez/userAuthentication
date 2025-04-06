using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UserAuthentication.Models.DB;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LoginHistory> LoginHistories { get; set; }

    public virtual DbSet<TokenBlacklist> TokenBlacklists { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=DESKTOP-2P21J4T\\SQLEXPRESS;initial catalog=UserAuthentication;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__LoginHis__6845136FDCB7DD63");

            entity.HasOne(d => d.User).WithMany(p => p.LoginHistories).HasConstraintName("FK__LoginHist__UserI__2B3F6F97");
        });

        modelBuilder.Entity<TokenBlacklist>(entity =>
        {
            entity.HasKey(e => e.TokenBlackListId).HasName("PK__TokenBla__DBC6C377ECC25DA1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B7C926380421646D");

            entity.Property(e => e.Locked).HasDefaultValue(false);
            entity.Property(e => e.FailedLoginCount).HasDefaultValue(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
