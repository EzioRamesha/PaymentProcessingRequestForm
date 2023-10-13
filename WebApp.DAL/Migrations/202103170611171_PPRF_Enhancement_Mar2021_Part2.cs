namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PPRF_Enhancement_Mar2021_Part2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentRequestForms", "BudgetValidFrom", c => c.DateTime());
            AddColumn("dbo.PaymentRequestForms", "BudgetValidTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentRequestForms", "BudgetValidTo");
            DropColumn("dbo.PaymentRequestForms", "BudgetValidFrom");
        }
    }
}
