using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SF.App.Core.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvitationMemberInvitation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    InvitorId = table.Column<long>(type: "bigint", nullable: false),
                    Invitors = table.Column<string>(type: "nvarchar(max)", maxLength: 100000, nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationMemberInvitation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvitationMemberInvitation");
        }
    }
}
