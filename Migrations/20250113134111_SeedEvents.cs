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
                    { 2, true, "Description for Event Two", new TimeSpan(0, 16, 0, 0, 0), new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location Two", new TimeSpan(0, 14, 0, 0, 0), "Event Two" },
                    { 3, true, "Description for Event 3", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 3", new TimeSpan(0, 10, 0, 0, 0), "Event 3" },
                    { 4, true, "Description for Event 4", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 4", new TimeSpan(0, 14, 0, 0, 0), "Event 4" },
                    { 5, true, "Description for Event 5", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 5", new TimeSpan(0, 10, 0, 0, 0), "Event 5" },
                    { 6, true, "Description for Event 6", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 6", new TimeSpan(0, 14, 0, 0, 0), "Event 6" },
                    { 7, true, "Description for Event 7", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 7", new TimeSpan(0, 10, 0, 0, 0), "Event 7" },
                    { 8, true, "Description for Event 8", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 8", new TimeSpan(0, 14, 0, 0, 0), "Event 8" },
                    { 9, true, "Description for Event 9", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Location 9", new TimeSpan(0, 10, 0, 0, 0), "Event 9" },
                    { 10, true, "das Ende", new TimeSpan(0, 12, 0, 0, 0), new DateTime(2025, 1, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), "WN.01.017", new TimeSpan(0, 0, 30, 0, 0), "The Reckoning" }
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
