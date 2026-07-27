using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refund_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    external_receipt_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fiscal_document_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fiscal_storage_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    registered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
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
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    vat_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payment_subject = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payment_mode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
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
                name: "ix_refunds_payment_id",
                table: "refunds",
                column: "payment_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds");

            migrationBuilder.DropTable(
                name: "receipt_items");

            migrationBuilder.DropTable(
                name: "receipts");

            migrationBuilder.DropIndex(
                name: "ix_refunds_payment_id",
                table: "refunds");
        }
    }
}
