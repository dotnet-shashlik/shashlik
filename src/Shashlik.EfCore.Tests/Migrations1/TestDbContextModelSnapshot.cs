﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shashlik.EfCore.Tests.Migrations1
{
    [DbContext(typeof(TestDbContext1))]
    partial class TestDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Shashlik.EfCore.Tests.Entities.Roles", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                    .HasMaxLength(255);

                b.Property<int>("UserId")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("Roles");
            });

            modelBuilder.Entity("Shashlik.EfCore.Tests.Entities.Users", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                b.Property<DateTime?>("DeleteTime")
                    .HasColumnType("datetime(6)");

                b.Property<bool>("IsDeleted")
                    .HasColumnType("tinyint(1)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                    .HasMaxLength(255);

                b.HasKey("Id");

                b.ToTable("Users");
            });

            modelBuilder.Entity("Shashlik.EfCore.Tests.Entities.Roles", b =>
            {
                b.HasOne("Shashlik.EfCore.Tests.Entities.Users", "User")
                    .WithMany("Roles")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
        }
    }
}