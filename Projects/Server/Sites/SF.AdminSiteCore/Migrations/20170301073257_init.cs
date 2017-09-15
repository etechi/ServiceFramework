using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SF.AdminSiteCore.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SysServiceInstance",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    CreateArguments = table.Column<string>(nullable: true),
                    DeclarationId = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 100, nullable: true),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    ImplementId = table.Column<string>(maxLength: 200, nullable: false),
                    IsDefaultService = table.Column<bool>(nullable: false),
                    LogicState = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(maxLength: 200, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysServiceInstance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysIdentSeed",
                columns: table => new
                {
                    Type = table.Column<string>(nullable: false),
                    NextValue = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysIdentSeed", x => x.Type);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ImplementId",
                table: "SysServiceInstance",
                column: "ImplementId");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_DeclarationId_IsDefaultService",
                table: "SysServiceInstance",
                columns: new[] { "DeclarationId", "IsDefaultService" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysServiceInstance");

            migrationBuilder.DropTable(
                name: "SysIdentSeed");
        }
    }
}
