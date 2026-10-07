using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PillarUtils.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifSentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationDate",
                table: "ArchiveItem",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationDate",
                table: "ArchiveItem");
        }
    }
}
