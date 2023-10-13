namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PPRF_Enhancement_Mar2021_Part3 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PaymentRequestForms", name: "BudgetPPRFNo_Id", newName: "BudgetPPRFNoId");
            RenameIndex(table: "dbo.PaymentRequestForms", name: "IX_BudgetPPRFNo_Id", newName: "IX_BudgetPPRFNoId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PaymentRequestForms", name: "IX_BudgetPPRFNoId", newName: "IX_BudgetPPRFNo_Id");
            RenameColumn(table: "dbo.PaymentRequestForms", name: "BudgetPPRFNoId", newName: "BudgetPPRFNo_Id");
        }
    }
}
