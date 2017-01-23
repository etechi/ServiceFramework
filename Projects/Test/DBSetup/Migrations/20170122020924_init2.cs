using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBSetup.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppAuthUserPhoneNumberIdent",
                table: "AppAuthUserPhoneNumberIdent");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AppAuthUserPhoneNumberIdent_Ident",
                table: "AppAuthUserPhoneNumberIdent",
                column: "Ident");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppAuthUserPhoneNumberIdent",
                table: "AppAuthUserPhoneNumberIdent",
                columns: new[] { "Ident", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_AppAuthUserPhoneNumberIdent_Ident",
                table: "AppAuthUserPhoneNumberIdent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppAuthUserPhoneNumberIdent",
                table: "AppAuthUserPhoneNumberIdent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppAuthUserPhoneNumberIdent",
                table: "AppAuthUserPhoneNumberIdent",
                column: "Ident");
        }
    }
}
