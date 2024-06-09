namespace eProcurement_PAUP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.item", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.item", "Category", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.item", "Category");
            DropColumn("dbo.item", "Quantity");
        }
    }
}
