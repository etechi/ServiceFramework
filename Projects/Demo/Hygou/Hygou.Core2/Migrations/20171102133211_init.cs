using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Hygou.Core2.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BizProductCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    BannerImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    MobileBannerImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MobileBannerUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Tag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductCategory_BizProductCategory_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BizProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentAuthor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentAuthor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ContainerId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonDocumentCategory_CommonDocumentCategory_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "CommonDocumentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentTag",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonTextMessageRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Args = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Headers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrackEntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonTextMessageRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FrontContent",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    FontIcon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemsData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProviderConfig = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UriTarget = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FrontSiteTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontSiteTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberInvitation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    InvitorId = table.Column<long>(type: "bigint", nullable: false),
                    Invitors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInvitation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MgrAdmin",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MgrAdmin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MgrMenu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ident = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MgrMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthClaimType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthClaimType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthClientConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    AbsoluteRefreshTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    AccessTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    AllowOfflineAccess = table.Column<bool>(type: "bit", nullable: false),
                    AllowRememberConsent = table.Column<bool>(type: "bit", nullable: false),
                    AllowedCorsOrigins = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AllowedGrantTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlwaysIncludeUserClaimsInIdToken = table.Column<bool>(type: "bit", nullable: false),
                    AuthorizationCodeLifetime = table.Column<int>(type: "int", nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(type: "bit", nullable: false),
                    ClientClaimsPrefix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConsentLifetime = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(type: "bit", nullable: false),
                    IdentityTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    RequireClientSecret = table.Column<bool>(type: "bit", nullable: false),
                    RequireConsent = table.Column<bool>(type: "bit", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SlidingRefreshTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthClientConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthOperation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthOperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthResource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysCallExpired",
                columns: table => new
                {
                    Callable = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CallArgument = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CallError = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecCount = table.Column<int>(type: "int", nullable: false),
                    ExecError = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Expired = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastExecTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysCallExpired", x => x.Callable);
                });

            migrationBuilder.CreateTable(
                name: "SysCallInstance",
                columns: table => new
                {
                    Callable = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CallArgument = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CallError = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CallTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DelaySecondsOnError = table.Column<int>(type: "int", nullable: false),
                    ErrorCount = table.Column<int>(type: "int", nullable: false),
                    ExecError = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Expire = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastExecTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysCallInstance", x => x.Callable);
                });

            migrationBuilder.CreateTable(
                name: "SysIdentSeed",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NextValue = table.Column<long>(type: "bigint", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysIdentSeed", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "SysServiceInstance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ContainerId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImplementId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ImplementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ServiceIdent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Setting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysServiceInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysServiceInstance_SysServiceInstance_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "SysServiceInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProduct",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CouponDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    MarketPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<double>(type: "float", nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PublishedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SellCount = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VIADSpecId = table.Column<long>(type: "bigint", nullable: true),
                    Visited = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProduct_BizProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BizProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductPropertyScope",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductPropertyScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductPropertyScope_BizProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BizProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocument",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: true),
                    ContainerId = table.Column<long>(type: "bigint", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ident = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonDocument_CommonDocumentAuthor_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "CommonDocumentAuthor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommonDocument_CommonDocumentCategory_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "CommonDocumentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FrontSite",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TemplateId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontSite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrontSite_FrontSiteTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "FrontSiteTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MgrMenuItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    ActionArgument = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FontIcon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MenuId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MgrMenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MgrMenuItem_MgrMenu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "MgrMenu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MgrMenuItem_MgrMenuItem_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MgrMenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthClient",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClientConfigId = table.Column<long>(type: "bigint", nullable: false),
                    ClientSecrets = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ClientUri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FrontChannelLogoutUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    PostLogoutRedirectUris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectUris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SubTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysAuthClient_SysAuthClientConfig_ClientConfigId",
                        column: x => x.ClientConfigId,
                        principalTable: "SysAuthClientConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthClientGrant",
                columns: table => new
                {
                    ClientConfigId = table.Column<long>(type: "bigint", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthClientGrant", x => new { x.ClientConfigId, x.ResourceId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_SysAuthClientGrant_SysAuthClientConfig_ClientConfigId",
                        column: x => x.ClientConfigId,
                        principalTable: "SysAuthClientConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthClientGrant_SysAuthOperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "SysAuthOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthClientGrant_SysAuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "SysAuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthOperationRequiredClaim",
                columns: table => new
                {
                    OperationId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ClaimTypeId = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthOperationRequiredClaim", x => new { x.OperationId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_SysAuthOperationRequiredClaim_SysAuthClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "SysAuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthOperationRequiredClaim_SysAuthResource_OperationId",
                        column: x => x.OperationId,
                        principalTable: "SysAuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthResourceRequiredClaim",
                columns: table => new
                {
                    ResourceId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ClaimTypeId = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthResourceRequiredClaim", x => new { x.ResourceId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_SysAuthResourceRequiredClaim_SysAuthClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "SysAuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthResourceRequiredClaim_SysAuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "SysAuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthResourceSupportedOperation",
                columns: table => new
                {
                    ResourceId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    OperationId = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthResourceSupportedOperation", x => new { x.ResourceId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_SysAuthResourceSupportedOperation_SysAuthOperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "SysAuthOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthResourceSupportedOperation_SysAuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "SysAuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthRoleClaimValue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TypeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthRoleClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysAuthRoleClaimValue_SysAuthRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SysAuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthRoleClaimValue_SysAuthClaimType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SysAuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthRoleGrant",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthRoleGrant", x => new { x.RoleId, x.ResourceId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_SysAuthRoleGrant_SysAuthOperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "SysAuthOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthRoleGrant_SysAuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "SysAuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthRoleGrant_SysAuthRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SysAuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductDetail_BizProduct_Id",
                        column: x => x.Id,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CategoryTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    SellerId = table.Column<long>(type: "bigint", nullable: false),
                    SourceItemId = table.Column<long>(type: "bigint", nullable: true),
                    SourceLevel = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductItem_BizProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductItem_BizProductItem_SourceItemId",
                        column: x => x.SourceItemId,
                        principalTable: "BizProductItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductSpec",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VIADSpecId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductSpec", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductSpec_BizProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductProperty",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductProperty_BizProductProperty_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BizProductProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductProperty_BizProductPropertyScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "BizProductPropertyScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductProperty_BizProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BizProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentTagRef",
                columns: table => new
                {
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentTagRef", x => new { x.DocumentId, x.TagId });
                    table.ForeignKey(
                        name: "FK_CommonDocumentTagRef_CommonDocument_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "CommonDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommonDocumentTagRef_CommonDocumentTag_TagId",
                        column: x => x.TagId,
                        principalTable: "CommonDocumentTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthClientClaimValue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthClientClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysAuthClientClaimValue_SysAuthClient_ClientId",
                        column: x => x.ClientId,
                        principalTable: "SysAuthClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthClientClaimValue_SysAuthClaimType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SysAuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MainClaimTypeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MainCredential = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SignupClientId = table.Column<long>(type: "bigint", nullable: true),
                    SignupExtraArgument = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysAuthUser_SysAuthClient_SignupClientId",
                        column: x => x.SignupClientId,
                        principalTable: "SysAuthClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductCategoryItem",
                columns: table => new
                {
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductCategoryItem", x => new { x.CategoryId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_BizProductCategoryItem_BizProductCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BizProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductCategoryItem_BizProductItem_ItemId",
                        column: x => x.ItemId,
                        principalTable: "BizProductItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductPropertyItem",
                columns: table => new
                {
                    PropertyId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductPropertyItem", x => new { x.PropertyId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_BizProductPropertyItem_BizProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductPropertyItem_BizProductProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "BizProductProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthUserClaimValue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthUserClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysAuthUserClaimValue_SysAuthClaimType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SysAuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthUserClaimValue_SysAuthUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysAuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthUserCredential",
                columns: table => new
                {
                    ClaimTypeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Credential = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfirmedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthUserCredential", x => new { x.ClaimTypeId, x.Credential });
                    table.ForeignKey(
                        name: "FK_SysAuthUserCredential_SysAuthClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "SysAuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthUserCredential_SysAuthUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysAuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthUserRole",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_SysAuthUserRole_SysAuthRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SysAuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SysAuthUserRole_SysAuthUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysAuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_OwnerUserId",
                table: "BizProduct",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_TypeId",
                table: "BizProduct",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_Order",
                table: "BizProduct",
                columns: new[] { "ObjectState", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_Price",
                table: "BizProduct",
                columns: new[] { "ObjectState", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_PublishedTime",
                table: "BizProduct",
                columns: new[] { "ObjectState", "PublishedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_SellCount",
                table: "BizProduct",
                columns: new[] { "ObjectState", "SellCount" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_Visited",
                table: "BizProduct",
                columns: new[] { "ObjectState", "Visited" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_Order",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_Price",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_PublishedTime",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "PublishedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_SellCount",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "SellCount" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_Visited",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "Visited" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategory_OwnerUserId",
                table: "BizProductCategory",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategory_ParentId",
                table: "BizProductCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategory_Tag",
                table: "BizProductCategory",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategoryItem_ItemId",
                table: "BizProductCategoryItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategoryItem_CategoryId_Order",
                table: "BizProductCategoryItem",
                columns: new[] { "CategoryId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductItem_ProductId",
                table: "BizProductItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductItem_SellerId",
                table: "BizProductItem",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductItem_SourceItemId",
                table: "BizProductItem",
                column: "SourceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductProperty_ScopeId",
                table: "BizProductProperty",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductProperty_ParentId_Order",
                table: "BizProductProperty",
                columns: new[] { "ParentId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductProperty_TypeId_ParentId_Name",
                table: "BizProductProperty",
                columns: new[] { "TypeId", "ParentId", "Name" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyItem_ProductId",
                table: "BizProductPropertyItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyItem_PropertyId_Order",
                table: "BizProductPropertyItem",
                columns: new[] { "PropertyId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyScope_Order",
                table: "BizProductPropertyScope",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyScope_TypeId_Name",
                table: "BizProductPropertyScope",
                columns: new[] { "TypeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BizProductSpec_ProductId",
                table: "BizProductSpec",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductType_Name",
                table: "BizProductType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BizProductType_Order",
                table: "BizProductType",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_AuthorId",
                table: "CommonDocument",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_CreatedTime",
                table: "CommonDocument",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_Ident",
                table: "CommonDocument",
                column: "Ident");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_Name",
                table: "CommonDocument",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_OwnerId",
                table: "CommonDocument",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_PublishDate",
                table: "CommonDocument",
                column: "PublishDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_ScopeId",
                table: "CommonDocument",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_UpdatorId",
                table: "CommonDocument",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_ContainerId_ItemOrder",
                table: "CommonDocument",
                columns: new[] { "ContainerId", "ItemOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocument_ContainerId_PublishDate",
                table: "CommonDocument",
                columns: new[] { "ContainerId", "PublishDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentAuthor_CreatedTime",
                table: "CommonDocumentAuthor",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentAuthor_Name",
                table: "CommonDocumentAuthor",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentAuthor_OwnerId",
                table: "CommonDocumentAuthor",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentAuthor_ScopeId",
                table: "CommonDocumentAuthor",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentAuthor_UpdatorId",
                table: "CommonDocumentAuthor",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_CreatedTime",
                table: "CommonDocumentCategory",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_Name",
                table: "CommonDocumentCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_OwnerId",
                table: "CommonDocumentCategory",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_ScopeId",
                table: "CommonDocumentCategory",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_UpdatorId",
                table: "CommonDocumentCategory",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_ContainerId_ItemOrder",
                table: "CommonDocumentCategory",
                columns: new[] { "ContainerId", "ItemOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_CreatedTime",
                table: "CommonDocumentTag",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_Name",
                table: "CommonDocumentTag",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_OwnerId",
                table: "CommonDocumentTag",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_ScopeId",
                table: "CommonDocumentTag",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_UpdatorId",
                table: "CommonDocumentTag",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_ScopeId_Name",
                table: "CommonDocumentTag",
                columns: new[] { "ScopeId", "Name" },
                unique: true,
                filter: "[ScopeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTagRef_TagId",
                table: "CommonDocumentTagRef",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonTextMessageRecord_ScopeId",
                table: "CommonTextMessageRecord",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonTextMessageRecord_Time",
                table: "CommonTextMessageRecord",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_CommonTextMessageRecord_UserId",
                table: "CommonTextMessageRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonTextMessageRecord_ServiceId_Time",
                table: "CommonTextMessageRecord",
                columns: new[] { "ServiceId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonTextMessageRecord_Status_Time",
                table: "CommonTextMessageRecord",
                columns: new[] { "Status", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonTextMessageRecord_UserId_Time",
                table: "CommonTextMessageRecord",
                columns: new[] { "UserId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_FrontSite_TemplateId",
                table: "FrontSite",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_CreatedTime",
                table: "Member",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Name",
                table: "Member",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Member_OwnerId",
                table: "Member",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ScopeId",
                table: "Member",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_UpdatorId",
                table: "Member",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitation_InvitorId",
                table: "MemberInvitation",
                column: "InvitorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitation_UserId",
                table: "MemberInvitation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrAdmin_CreatedTime",
                table: "MgrAdmin",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_MgrAdmin_Name",
                table: "MgrAdmin",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MgrAdmin_OwnerId",
                table: "MgrAdmin",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrAdmin_ScopeId",
                table: "MgrAdmin",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrAdmin_UpdatorId",
                table: "MgrAdmin",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenu_CreatedTime",
                table: "MgrMenu",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenu_Name",
                table: "MgrMenu",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenu_OwnerId",
                table: "MgrMenu",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenu_ScopeId",
                table: "MgrMenu",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenu_UpdatorId",
                table: "MgrMenu",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenu_ScopeId_Ident",
                table: "MgrMenu",
                columns: new[] { "ScopeId", "Ident" });

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_CreatedTime",
                table: "MgrMenuItem",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_MenuId",
                table: "MgrMenuItem",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_Name",
                table: "MgrMenuItem",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_OwnerId",
                table: "MgrMenuItem",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_ParentId",
                table: "MgrMenuItem",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_ScopeId",
                table: "MgrMenuItem",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrMenuItem_UpdatorId",
                table: "MgrMenuItem",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClaimType_CreatedTime",
                table: "SysAuthClaimType",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClaimType_Name",
                table: "SysAuthClaimType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClaimType_OwnerId",
                table: "SysAuthClaimType",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClaimType_ScopeId",
                table: "SysAuthClaimType",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClaimType_UpdatorId",
                table: "SysAuthClaimType",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClient_ClientConfigId",
                table: "SysAuthClient",
                column: "ClientConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClient_CreatedTime",
                table: "SysAuthClient",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClient_Name",
                table: "SysAuthClient",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClient_OwnerId",
                table: "SysAuthClient",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClient_ScopeId",
                table: "SysAuthClient",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClient_UpdatorId",
                table: "SysAuthClient",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientClaimValue_ClientId",
                table: "SysAuthClientClaimValue",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientClaimValue_TypeId",
                table: "SysAuthClientClaimValue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientConfig_CreatedTime",
                table: "SysAuthClientConfig",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientConfig_Name",
                table: "SysAuthClientConfig",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientConfig_OwnerId",
                table: "SysAuthClientConfig",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientConfig_ScopeId",
                table: "SysAuthClientConfig",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientConfig_UpdatorId",
                table: "SysAuthClientConfig",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientGrant_OperationId",
                table: "SysAuthClientGrant",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthClientGrant_ResourceId",
                table: "SysAuthClientGrant",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthOperation_CreatedTime",
                table: "SysAuthOperation",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthOperation_Name",
                table: "SysAuthOperation",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthOperation_OwnerId",
                table: "SysAuthOperation",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthOperation_ScopeId",
                table: "SysAuthOperation",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthOperation_UpdatorId",
                table: "SysAuthOperation",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthOperationRequiredClaim_ClaimTypeId",
                table: "SysAuthOperationRequiredClaim",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResource_CreatedTime",
                table: "SysAuthResource",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResource_Name",
                table: "SysAuthResource",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResource_OwnerId",
                table: "SysAuthResource",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResource_ScopeId",
                table: "SysAuthResource",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResource_UpdatorId",
                table: "SysAuthResource",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResourceRequiredClaim_ClaimTypeId",
                table: "SysAuthResourceRequiredClaim",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthResourceSupportedOperation_OperationId",
                table: "SysAuthResourceSupportedOperation",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRole_CreatedTime",
                table: "SysAuthRole",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRole_Name",
                table: "SysAuthRole",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRole_OwnerId",
                table: "SysAuthRole",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRole_ScopeId",
                table: "SysAuthRole",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRole_UpdatorId",
                table: "SysAuthRole",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRoleClaimValue_RoleId",
                table: "SysAuthRoleClaimValue",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRoleClaimValue_TypeId",
                table: "SysAuthRoleClaimValue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRoleGrant_OperationId",
                table: "SysAuthRoleGrant",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthRoleGrant_ResourceId",
                table: "SysAuthRoleGrant",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_CreatedTime",
                table: "SysAuthUser",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_MainClaimTypeId",
                table: "SysAuthUser",
                column: "MainClaimTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_MainCredential",
                table: "SysAuthUser",
                column: "MainCredential",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_SignupClientId",
                table: "SysAuthUser",
                column: "SignupClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUserClaimValue_TypeId",
                table: "SysAuthUserClaimValue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUserClaimValue_UserId",
                table: "SysAuthUserClaimValue",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUserCredential_ClaimTypeId",
                table: "SysAuthUserCredential",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUserCredential_UserId",
                table: "SysAuthUserCredential",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUserRole_RoleId",
                table: "SysAuthUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SysCallInstance_CallTime",
                table: "SysCallInstance",
                column: "CallTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_CreatedTime",
                table: "SysServiceInstance",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ImplementId",
                table: "SysServiceInstance",
                column: "ImplementId");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_Name",
                table: "SysServiceInstance",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_OwnerId",
                table: "SysServiceInstance",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ScopeId",
                table: "SysServiceInstance",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ServiceIdent",
                table: "SysServiceInstance",
                column: "ServiceIdent");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_UpdatorId",
                table: "SysServiceInstance",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ContainerId_ItemOrder",
                table: "SysServiceInstance",
                columns: new[] { "ContainerId", "ItemOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ContainerId_ServiceId",
                table: "SysServiceInstance",
                columns: new[] { "ContainerId", "ServiceId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BizProductCategoryItem");

            migrationBuilder.DropTable(
                name: "BizProductDetail");

            migrationBuilder.DropTable(
                name: "BizProductPropertyItem");

            migrationBuilder.DropTable(
                name: "BizProductSpec");

            migrationBuilder.DropTable(
                name: "CommonDocumentTagRef");

            migrationBuilder.DropTable(
                name: "CommonTextMessageRecord");

            migrationBuilder.DropTable(
                name: "FrontContent");

            migrationBuilder.DropTable(
                name: "FrontSite");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "MemberInvitation");

            migrationBuilder.DropTable(
                name: "MgrAdmin");

            migrationBuilder.DropTable(
                name: "MgrMenuItem");

            migrationBuilder.DropTable(
                name: "SysAuthClientClaimValue");

            migrationBuilder.DropTable(
                name: "SysAuthClientGrant");

            migrationBuilder.DropTable(
                name: "SysAuthOperationRequiredClaim");

            migrationBuilder.DropTable(
                name: "SysAuthResourceRequiredClaim");

            migrationBuilder.DropTable(
                name: "SysAuthResourceSupportedOperation");

            migrationBuilder.DropTable(
                name: "SysAuthRoleClaimValue");

            migrationBuilder.DropTable(
                name: "SysAuthRoleGrant");

            migrationBuilder.DropTable(
                name: "SysAuthUserClaimValue");

            migrationBuilder.DropTable(
                name: "SysAuthUserCredential");

            migrationBuilder.DropTable(
                name: "SysAuthUserRole");

            migrationBuilder.DropTable(
                name: "SysCallExpired");

            migrationBuilder.DropTable(
                name: "SysCallInstance");

            migrationBuilder.DropTable(
                name: "SysIdentSeed");

            migrationBuilder.DropTable(
                name: "SysServiceInstance");

            migrationBuilder.DropTable(
                name: "BizProductCategory");

            migrationBuilder.DropTable(
                name: "BizProductItem");

            migrationBuilder.DropTable(
                name: "BizProductProperty");

            migrationBuilder.DropTable(
                name: "CommonDocument");

            migrationBuilder.DropTable(
                name: "CommonDocumentTag");

            migrationBuilder.DropTable(
                name: "FrontSiteTemplate");

            migrationBuilder.DropTable(
                name: "MgrMenu");

            migrationBuilder.DropTable(
                name: "SysAuthOperation");

            migrationBuilder.DropTable(
                name: "SysAuthResource");

            migrationBuilder.DropTable(
                name: "SysAuthClaimType");

            migrationBuilder.DropTable(
                name: "SysAuthRole");

            migrationBuilder.DropTable(
                name: "SysAuthUser");

            migrationBuilder.DropTable(
                name: "BizProduct");

            migrationBuilder.DropTable(
                name: "BizProductPropertyScope");

            migrationBuilder.DropTable(
                name: "CommonDocumentAuthor");

            migrationBuilder.DropTable(
                name: "CommonDocumentCategory");

            migrationBuilder.DropTable(
                name: "SysAuthClient");

            migrationBuilder.DropTable(
                name: "BizProductType");

            migrationBuilder.DropTable(
                name: "SysAuthClientConfig");
        }
    }
}
