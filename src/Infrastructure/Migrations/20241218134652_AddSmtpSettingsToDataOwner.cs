using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSmtpSettingsToDataOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SmtpSettings",
                table: "DataOwner",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "SmtpSettings", table: "DataOwner");
        }
    }
}
