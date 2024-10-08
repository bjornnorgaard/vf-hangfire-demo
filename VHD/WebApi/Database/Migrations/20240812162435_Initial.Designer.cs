﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApi.Database;

#nullable disable

namespace WebApi.Database.Migrations
{
    [DbContext(typeof(WindContext))]
    [Migration("20240812162435_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebApi.Database.Models.Turbine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Disabled")
                        .HasColumnType("boolean");

                    b.Property<double>("Efficiency")
                        .HasColumnType("double precision");

                    b.Property<string>("Park")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PowerKiloWatts")
                        .HasColumnType("integer");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UptimeSeconds")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Turbines");
                });
#pragma warning restore 612, 618
        }
    }
}
