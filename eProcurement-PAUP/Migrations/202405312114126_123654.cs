namespace eProcurement_PAUP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _123654 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderViewModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SupplierID = c.Int(nullable: false),
                        CustomerName = c.String(),
                        SupplierName = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.permissions",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        username = c.String(nullable: false),
                        firstName = c.String(nullable: false),
                        lastName = c.String(nullable: false),
                        email = c.String(nullable: false),
                        Password = c.String(),
                        PermissionID = c.String(nullable: false, maxLength: 128),
                        Avatar = c.Binary(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.permissions", t => t.PermissionID, cascadeDelete: true)
                .Index(t => t.PermissionID);
            
            AddColumn("dbo.item", "SupplierID", c => c.Int(nullable: false));
            AddColumn("dbo.item", "OrderViewModel_ID", c => c.Int());
            CreateIndex("dbo.item", "SupplierID");
            CreateIndex("dbo.item", "OrderViewModel_ID");
            AddForeignKey("dbo.item", "SupplierID", "dbo.supplier", "ID", cascadeDelete: true);
            AddForeignKey("dbo.item", "OrderViewModel_ID", "dbo.OrderViewModels", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.users", "PermissionID", "dbo.permissions");
            DropForeignKey("dbo.item", "OrderViewModel_ID", "dbo.OrderViewModels");
            DropForeignKey("dbo.item", "SupplierID", "dbo.supplier");
            DropIndex("dbo.users", new[] { "PermissionID" });
            DropIndex("dbo.item", new[] { "OrderViewModel_ID" });
            DropIndex("dbo.item", new[] { "SupplierID" });
            DropColumn("dbo.item", "OrderViewModel_ID");
            DropColumn("dbo.item", "SupplierID");
            DropTable("dbo.users");
            DropTable("dbo.permissions");
            DropTable("dbo.OrderViewModels");
        }
    }
}
