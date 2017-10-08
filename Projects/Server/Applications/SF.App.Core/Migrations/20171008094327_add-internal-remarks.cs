using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SF.App.Core.Migrations
{
    public partial class addinternalremarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "UserMember",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "SysServiceInstance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "MgrSysAdmin",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "MgrMenuItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "MgrMenu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "MgrBizAdmin",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "CommonDocumentTag",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "CommonDocumentCategory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "CommonDocumentAuthor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "CommonDocument",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "UserMember");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "SysServiceInstance");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "MgrSysAdmin");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "MgrMenuItem");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "MgrMenu");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "MgrBizAdmin");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "CommonDocumentTag");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "CommonDocumentCategory");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "CommonDocumentAuthor");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "CommonDocument");
        }
    }
}
