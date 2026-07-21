using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MonthlyReplenishmentRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RecommendedForMonth",
                table: "replenishment_recommendations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_replenishment_recommendations_ProductId_RecommendedForMonth",
                table: "replenishment_recommendations",
                columns: new[] { "ProductId", "RecommendedForMonth" });

            migrationBuilder.DropIndex(
                name: "IX_replenishment_recommendations_ProductId_GeneratedAtUtc",
                table: "replenishment_recommendations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_replenishment_recommendations_ProductId_GeneratedAtUtc",
                table: "replenishment_recommendations",
                columns: new[] { "ProductId", "GeneratedAtUtc" });

            migrationBuilder.DropIndex(
                name: "IX_replenishment_recommendations_ProductId_RecommendedForMonth",
                table: "replenishment_recommendations");

            migrationBuilder.DropColumn(
                name: "RecommendedForMonth",
                table: "replenishment_recommendations");

        }
    }
}
