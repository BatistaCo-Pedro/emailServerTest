﻿// <auto-generated />
using System;
using App.Server.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.Server.Notification.Infrastructure.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    [Migration("20241016062200_AddInitialStructureForTemplating")]
    partial class AddInitialStructureForTemplating
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.DataOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name", "Source")
                        .IsUnique();

                    b.ToTable("DataOwner");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailBodyContent", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<byte[]>("Body")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CultureCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("EmailTemplateId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("JsonStructure")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("Stamp")
                        .HasColumnType("integer");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmailTemplateId");

                    b.HasIndex("CultureCode", "EmailTemplateId")
                        .IsUnique();

                    b.ToTable("EmailBodyContent");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailPreset", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<Guid>("DataOwnerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EmailTemplateId")
                        .HasColumnType("uuid");

                    b.Property<int>("Stamp")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DataOwnerId");

                    b.HasIndex("EmailTemplateId");

                    b.ToTable("EmailPreset");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<Guid?>("DataOwnerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCustom")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Stamp")
                        .HasColumnType("integer");

                    b.Property<Guid>("TemplateTypeId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DataOwnerId");

                    b.HasIndex("TemplateTypeId");

                    b.HasIndex("Name", "DataOwnerId")
                        .IsUnique();

                    NpgsqlIndexBuilderExtensions.AreNullsDistinct(b.HasIndex("Name", "DataOwnerId"), false);

                    b.ToTable("EmailTemplate");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.TemplateType", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("TemplateType");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailBodyContent", b =>
                {
                    b.HasOne("App.Server.Notification.Application.Domain.Entities.EmailTemplate", "EmailTemplate")
                        .WithMany("EmailBodyContents")
                        .HasForeignKey("EmailTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("App.Server.Notification.Application.Domain.Entities.JsonEntities.MergeTag", "MergeTags", b1 =>
                        {
                            b1.Property<Guid>("EmailBodyContentId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "name");

                            b1.Property<string>("ShortCode")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "shortCode");

                            b1.Property<string>("TypeName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "typeName");

                            b1.HasKey("EmailBodyContentId", "Id");

                            b1.ToTable("EmailBodyContent");

                            b1.ToJson("MergeTags");

                            b1.WithOwner()
                                .HasForeignKey("EmailBodyContentId");
                        });

                    b.Navigation("EmailTemplate");

                    b.Navigation("MergeTags");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailPreset", b =>
                {
                    b.HasOne("App.Server.Notification.Application.Domain.Entities.DataOwner", "DataOwner")
                        .WithMany()
                        .HasForeignKey("DataOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Server.Notification.Application.Domain.Entities.EmailTemplate", "EmailTemplate")
                        .WithMany("EmailPresets")
                        .HasForeignKey("EmailTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("App.Server.Notification.Application.Domain.Entities.JsonEntities.CustomMergeTag", "Data", b1 =>
                        {
                            b1.Property<Guid>("EmailPresetId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "name");

                            b1.Property<string>("ShortCode")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "shortCode");

                            b1.Property<string>("StringValue")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "stringValue");

                            b1.Property<string>("TypeName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasAnnotation("Relational:JsonPropertyName", "typeName");

                            b1.HasKey("EmailPresetId", "Id");

                            b1.ToTable("EmailPreset");

                            b1.ToJson("Data");

                            b1.WithOwner()
                                .HasForeignKey("EmailPresetId");
                        });

                    b.Navigation("Data");

                    b.Navigation("DataOwner");

                    b.Navigation("EmailTemplate");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailTemplate", b =>
                {
                    b.HasOne("App.Server.Notification.Application.Domain.Entities.DataOwner", "DataOwner")
                        .WithMany()
                        .HasForeignKey("DataOwnerId");

                    b.HasOne("App.Server.Notification.Application.Domain.Entities.TemplateType", "TemplateType")
                        .WithMany()
                        .HasForeignKey("TemplateTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DataOwner");

                    b.Navigation("TemplateType");
                });

            modelBuilder.Entity("App.Server.Notification.Application.Domain.Entities.EmailTemplate", b =>
                {
                    b.Navigation("EmailBodyContents");

                    b.Navigation("EmailPresets");
                });
#pragma warning restore 612, 618
        }
    }
}
