using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApp.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MemberEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR", maxLength: 36, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "DATETIME2", nullable: false),
                    ImageUrl = table.Column<string>(type: "NVARCHAR", nullable: true),
                    DisplayName = table.Column<string>(type: "NVARCHAR", maxLength: 100, nullable: false),
                    Created = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    LastActive = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Gender = table.Column<string>(type: "NVARCHAR", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR", maxLength: 250, nullable: true),
                    City = table.Column<string>(type: "NVARCHAR", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "NVARCHAR", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "NVARCHAR", nullable: false),
                    PublicId = table.Column<string>(type: "NVARCHAR", nullable: true),
                    MemberId = table.Column<string>(type: "NVARCHAR", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_MemberId",
                table: "Photos",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Users");
        }
    }
}
