using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Hygou.Core2.Migrations
{
    public partial class addusermember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "UserMember");
        }
    }
}
