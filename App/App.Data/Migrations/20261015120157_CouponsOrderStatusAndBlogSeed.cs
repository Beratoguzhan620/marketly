using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace App.Data.Migrations
{
    /// <inheritdoc />
    public partial class CouponsOrderStatusAndBlogSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelBlogCategories_Blogs_BlogId",
                table: "RelBlogCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_RelBlogTags_Blogs_BlogId",
                table: "RelBlogTags");

            migrationBuilder.DropIndex(
                name: "IX_RelBlogTags_BlogId",
                table: "RelBlogTags");

            migrationBuilder.DeleteData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.AlterColumn<string>(
                name: "ResetPasswordToken",
                table: "Users",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordTokenExpiresAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Orders",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Orders",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUpdatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DiscountRate = table.Column<byte>(type: "tinyint", nullable: false),
                    MinimumOrderTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.CheckConstraint("CK_Coupons_DiscountRate", "[DiscountRate] BETWEEN 1 AND 90");
                });

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Tarifler");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Mevsim rehberi");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Üretici hikâyeleri");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Beslenme");

            migrationBuilder.InsertData(
                table: "BlogComments",
                columns: new[] { "Id", "BlogId", "Comment", "CreatedAt", "Email", "IsApproved", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Fırında yumurtalı sebze fikri çok işime yaradı, misafirler bayıldı.", new DateTime(2026, 1, 19, 9, 0, 0, 0, DateTimeKind.Utc), "buyer@example.com", true, "Zeynep Kaya" },
                    { 2, 2, "Elmaları ayrı saklamayı bilmiyordum, teşekkürler.", new DateTime(2026, 1, 24, 9, 0, 0, 0, DateTimeKind.Utc), "burak.sahin@example.com", true, "Burak Şahin" },
                    { 3, 2, "Muz sapı yöntemi gerçekten işe yarıyor.", new DateTime(2026, 1, 25, 9, 0, 0, 0, DateTimeKind.Utc), "elif.arslan@example.com", true, "Elif Arslan" }
                });

            migrationBuilder.InsertData(
                table: "BlogComments",
                columns: new[] { "Id", "BlogId", "Comment", "CreatedAt", "Email", "Name" },
                values: new object[] { 4, 5, "Islatma suyunu dökmek gerekiyor mu, emin değildim; artık netleşti.", new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), "deniz.celik@example.com", "Deniz Çelik" });

            migrationBuilder.InsertData(
                table: "BlogTags",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "kahvaltı" },
                    { 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "meyve" },
                    { 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "sebze" },
                    { 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "saklama" },
                    { 5, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "yerel üretim" },
                    { 6, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "pratik" }
                });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Kalabalık bir sofrada her şeyi aynı anda sıcak tutmak zordur. Önceden hazırlanabilen tarifler seçmek işi çok kolaylaştırır.\n\nFırında yumurtalı sebze, bir gece önceden hazırlanan yulaf kavanozları ve büyük bir tepside kızartılmış domates biber bunların başında gelir.\n\nPeynir ve zeytin tabağını servisten yarım saat önce hazırlayın; oda sıcaklığına gelen peynirin tadı daha iyi çıkar. Meyveleri en son doğrayın ki kararmasın.", new DateTime(2026, 1, 18, 9, 0, 0, 0, DateTimeKind.Utc), null, "Kalabalık kahvaltı için 6 pratik fikir", 2 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Portakal, mandalina ve elma kışın en çok tüketilen meyvelerdir. Doğru saklandıklarında haftalarca taze kalırlar.\n\nNarenciyeyi buzdolabının sebzeliğinde, poşetsiz saklayın. Elmalar etilen gazı saldığı için diğer meyvelerden ayrı durmalıdır.\n\nMuzu sapından streç filmle sarmak olgunlaşmayı birkaç gün yavaşlatır. Ezilen meyveleri ayırın; tek bir çürük meyve bütün kasayı etkiler.", new DateTime(2026, 1, 23, 9, 0, 0, 0, DateTimeKind.Utc), null, "Kış meyvelerini daha uzun saklamanın yolları", 4 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Siyah üzümlerimizin geldiği bağda hasat sabah erken başlar. Güneş yükselmeden kesilen salkımlar daha uzun dayanır.\n\nBağın sahibi Hasan Bey, üç kuşaktır aynı topraklarda çalışıyor. Kimyasal ilaç yerine yaprak bakımına ve havalandırmaya güveniyor.\n\nHasat edilen üzümler aynı gün soğuk hava deposuna giriyor ve ertesi gün Marketly üzerinden sipariş veren müşterilere yola çıkıyor.", new DateTime(2026, 1, 29, 9, 0, 0, 0, DateTimeKind.Utc), null, "Ege'de bir bağ bozumu günü", 2 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Ispanak, demir ve folat açısından zengin bir yapraklı sebzedir; ancak uzun süre haşlamak bu değerlerin bir kısmını suya geçirir.\n\nKısa süre buharda pişirmek ya da az zeytinyağında soteleyip hemen ocaktan almak en iyi yöntemdir. Üzerine sıkılan birkaç damla limon demirin emilimini artırır.\n\nTaze ıspanağı yıkamadan, kâğıt havluya sararak buzdolabında saklayın; nem yaprakların çabuk solmasına neden olur.", new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), null, "Ispanağı besin değerini kaybetmeden pişirmek", 4 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Nohut ve kuru fasulyeyi bir gece önceden ıslatmak pişirme süresini neredeyse yarıya indirir ve sindirimi kolaylaştırır.\n\nMercimek ve bulgur için ıslatmaya gerek yoktur; yıkamak yeterlidir. Islatma suyunu dökmek, gaz yapan şekerlerin bir kısmını uzaklaştırır.\n\nZaman kısıtlıysa bakliyatı kaynar suda beş dakika haşlayıp bir saat dinlendirmek de iş görür.", new DateTime(2026, 2, 8, 9, 0, 0, 0, DateTimeKind.Utc), null, "Bakliyatı ıslatmak gerçekten gerekli mi?", 2 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Ürün sofraya ne kadar kısa yoldan gelirse o kadar taze kalır. Yerel üreticiden alışveriş, depoda geçen günleri ortadan kaldırır.\n\nÜretici de aracı payı ödemediği için emeğinin karşılığını daha adil alır. Marketly'de her ürünün satıcısını görebilmenizin nedeni budur.\n\nMevsiminde alışveriş yapmak hem lezzet hem bütçe için en iyi seçimdir.", new DateTime(2026, 2, 13, 9, 0, 0, 0, DateTimeKind.Utc), null, "Yerel üreticiden almak neyi değiştirir?", 4 });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "DiscountRate", "Enabled", "EndDate", "MinimumOrderTotal", "StartDate", "UsageLimit" },
                values: new object[,]
                {
                    { 1, "HOSGELDIN10", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), (byte)10, true, new DateTime(2029, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 200m, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, "BUYUKSEPET20", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), (byte)20, true, new DateTime(2029, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 750m, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 500 },
                    { 3, "KIS2026", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), (byte)15, true, new DateTime(2026, 3, 15, 9, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CouponCode", "Status", "StatusUpdatedAt" },
                values: new object[] { null, (byte)2, new DateTime(2026, 1, 31, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CouponCode", "Status", "StatusUpdatedAt" },
                values: new object[] { null, (byte)2, new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CouponCode", "Status", "StatusUpdatedAt" },
                values: new object[] { null, (byte)2, new DateTime(2026, 2, 6, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CouponCode", "Status", "StatusUpdatedAt" },
                values: new object[] { null, (byte)1, new DateTime(2026, 2, 8, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CouponCode", "StatusUpdatedAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CouponCode", "Status", "StatusUpdatedAt" },
                values: new object[] { null, (byte)3, new DateTime(2026, 2, 13, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "RelBlogCategories",
                columns: new[] { "Id", "BlogId", "CategoryId", "CreatedAt" },
                values: new object[,]
                {
                    { 11, 1, 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 2, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 3, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 4, 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 4, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 5, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 6, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "ResetPasswordTokenExpiresAt",
                value: null);

            migrationBuilder.InsertData(
                table: "RelBlogTags",
                columns: new[] { "Id", "BlogId", "CreatedAt", "TagId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 3, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 4, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 5, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 5 },
                    { 6, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 7, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 8, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 9, 5, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 10, 6, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelBlogTags_BlogId_TagId",
                table: "RelBlogTags",
                columns: new[] { "BlogId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BlogTags_Name",
                table: "BlogTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RelBlogCategories_Blogs_BlogId",
                table: "RelBlogCategories",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RelBlogTags_Blogs_BlogId",
                table: "RelBlogTags",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelBlogCategories_Blogs_BlogId",
                table: "RelBlogCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_RelBlogTags_Blogs_BlogId",
                table: "RelBlogTags");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_RelBlogTags_BlogId_TagId",
                table: "RelBlogTags");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_BlogTags_Name",
                table: "BlogTags");

            migrationBuilder.DeleteData(
                table: "BlogComments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BlogComments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BlogComments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BlogComments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "RelBlogCategories",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RelBlogTags",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "BlogTags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BlogTags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BlogTags",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BlogTags",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BlogTags",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "BlogTags",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpiresAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StatusUpdatedAt",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "ResetPasswordToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Beauty");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Food");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Life Style");

            migrationBuilder.UpdateData(
                table: "BlogCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Travel");

            migrationBuilder.InsertData(
                table: "BlogCategories",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 5, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Fashion" },
                    { 7, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Education" },
                    { 8, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Entertainment" },
                    { 9, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Sports" }
                });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Visiting a clean farm in the US is an enlightening experience. The farm, with its well-maintained fields and healthy livestock, showcases the best of American agriculture. The cleanliness of the farm is a testament to the hard work and dedication of the farmers. They follow strict hygiene and cleanliness standards to ensure the health and safety of their animals and crops. The farm also uses sustainable farming practices to protect the environment. A visit to such a farm is not only educational but also inspiring, as it shows the importance of cleanliness and sustainability in farming.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "/theme/img/blog/blog-3.jpg", "Visit the clean farm in the US", 1 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Preparing breakfast for a large group requires careful planning and organization. Start by choosing recipes that can be made in large quantities, such as scrambled eggs, pancakes, or a breakfast casserole. Consider making items that can be prepared ahead of time, like muffins or fruit salad. Set up a serving station with all the necessary utensils, plates, and cups. Cook as much as you can in advance to minimize stress on the day of the event. Remember to cater to different dietary needs and preferences. With these tips, you'll be able to host a successful breakfast for 30 people.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "/theme/img/blog/blog-2.jpg", "6 ways to prepare breakfast for 30", 1 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Cooking can be a daunting task for many, especially when it comes to preparing meals for a family or a large group. However, with the right tips and techniques, it can be made simple and enjoyable. Start by planning your meals in advance and doing a weekly grocery shop. This not only saves time but also ensures you have all the ingredients you need. Use fresh ingredients wherever possible as they provide the best flavor. Don't be afraid to experiment with different herbs and spices to add a unique twist to your dishes. Remember, the key to simple cooking is preparation and using the right tools. Invest in a good set of knives, pots, and pans. Lastly, don't forget to clean as you go. This will keep your kitchen tidy and make the cooking process much more manageable.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "/theme/img/blog/blog-1.jpg", "Cooking tips make cooking simple", 1 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Organic food has become increasingly popular in recent years, with many people choosing to buy organic produce over conventionally grown food. There are several benefits to eating organic food, including better taste, higher nutritional value, and fewer pesticides. Organic farming practices are also better for the environment, as they reduce pollution and conserve water and soil. Additionally, organic food is often fresher and free from harmful additives and preservatives. While organic food can be more expensive, many people believe the health benefits are worth the extra cost. Overall, eating organic food is a great way to support your health and the environment.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "/theme/img/blog/blog-4.jpg", "The benefits of eating organic food", 1 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Fruits are an essential part of a healthy diet, providing essential vitamins, minerals, and fiber. However, it's essential to consume them in moderation and avoid overeating. While fruits are nutritious, they also contain natural sugars that can contribute to weight gain if consumed in excess. To include fruits in your diet, aim to eat a variety of colors and types to ensure you're getting a wide range of nutrients. Fresh, frozen, and dried fruits are all healthy options, but be mindful of added sugars in canned or packaged fruits. Avoid fruit juices and smoothies, as they can be high in sugar and calories. Instead, opt for whole fruits, which provide more fiber and nutrients. By following these tips, you can enjoy the health benefits of fruits without overdoing it.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "/theme/img/blog/blog-5.jpg", "How to (and not to) include fruits in your diet", 1 });

            migrationBuilder.UpdateData(
                table: "Blogs",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Content", "CreatedAt", "ImageUrl", "Title", "UserId" },
                values: new object[] { "Garlic is a popular ingredient in many dishes, known for its strong flavor and health benefits. However, there are times when you may need to remove garlic from the menu. Some people are allergic to garlic and can experience symptoms like hives, itching, or difficulty breathing after consuming it. If you're cooking for someone with a garlic allergy, it's essential to avoid using garlic in your dishes. Additionally, some people may have a sensitivity to garlic that causes digestive issues like bloating or gas. In these cases, it's best to limit or avoid garlic in your cooking. While garlic is a versatile and flavorful ingredient, it's essential to be mindful of people's dietary needs and preferences when preparing meals.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "/theme/img/blog/blog-6.jpg", "The moment You Need To Remove Garlic From The Menu", 1 });

            migrationBuilder.InsertData(
                table: "Blogs",
                columns: new[] { "Id", "Content", "CreatedAt", "Enabled", "ImageUrl", "Title", "UserId" },
                values: new object[] { 7, "Organic food has become increasingly popular in recent years, with many people choosing to buy organic produce over conventionally grown food. While organic food is often more expensive than non-organic options, there are several factors that contribute to the higher cost. Organic farming practices are more labor-intensive and require more time and effort than conventional farming methods. Additionally, organic farmers often pay higher prices for organic seeds, fertilizers, and pest control methods. The certification process for organic food is also costly, as farmers must meet strict standards to be certified organic. While the cost of organic food can be a barrier for some consumers, many people believe the health and environmental benefits are worth the extra expense.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-1.jpg", "Cost anaylsis of organic food", 1 });

            migrationBuilder.InsertData(
                table: "RelBlogCategories",
                columns: new[] { "Id", "BlogId", "CategoryId", "CreatedAt" },
                values: new object[,]
                {
                    { 1, 1, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 3, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 4, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 5, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 6, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 7, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelBlogTags_BlogId",
                table: "RelBlogTags",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_RelBlogCategories_Blogs_BlogId",
                table: "RelBlogCategories",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelBlogTags_Blogs_BlogId",
                table: "RelBlogTags",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }
    }
}
