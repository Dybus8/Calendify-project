using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StarterKit.Migrations
{
    /// <inheritdoc />
    public partial class SeedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "AdminApproval", "Description", "EndTime", "EventDate", "Location", "StartTime", "Title" },
                values: new object[,]
                {
                    { 1, true, "Description for Event One", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location One", new TimeSpan(0, 10, 0, 0, 0), "Event One" },
                    { 2, true, "Description for Event Two", new TimeSpan(0, 16, 0, 0, 0), new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location Two", new TimeSpan(0, 14, 0, 0, 0), "Event Two" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "EventId",
                keyValue: 2);
        }
    }
}
