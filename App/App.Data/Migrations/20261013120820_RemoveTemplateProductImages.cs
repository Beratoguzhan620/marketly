using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace App.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemplateProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 42);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "CreatedAt", "FileKey", "ProductId", "Url" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 1, "/theme/img/product/product-2.jpg" },
                    { 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 2, "/theme/img/product/product-2.jpg" },
                    { 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 3, "/theme/img/product/product-3.jpg" },
                    { 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 4, "/theme/img/product/product-3.jpg" },
                    { 5, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 5, "/theme/img/product/product-4.jpg" },
                    { 6, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 6, "/theme/img/product/product-4.jpg" },
                    { 7, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 7, "/theme/img/product/product-6.jpg" },
                    { 8, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 8, "/theme/img/product/product-6.jpg" },
                    { 9, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 9, "/theme/img/product/product-7.jpg" },
                    { 10, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 10, "/theme/img/product/product-7.jpg" },
                    { 11, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 11, "/theme/img/product/product-8.jpg" },
                    { 12, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 12, "/theme/img/product/product-8.jpg" },
                    { 13, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 13, "/theme/img/categories/cat-1.jpg" },
                    { 14, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 14, "/theme/img/categories/cat-1.jpg" },
                    { 15, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 15, "/theme/img/product/product-12.jpg" },
                    { 16, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 16, "/theme/img/product/product-12.jpg" },
                    { 17, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 17, "/theme/img/latest-product/lp-1.jpg" },
                    { 18, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 18, "/theme/img/latest-product/lp-1.jpg" },
                    { 19, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 19, "/theme/img/latest-product/lp-2.jpg" },
                    { 20, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 20, "/theme/img/latest-product/lp-2.jpg" },
                    { 21, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 21, "/theme/img/latest-product/lp-2.jpg" },
                    { 22, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 22, "/theme/img/latest-product/lp-2.jpg" },
                    { 23, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 23, "/theme/img/product/discount/pd-2.jpg" },
                    { 24, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 24, "/theme/img/categories/cat-3.jpg" },
                    { 25, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 25, "/theme/img/product/discount/pd-2.jpg" },
                    { 26, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 26, "/theme/img/categories/cat-3.jpg" },
                    { 27, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 27, "/theme/img/product/product-1.jpg" },
                    { 28, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 28, "/theme/img/featured/feature-1.jpg" },
                    { 29, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 29, "/theme/img/product/product-1.jpg" },
                    { 30, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 30, "/theme/img/featured/feature-1.jpg" },
                    { 31, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 31, "/theme/img/product/product-1.jpg" },
                    { 32, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 32, "/theme/img/product/product-10.jpg" },
                    { 33, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 33, "/theme/img/latest-product/lp-3.jpg" },
                    { 34, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 34, "/theme/img/product/product-10.jpg" },
                    { 35, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 35, "/theme/img/latest-product/lp-3.jpg" },
                    { 36, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 36, "/theme/img/product/product-10.jpg" },
                    { 37, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 37, "/theme/img/product/product-5.jpg" },
                    { 38, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 38, "/theme/img/featured/feature-6.jpg" },
                    { 39, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 39, "/theme/img/product/product-5.jpg" },
                    { 40, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 40, "/theme/img/featured/feature-6.jpg" },
                    { 41, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 41, "/theme/img/product/product-5.jpg" },
                    { 42, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, 42, "/theme/img/featured/feature-6.jpg" }
                });
        }
    }
}
