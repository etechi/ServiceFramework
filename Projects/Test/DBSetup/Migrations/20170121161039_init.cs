using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBSetup.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAuthUser",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: true),
                    Icon = table.Column<string>(maxLength: 200, nullable: true),
                    Image = table.Column<string>(maxLength: 200, nullable: true),
                    InviterId = table.Column<long>(nullable: true),
                    LastAddress = table.Column<string>(maxLength: 100, nullable: true),
                    LastDeviceType = table.Column<int>(nullable: false),
                    LastSigninTime = table.Column<DateTime>(nullable: true),
                    LockoutEndDateUtc = table.Column<DateTime>(nullable: true),
                    NickName = table.Column<string>(maxLength: 100, nullable: true),
                    NoIdents = table.Column<bool>(nullable: false),
                    ObjectState = table.Column<byte>(nullable: false),
                    PasswordHash = table.Column<string>(maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: true),
                    SecurityStamp = table.Column<string>(maxLength: 100, nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    SigninCount = table.Column<int>(nullable: false),
                    SignupAddress = table.Column<string>(maxLength: 100, nullable: true),
                    SignupDeviceType = table.Column<int>(nullable: false),
                    SignupIdentProvider = table.Column<string>(maxLength: 50, nullable: true),
                    SignupIdentValue = table.Column<string>(maxLength: 200, nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    UserType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAuthUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppAuthUserPhoneNumberIdent",
                columns: table => new
                {
                    Ident = table.Column<string>(maxLength: 100, nullable: false),
                    ConfirmedTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAuthUserPhoneNumberIdent", x => x.Ident);
                });

            migrationBuilder.CreateTable(
                name: "AppSysIdentSeed",
                columns: table => new
                {
                    Type = table.Column<string>(nullable: false),
                    NextValue = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSysIdentSeed", x => x.Type);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUser_CreatedTime",
                table: "AppAuthUser",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUser_LastSigninTime",
                table: "AppAuthUser",
                column: "LastSigninTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUser_NoIdents",
                table: "AppAuthUser",
                column: "NoIdents");

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUser_SignupIdentProvider",
                table: "AppAuthUser",
                column: "SignupIdentProvider");

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUser_SignupIdentValue",
                table: "AppAuthUser",
                column: "SignupIdentValue");

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUser_UserType",
                table: "AppAuthUser",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "IX_AppAuthUserPhoneNumberIdent_UserId",
                table: "AppAuthUserPhoneNumberIdent",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAuthUser");

            migrationBuilder.DropTable(
                name: "AppAuthUserPhoneNumberIdent");

            migrationBuilder.DropTable(
                name: "AppSysIdentSeed");
        }
    }
}
