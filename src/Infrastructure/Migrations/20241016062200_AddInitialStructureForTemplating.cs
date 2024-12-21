using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialStructureForTemplating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Source = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataOwner", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "TemplateType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateType", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    TemplateTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataOwnerId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailTemplate_DataOwner_DataOwnerId",
                        column: x => x.DataOwnerId,
                        principalTable: "DataOwner",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_EmailTemplate_TemplateType_TemplateTypeId",
                        column: x => x.TemplateTypeId,
                        principalTable: "TemplateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "EmailBodyContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    EmailTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Body = table.Column<byte[]>(type: "bytea", nullable: false),
                    JsonStructure = table.Column<byte[]>(type: "bytea", nullable: false),
                    CultureCode = table.Column<string>(type: "text", nullable: false),
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
                    MergeTags = table.Column<string>(type: "jsonb", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBodyContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailBodyContent_EmailTemplate_EmailTemplateId",
                        column: x => x.EmailTemplateId,
                        principalTable: "EmailTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "EmailPreset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "IX_DataOwner_Name_Source",
                table: "DataOwner",
                columns: new[] { "Name", "Source" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailBodyContent_CultureCode_EmailTemplateId",
                table: "EmailBodyContent",
                columns: new[] { "CultureCode", "EmailTemplateId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_EmailBodyContent_EmailTemplateId",
                table: "EmailBodyContent",
                column: "EmailTemplateId"
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

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_DataOwnerId",
                table: "EmailTemplate",
                column: "DataOwnerId"
            );

            migrationBuilder
                .CreateIndex(
                    name: "IX_EmailTemplate_Name_DataOwnerId",
                    table: "EmailTemplate",
                    columns: new[] { "Name", "DataOwnerId" },
                    unique: true
                )
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_TemplateTypeId",
                table: "EmailTemplate",
                column: "TemplateTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TemplateType_Name",
                table: "TemplateType",
                column: "Name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EmailBodyContent");

            migrationBuilder.DropTable(name: "EmailPreset");

            migrationBuilder.DropTable(name: "EmailTemplate");

            migrationBuilder.DropTable(name: "DataOwner");

            migrationBuilder.DropTable(name: "TemplateType");
        }
    }
}
