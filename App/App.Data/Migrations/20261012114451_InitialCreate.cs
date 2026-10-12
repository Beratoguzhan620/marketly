using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace App.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconCssClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountRate = table.Column<byte>(type: "tinyint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDiscounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    HasSellerRequest = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    DiscountId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StockAmount = table.Column<byte>(type: "tinyint", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_ProductDiscounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "ProductDiscounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlogComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogComments_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelBlogCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelBlogCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelBlogCategories_BlogCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelBlogCategories_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelBlogTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelBlogTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelBlogTags_BlogTags_TagId",
                        column: x => x.TagId,
                        principalTable: "BlogTags",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RelBlogTags_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StarCount = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)3),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductComments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BlogCategories",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Beauty" },
                    { 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Food" },
                    { 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Life Style" },
                    { 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Travel" },
                    { 5, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Fashion" },
                    { 7, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Education" },
                    { 8, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Entertainment" },
                    { 9, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Sports" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "IconCssClass", "Name" },
                values: new object[,]
                {
                    { 1, "Red", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Et & Tavuk" },
                    { 2, "Green", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Sebze" },
                    { 3, "Orange", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Meyve" },
                    { 4, "Brown", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Kuruyemiş" },
                    { 5, "Blue", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Deniz Ürünleri" },
                    { 6, "Yellow", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Süt & Kahvaltılık" },
                    { 7, "Pink", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Hazır Yemek" },
                    { 8, "Grey", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "Tahıl & Bakliyat" },
                    { 9, "Purple", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "", "İçecek" }
                });

            migrationBuilder.InsertData(
                table: "ProductDiscounts",
                columns: new[] { "Id", "CreatedAt", "DiscountRate", "Enabled", "EndDate", "StartDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), (byte)10, true, new DateTime(2028, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), (byte)20, true, new DateTime(2028, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), (byte)30, true, new DateTime(2028, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "admin" },
                    { 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "seller" },
                    { 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "buyer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Enabled", "FirstName", "HasSellerRequest", "LastName", "Password", "ResetPasswordToken", "RoleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "admin@example.com", true, "Selin", false, "Yıldız", "PBKDF2$SHA256$100000$j7BG0FobB0PfS1sS6TxDoA==$SNvFyZnI3WO/52fsXHx33QuL3fFjJGFQcvnx23D0xUA=", null, 1 },
                    { 2, new DateTime(2026, 1, 16, 9, 0, 0, 0, DateTimeKind.Utc), "seller@example.com", true, "Mert", false, "Aksoy", "PBKDF2$SHA256$100000$J9NSFodmfLwohqPNv2wxnA==$Uf1R1y72X8uIFBpsvtjaB7g8Iq43SUCug6w/WCg6ios=", null, 2 },
                    { 3, new DateTime(2026, 1, 17, 9, 0, 0, 0, DateTimeKind.Utc), "buyer@example.com", true, "Zeynep", false, "Kaya", "PBKDF2$SHA256$100000$JckgF5XTvgbkX76iZTvvHA==$2VwWMQP1tChJ/1sXJO4hYdvLFUfLgEFDIXnFipkSQTQ=", null, 3 },
                    { 4, new DateTime(2026, 1, 18, 9, 0, 0, 0, DateTimeKind.Utc), "ece.demir@example.com", true, "Ece", false, "Demir", "PBKDF2$SHA256$100000$7SJR6Gh6ZuxA7DSfIUb11A==$vE6/zAnwwT2liBMsukROHCV5CbV6/QdcWAq6pNVK7n4=", null, 2 },
                    { 5, new DateTime(2026, 1, 19, 9, 0, 0, 0, DateTimeKind.Utc), "can.ozturk@example.com", true, "Can", false, "Öztürk", "PBKDF2$SHA256$100000$bTZmf1M+6AS7I3dAk9F75Q==$n93i2P5h6HDQydpjHwVDYL2+xVMGQ3CUD5BZfu7uQTU=", null, 2 },
                    { 6, new DateTime(2026, 1, 20, 9, 0, 0, 0, DateTimeKind.Utc), "burak.sahin@example.com", true, "Burak", false, "Şahin", "PBKDF2$SHA256$100000$oZWe2/+ZtrZHyZKGcraqyg==$V/j+Gg3u5PpNKcoanoBLpfDmzQnjTu6sCzPrL9z3v2w=", null, 3 },
                    { 7, new DateTime(2026, 1, 21, 9, 0, 0, 0, DateTimeKind.Utc), "elif.arslan@example.com", true, "Elif", false, "Arslan", "PBKDF2$SHA256$100000$98Ew7jvHIeazB1EZMM9LkQ==$ya2mAmHGwwDtyynoGCt2KdVZvNXEbBAsRI8EdHdrTFU=", null, 3 },
                    { 8, new DateTime(2026, 1, 22, 9, 0, 0, 0, DateTimeKind.Utc), "deniz.celik@example.com", true, "Deniz", false, "Çelik", "PBKDF2$SHA256$100000$FbdScsnyf1oZdqyp4m0XFg==$Mh+1x/gZsfVHerhelZF9/3oquThA9i0WGeilIwgP7Ac=", null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Blogs",
                columns: new[] { "Id", "Content", "CreatedAt", "Enabled", "ImageUrl", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, "Visiting a clean farm in the US is an enlightening experience. The farm, with its well-maintained fields and healthy livestock, showcases the best of American agriculture. The cleanliness of the farm is a testament to the hard work and dedication of the farmers. They follow strict hygiene and cleanliness standards to ensure the health and safety of their animals and crops. The farm also uses sustainable farming practices to protect the environment. A visit to such a farm is not only educational but also inspiring, as it shows the importance of cleanliness and sustainability in farming.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-3.jpg", "Visit the clean farm in the US", 1 },
                    { 2, "Preparing breakfast for a large group requires careful planning and organization. Start by choosing recipes that can be made in large quantities, such as scrambled eggs, pancakes, or a breakfast casserole. Consider making items that can be prepared ahead of time, like muffins or fruit salad. Set up a serving station with all the necessary utensils, plates, and cups. Cook as much as you can in advance to minimize stress on the day of the event. Remember to cater to different dietary needs and preferences. With these tips, you'll be able to host a successful breakfast for 30 people.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-2.jpg", "6 ways to prepare breakfast for 30", 1 },
                    { 3, "Cooking can be a daunting task for many, especially when it comes to preparing meals for a family or a large group. However, with the right tips and techniques, it can be made simple and enjoyable. Start by planning your meals in advance and doing a weekly grocery shop. This not only saves time but also ensures you have all the ingredients you need. Use fresh ingredients wherever possible as they provide the best flavor. Don't be afraid to experiment with different herbs and spices to add a unique twist to your dishes. Remember, the key to simple cooking is preparation and using the right tools. Invest in a good set of knives, pots, and pans. Lastly, don't forget to clean as you go. This will keep your kitchen tidy and make the cooking process much more manageable.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-1.jpg", "Cooking tips make cooking simple", 1 },
                    { 4, "Organic food has become increasingly popular in recent years, with many people choosing to buy organic produce over conventionally grown food. There are several benefits to eating organic food, including better taste, higher nutritional value, and fewer pesticides. Organic farming practices are also better for the environment, as they reduce pollution and conserve water and soil. Additionally, organic food is often fresher and free from harmful additives and preservatives. While organic food can be more expensive, many people believe the health benefits are worth the extra cost. Overall, eating organic food is a great way to support your health and the environment.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-4.jpg", "The benefits of eating organic food", 1 },
                    { 5, "Fruits are an essential part of a healthy diet, providing essential vitamins, minerals, and fiber. However, it's essential to consume them in moderation and avoid overeating. While fruits are nutritious, they also contain natural sugars that can contribute to weight gain if consumed in excess. To include fruits in your diet, aim to eat a variety of colors and types to ensure you're getting a wide range of nutrients. Fresh, frozen, and dried fruits are all healthy options, but be mindful of added sugars in canned or packaged fruits. Avoid fruit juices and smoothies, as they can be high in sugar and calories. Instead, opt for whole fruits, which provide more fiber and nutrients. By following these tips, you can enjoy the health benefits of fruits without overdoing it.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-5.jpg", "How to (and not to) include fruits in your diet", 1 },
                    { 6, "Garlic is a popular ingredient in many dishes, known for its strong flavor and health benefits. However, there are times when you may need to remove garlic from the menu. Some people are allergic to garlic and can experience symptoms like hives, itching, or difficulty breathing after consuming it. If you're cooking for someone with a garlic allergy, it's essential to avoid using garlic in your dishes. Additionally, some people may have a sensitivity to garlic that causes digestive issues like bloating or gas. In these cases, it's best to limit or avoid garlic in your cooking. While garlic is a versatile and flavorful ingredient, it's essential to be mindful of people's dietary needs and preferences when preparing meals.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-6.jpg", "The moment You Need To Remove Garlic From The Menu", 1 },
                    { 7, "Organic food has become increasingly popular in recent years, with many people choosing to buy organic produce over conventionally grown food. While organic food is often more expensive than non-organic options, there are several factors that contribute to the higher cost. Organic farming practices are more labor-intensive and require more time and effort than conventional farming methods. Additionally, organic farmers often pay higher prices for organic seeds, fertilizers, and pest control methods. The certification process for organic food is also costly, as farmers must meet strict standards to be certified organic. While the cost of organic food can be a barrier for some consumers, many people believe the health and environmental benefits are worth the extra expense.", new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "/theme/img/blog/blog-1.jpg", "Cost anaylsis of organic food", 1 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "Address", "CreatedAt", "OrderCode", "UserId" },
                values: new object[,]
                {
                    { 1, "Caferağa Mah. Moda Cad. No: 12 Kadıköy / İstanbul", new DateTime(2026, 1, 28, 9, 0, 0, 0, DateTimeKind.Utc), "MK-2026-0001", 3 },
                    { 2, "Kızılay Mah. Atatürk Blv. No: 45 Çankaya / Ankara", new DateTime(2026, 1, 31, 9, 0, 0, 0, DateTimeKind.Utc), "MK-2026-0002", 6 },
                    { 3, "Alsancak Mah. Kıbrıs Şehitleri Cad. No: 8 Konak / İzmir", new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), "MK-2026-0003", 7 },
                    { 4, "Caferağa Mah. Moda Cad. No: 12 Kadıköy / İstanbul", new DateTime(2026, 2, 6, 9, 0, 0, 0, DateTimeKind.Utc), "MK-2026-0004", 3 },
                    { 5, "Bahçelievler Mah. Dalyan Sok. No: 3 Nilüfer / Bursa", new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), "MK-2026-0005", 8 },
                    { 6, "Kızılay Mah. Atatürk Blv. No: 45 Çankaya / Ankara", new DateTime(2026, 2, 12, 9, 0, 0, 0, DateTimeKind.Utc), "MK-2026-0006", 6 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DiscountId", "Enabled", "Name", "Price", "SellerId", "StockAmount" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 1, 16, 9, 0, 0, 0, DateTimeKind.Utc), "İthal Cavendish muz, 1 kg. Olgunlaşmaya yakın toplanır, oda sıcaklığında 2-3 günde tam kıvamına gelir.", null, true, "Muz", 64.90m, 2, (byte)120 },
                    { 2, 3, new DateTime(2026, 1, 17, 9, 0, 0, 0, DateTimeKind.Utc), "Sertifikalı organik tarımdan muz, 2 kg'lık paket.", 1, true, "Organik Muz", 129.90m, 4, (byte)60 },
                    { 3, 3, new DateTime(2026, 1, 18, 9, 0, 0, 0, DateTimeKind.Utc), "Tropik aromalı guava, 500 g. Smoothie ve meyve salataları için ideal.", null, true, "Guava", 89.90m, 2, (byte)40 },
                    { 4, 3, new DateTime(2026, 1, 19, 9, 0, 0, 0, DateTimeKind.Utc), "Özenle seçilmiş guavalar, 1 kg'lık kutu.", 2, true, "Guava Kutusu", 159.90m, 4, (byte)25 },
                    { 5, 3, new DateTime(2026, 1, 20, 9, 0, 0, 0, DateTimeKind.Utc), "Ege bağlarından siyah üzüm, 1 kg.", null, true, "Siyah Üzüm", 74.90m, 2, (byte)80 },
                    { 6, 3, new DateTime(2026, 1, 21, 9, 0, 0, 0, DateTimeKind.Utc), "Tatlı ve çekirdeksiz sofralık üzüm, 500 g.", null, true, "Çekirdeksiz Üzüm", 59.90m, 4, (byte)90 },
                    { 7, 3, new DateTime(2026, 1, 22, 9, 0, 0, 0, DateTimeKind.Utc), "Yeme olgunluğunda mango, adet fiyatıdır.", null, true, "Mango", 54.90m, 2, (byte)70 },
                    { 8, 3, new DateTime(2026, 1, 23, 9, 0, 0, 0, DateTimeKind.Utc), "Üç adet olgun mango, paylaşmak için ideal paket.", 1, true, "Mango 3'lü Paket", 149.90m, 4, (byte)35 },
                    { 9, 3, new DateTime(2026, 1, 24, 9, 0, 0, 0, DateTimeKind.Utc), "Günlük kesilmiş karpuz dilimi, yaklaşık 1 kg.", null, true, "Karpuz Dilimi", 39.90m, 2, (byte)50 },
                    { 10, 3, new DateTime(2026, 1, 25, 9, 0, 0, 0, DateTimeKind.Utc), "Diyarbakır karpuzu, ortalama 6-8 kg.", null, true, "Bütün Karpuz", 119.90m, 4, (byte)30 },
                    { 11, 3, new DateTime(2026, 1, 26, 9, 0, 0, 0, DateTimeKind.Utc), "Starking kırmızı elma, 1 kg.", null, true, "Kırmızı Elma", 44.90m, 2, (byte)150 },
                    { 12, 3, new DateTime(2026, 1, 27, 9, 0, 0, 0, DateTimeKind.Utc), "Coğrafi işaretli Amasya elması, 2 kg.", 2, true, "Amasya Elması", 84.90m, 4, (byte)100 },
                    { 13, 3, new DateTime(2026, 1, 28, 9, 0, 0, 0, DateTimeKind.Utc), "Sofralık Washington portakalı, 1 kg.", null, true, "Washington Portakal", 49.90m, 2, (byte)140 },
                    { 14, 3, new DateTime(2026, 1, 29, 9, 0, 0, 0, DateTimeKind.Utc), "Bol sulu sıkmalık portakal, 3 kg'lık file.", null, true, "Sıkmalık Portakal", 109.90m, 4, (byte)80 },
                    { 15, 3, new DateTime(2026, 1, 30, 9, 0, 0, 0, DateTimeKind.Utc), "Mevsimin en iyi meyvelerinden hazırlanan 3 kg'lık sepet.", 3, true, "Karışık Meyve Sepeti", 249.90m, 2, (byte)20 },
                    { 16, 3, new DateTime(2026, 1, 31, 9, 0, 0, 0, DateTimeKind.Utc), "Haftalık değişen mevsim meyveleri, 2 kg.", null, true, "Mevsim Meyve Kutusu", 189.90m, 4, (byte)25 },
                    { 17, 2, new DateTime(2026, 2, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Yıkanmış ve ayıklanmış taze ıspanak, 500 g.", null, true, "Ispanak", 34.90m, 2, (byte)90 },
                    { 18, 2, new DateTime(2026, 2, 2, 9, 0, 0, 0, DateTimeKind.Utc), "Açık tarladan demet ıspanak, yaklaşık 1 kg.", null, true, "Köy Ispanağı", 44.90m, 4, (byte)60 },
                    { 19, 2, new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), "Kırmızı, sarı ve turuncu kapya biber karışımı, 500 g.", null, true, "Renkli Biber", 69.90m, 2, (byte)75 },
                    { 20, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Dolma için ideal iri dolmalık biber, 1 kg.", null, true, "Dolmalık Biber", 54.90m, 4, (byte)85 },
                    { 21, 2, new DateTime(2026, 1, 16, 9, 0, 0, 0, DateTimeKind.Utc), "Közleme ve salata için kırmızı kapya, 500 g.", 1, true, "Kırmızı Kapya Biber", 49.90m, 2, (byte)70 },
                    { 22, 2, new DateTime(2026, 1, 17, 9, 0, 0, 0, DateTimeKind.Utc), "Tatlı sarı biber, 500 g.", null, true, "Sarı Biber", 59.90m, 4, (byte)55 },
                    { 23, 2, new DateTime(2026, 1, 18, 9, 0, 0, 0, DateTimeKind.Utc), "Domates, biber, patlıcan ve yeşilliklerden oluşan 4 kg'lık sepet.", null, true, "Sebze Sepeti", 219.90m, 2, (byte)30 },
                    { 24, 2, new DateTime(2026, 1, 19, 9, 0, 0, 0, DateTimeKind.Utc), "Bir haftalık ihtiyaç için planlanmış 6 kg sebze.", 2, true, "Haftalık Sebze Kutusu", 289.90m, 4, (byte)25 },
                    { 25, 2, new DateTime(2026, 1, 20, 9, 0, 0, 0, DateTimeKind.Utc), "Sertifikalı organik sebzelerden 4 kg'lık paket.", null, true, "Organik Sebze Paketi", 329.90m, 2, (byte)15 },
                    { 26, 2, new DateTime(2026, 1, 21, 9, 0, 0, 0, DateTimeKind.Utc), "Yerel üreticilerden sebze ve yeşillik karışımı, 5 kg.", 3, true, "Köy Sepeti", 259.90m, 4, (byte)20 },
                    { 27, 1, new DateTime(2026, 1, 22, 9, 0, 0, 0, DateTimeKind.Utc), "Dana butundan kuşbaşı, 500 g. Vakumlu ambalajda.", null, true, "Dana Kuşbaşı", 289.90m, 5, (byte)60 },
                    { 28, 1, new DateTime(2026, 1, 23, 9, 0, 0, 0, DateTimeKind.Utc), "Dana butundan kuşbaşı, 1 kg. Vakumlu ambalajda.", 1, true, "Dana Kuşbaşı 1 kg", 559.90m, 5, (byte)40 },
                    { 29, 1, new DateTime(2026, 1, 24, 9, 0, 0, 0, DateTimeKind.Utc), "Kuzu butundan kuşbaşı, 500 g.", null, true, "Kuzu Kuşbaşı", 349.90m, 5, (byte)35 },
                    { 30, 1, new DateTime(2026, 1, 25, 9, 0, 0, 0, DateTimeKind.Utc), "İnce doğranmış sote'lik dana eti, 500 g.", null, true, "Sote'lik Dana", 299.90m, 5, (byte)45 },
                    { 31, 1, new DateTime(2026, 1, 26, 9, 0, 0, 0, DateTimeKind.Utc), "Kavurma için hazırlanmış dana eti, 1 kg.", 2, true, "Kavurmalık Et", 579.90m, 5, (byte)25 },
                    { 32, 1, new DateTime(2026, 1, 27, 9, 0, 0, 0, DateTimeKind.Utc), "Taze tavuk baget, 1 kg.", null, true, "Tavuk Baget", 129.90m, 5, (byte)80 },
                    { 33, 1, new DateTime(2026, 1, 28, 9, 0, 0, 0, DateTimeKind.Utc), "Baharatlı galeta ununa bulanmış, pişmeye hazır baget, 6 adet.", null, true, "Çıtır Tavuk Baget", 159.90m, 5, (byte)50 },
                    { 34, 1, new DateTime(2026, 1, 29, 9, 0, 0, 0, DateTimeKind.Utc), "Fırın için marine edilmiş tavuk but, 1 kg.", null, true, "Fırın Tavuk But", 139.90m, 5, (byte)60 },
                    { 35, 1, new DateTime(2026, 1, 30, 9, 0, 0, 0, DateTimeKind.Utc), "Kekik ve sarımsaklı sosla marine baget, 1 kg.", 1, true, "Marine Tavuk Baget", 149.90m, 5, (byte)55 },
                    { 36, 1, new DateTime(2026, 1, 31, 9, 0, 0, 0, DateTimeKind.Utc), "Baget, but ve kanat karışık ızgara paketi, 1,5 kg.", null, true, "Izgaralık Tavuk Paketi", 219.90m, 5, (byte)30 },
                    { 37, 7, new DateTime(2026, 2, 1, 9, 0, 0, 0, DateTimeKind.Utc), "180 g dana köfte, cheddar, marul, domates ve ev yapımı sos.", null, true, "Klasik Burger", 189.90m, 5, (byte)40 },
                    { 38, 7, new DateTime(2026, 2, 2, 9, 0, 0, 0, DateTimeKind.Utc), "İki kat dana köfte ve çift cheddar.", null, true, "Double Cheeseburger", 259.90m, 5, (byte)30 },
                    { 39, 7, new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), "Barbekü sos, karamelize soğan ve füme dana göğüs.", 3, true, "BBQ Burger", 219.90m, 5, (byte)30 },
                    { 40, 7, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Sote mantar ve İsviçre peyniriyle dana burger.", null, true, "Mantarlı Burger", 209.90m, 5, (byte)25 },
                    { 41, 7, new DateTime(2026, 1, 16, 9, 0, 0, 0, DateTimeKind.Utc), "İki klasik burger, patates kızartması ve sos çeşitleri.", 2, true, "Burger Seti (2 kişilik)", 399.90m, 5, (byte)20 },
                    { 42, 7, new DateTime(2026, 1, 17, 9, 0, 0, 0, DateTimeKind.Utc), "Pişirmeye hazır 4 adet 150 g dana burger köftesi.", null, true, "Ev Yapımı Burger Köftesi", 239.90m, 5, (byte)35 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "CreatedAt", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 9, 0, 0, 0, DateTimeKind.Utc), 1, 1, (byte)2, 64.90m },
                    { 2, new DateTime(2026, 1, 28, 9, 0, 0, 0, DateTimeKind.Utc), 1, 11, (byte)1, 44.90m },
                    { 3, new DateTime(2026, 1, 28, 9, 0, 0, 0, DateTimeKind.Utc), 1, 17, (byte)1, 34.90m },
                    { 4, new DateTime(2026, 1, 31, 9, 0, 0, 0, DateTimeKind.Utc), 2, 27, (byte)1, 289.90m },
                    { 5, new DateTime(2026, 1, 31, 9, 0, 0, 0, DateTimeKind.Utc), 2, 32, (byte)2, 129.90m },
                    { 6, new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), 3, 15, (byte)1, 249.90m },
                    { 7, new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), 3, 23, (byte)1, 219.90m },
                    { 8, new DateTime(2026, 2, 3, 9, 0, 0, 0, DateTimeKind.Utc), 3, 13, (byte)2, 49.90m },
                    { 9, new DateTime(2026, 2, 6, 9, 0, 0, 0, DateTimeKind.Utc), 4, 37, (byte)2, 189.90m },
                    { 10, new DateTime(2026, 2, 6, 9, 0, 0, 0, DateTimeKind.Utc), 4, 42, (byte)1, 239.90m },
                    { 11, new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), 5, 5, (byte)1, 74.90m },
                    { 12, new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), 5, 7, (byte)3, 54.90m },
                    { 13, new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), 5, 19, (byte)1, 69.90m },
                    { 14, new DateTime(2026, 2, 12, 9, 0, 0, 0, DateTimeKind.Utc), 6, 24, (byte)1, 289.90m },
                    { 15, new DateTime(2026, 2, 12, 9, 0, 0, 0, DateTimeKind.Utc), 6, 31, (byte)1, 579.90m }
                });

            migrationBuilder.InsertData(
                table: "ProductComments",
                columns: new[] { "Id", "CreatedAt", "IsConfirmed", "ProductId", "StarCount", "Text", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 5, 9, 0, 0, 0, DateTimeKind.Utc), true, 1, (byte)5, "Muzlar tam kıvamında geldi, paketleme de çok özenliydi.", 3 },
                    { 2, new DateTime(2026, 2, 6, 9, 0, 0, 0, DateTimeKind.Utc), true, 1, (byte)4, "Lezzetli ama bir iki tanesi biraz yeşildi, iki günde olgunlaştı.", 6 },
                    { 3, new DateTime(2026, 2, 7, 9, 0, 0, 0, DateTimeKind.Utc), true, 11, (byte)5, "Elmalar sert ve sulu, tam istediğim gibi.", 7 },
                    { 4, new DateTime(2026, 2, 8, 9, 0, 0, 0, DateTimeKind.Utc), true, 15, (byte)5, "Sepetteki meyvelerin hepsi tazeydi, misafirlerim çok beğendi.", 3 },
                    { 5, new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), true, 17, (byte)4, "Ispanak temiz geldi, yıkamadan kullanabildim.", 8 },
                    { 6, new DateTime(2026, 2, 10, 9, 0, 0, 0, DateTimeKind.Utc), true, 23, (byte)5, "Haftalık alışverişimi tamamen bu sepetle çözüyorum.", 6 },
                    { 7, new DateTime(2026, 2, 11, 9, 0, 0, 0, DateTimeKind.Utc), true, 27, (byte)5, "Et çok kaliteli, kuşbaşılar eşit doğranmış.", 7 },
                    { 8, new DateTime(2026, 2, 12, 9, 0, 0, 0, DateTimeKind.Utc), true, 32, (byte)4, "Bagetler büyük boy, marine etmek için çok uygun.", 3 },
                    { 9, new DateTime(2026, 2, 13, 9, 0, 0, 0, DateTimeKind.Utc), true, 37, (byte)5, "Köftesi gerçekten ev yapımı tadında.", 8 }
                });

            migrationBuilder.InsertData(
                table: "ProductComments",
                columns: new[] { "Id", "CreatedAt", "ProductId", "StarCount", "Text", "UserId" },
                values: new object[,]
                {
                    { 10, new DateTime(2026, 2, 14, 9, 0, 0, 0, DateTimeKind.Utc), 39, (byte)3, "Sos biraz fazla tatlıydı ama genel olarak güzel.", 6 },
                    { 11, new DateTime(2026, 2, 15, 9, 0, 0, 0, DateTimeKind.Utc), 13, (byte)4, "Portakallar sulu, kabukları biraz kalın.", 7 },
                    { 12, new DateTime(2026, 2, 16, 9, 0, 0, 0, DateTimeKind.Utc), 42, (byte)5, "Dondurucuya atıp ihtiyaç oldukça pişiriyorum, çok pratik.", 3 }
                });

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
                name: "IX_BlogComments_BlogId",
                table: "BlogComments",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_UserId",
                table: "Blogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_ProductId",
                table: "OrderItems",
                columns: new[] { "OrderId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderCode",
                table: "Orders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComments_ProductId",
                table: "ProductComments",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComments_UserId",
                table: "ProductComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DiscountId",
                table: "Products",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SellerId",
                table: "Products",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_RelBlogCategories_BlogId_CategoryId",
                table: "RelBlogCategories",
                columns: new[] { "BlogId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelBlogCategories_CategoryId",
                table: "RelBlogCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RelBlogTags_BlogId",
                table: "RelBlogTags",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_RelBlogTags_TagId",
                table: "RelBlogTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogComments");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductComments");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "RelBlogCategories");

            migrationBuilder.DropTable(
                name: "RelBlogTags");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "BlogTags");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ProductDiscounts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
