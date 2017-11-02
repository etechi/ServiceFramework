using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Hygou.Core2.Migrations
{
    public partial class userstructchanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectState",
                table: "SysAuthUser");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SysAuthUser",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "SysAuthUser",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "SysAuthUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "LogicState",
                table: "SysAuthUser",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Memo",
                table: "SysAuthUser",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "SysAuthUser",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "SysAuthUser",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ScopeId",
                table: "SysAuthUser",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "SysAuthUser",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "SysAuthUser",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SysAuthUser",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "UpdatorId",
                table: "SysAuthUser",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_Name",
                table: "SysAuthUser",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_OwnerId",
                table: "SysAuthUser",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_ScopeId",
                table: "SysAuthUser",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysAuthUser_UpdatorId",
                table: "SysAuthUser",
                column: "UpdatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_Name",
                table: "SysAuthUser");

            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_OwnerId",
                table: "SysAuthUser");

            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_ScopeId",
                table: "SysAuthUser");

            migrationBuilder.DropIndex(
                name: "IX_SysAuthUser_UpdatorId",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "LogicState",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "Memo",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SysAuthUser");

            migrationBuilder.DropColumn(
                name: "UpdatorId",
                table: "SysAuthUser");

            migrationBuilder.AddColumn<byte>(
                name: "ObjectState",
                table: "SysAuthUser",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
