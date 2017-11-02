using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Hygou.Core2.Migrations
{
    public partial class addmaincredentialkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_MainClaimTypeId",
                table: "SysAuthUser");

            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_MainCredential",
                table: "SysAuthUser");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_MainClaimTypeId_MainCredential",
                table: "SysAuthUser",
                columns: new[] { "MainClaimTypeId", "MainCredential" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_MainClaimTypeId_MainCredential",
                table: "SysAuthUser");

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
        }
    }
}
