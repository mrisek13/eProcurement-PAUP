namespace eProcurement_PAUP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderedquantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.item", "OrderedQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.item", "OrderedQuantity");
        }
    }
}
