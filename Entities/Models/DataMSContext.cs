﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Entities.Models
{
    public partial class DataMSContext : DbContext
    {
        public DataMSContext()
        {
        }

        public DataMSContext(DbContextOptions<DataMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AllCode> AllCodes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuPermission> MenuPermissions { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDepart> UserDeparts { get; set; }
        public virtual DbSet<UserPosition> UserPositions { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<VehicleInspection> VehicleInspections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=103.163.216.41;Initial Catalog=Cargill;Persist Security Info=True;User ID=us;Password=us@585668;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllCode>(entity =>
            {
                entity.ToTable("AllCode");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(300);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DepartmentCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentName).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.FullParent)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsReport).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FullParent)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.Link).HasMaxLength(200);

                entity.Property(e => e.MenuCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermission");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Menu");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Permission");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Role");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Avata).HasMaxLength(500);

                entity.Property(e => e.BirthDay).HasColumnType("datetime");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.NickName).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(2500);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ResetPassword)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserDepart>(entity =>
            {
                entity.ToTable("UserDepart");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.JoinDate).HasColumnType("date");

                entity.Property(e => e.LeaveDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserPosition>(entity =>
            {
                entity.ToTable("UserPosition");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_User");
            });

            modelBuilder.Entity<VehicleInspection>(entity =>
            {
                entity.ToTable("VehicleInspection");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerName).HasMaxLength(500);

                entity.Property(e => e.DriverName).HasMaxLength(500);

                entity.Property(e => e.IssueCreateDate).HasColumnType("datetime");

                entity.Property(e => e.IssueUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.LicenseNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RegisterDateOnline).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VehicleLoad).HasMaxLength(50);

                entity.Property(e => e.VehicleNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VehicleTroughTimeComeIn).HasColumnType("datetime");

                entity.Property(e => e.VehicleTroughTimeComeOut).HasColumnType("datetime");

                entity.Property(e => e.VehicleTroughWeight).HasColumnType("decimal(8, 0)");

                entity.Property(e => e.VehicleWeighingTimeComeIn).HasColumnType("datetime");

                entity.Property(e => e.VehicleWeighingTimeComeOut).HasColumnType("datetime");

                entity.Property(e => e.VehicleWeighingTimeComplete).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
