using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Src.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeduserfullNametorecipeintname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress_CreatedOn",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_UpdatedOn",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_UserFullName",
                table: "Orders",
                newName: "ShippingAddress_RecipientName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingAddress_RecipientName",
                table: "Orders",
                newName: "ShippingAddress_UserFullName");

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingAddress_CreatedOn",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddress_Id",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingAddress_UpdatedOn",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }
    }
}
