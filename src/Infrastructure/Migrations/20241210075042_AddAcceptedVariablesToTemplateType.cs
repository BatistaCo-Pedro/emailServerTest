using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcceptedVariablesToTemplateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcceptedMergeTags",
                table: "TemplateType",
                type: "jsonb",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TemplateType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TemplateType",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "Stamp",
                table: "TemplateType",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TemplateType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TemplateType",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DataOwner",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DataOwner",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "Stamp",
                table: "DataOwner",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DataOwner",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "DataOwner",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AcceptedMergeTags", table: "TemplateType");

            migrationBuilder.DropColumn(name: "CreatedAt", table: "TemplateType");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "TemplateType");

            migrationBuilder.DropColumn(name: "Stamp", table: "TemplateType");

            migrationBuilder.DropColumn(name: "UpdatedAt", table: "TemplateType");

            migrationBuilder.DropColumn(name: "UpdatedBy", table: "TemplateType");

            migrationBuilder.DropColumn(name: "CreatedAt", table: "DataOwner");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "DataOwner");

            migrationBuilder.DropColumn(name: "Stamp", table: "DataOwner");

            migrationBuilder.DropColumn(name: "UpdatedAt", table: "DataOwner");

            migrationBuilder.DropColumn(name: "UpdatedBy", table: "DataOwner");
        }
    }
}
