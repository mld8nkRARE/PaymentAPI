using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken_and_FixUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payments_asp_net_users_user_id1",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_payments_user_id1",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id1",
                table: "payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id1",
                table: "payments",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_payments_asp_net_users_user_id1",
                table: "payments",
                column: "user_id1",
                principalTable: "AspNetUsers",
                principalColumn: "id");
        }
    }
}
