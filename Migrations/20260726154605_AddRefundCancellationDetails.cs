using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundCancellationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds");

            migrationBuilder.DropTable(
                name: "receipt_items");

            migrationBuilder.DropTable(
                name: "receipts");

            migrationBuilder.AddColumn<string>(
                name: "cancellation_party",
                table: "refunds",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cancellation_reason",
                table: "refunds",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "refunds",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id1",
                table: "payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expire_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_refunds_external_refund_id",
                table: "refunds",
                column: "external_refund_id",
                unique: true,
                filter: "\"external_refund_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id1",
                table: "payments",
                column: "user_id1");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_payments_asp_net_users_user_id1",
                table: "payments",
                column: "user_id1",
                principalTable: "AspNetUsers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds",
                column: "payment_id",
                principalTable: "payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payments_asp_net_users_user_id1",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "ix_refunds_external_refund_id",
                table: "refunds");

            migrationBuilder.DropIndex(
                name: "ix_payments_user_id1",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "cancellation_party",
                table: "refunds");

            migrationBuilder.DropColumn(
                name: "cancellation_reason",
                table: "refunds");

            migrationBuilder.DropColumn(
                name: "description",
                table: "refunds");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "payments");

            migrationBuilder.CreateTable(
                name: "receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    external_receipt_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fiscal_document_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fiscal_storage_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    refund_id = table.Column<Guid>(type: "uuid", nullable: true),
                    registered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_receipts", x => x.id);
                    table.ForeignKey(
                        name: "fk_receipts_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "receipt_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    payment_mode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payment_subject = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_receipt_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_receipt_items_receipts_receipt_id",
                        column: x => x.receipt_id,
                        principalTable: "receipts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_receipt_items_receipt_id",
                table: "receipt_items",
                column: "receipt_id");

            migrationBuilder.CreateIndex(
                name: "ix_receipts_payment_id",
                table: "receipts",
                column: "payment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds",
                column: "payment_id",
                principalTable: "payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
