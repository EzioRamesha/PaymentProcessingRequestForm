namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDbContext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "PayingEntitiesId", c => c.String());
         
            CreateTable(
                "dbo.RejectTypes",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    Description = c.String(),
                    IsEnabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);
            CreateTable(
               "dbo.DepartmentsAccounts",
               c => new
               {
                   Id = c.Guid(nullable: false, identity: true),
                   Name = c.String(),
                   Code = c.String(),
                   Description = c.String(),
                   IsEnabled = c.Boolean(nullable: false),
               })
               .PrimaryKey(t => t.Id);
            AddColumn("dbo.PaymentRequestForms", "DepartmentsAccountId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PaymentRequestForms", "DepartmentsAccountId");
            AddColumn("dbo.DepartmentsAccounts", "PayingEntitiesId", c => c.String());
            AddColumn("dbo.DepartmentsAccounts", "DepartmentId", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "UrgentRemark", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "UrgentFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.PaymentRequestForms", "UrgentFlag", c => c.Boolean(nullable: false));
            AddColumn("dbo.PaymentRequestForms", "Note", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "CloseReason", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "CloseRemark", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "CancelPPRFReason", c => c.String());
            AddColumn("dbo.PaymentRequestForms", "CancelPPRFRemark", c => c.String());

            CreateTable(
                "dbo.CancelPPRFReasonTypes",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    Description = c.String(),
                    IsEnabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ClosePPRFReasonTypes",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    Description = c.String(),
                    IsEnabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);
            AddForeignKey("dbo.PaymentRequestForms", "DepartmentsAccountId", "dbo.DepartmentsAccounts", "Id");
        }

        public override void Down()
        {
            DropColumn("dbo.Departments", "PayingEntitiesId");
            DropTable("dbo.RejectTypes");
            DropTable("dbo.DepartmentsAccounts");
            DropIndex("dbo.PaymentRequestForms", new[] { "DepartmentsAccountId" });
            DropColumn("dbo.DepartmentsAccounts", "DepartmentId");
            DropColumn("dbo.DepartmentsAccounts", "PayingEntitiesId");
            DropColumn("dbo.PaymentRequestForms", "UrgentFlag");
            DropColumn("dbo.PaymentRequestForms", "UrgentRemark");
            AlterColumn("dbo.PaymentRequestForms", "UrgentFlag", c => c.String());
            DropColumn("dbo.PaymentRequestForms", "Note");
            DropColumn("dbo.PaymentRequestForms", "CancelPPRFRemark");
            DropColumn("dbo.PaymentRequestForms", "CancelPPRFReason");
            DropColumn("dbo.PaymentRequestForms", "CloseRemark");
            DropColumn("dbo.PaymentRequestForms", "CloseReason");
            DropTable("dbo.ClosePPRFReasonTypes");
            DropTable("dbo.CancelPPRFReasonTypes");
            DropForeignKey("dbo.PaymentRequestForms", "DepartmentsAccountId", "dbo.DepartmentsAccounts");
        }
    }
}
