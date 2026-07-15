using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Actor = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Module = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Details = table.Column<string>(type: "varchar(1200)", maxLength: 1200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Identification = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Zone = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "forecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GeneratedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProjectedSales = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    GeneratedBy = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forecasts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductType = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reference = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Brand = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvailableUnits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Identification = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Identification = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "uploaded_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FileName = table.Column<string>(type: "varchar(260)", maxLength: 260, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileType = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UploadedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    ValidRecords = table.Column<int>(type: "int", nullable: false),
                    InvalidRecords = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uploaded_files", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "replenishment_recommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GeneratedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RecommendedUnits = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    Rationale = table.Column<string>(type: "varchar(1200)", maxLength: 1200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReviewedBy = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReviewNotes = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_replenishment_recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_replenishment_recommendations_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InvoiceNumber = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SupplierId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SellerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SaleDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SaleAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sales_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sales_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sales_sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sales_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "upload_errors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UploadedFileId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorMessage = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_upload_errors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_upload_errors_uploaded_files_UploadedFileId",
                        column: x => x.UploadedFileId,
                        principalTable: "uploaded_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Module",
                table: "audit_logs",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_OccurredAtUtc",
                table: "audit_logs",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_customers_City",
                table: "customers",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_customers_Identification",
                table: "customers",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_forecasts_GeneratedAtUtc",
                table: "forecasts",
                column: "GeneratedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_products_Reference",
                table: "products",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_replenishment_recommendations_ProductId_GeneratedAtUtc",
                table: "replenishment_recommendations",
                columns: new[] { "ProductId", "GeneratedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_replenishment_recommendations_Status",
                table: "replenishment_recommendations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_sales_CustomerId_SaleDate",
                table: "sales",
                columns: new[] { "CustomerId", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_sales_ProductId_SaleDate",
                table: "sales",
                columns: new[] { "ProductId", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_sales_SaleDate",
                table: "sales",
                column: "SaleDate");

            migrationBuilder.CreateIndex(
                name: "IX_sales_SellerId_SaleDate",
                table: "sales",
                columns: new[] { "SellerId", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_sales_SupplierId_SaleDate",
                table: "sales",
                columns: new[] { "SupplierId", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_sellers_Identification",
                table: "sellers",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Identification",
                table: "suppliers",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_upload_errors_UploadedFileId",
                table: "upload_errors",
                column: "UploadedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_uploaded_files_UploadedAtUtc",
                table: "uploaded_files",
                column: "UploadedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "forecasts");

            migrationBuilder.DropTable(
                name: "replenishment_recommendations");

            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropTable(
                name: "upload_errors");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "sellers");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "uploaded_files");
        }
    }
}
