using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultEmailTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Stamp = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailSettings_DataOwner_DataOwnerId",
                        column: x => x.DataOwnerId,
                        principalTable: "DataOwner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_EmailSettings_EmailTemplate_DefaultEmailTemplateId",
                        column: x => x.DefaultEmailTemplateId,
                        principalTable: "EmailTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_EmailSettings_TemplateType_TemplateTypeId",
                        column: x => x.TemplateTypeId,
                        principalTable: "TemplateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailSettings_DataOwnerId",
                table: "EmailSettings",
                column: "DataOwnerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailSettings_DefaultEmailTemplateId",
                table: "EmailSettings",
                column: "DefaultEmailTemplateId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailSettings_TemplateTypeId_DataOwnerId_DefaultEmailTempla~",
                table: "EmailSettings",
                columns: new[] { "TemplateTypeId", "DataOwnerId", "DefaultEmailTemplateId" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EmailSettings");
        }
    }
}
