using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailPresetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EmailPreset");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "DataOwner",
                type: "jsonb",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Data", table: "DataOwner");

            migrationBuilder.CreateTable(
                name: "EmailPreset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Stamp = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "jsonb", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPreset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailPreset_DataOwner_DataOwnerId",
                        column: x => x.DataOwnerId,
                        principalTable: "DataOwner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_EmailPreset_EmailTemplate_EmailTemplateId",
                        column: x => x.EmailTemplateId,
                        principalTable: "EmailTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailPreset_DataOwnerId",
                table: "EmailPreset",
                column: "DataOwnerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailPreset_EmailTemplateId",
                table: "EmailPreset",
                column: "EmailTemplateId"
            );
        }
    }
}
