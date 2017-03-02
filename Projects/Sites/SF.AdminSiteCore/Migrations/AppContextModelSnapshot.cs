using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SF.AdminSiteCore;
using SF.Data;

namespace SF.AdminSiteCore.Migrations
{
    [DbContext(typeof(AppContext))]
    partial class AppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("SF.Core.ManagedServices.Admin.DataModels.ServiceInstance", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("CreateArguments");

                    b.Property<string>("DeclarationId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Image")
                        .HasMaxLength(100);

                    b.Property<string>("ImplementId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("IsDefaultService");

                    b.Property<byte>("LogicState");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Remarks")
                        .HasMaxLength(200);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("ImplementId");

                    b.HasIndex("DeclarationId", "IsDefaultService");

                    b.ToTable("SysServiceInstance");
                });

            modelBuilder.Entity("SF.Data.IdentGenerator.DataModels.IdentSeed", b =>
                {
                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("NextValue");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Type");

                    b.ToTable("SysIdentSeed");
                });
        }
    }
}
