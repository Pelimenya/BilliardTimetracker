using System;
using System.Collections.Generic;
using BilliardTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BilliardTimeTracker.Context;

public partial class ContextDB : DbContext
{
    public ContextDB()
    {
    }

    public ContextDB(DbContextOptions<ContextDB> options)
        : base(options)
    {
    }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableRate> TableRates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("host=localhost; port=5432; database=BilliardTimetrackerDB; username=Pelimenya; password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rate>(entity =>
        {
            entity.HasKey(e => e.RateId).HasName("Rates_pkey");

            entity.Property(e => e.RateId).HasColumnName("RateID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.RateName).HasMaxLength(50);
            entity.Property(e => e.RatePerHour).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("Sessions_pkey");

            entity.Property(e => e.SessionId).HasColumnName("SessionID");
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.EndTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.StartTime).HasColumnType("timestamp without time zone");
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Table).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Sessions_TableID_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Sessions_UserID_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("Tables_pkey");

            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.TableName).HasMaxLength(50);
        });

        modelBuilder.Entity<TableRate>(entity =>
        {
            entity.HasKey(e => e.TableRateId).HasName("TableRates_pkey");

            entity.Property(e => e.TableRateId).HasColumnName("TableRateID");
            entity.Property(e => e.RateId).HasColumnName("RateID");
            entity.Property(e => e.TableId).HasColumnName("TableID");

            entity.HasOne(d => d.Rate).WithMany(p => p.TableRates)
                .HasForeignKey(d => d.RateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TableRates_RateID_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.TableRates)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TableRates_TableID_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Users_pkey");

            entity.HasIndex(e => e.Username, "Users_Username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'User'::character varying");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
