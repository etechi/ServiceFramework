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
                    ImplementType = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    LogicState = table.Column<byte>(type: "tinyint", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceIdent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_SysCallInstance_CallTime",
                table: "SysCallInstance",
                column: "CallTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_CreatedTime",
                table: "SysServiceInstance",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ImplementType",
                table: "SysServiceInstance",
                column: "ImplementType");

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
                name: "IX_SysServiceInstance_ContainerId_ServiceType",
                table: "SysServiceInstance",
                columns: new[] { "ContainerId", "ServiceType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysCallExpired");

            migrationBuilder.DropTable(
                name: "SysCallInstance");

            migrationBuilder.DropTable(
                name: "SysIdentSeed");

            migrationBuilder.DropTable(
                name: "SysServiceInstance");
        }
    }
}
