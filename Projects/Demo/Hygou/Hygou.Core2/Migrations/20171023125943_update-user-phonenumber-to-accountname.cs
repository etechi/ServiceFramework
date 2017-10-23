using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Hygou.Core2.Migrations
{
    public partial class updateuserphonenumbertoaccountname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserMember_PhoneNumber",
                table: "UserMember");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserMember");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "UserMember",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "MgrSysAdmin",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SignupIdentityId",
                table: "MgrSysAdmin",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "MgrBizAdmin",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SignupIdentityId",
                table: "MgrBizAdmin",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_AccountName",
                table: "UserMember",
                column: "AccountName");

            migrationBuilder.CreateIndex(
                name: "IX_MgrSysAdmin_AccountName",
                table: "MgrSysAdmin",
                column: "AccountName");

            migrationBuilder.CreateIndex(
                name: "IX_MgrBizAdmin_AccountName",
                table: "MgrBizAdmin",
                column: "AccountName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserMember_AccountName",
                table: "UserMember");

            migrationBuilder.DropIndex(
                name: "IX_MgrSysAdmin_AccountName",
                table: "MgrSysAdmin");

            migrationBuilder.DropIndex(
                name: "IX_MgrBizAdmin_AccountName",
                table: "MgrBizAdmin");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "UserMember");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "MgrSysAdmin");

            migrationBuilder.DropColumn(
                name: "SignupIdentityId",
                table: "MgrSysAdmin");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "MgrBizAdmin");

            migrationBuilder.DropColumn(
                name: "SignupIdentityId",
                table: "MgrBizAdmin");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserMember",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMember_PhoneNumber",
                table: "UserMember",
                column: "PhoneNumber");
        }
    }
}
