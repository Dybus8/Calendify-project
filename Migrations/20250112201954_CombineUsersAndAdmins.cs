using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StarterKit.Migrations
{
    /// <inheritdoc />
    public partial class CombineUsersAndAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_User_UserId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Attendance_Event_EventId",
                table: "Event_Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Attendance_User_UserId",
                table: "Event_Attendance");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event_Attendance",
                table: "Event_Attendance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendance",
                table: "Attendance");

            migrationBuilder.RenameTable(
                name: "Event_Attendance",
                newName: "EventAttendances");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "Attendance",
                newName: "Attendances");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EventAttendances",
                newName: "UserAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_Attendance_UserId",
                table: "EventAttendances",
                newName: "IX_EventAttendances_UserAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_Attendance_EventId",
                table: "EventAttendances",
                newName: "IX_EventAttendances_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendance_UserId",
                table: "Attendances",
                newName: "IX_Attendances_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "Event_AttendanceId",
                table: "EventAttendances",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EventAttendances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendances",
                table: "EventAttendances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "AttendanceId");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    RecuringDays = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserAccounts",
                columns: new[] { "Id", "Email", "FirstName", "IsAdmin", "LastName", "Password", "RecuringDays", "UserName" },
                values: new object[,]
                {
                    { 1, "admin1@example.com", "Admin", true, "One", "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8", "mo,tu,we,th,fr", "admin1" },
                    { 2, "admin2@example.com", "Admin", true, "Two", "5c4e1640360113f89d47d5ca41653d050e6a5f8ee6612530a351067f55aa9e5c", "mo,tu,we,th,fr", "admin2" },
                    { 3, "admin3@example.com", "Admin", true, "Three", "936a185caaa266bb9cbe981e9e05cb78cd732b0b3280eb944412bb6f8f8f07af", "mo,tu,we,th,fr", "admin3" },
                    { 4, "admin4@example.com", "Admin", true, "Four", "925d2e9bb3679c1b9dd58ab20bb974fdc61f3ff4db5e12bb54fab0600261c7b3", "mo,tu,we,th,fr", "admin4" },
                    { 5, "admin5@example.com", "Admin", true, "Five", "45a03d81e9f63aae2d90c8dc1e996764031bf68982a86246f0119b38305de791", "mo,tu,we,th,fr", "admin5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_UserName",
                table: "UserAccounts",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_UserAccounts_UserId",
                table: "Attendances",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendances_Events_EventId",
                table: "EventAttendances",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendances_UserAccounts_UserAccountId",
                table: "EventAttendances",
                column: "UserAccountId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_UserAccounts_UserId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendances_Events_EventId",
                table: "EventAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendances_UserAccounts_UserAccountId",
                table: "EventAttendances");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendances",
                table: "EventAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EventAttendances");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "EventAttendances",
                newName: "Event_Attendance");

            migrationBuilder.RenameTable(
                name: "Attendances",
                newName: "Attendance");

            migrationBuilder.RenameColumn(
                name: "UserAccountId",
                table: "Event_Attendance",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendances_UserAccountId",
                table: "Event_Attendance",
                newName: "IX_Event_Attendance_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendances_EventId",
                table: "Event_Attendance",
                newName: "IX_Event_Attendance_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_UserId",
                table: "Attendance",
                newName: "IX_Attendance_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "Event_AttendanceId",
                table: "Event_Attendance",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event_Attendance",
                table: "Event_Attendance",
                column: "Event_AttendanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendance",
                table: "Attendance",
                column: "AttendanceId");

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    RecuringDays = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "AdminId", "Email", "Password", "UserName" },
                values: new object[,]
                {
                    { 1, "admin1@example.com", "^�H��(qQ��o��)'s`=\rj���*�rB�", "admin1" },
                    { 2, "admin2@example.com", "\\N@6��G��Ae=j_��a%0�QU��\\", "admin2" },
                    { 3, "admin3@example.com", "�j\\��f������x�s+2��D�o���", "admin3" },
                    { 4, "admin4@example.com", "�].��g��Պ��t��?��^�T��`aǳ", "admin4" },
                    { 5, "admin5@example.com", "E�=���:�-����gd����bF��80]�", "admin5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_UserName",
                table: "Admin",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_User_UserId",
                table: "Attendance",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Attendance_Event_EventId",
                table: "Event_Attendance",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Attendance_User_UserId",
                table: "Event_Attendance",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
