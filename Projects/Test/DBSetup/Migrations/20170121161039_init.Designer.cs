using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DBSetup;
using SF;
using SF.Data;
using SF.Auth.Users;

namespace DBSetup.Migrations
{
    [DbContext(typeof(AppContext))]
    [Migration("20170121161039_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SF.Auth.Users.EFCore.DataModels.User", b =>
                {
                    b.Property<long>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Email")
                        .HasMaxLength(200);

                    b.Property<string>("Icon")
                        .HasMaxLength(200);

                    b.Property<string>("Image")
                        .HasMaxLength(200);

                    b.Property<long?>("InviterId");

                    b.Property<string>("LastAddress")
                        .HasMaxLength(100);

                    b.Property<int>("LastDeviceType");

                    b.Property<DateTime?>("LastSigninTime");

                    b.Property<DateTime?>("LockoutEndDateUtc");

                    b.Property<string>("NickName")
                        .HasMaxLength(100);

                    b.Property<bool>("NoIdents");

                    b.Property<byte>("ObjectState");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(100);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50);

                    b.Property<string>("SecurityStamp")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Sex");

                    b.Property<int>("SigninCount");

                    b.Property<string>("SignupAddress")
                        .HasMaxLength(100);

                    b.Property<int>("SignupDeviceType");

                    b.Property<string>("SignupIdentProvider")
                        .HasMaxLength(50);

                    b.Property<string>("SignupIdentValue")
                        .HasMaxLength(200);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<string>("UserName")
                        .HasMaxLength(100);

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("LastSigninTime");

                    b.HasIndex("NoIdents");

                    b.HasIndex("SignupIdentProvider");

                    b.HasIndex("SignupIdentValue");

                    b.HasIndex("UserType");

                    b.ToTable("AppAuthUser");
                });

            modelBuilder.Entity("SF.Auth.Users.EFCore.DataModels.UserPhoneNumberIdent", b =>
                {
                    b.Property<string>("Ident")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<DateTime?>("ConfirmedTime");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<long>("UserId");

                    b.HasKey("Ident");

                    b.HasIndex("UserId");

                    b.ToTable("AppAuthUserPhoneNumberIdent");
                });

            modelBuilder.Entity("SF.Data.IdentGenerator.EFCore.DataModels.IdentSeed", b =>
                {
                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("NextValue");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Type");

                    b.ToTable("AppSysIdentSeed");
                });
        }
    }
}
