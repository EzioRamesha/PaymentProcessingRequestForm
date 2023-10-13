namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayingEntityAmountRangePPRF : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmountRangeEmails",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EntityAmountRangeId = c.Guid(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EntityAmountRanges", t => t.EntityAmountRangeId, cascadeDelete: true)
                .Index(t => t.EntityAmountRangeId);
            
            CreateTable(
                "dbo.EntityAmountRanges",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PayingEntityId = c.Guid(nullable: false),
                        AmountRangeFrom = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountRangeTo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PayingEntities", t => t.PayingEntityId, cascadeDelete: true)
                .Index(t => t.PayingEntityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EntityAmountRanges", "PayingEntityId", "dbo.PayingEntities");
            DropForeignKey("dbo.AmountRangeEmails", "EntityAmountRangeId", "dbo.EntityAmountRanges");
            DropIndex("dbo.EntityAmountRanges", new[] { "PayingEntityId" });
            DropIndex("dbo.AmountRangeEmails", new[] { "EntityAmountRangeId" });
            DropTable("dbo.EntityAmountRanges");
            DropTable("dbo.AmountRangeEmails");
        }
    }
}
