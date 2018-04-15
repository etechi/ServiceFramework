using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFShop.Backend.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthClaimType",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthClaimType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthClientConfig",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AbsoluteRefreshTokenLifetime = table.Column<int>(nullable: false),
                    AccessTokenLifetime = table.Column<int>(nullable: false),
                    AllowOfflineAccess = table.Column<bool>(nullable: false),
                    AllowRememberConsent = table.Column<bool>(nullable: false),
                    AllowedCorsOrigins = table.Column<string>(maxLength: 200, nullable: true),
                    AllowedGrantTypes = table.Column<string>(nullable: true),
                    AlwaysIncludeUserClaimsInIdToken = table.Column<bool>(nullable: false),
                    AuthorizationCodeLifetime = table.Column<int>(nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                    ClientClaimsPrefix = table.Column<string>(maxLength: 100, nullable: true),
                    ConsentLifetime = table.Column<int>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(nullable: false),
                    IdentityTokenLifetime = table.Column<int>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    RequireClientSecret = table.Column<bool>(nullable: false),
                    RequireConsent = table.Column<bool>(nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SlidingRefreshTokenLifetime = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthClientConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthGrant",
                columns: table => new
                {
                    Id = table.Column<long>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthGrant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthOperation",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthOperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthResource",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    IsIdentityResource = table.Column<bool>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthRole",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    IsSysRole = table.Column<bool>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthScope",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthScope", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackEndAdminConsole",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Ident = table.Column<string>(maxLength: 100, nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Menus = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    Pages = table.Column<string>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackEndAdminConsole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BizProductCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    BannerImage = table.Column<string>(maxLength: 200, nullable: true),
                    BannerUrl = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    ItemCount = table.Column<int>(nullable: false),
                    MobileBannerImage = table.Column<string>(maxLength: 200, nullable: true),
                    MobileBannerUrl = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    OwnerUserId = table.Column<long>(nullable: false),
                    ParentId = table.Column<long>(nullable: true),
                    Tag = table.Column<string>(maxLength: 20, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false)
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
                    Id = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ProductCount = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Unit = table.Column<string>(maxLength: 20, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentScope",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentScope", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonFrontContent",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(maxLength: 200, nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    FontIcon = table.Column<string>(maxLength: 100, nullable: true),
                    Icon = table.Column<string>(maxLength: 200, nullable: true),
                    Image = table.Column<string>(maxLength: 200, nullable: true),
                    ItemsData = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ProviderConfig = table.Column<string>(nullable: true),
                    ProviderType = table.Column<string>(maxLength: 100, nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    Title1 = table.Column<string>(maxLength: 200, nullable: true),
                    Title2 = table.Column<string>(maxLength: 200, nullable: true),
                    Title3 = table.Column<string>(maxLength: 200, nullable: true),
                    Uri = table.Column<string>(maxLength: 200, nullable: true),
                    UriTarget = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonFrontContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonFrontSiteTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonFrontSiteTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonNotificationSendPolicy",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Actions = table.Column<string>(nullable: true),
                    ContentTemplate = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Ident = table.Column<string>(maxLength: 100, nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    NameTemplate = table.Column<string>(maxLength: 100, nullable: true),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotificationSendPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonNotificationUserStatus",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Received = table.Column<int>(nullable: false),
                    ReceivedUnreaded = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotificationUserStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysIdentSeed",
                columns: table => new
                {
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    NextValue = table.Column<long>(nullable: false),
                    Section = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysIdentSeed", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "SysMenu",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Ident = table.Column<string>(maxLength: 100, nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    Items = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysReminder",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    BizIdent = table.Column<long>(nullable: false),
                    BizIdentType = table.Column<string>(nullable: true),
                    BizType = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 1000, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    RemindableName = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TaskExecCount = table.Column<int>(nullable: false),
                    TaskLastExecTime = table.Column<DateTime>(nullable: true),
                    TaskMessage = table.Column<string>(nullable: true),
                    TaskNextExecTime = table.Column<DateTime>(nullable: true),
                    TaskStartTime = table.Column<DateTime>(nullable: true),
                    TaskState = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysReminder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysRemindRecord",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    BizIdent = table.Column<long>(nullable: false),
                    BizIdentType = table.Column<string>(nullable: true),
                    BizType = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 1000, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    RemindableName = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TaskExecCount = table.Column<int>(nullable: false),
                    TaskLastExecTime = table.Column<DateTime>(nullable: true),
                    TaskMessage = table.Column<string>(nullable: true),
                    TaskNextExecTime = table.Column<DateTime>(nullable: true),
                    TaskStartTime = table.Column<DateTime>(nullable: true),
                    TaskState = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRemindRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysServiceInstance",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ContainerId = table.Column<long>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    ImplementId = table.Column<string>(maxLength: 40, nullable: false),
                    ImplementType = table.Column<string>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    ItemOrder = table.Column<int>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    ServiceId = table.Column<string>(maxLength: 40, nullable: false),
                    ServiceIdent = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceType = table.Column<string>(nullable: false),
                    Setting = table.Column<string>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
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
                name: "AuthClient",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    ClientConfigId = table.Column<long>(nullable: false),
                    ClientSecrets = table.Column<string>(maxLength: 200, nullable: true),
                    ClientUri = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    FrontChannelLogoutUri = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(nullable: true),
                    RedirectUris = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthClient_AuthClientConfig_ClientConfigId",
                        column: x => x.ClientConfigId,
                        principalTable: "AuthClientConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthGrantItem",
                columns: table => new
                {
                    GrantId = table.Column<long>(nullable: false),
                    ServiceId = table.Column<string>(maxLength: 200, nullable: false),
                    ServiceMethodId = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthGrantItem", x => new { x.GrantId, x.ServiceId, x.ServiceMethodId });
                    table.ForeignKey(
                        name: "FK_AuthGrantItem_AuthGrant_GrantId",
                        column: x => x.GrantId,
                        principalTable: "AuthGrant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthOperationRequiredClaim",
                columns: table => new
                {
                    OperationId = table.Column<string>(nullable: false),
                    ClaimTypeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthOperationRequiredClaim", x => new { x.OperationId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_AuthOperationRequiredClaim_AuthClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "AuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthOperationRequiredClaim_AuthResource_OperationId",
                        column: x => x.OperationId,
                        principalTable: "AuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthResourceRequiredClaim",
                columns: table => new
                {
                    ResourceId = table.Column<string>(nullable: false),
                    ClaimTypeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthResourceRequiredClaim", x => new { x.ResourceId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_AuthResourceRequiredClaim_AuthClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "AuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthResourceRequiredClaim_AuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "AuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthResourceSupportedOperation",
                columns: table => new
                {
                    ResourceId = table.Column<string>(nullable: false),
                    OperationId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthResourceSupportedOperation", x => new { x.ResourceId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_AuthResourceSupportedOperation_AuthOperation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "AuthOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthResourceSupportedOperation_AuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "AuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthRoleClaimValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    RoleId = table.Column<string>(nullable: true),
                    TypeId = table.Column<string>(maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRoleClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthRoleClaimValue_AuthRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthRoleClaimValue_AuthClaimType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthRoleGrant",
                columns: table => new
                {
                    RoleId = table.Column<string>(nullable: false),
                    DstGrantId = table.Column<long>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRoleGrant", x => new { x.RoleId, x.DstGrantId });
                    table.ForeignKey(
                        name: "FK_AuthRoleGrant_AuthGrant_DstGrantId",
                        column: x => x.DstGrantId,
                        principalTable: "AuthGrant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthRoleGrant_AuthRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthClientScope",
                columns: table => new
                {
                    ClientConfigId = table.Column<long>(nullable: false),
                    ScopeId = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthClientScope", x => new { x.ClientConfigId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_AuthClientScope_AuthClientConfig_ClientConfigId",
                        column: x => x.ClientConfigId,
                        principalTable: "AuthClientConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthClientScope_AuthScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "AuthScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthDataScopeResource",
                columns: table => new
                {
                    ScopeId = table.Column<string>(nullable: false),
                    ResourceId = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthDataScopeResource", x => new { x.ScopeId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_AuthDataScopeResource_AuthResource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "AuthResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthDataScopeResource_AuthScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "AuthScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BackEndAdminHotMenuCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ConsoleId = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FontIcon = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackEndAdminHotMenuCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackEndAdminHotMenuCategory_BackEndAdminConsole_ConsoleId",
                        column: x => x.ConsoleId,
                        principalTable: "BackEndAdminConsole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BackEndAdminHotQuery",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ConsoleId = table.Column<long>(nullable: false),
                    ContentId = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    PageId = table.Column<long>(nullable: false),
                    Query = table.Column<string>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackEndAdminHotQuery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackEndAdminHotQuery_BackEndAdminConsole_ConsoleId",
                        column: x => x.ConsoleId,
                        principalTable: "BackEndAdminConsole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CouponDisabled = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    IsVirtual = table.Column<bool>(nullable: false),
                    MarketPrice = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    Order = table.Column<double>(nullable: false),
                    OwnerUserId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PublishedTime = table.Column<DateTime>(nullable: true),
                    SellCount = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    TypeId = table.Column<long>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    VIADSpecId = table.Column<long>(nullable: true),
                    Visited = table.Column<int>(nullable: false)
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
                    Id = table.Column<long>(nullable: false),
                    Icon = table.Column<string>(maxLength: 200, nullable: true),
                    Image = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    TypeId = table.Column<long>(nullable: false)
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
                name: "CommonDocumentAuthor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ScopeId = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentAuthor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonDocumentAuthor_CommonDocumentScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "CommonDocumentScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ContainerId = table.Column<long>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    ItemOrder = table.Column<int>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ScopeId = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CommonDocumentCategory_CommonDocumentScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "CommonDocumentScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonDocumentTag",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ScopeId = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonDocumentTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonDocumentTag_CommonDocumentScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "CommonDocumentScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonFrontSite",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    TemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonFrontSite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonFrontSite_CommonFrontSiteTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "CommonFrontSiteTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonNotification",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ActionCount = table.Column<int>(nullable: false),
                    Args = table.Column<string>(nullable: true),
                    BizIdent = table.Column<string>(maxLength: 100, nullable: true),
                    CompletedActionCount = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    Error = table.Column<string>(maxLength: 1000, nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    Image = table.Column<string>(maxLength: 50, nullable: true),
                    Link = table.Column<string>(maxLength: 200, nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Mode = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    PolicyId = table.Column<long>(nullable: true),
                    SenderId = table.Column<long>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonNotification_CommonNotificationSendPolicy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "CommonNotificationSendPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthClientClaimValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ClientId = table.Column<string>(maxLength: 100, nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    TypeId = table.Column<string>(maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthClientClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthClientClaimValue_AuthClient_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AuthClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthClientClaimValue_AuthClaimType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthUser",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    MainClaimTypeId = table.Column<string>(maxLength: 100, nullable: false),
                    MainCredential = table.Column<string>(maxLength: 200, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    PasswordHash = table.Column<string>(maxLength: 100, nullable: true),
                    SecurityStamp = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SignupClientId = table.Column<string>(maxLength: 100, nullable: true),
                    SignupExtraArgument = table.Column<string>(maxLength: 200, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthUser_AuthClient_SignupClientId",
                        column: x => x.SignupClientId,
                        principalTable: "AuthClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BackEndAdminHotMenuItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false),
                    ConsoleId = table.Column<long>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FontIcon = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackEndAdminHotMenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackEndAdminHotMenuItem_BackEndAdminHotMenuCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BackEndAdminHotMenuCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackEndAdminHotMenuItem_BackEndAdminConsole_ConsoleId",
                        column: x => x.ConsoleId,
                        principalTable: "BackEndAdminConsole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Detail = table.Column<string>(nullable: true),
                    Images = table.Column<string>(nullable: true)
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
                    Id = table.Column<long>(nullable: false),
                    CategoryTags = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    ObjectState = table.Column<byte>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    SellerId = table.Column<long>(nullable: false),
                    SourceItemId = table.Column<long>(nullable: true),
                    SourceLevel = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    Image = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    VIADSpecId = table.Column<long>(nullable: true)
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
                    Id = table.Column<long>(nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ParentId = table.Column<long>(nullable: true),
                    ScopeId = table.Column<long>(nullable: false),
                    TypeId = table.Column<long>(nullable: false)
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
                name: "CommonDocument",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AuthorId = table.Column<long>(nullable: true),
                    ContainerId = table.Column<long>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Ident = table.Column<string>(maxLength: 50, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    InternalRemarks = table.Column<string>(nullable: true),
                    ItemOrder = table.Column<int>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Memo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 100, nullable: true),
                    ScopeId = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    SubTitle = table.Column<string>(maxLength: 100, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CommonDocument_CommonDocumentScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "CommonDocumentScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonNotificationSendRecord",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Args = table.Column<string>(nullable: true),
                    BizIdent = table.Column<string>(maxLength: 100, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Error = table.Column<string>(maxLength: 1000, nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    LastSendTime = table.Column<DateTime>(nullable: true),
                    NotificationId = table.Column<long>(nullable: false),
                    ProviderId = table.Column<long>(nullable: false),
                    Result = table.Column<string>(maxLength: 1000, nullable: true),
                    RetryInterval = table.Column<int>(nullable: false),
                    RetryLimit = table.Column<int>(nullable: false),
                    ScopeId = table.Column<long>(nullable: true),
                    SendCount = table.Column<int>(nullable: false),
                    SendTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Target = table.Column<string>(nullable: true),
                    TargetId = table.Column<long>(nullable: true),
                    Template = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotificationSendRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonNotificationSendRecord_CommonNotification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "CommonNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonNotificationTarget",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    NotificationId = table.Column<long>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Mode = table.Column<byte>(nullable: false),
                    ReadTime = table.Column<DateTime>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonNotificationTarget", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_CommonNotificationTarget_CommonNotification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "CommonNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthUserClaimValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    TypeId = table.Column<string>(maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthUserClaimValue_AuthClaimType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthUserClaimValue_AuthUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthUserCredential",
                columns: table => new
                {
                    ClaimTypeId = table.Column<string>(maxLength: 100, nullable: false),
                    Credential = table.Column<string>(maxLength: 200, nullable: false),
                    ConfirmedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserCredential", x => new { x.ClaimTypeId, x.Credential });
                    table.ForeignKey(
                        name: "FK_AuthUserCredential_AuthClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "AuthClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthUserCredential_AuthUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthUserRole",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AuthUserRole_AuthRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthUserRole_AuthUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductCategoryItem",
                columns: table => new
                {
                    CategoryId = table.Column<long>(nullable: false),
                    ItemId = table.Column<long>(nullable: false),
                    Order = table.Column<long>(nullable: false)
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
                    PropertyId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Order = table.Column<double>(nullable: false)
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
                name: "CommonDocumentTagRef",
                columns: table => new
                {
                    DocumentId = table.Column<long>(nullable: false),
                    TagId = table.Column<long>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_AuthClaimType_CreatedTime",
                table: "AuthClaimType",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClaimType_Name",
                table: "AuthClaimType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthClaimType_OwnerId",
                table: "AuthClaimType",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClaimType_ServiceDataScopeId",
                table: "AuthClaimType",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClaimType_UpdatorId",
                table: "AuthClaimType",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClient_ClientConfigId",
                table: "AuthClient",
                column: "ClientConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClient_CreatedTime",
                table: "AuthClient",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClient_Name",
                table: "AuthClient",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClient_OwnerId",
                table: "AuthClient",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClient_ServiceDataScopeId",
                table: "AuthClient",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClient_UpdatorId",
                table: "AuthClient",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientClaimValue_ClientId",
                table: "AuthClientClaimValue",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientClaimValue_TypeId",
                table: "AuthClientClaimValue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientConfig_CreatedTime",
                table: "AuthClientConfig",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientConfig_Name",
                table: "AuthClientConfig",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientConfig_OwnerId",
                table: "AuthClientConfig",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientConfig_ServiceDataScopeId",
                table: "AuthClientConfig",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientConfig_UpdatorId",
                table: "AuthClientConfig",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthClientScope_ScopeId",
                table: "AuthClientScope",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthDataScopeResource_ResourceId",
                table: "AuthDataScopeResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthGrant_CreatedTime",
                table: "AuthGrant",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthGrant_Name",
                table: "AuthGrant",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthGrant_OwnerId",
                table: "AuthGrant",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthGrant_ServiceDataScopeId",
                table: "AuthGrant",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthGrant_UpdatorId",
                table: "AuthGrant",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthOperation_CreatedTime",
                table: "AuthOperation",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthOperation_Name",
                table: "AuthOperation",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AuthOperation_OwnerId",
                table: "AuthOperation",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthOperation_ServiceDataScopeId",
                table: "AuthOperation",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthOperation_UpdatorId",
                table: "AuthOperation",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthOperationRequiredClaim_ClaimTypeId",
                table: "AuthOperationRequiredClaim",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResource_CreatedTime",
                table: "AuthResource",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResource_Name",
                table: "AuthResource",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResource_OwnerId",
                table: "AuthResource",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResource_ServiceDataScopeId",
                table: "AuthResource",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResource_UpdatorId",
                table: "AuthResource",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResourceRequiredClaim_ClaimTypeId",
                table: "AuthResourceRequiredClaim",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthResourceSupportedOperation_OperationId",
                table: "AuthResourceSupportedOperation",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRole_CreatedTime",
                table: "AuthRole",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRole_Name",
                table: "AuthRole",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthRole_OwnerId",
                table: "AuthRole",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRole_ServiceDataScopeId",
                table: "AuthRole",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRole_UpdatorId",
                table: "AuthRole",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRoleClaimValue_RoleId",
                table: "AuthRoleClaimValue",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRoleClaimValue_TypeId",
                table: "AuthRoleClaimValue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRoleGrant_DstGrantId",
                table: "AuthRoleGrant",
                column: "DstGrantId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthScope_CreatedTime",
                table: "AuthScope",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthScope_Name",
                table: "AuthScope",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AuthScope_OwnerId",
                table: "AuthScope",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthScope_ServiceDataScopeId",
                table: "AuthScope",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthScope_UpdatorId",
                table: "AuthScope",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_CreatedTime",
                table: "AuthUser",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_Name",
                table: "AuthUser",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_OwnerId",
                table: "AuthUser",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_ServiceDataScopeId",
                table: "AuthUser",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_SignupClientId",
                table: "AuthUser",
                column: "SignupClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_UpdatorId",
                table: "AuthUser",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_MainClaimTypeId_MainCredential",
                table: "AuthUser",
                columns: new[] { "MainClaimTypeId", "MainCredential" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserClaimValue_TypeId",
                table: "AuthUserClaimValue",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserClaimValue_UserId",
                table: "AuthUserClaimValue",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserCredential_ClaimTypeId",
                table: "AuthUserCredential",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserCredential_UserId",
                table: "AuthUserCredential",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserRole_RoleId",
                table: "AuthUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminConsole_CreatedTime",
                table: "BackEndAdminConsole",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminConsole_Ident",
                table: "BackEndAdminConsole",
                column: "Ident");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminConsole_Name",
                table: "BackEndAdminConsole",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminConsole_OwnerId",
                table: "BackEndAdminConsole",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminConsole_ServiceDataScopeId",
                table: "BackEndAdminConsole",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminConsole_UpdatorId",
                table: "BackEndAdminConsole",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuCategory_ConsoleId",
                table: "BackEndAdminHotMenuCategory",
                column: "ConsoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuCategory_CreatedTime",
                table: "BackEndAdminHotMenuCategory",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuCategory_Name",
                table: "BackEndAdminHotMenuCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuCategory_OwnerId",
                table: "BackEndAdminHotMenuCategory",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuCategory_ServiceDataScopeId",
                table: "BackEndAdminHotMenuCategory",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuCategory_UpdatorId",
                table: "BackEndAdminHotMenuCategory",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_CategoryId",
                table: "BackEndAdminHotMenuItem",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_ConsoleId",
                table: "BackEndAdminHotMenuItem",
                column: "ConsoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_CreatedTime",
                table: "BackEndAdminHotMenuItem",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_Name",
                table: "BackEndAdminHotMenuItem",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_OwnerId",
                table: "BackEndAdminHotMenuItem",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_ServiceDataScopeId",
                table: "BackEndAdminHotMenuItem",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotMenuItem_UpdatorId",
                table: "BackEndAdminHotMenuItem",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotQuery_ConsoleId",
                table: "BackEndAdminHotQuery",
                column: "ConsoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotQuery_CreatedTime",
                table: "BackEndAdminHotQuery",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotQuery_Name",
                table: "BackEndAdminHotQuery",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotQuery_OwnerId",
                table: "BackEndAdminHotQuery",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotQuery_ServiceDataScopeId",
                table: "BackEndAdminHotQuery",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BackEndAdminHotQuery_UpdatorId",
                table: "BackEndAdminHotQuery",
                column: "UpdatorId");

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
                name: "IX_CommonDocument_ServiceDataScopeId",
                table: "CommonDocument",
                column: "ServiceDataScopeId");

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
                name: "IX_CommonDocumentAuthor_ServiceDataScopeId",
                table: "CommonDocumentAuthor",
                column: "ServiceDataScopeId");

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
                name: "IX_CommonDocumentCategory_ServiceDataScopeId",
                table: "CommonDocumentCategory",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_UpdatorId",
                table: "CommonDocumentCategory",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentCategory_ContainerId_ItemOrder",
                table: "CommonDocumentCategory",
                columns: new[] { "ContainerId", "ItemOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentScope_CreatedTime",
                table: "CommonDocumentScope",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentScope_Name",
                table: "CommonDocumentScope",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentScope_OwnerId",
                table: "CommonDocumentScope",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentScope_ServiceDataScopeId",
                table: "CommonDocumentScope",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentScope_UpdatorId",
                table: "CommonDocumentScope",
                column: "UpdatorId");

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
                name: "IX_CommonDocumentTag_ServiceDataScopeId",
                table: "CommonDocumentTag",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_UpdatorId",
                table: "CommonDocumentTag",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTag_ServiceDataScopeId_Name",
                table: "CommonDocumentTag",
                columns: new[] { "ServiceDataScopeId", "Name" },
                unique: true,
                filter: "[ServiceDataScopeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CommonDocumentTagRef_TagId",
                table: "CommonDocumentTagRef",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonFrontSite_TemplateId",
                table: "CommonFrontSite",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotification_BizIdent",
                table: "CommonNotification",
                column: "BizIdent");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotification_PolicyId",
                table: "CommonNotification",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotification_SenderId",
                table: "CommonNotification",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotification_Expires_Time",
                table: "CommonNotification",
                columns: new[] { "Expires", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendPolicy_CreatedTime",
                table: "CommonNotificationSendPolicy",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendPolicy_Ident",
                table: "CommonNotificationSendPolicy",
                column: "Ident");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendPolicy_Name",
                table: "CommonNotificationSendPolicy",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendPolicy_OwnerId",
                table: "CommonNotificationSendPolicy",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendPolicy_ServiceDataScopeId",
                table: "CommonNotificationSendPolicy",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendPolicy_UpdatorId",
                table: "CommonNotificationSendPolicy",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_BizIdent",
                table: "CommonNotificationSendRecord",
                column: "BizIdent");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_NotificationId",
                table: "CommonNotificationSendRecord",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_ProviderId",
                table: "CommonNotificationSendRecord",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_ScopeId",
                table: "CommonNotificationSendRecord",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_TargetId",
                table: "CommonNotificationSendRecord",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_Time",
                table: "CommonNotificationSendRecord",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationSendRecord_UserId",
                table: "CommonNotificationSendRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationTarget_NotificationId",
                table: "CommonNotificationTarget",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonNotificationTarget_UserId_LogicState_Time",
                table: "CommonNotificationTarget",
                columns: new[] { "UserId", "LogicState", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_SysMenu_CreatedTime",
                table: "SysMenu",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysMenu_Name",
                table: "SysMenu",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysMenu_OwnerId",
                table: "SysMenu",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysMenu_ServiceDataScopeId",
                table: "SysMenu",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysMenu_UpdatorId",
                table: "SysMenu",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysMenu_ServiceDataScopeId_Ident",
                table: "SysMenu",
                columns: new[] { "ServiceDataScopeId", "Ident" });

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_CreatedTime",
                table: "SysReminder",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_Name",
                table: "SysReminder",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_OwnerId",
                table: "SysReminder",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_ServiceDataScopeId",
                table: "SysReminder",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_UpdatorId",
                table: "SysReminder",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_TaskState_TaskNextExecTime",
                table: "SysReminder",
                columns: new[] { "TaskState", "TaskNextExecTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_BizType_BizIdentType_BizIdent",
                table: "SysReminder",
                columns: new[] { "BizType", "BizIdentType", "BizIdent" },
                unique: true,
                filter: "[BizType] IS NOT NULL AND [BizIdentType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_CreatedTime",
                table: "SysRemindRecord",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_Name",
                table: "SysRemindRecord",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_OwnerId",
                table: "SysRemindRecord",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_ServiceDataScopeId",
                table: "SysRemindRecord",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_UpdatorId",
                table: "SysRemindRecord",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_TaskState_TaskNextExecTime",
                table: "SysRemindRecord",
                columns: new[] { "TaskState", "TaskNextExecTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_BizType_BizIdentType_BizIdent",
                table: "SysRemindRecord",
                columns: new[] { "BizType", "BizIdentType", "BizIdent" });

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
                name: "IX_SysServiceInstance_ServiceDataScopeId",
                table: "SysServiceInstance",
                column: "ServiceDataScopeId");

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
                name: "AuthClientClaimValue");

            migrationBuilder.DropTable(
                name: "AuthClientScope");

            migrationBuilder.DropTable(
                name: "AuthDataScopeResource");

            migrationBuilder.DropTable(
                name: "AuthGrantItem");

            migrationBuilder.DropTable(
                name: "AuthOperationRequiredClaim");

            migrationBuilder.DropTable(
                name: "AuthResourceRequiredClaim");

            migrationBuilder.DropTable(
                name: "AuthResourceSupportedOperation");

            migrationBuilder.DropTable(
                name: "AuthRoleClaimValue");

            migrationBuilder.DropTable(
                name: "AuthRoleGrant");

            migrationBuilder.DropTable(
                name: "AuthUserClaimValue");

            migrationBuilder.DropTable(
                name: "AuthUserCredential");

            migrationBuilder.DropTable(
                name: "AuthUserRole");

            migrationBuilder.DropTable(
                name: "BackEndAdminHotMenuItem");

            migrationBuilder.DropTable(
                name: "BackEndAdminHotQuery");

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
                name: "CommonFrontContent");

            migrationBuilder.DropTable(
                name: "CommonFrontSite");

            migrationBuilder.DropTable(
                name: "CommonNotificationSendRecord");

            migrationBuilder.DropTable(
                name: "CommonNotificationTarget");

            migrationBuilder.DropTable(
                name: "CommonNotificationUserStatus");

            migrationBuilder.DropTable(
                name: "SysIdentSeed");

            migrationBuilder.DropTable(
                name: "SysMenu");

            migrationBuilder.DropTable(
                name: "SysReminder");

            migrationBuilder.DropTable(
                name: "SysRemindRecord");

            migrationBuilder.DropTable(
                name: "SysServiceInstance");

            migrationBuilder.DropTable(
                name: "AuthScope");

            migrationBuilder.DropTable(
                name: "AuthOperation");

            migrationBuilder.DropTable(
                name: "AuthResource");

            migrationBuilder.DropTable(
                name: "AuthGrant");

            migrationBuilder.DropTable(
                name: "AuthClaimType");

            migrationBuilder.DropTable(
                name: "AuthRole");

            migrationBuilder.DropTable(
                name: "AuthUser");

            migrationBuilder.DropTable(
                name: "BackEndAdminHotMenuCategory");

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
                name: "CommonFrontSiteTemplate");

            migrationBuilder.DropTable(
                name: "CommonNotification");

            migrationBuilder.DropTable(
                name: "AuthClient");

            migrationBuilder.DropTable(
                name: "BackEndAdminConsole");

            migrationBuilder.DropTable(
                name: "BizProduct");

            migrationBuilder.DropTable(
                name: "BizProductPropertyScope");

            migrationBuilder.DropTable(
                name: "CommonDocumentAuthor");

            migrationBuilder.DropTable(
                name: "CommonDocumentCategory");

            migrationBuilder.DropTable(
                name: "CommonNotificationSendPolicy");

            migrationBuilder.DropTable(
                name: "AuthClientConfig");

            migrationBuilder.DropTable(
                name: "BizProductType");

            migrationBuilder.DropTable(
                name: "CommonDocumentScope");
        }
    }
}
