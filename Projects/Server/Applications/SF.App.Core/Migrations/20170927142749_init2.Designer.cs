﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SF.Applications;
using SF.Common.TextMessages.Management;
using SF.Entities;
using SF.Management.MenuServices.Models;
using System;

namespace SF.App.Core.Migrations
{
    [DbContext(typeof(SFDbContext))]
    [Migration("20170927142749_init2")]
    partial class init2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InvitationMemberInvitation", b =>
                {
                    b.Property<long>("Id");

                    b.Property<long>("InvitorId");

                    b.Property<string>("Invitors")
                        .HasMaxLength(100000);

                    b.Property<DateTime>("Time");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.ToTable("InvitationMemberInvitation");
                });

            modelBuilder.Entity("SF.Auth.Identities.Entity.DataModels.Identity", b =>
                {
                    b.Property<long>("Id");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte>("ObjectState");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("ScopeId");

                    b.Property<string>("SecurityStamp")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("SignupExtraArgument")
                        .HasMaxLength(200);

                    b.Property<long>("SignupIdentProviderId");

                    b.Property<string>("SignupIdentValue")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("UpdatedTime");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("SignupIdentProviderId");

                    b.HasIndex("SignupIdentValue");

                    b.ToTable("SysAuthIdentity");
                });

            modelBuilder.Entity("SF.Auth.Identities.Entity.DataModels.IdentityCredential", b =>
                {
                    b.Property<long>("ScopeId");

                    b.Property<long>("ProviderId");

                    b.Property<string>("Credential")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("ConfirmedTime");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<long>("IdentityId");

                    b.Property<string>("UnionIdent")
                        .HasMaxLength(100);

                    b.HasKey("ScopeId", "ProviderId", "Credential");

                    b.HasIndex("IdentityId");

                    b.HasIndex("ScopeId", "ProviderId", "UnionIdent");

                    b.ToTable("SysAuthIdentityCredential");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.Document", b =>
                {
                    b.Property<long>("Id");

                    b.Property<long?>("AuthorId");

                    b.Property<long?>("ContainerId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Ident")
                        .HasMaxLength(50);

                    b.Property<string>("Image")
                        .HasMaxLength(100);

                    b.Property<int>("ItemOrder");

                    b.Property<byte>("LogicState");

                    b.Property<string>("Memo")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<DateTime?>("PublishDate");

                    b.Property<string>("Remarks")
                        .HasMaxLength(100);

                    b.Property<long?>("ScopeId");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Ident");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PublishDate");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.HasIndex("ContainerId", "ItemOrder");

                    b.HasIndex("ContainerId", "PublishDate");

                    b.ToTable("CommonDocument");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.DocumentAuthor", b =>
                {
                    b.Property<long>("Id");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Image")
                        .HasMaxLength(100);

                    b.Property<byte>("LogicState");

                    b.Property<string>("Memo")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<string>("Remarks")
                        .HasMaxLength(100);

                    b.Property<long?>("ScopeId");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.ToTable("CommonDocumentAuthor");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.DocumentCategory", b =>
                {
                    b.Property<long>("Id");

                    b.Property<long?>("ContainerId");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Image")
                        .HasMaxLength(100);

                    b.Property<int>("ItemOrder");

                    b.Property<byte>("LogicState");

                    b.Property<string>("Memo")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<string>("Remarks")
                        .HasMaxLength(100);

                    b.Property<long?>("ScopeId");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.HasIndex("ContainerId", "ItemOrder");

                    b.ToTable("CommonDocumentCategory");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.DocumentTag", b =>
                {
                    b.Property<long>("Id");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<byte>("LogicState");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<long?>("ScopeId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.HasIndex("ScopeId", "Name")
                        .IsUnique()
                        .HasFilter("[ScopeId] IS NOT NULL");

                    b.ToTable("CommonDocumentTag");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.DocumentTagReference", b =>
                {
                    b.Property<long>("DocumentId");

                    b.Property<long>("TagId");

                    b.HasKey("DocumentId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("CommonDocumentTagRef");
                });

            modelBuilder.Entity("SF.Common.TextMessages.Management.DataModels.TextMessageRecord", b =>
                {
                    b.Property<long>("Id");

                    b.Property<string>("Args")
                        .HasMaxLength(1000);

                    b.Property<string>("Body")
                        .HasMaxLength(1000);

                    b.Property<DateTime?>("CompletedTime");

                    b.Property<string>("Error");

                    b.Property<string>("Headers")
                        .HasMaxLength(1000);

                    b.Property<string>("Result");

                    b.Property<long?>("ScopeId");

                    b.Property<string>("Sender")
                        .HasMaxLength(100);

                    b.Property<long>("ServiceId");

                    b.Property<int>("Status");

                    b.Property<string>("Target")
                        .IsRequired();

                    b.Property<DateTime>("Time");

                    b.Property<string>("Title")
                        .HasMaxLength(100);

                    b.Property<string>("TrackEntityId")
                        .HasMaxLength(100);

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ScopeId");

                    b.HasIndex("Time");

                    b.HasIndex("UserId");

                    b.HasIndex("ServiceId", "Time");

                    b.HasIndex("Status", "Time");

                    b.HasIndex("UserId", "Time");

                    b.ToTable("CommonTextMessageRecord");
                });

            modelBuilder.Entity("SF.Core.CallPlans.Storage.DataModels.CallExpired", b =>
                {
                    b.Property<string>("Callable")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200);

                    b.Property<string>("CallArgument")
                        .HasMaxLength(200);

                    b.Property<string>("CallError")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreateTime");

                    b.Property<int>("ExecCount");

                    b.Property<string>("ExecError")
                        .HasMaxLength(200);

                    b.Property<DateTime>("Expired");

                    b.Property<DateTime?>("LastExecTime");

                    b.Property<string>("Title")
                        .HasMaxLength(100);

                    b.HasKey("Callable");

                    b.ToTable("SysCallExpired");
                });

            modelBuilder.Entity("SF.Core.CallPlans.Storage.DataModels.CallInstance", b =>
                {
                    b.Property<string>("Callable")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200);

                    b.Property<string>("CallArgument")
                        .HasMaxLength(200);

                    b.Property<string>("CallError")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CallTime");

                    b.Property<DateTime>("CreateTime");

                    b.Property<int>("DelaySecondsOnError");

                    b.Property<int>("ErrorCount");

                    b.Property<string>("ExecError")
                        .HasMaxLength(200);

                    b.Property<DateTime>("Expire");

                    b.Property<DateTime?>("LastExecTime");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Callable");

                    b.HasIndex("CallTime");

                    b.ToTable("SysCallInstance");
                });

            modelBuilder.Entity("SF.Core.ServiceManagement.Management.DataModels.ServiceInstance", b =>
                {
                    b.Property<long>("Id");

                    b.Property<long?>("ContainerId");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Image")
                        .HasMaxLength(100);

                    b.Property<string>("ImplementType")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<int>("ItemOrder");

                    b.Property<byte>("LogicState");

                    b.Property<string>("Memo")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<string>("Remarks")
                        .HasMaxLength(100);

                    b.Property<long?>("ScopeId");

                    b.Property<string>("ServiceIdent")
                        .HasMaxLength(200);

                    b.Property<string>("ServiceType")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Setting");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("ImplementType");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("ServiceIdent");

                    b.HasIndex("UpdatorId");

                    b.HasIndex("ContainerId", "ItemOrder");

                    b.HasIndex("ContainerId", "ServiceType");

                    b.ToTable("SysServiceInstance");
                });

            modelBuilder.Entity("SF.Data.IdentGenerator.DataModels.IdentSeed", b =>
                {
                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<long>("NextValue");

                    b.Property<int>("Section");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Type");

                    b.ToTable("SysIdentSeed");
                });

            modelBuilder.Entity("SF.Management.BizAdmins.Entity.DataModels.BizAdmin", b =>
                {
                    b.Property<long>("Id");

                    b.Property<string>("Account")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<byte>("LogicState");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<long?>("ScopeId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("Account");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.ToTable("MgrBizAdmin");
                });

            modelBuilder.Entity("SF.Management.FrontEndContents.DataModels.Content", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("Disabled");

                    b.Property<string>("FontIcon")
                        .HasMaxLength(100);

                    b.Property<string>("Icon")
                        .HasMaxLength(200);

                    b.Property<string>("Image")
                        .HasMaxLength(200);

                    b.Property<string>("ItemsData");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("ProviderConfig");

                    b.Property<string>("ProviderType")
                        .HasMaxLength(100);

                    b.Property<string>("Summary");

                    b.Property<string>("Title1")
                        .HasMaxLength(200);

                    b.Property<string>("Title2")
                        .HasMaxLength(200);

                    b.Property<string>("Title3")
                        .HasMaxLength(200);

                    b.Property<string>("Uri")
                        .HasMaxLength(200);

                    b.Property<string>("UriTarget")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("FrontContent");
                });

            modelBuilder.Entity("SF.Management.FrontEndContents.DataModels.Site", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<long>("TemplateId");

                    b.HasKey("Id");

                    b.HasIndex("TemplateId");

                    b.ToTable("FrontSite");
                });

            modelBuilder.Entity("SF.Management.FrontEndContents.DataModels.SiteTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("FrontSiteTemplate");
                });

            modelBuilder.Entity("SF.Management.MenuServices.Entity.DataModels.Menu", b =>
                {
                    b.Property<long>("Id");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Ident")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte>("LogicState");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<long?>("ScopeId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.HasIndex("ScopeId", "Ident");

                    b.ToTable("MgrMenu");
                });

            modelBuilder.Entity("SF.Management.MenuServices.Entity.DataModels.MenuItem", b =>
                {
                    b.Property<long>("Id");

                    b.Property<int>("Action");

                    b.Property<string>("ActionArgument")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("FontIcon")
                        .HasMaxLength(100);

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<string>("Image")
                        .HasMaxLength(100);

                    b.Property<byte>("LogicState");

                    b.Property<string>("Memo")
                        .HasMaxLength(200);

                    b.Property<long>("MenuId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<long?>("ParentId");

                    b.Property<string>("Remarks")
                        .HasMaxLength(100);

                    b.Property<long?>("ScopeId");

                    b.Property<long?>("ServiceId");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("MenuId");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.ToTable("MgrMenuItem");
                });

            modelBuilder.Entity("SF.Management.SysAdmins.Entity.DataModels.SysAdmin", b =>
                {
                    b.Property<long>("Id");

                    b.Property<string>("Account")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("Icon")
                        .HasMaxLength(100);

                    b.Property<byte>("LogicState");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<long?>("ScopeId");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("UpdatedTime");

                    b.Property<long>("UpdatorId");

                    b.HasKey("Id");

                    b.HasIndex("Account");

                    b.HasIndex("CreatedTime");

                    b.HasIndex("Name");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ScopeId");

                    b.HasIndex("UpdatorId");

                    b.ToTable("MgrSysAdmin");
                });

            modelBuilder.Entity("SF.Auth.Identities.Entity.DataModels.IdentityCredential", b =>
                {
                    b.HasOne("SF.Auth.Identities.Entity.DataModels.Identity", "Identity")
                        .WithMany("Credentials")
                        .HasForeignKey("IdentityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.Document", b =>
                {
                    b.HasOne("SF.Common.Documents.DataModels.DocumentAuthor", "Author")
                        .WithMany("Documents")
                        .HasForeignKey("AuthorId");

                    b.HasOne("SF.Common.Documents.DataModels.DocumentCategory", "Container")
                        .WithMany("Items")
                        .HasForeignKey("ContainerId");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.DocumentCategory", b =>
                {
                    b.HasOne("SF.Common.Documents.DataModels.DocumentCategory", "Container")
                        .WithMany("Children")
                        .HasForeignKey("ContainerId");
                });

            modelBuilder.Entity("SF.Common.Documents.DataModels.DocumentTagReference", b =>
                {
                    b.HasOne("SF.Common.Documents.DataModels.Document", "Document")
                        .WithMany("Tags")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SF.Common.Documents.DataModels.DocumentTag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SF.Core.ServiceManagement.Management.DataModels.ServiceInstance", b =>
                {
                    b.HasOne("SF.Core.ServiceManagement.Management.DataModels.ServiceInstance", "Container")
                        .WithMany("Children")
                        .HasForeignKey("ContainerId");
                });

            modelBuilder.Entity("SF.Management.FrontEndContents.DataModels.Site", b =>
                {
                    b.HasOne("SF.Management.FrontEndContents.DataModels.SiteTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SF.Management.MenuServices.Entity.DataModels.MenuItem", b =>
                {
                    b.HasOne("SF.Management.MenuServices.Entity.DataModels.Menu", "Menu")
                        .WithMany("Items")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SF.Management.MenuServices.Entity.DataModels.MenuItem", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}
