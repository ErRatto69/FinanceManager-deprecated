using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace FinanceApp.Models;

public partial class HomeFinanceContext : DbContext
{
    public HomeFinanceContext()
    {
    }

    public HomeFinanceContext(DbContextOptions<HomeFinanceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<GroceryListItem> GroceryListItems { get; set; }

    public virtual DbSet<GroceryProduct> GroceryProducts { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'User'")
                .HasColumnType("enum('Admin','User')");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ColorHex)
                .HasMaxLength(7)
                .HasDefaultValueSql("'#3498db'");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasDefaultValueSql("'fa-solid fa-tag'");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<GroceryListItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.AddedByUserId, "AddedByUserId");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddedByUserId).HasColumnType("int(11)");
            entity.Property(e => e.IsChecked).HasDefaultValueSql("'0'");
            entity.Property(e => e.ProductId).HasColumnType("int(11)");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.AddedByUser).WithMany(p => p.GroceryListItems)
                .HasForeignKey(d => d.AddedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("GroceryListItems_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.GroceryListItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("GroceryListItems_ibfk_1");
        });

        modelBuilder.Entity<GroceryProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DefaultPrice)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CategoryId).HasColumnType("int(11)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'CHF'");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RenewalDay).HasColumnType("int(11)");

            entity.HasOne(d => d.Category).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Subscriptions_ibfk_1");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.AmountInChf)
                .HasPrecision(10, 2)
                .HasComputedColumnSql("`Amount` * `ExchangeRate`", true)
                .HasColumnName("AmountInCHF");
            entity.Property(e => e.CategoryId).HasColumnType("int(11)");
            entity.Property(e => e.CreatedByUserId).HasColumnType("int(11)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'CHF'");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ExchangeRate)
                .HasPrecision(10, 4)
                .HasDefaultValueSql("'1.0000'");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transactions_ibfk_1");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transactions_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
