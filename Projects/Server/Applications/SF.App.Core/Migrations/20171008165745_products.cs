using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SF.App.Core.Migrations
{
    public partial class products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocument_CommonDocumentAuthor_AuthorId",
                table: "CommonDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocument_CommonDocumentCategory_ContainerId",
                table: "CommonDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocumentCategory_CommonDocumentCategory_ContainerId",
                table: "CommonDocumentCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocument_DocumentId",
                table: "CommonDocumentTagRef");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocumentTag_TagId",
                table: "CommonDocumentTagRef");

            migrationBuilder.DropForeignKey(
                name: "FK_FrontSite_FrontSiteTemplate_TemplateId",
                table: "FrontSite");

            migrationBuilder.DropForeignKey(
                name: "FK_MgrMenuItem_MgrMenu_MenuId",
                table: "MgrMenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_MgrMenuItem_MgrMenuItem_ParentId",
                table: "MgrMenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_SysAuthIdentityCredential_SysAuthIdentity_IdentityId",
                table: "SysAuthIdentityCredential");

            migrationBuilder.DropForeignKey(
                name: "FK_SysServiceInstance_SysServiceInstance_ContainerId",
                table: "SysServiceInstance");

            migrationBuilder.CreateTable(
                name: "BizProductCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    BannerImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    MobileBannerImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MobileBannerUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Tag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductCategory_BizProductCategory_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BizProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BizProduct",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CouponDisabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    MarketPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<double>(type: "float", nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PublishedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SellCount = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VIADSpecId = table.Column<long>(type: "bigint", nullable: true),
                    Visited = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProduct_BizProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BizProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductPropertyScope",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductPropertyScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductPropertyScope_BizProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BizProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "app_biz_product_spec",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VIADSpecId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_biz_product_spec", x => x.Id);
                    table.ForeignKey(
                        name: "FK_app_biz_product_spec_BizProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductDetail_BizProduct_Id",
                        column: x => x.Id,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CategoryTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    SellerId = table.Column<long>(type: "bigint", nullable: false),
                    SourceItemId = table.Column<long>(type: "bigint", nullable: true),
                    SourceLevel = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductItem_BizProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductItem_BizProductItem_SourceItemId",
                        column: x => x.SourceItemId,
                        principalTable: "BizProductItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductProperty",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ObjectState = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    ScopeId = table.Column<long>(type: "bigint", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BizProductProperty_BizProductProperty_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BizProductProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductProperty_BizProductPropertyScope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "BizProductPropertyScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductProperty_BizProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BizProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductCategoryItem",
                columns: table => new
                {
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductCategoryItem", x => new { x.CategoryId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_BizProductCategoryItem_BizProductCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BizProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductCategoryItem_BizProductItem_ItemId",
                        column: x => x.ItemId,
                        principalTable: "BizProductItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BizProductPropertyItem",
                columns: table => new
                {
                    PropertyId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Order = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizProductPropertyItem", x => new { x.PropertyId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_BizProductPropertyItem_BizProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "BizProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BizProductPropertyItem_BizProductProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "BizProductProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_biz_product_spec_ProductId",
                table: "app_biz_product_spec",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_OwnerUserId",
                table: "BizProduct",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_TypeId",
                table: "BizProduct",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_Order",
                table: "BizProduct",
                columns: new[] { "ObjectState", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_Price",
                table: "BizProduct",
                columns: new[] { "ObjectState", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_PublishedTime",
                table: "BizProduct",
                columns: new[] { "ObjectState", "PublishedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_SellCount",
                table: "BizProduct",
                columns: new[] { "ObjectState", "SellCount" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_Visited",
                table: "BizProduct",
                columns: new[] { "ObjectState", "Visited" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_Order",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_Price",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_PublishedTime",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "PublishedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_SellCount",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "SellCount" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProduct_ObjectState_TypeId_Visited",
                table: "BizProduct",
                columns: new[] { "ObjectState", "TypeId", "Visited" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategory_OwnerUserId",
                table: "BizProductCategory",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategory_ParentId",
                table: "BizProductCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategory_Tag",
                table: "BizProductCategory",
                column: "Tag");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategoryItem_ItemId",
                table: "BizProductCategoryItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductCategoryItem_CategoryId_Order",
                table: "BizProductCategoryItem",
                columns: new[] { "CategoryId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductItem_ProductId",
                table: "BizProductItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductItem_SellerId",
                table: "BizProductItem",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductItem_SourceItemId",
                table: "BizProductItem",
                column: "SourceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductProperty_ScopeId",
                table: "BizProductProperty",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductProperty_ParentId_Order",
                table: "BizProductProperty",
                columns: new[] { "ParentId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductProperty_TypeId_ParentId_Name",
                table: "BizProductProperty",
                columns: new[] { "TypeId", "ParentId", "Name" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyItem_ProductId",
                table: "BizProductPropertyItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyItem_PropertyId_Order",
                table: "BizProductPropertyItem",
                columns: new[] { "PropertyId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyScope_Order",
                table: "BizProductPropertyScope",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_BizProductPropertyScope_TypeId_Name",
                table: "BizProductPropertyScope",
                columns: new[] { "TypeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BizProductType_Name",
                table: "BizProductType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BizProductType_Order",
                table: "BizProductType",
                column: "Order");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocument_CommonDocumentAuthor_AuthorId",
                table: "CommonDocument",
                column: "AuthorId",
                principalTable: "CommonDocumentAuthor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocument_CommonDocumentCategory_ContainerId",
                table: "CommonDocument",
                column: "ContainerId",
                principalTable: "CommonDocumentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocumentCategory_CommonDocumentCategory_ContainerId",
                table: "CommonDocumentCategory",
                column: "ContainerId",
                principalTable: "CommonDocumentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocument_DocumentId",
                table: "CommonDocumentTagRef",
                column: "DocumentId",
                principalTable: "CommonDocument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocumentTag_TagId",
                table: "CommonDocumentTagRef",
                column: "TagId",
                principalTable: "CommonDocumentTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FrontSite_FrontSiteTemplate_TemplateId",
                table: "FrontSite",
                column: "TemplateId",
                principalTable: "FrontSiteTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MgrMenuItem_MgrMenu_MenuId",
                table: "MgrMenuItem",
                column: "MenuId",
                principalTable: "MgrMenu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MgrMenuItem_MgrMenuItem_ParentId",
                table: "MgrMenuItem",
                column: "ParentId",
                principalTable: "MgrMenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SysAuthIdentityCredential_SysAuthIdentity_IdentityId",
                table: "SysAuthIdentityCredential",
                column: "IdentityId",
                principalTable: "SysAuthIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SysServiceInstance_SysServiceInstance_ContainerId",
                table: "SysServiceInstance",
                column: "ContainerId",
                principalTable: "SysServiceInstance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocument_CommonDocumentAuthor_AuthorId",
                table: "CommonDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocument_CommonDocumentCategory_ContainerId",
                table: "CommonDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocumentCategory_CommonDocumentCategory_ContainerId",
                table: "CommonDocumentCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocument_DocumentId",
                table: "CommonDocumentTagRef");

            migrationBuilder.DropForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocumentTag_TagId",
                table: "CommonDocumentTagRef");

            migrationBuilder.DropForeignKey(
                name: "FK_FrontSite_FrontSiteTemplate_TemplateId",
                table: "FrontSite");

            migrationBuilder.DropForeignKey(
                name: "FK_MgrMenuItem_MgrMenu_MenuId",
                table: "MgrMenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_MgrMenuItem_MgrMenuItem_ParentId",
                table: "MgrMenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_SysAuthIdentityCredential_SysAuthIdentity_IdentityId",
                table: "SysAuthIdentityCredential");

            migrationBuilder.DropForeignKey(
                name: "FK_SysServiceInstance_SysServiceInstance_ContainerId",
                table: "SysServiceInstance");

            migrationBuilder.DropTable(
                name: "app_biz_product_spec");

            migrationBuilder.DropTable(
                name: "BizProductCategoryItem");

            migrationBuilder.DropTable(
                name: "BizProductDetail");

            migrationBuilder.DropTable(
                name: "BizProductPropertyItem");

            migrationBuilder.DropTable(
                name: "BizProductCategory");

            migrationBuilder.DropTable(
                name: "BizProductItem");

            migrationBuilder.DropTable(
                name: "BizProductProperty");

            migrationBuilder.DropTable(
                name: "BizProduct");

            migrationBuilder.DropTable(
                name: "BizProductPropertyScope");

            migrationBuilder.DropTable(
                name: "BizProductType");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocument_CommonDocumentAuthor_AuthorId",
                table: "CommonDocument",
                column: "AuthorId",
                principalTable: "CommonDocumentAuthor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocument_CommonDocumentCategory_ContainerId",
                table: "CommonDocument",
                column: "ContainerId",
                principalTable: "CommonDocumentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocumentCategory_CommonDocumentCategory_ContainerId",
                table: "CommonDocumentCategory",
                column: "ContainerId",
                principalTable: "CommonDocumentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocument_DocumentId",
                table: "CommonDocumentTagRef",
                column: "DocumentId",
                principalTable: "CommonDocument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonDocumentTagRef_CommonDocumentTag_TagId",
                table: "CommonDocumentTagRef",
                column: "TagId",
                principalTable: "CommonDocumentTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FrontSite_FrontSiteTemplate_TemplateId",
                table: "FrontSite",
                column: "TemplateId",
                principalTable: "FrontSiteTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MgrMenuItem_MgrMenu_MenuId",
                table: "MgrMenuItem",
                column: "MenuId",
                principalTable: "MgrMenu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MgrMenuItem_MgrMenuItem_ParentId",
                table: "MgrMenuItem",
                column: "ParentId",
                principalTable: "MgrMenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SysAuthIdentityCredential_SysAuthIdentity_IdentityId",
                table: "SysAuthIdentityCredential",
                column: "IdentityId",
                principalTable: "SysAuthIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SysServiceInstance_SysServiceInstance_ContainerId",
                table: "SysServiceInstance",
                column: "ContainerId",
                principalTable: "SysServiceInstance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
