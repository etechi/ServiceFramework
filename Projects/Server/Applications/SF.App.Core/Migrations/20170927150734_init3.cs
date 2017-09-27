using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SF.App.Core.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InvitationMemberInvitation_InvitorId",
                table: "InvitationMemberInvitation",
                column: "InvitorId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationMemberInvitation_UserId",
                table: "InvitationMemberInvitation",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvitationMemberInvitation_InvitorId",
                table: "InvitationMemberInvitation");

            migrationBuilder.DropIndex(
                name: "IX_InvitationMemberInvitation_UserId",
                table: "InvitationMemberInvitation");
        }
    }
}
