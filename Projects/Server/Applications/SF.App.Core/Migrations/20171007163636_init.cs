#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SF.App.Core.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommonDocumentAuthor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                name: "MgrBizAdmin",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_MgrBizAdmin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MgrMenu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ident = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                name: "MgrSysAdmin",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_MgrSysAdmin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthIdentity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScopeId = table.Column<long>(type: "bigint", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SignupExtraArgument = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SignupIdentProviderId = table.Column<long>(type: "bigint", nullable: false),
                    SignupIdentValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthIdentity", x => x.Id);
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
                name: "UserMember",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    SignupIdentityId = table.Column<long>(type: "bigint", nullable: false),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMember", x => x.Id);
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
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MgrMenuItem_MgrMenuItem_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MgrMenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysAuthIdentityCredential",
                columns: table => new
                {
                    ScopeId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    Credential = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfirmedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentityId = table.Column<long>(type: "bigint", nullable: false),
                    UnionIdent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysAuthIdentityCredential", x => new { x.ScopeId, x.ProviderId, x.Credential });
                    table.ForeignKey(
                        name: "FK_SysAuthIdentityCredential_SysAuthIdentity_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "SysAuthIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonDocumentTagRef_CommonDocumentTag_TagId",
                        column: x => x.TagId,
                        principalTable: "CommonDocumentTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_MemberInvitation_InvitorId",
                table: "MemberInvitation",
                column: "InvitorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitation_UserId",
                table: "MemberInvitation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_Account",
                table: "MgrBizAdmin",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_CreatedTime",
                table: "MgrBizAdmin",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_Name",
                table: "MgrBizAdmin",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_OwnerId",
                table: "MgrBizAdmin",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_ScopeId",
                table: "MgrBizAdmin",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_UpdatorId",
                table: "MgrBizAdmin",
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
                name: "IX_MgrSysAdmin_Account",
                table: "MgrSysAdmin",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "IX_MgrSysAdmin_CreatedTime",
                table: "MgrSysAdmin",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_MgrSysAdmin_Name",
                table: "MgrSysAdmin",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MgrSysAdmin_OwnerId",
                table: "MgrSysAdmin",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrSysAdmin_ScopeId",
                table: "MgrSysAdmin",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_MgrSysAdmin_UpdatorId",
                table: "MgrSysAdmin",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentity_CreatedTime",
                table: "SysAuthIdentity",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentity_OwnerId",
                table: "SysAuthIdentity",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentity_ScopeId",
                table: "SysAuthIdentity",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentity_SignupIdentProviderId",
                table: "SysAuthIdentity",
                column: "SignupIdentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentity_SignupIdentValue",
                table: "SysAuthIdentity",
                column: "SignupIdentValue");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentityCredential_IdentityId",
                table: "SysAuthIdentityCredential",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthIdentityCredential_ScopeId_ProviderId_UnionIdent",
                table: "SysAuthIdentityCredential",
                columns: new[] { "ScopeId", "ProviderId", "UnionIdent" });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_CreatedTime",
                table: "UserMember",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_Name",
                table: "UserMember",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_OwnerId",
                table: "UserMember",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_PhoneNumber",
                table: "UserMember",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_ScopeId",
                table: "UserMember",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_UpdatorId",
                table: "UserMember",
                column: "UpdatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonDocumentTagRef");

            migrationBuilder.DropTable(
                name: "CommonTextMessageRecord");

            migrationBuilder.DropTable(
                name: "FrontContent");

            migrationBuilder.DropTable(
                name: "FrontSite");

            migrationBuilder.DropTable(
                name: "MemberInvitation");

            migrationBuilder.DropTable(
                name: "MgrBizAdmin");

            migrationBuilder.DropTable(
                name: "MgrMenuItem");

            migrationBuilder.DropTable(
                name: "MgrSysAdmin");

            migrationBuilder.DropTable(
                name: "SysAuthIdentityCredential");

            migrationBuilder.DropTable(
                name: "SysCallExpired");

            migrationBuilder.DropTable(
                name: "SysCallInstance");

            migrationBuilder.DropTable(
                name: "SysIdentSeed");

            migrationBuilder.DropTable(
                name: "SysServiceInstance");

            migrationBuilder.DropTable(
                name: "UserMember");

            migrationBuilder.DropTable(
                name: "CommonDocument");

            migrationBuilder.DropTable(
                name: "CommonDocumentTag");

            migrationBuilder.DropTable(
                name: "FrontSiteTemplate");

            migrationBuilder.DropTable(
                name: "MgrMenu");

            migrationBuilder.DropTable(
                name: "SysAuthIdentity");

            migrationBuilder.DropTable(
                name: "CommonDocumentAuthor");

            migrationBuilder.DropTable(
                name: "CommonDocumentCategory");
        }
    }
}
