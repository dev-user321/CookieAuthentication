﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookieAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBasketTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Baskets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Baskets");
        }
    }
}
