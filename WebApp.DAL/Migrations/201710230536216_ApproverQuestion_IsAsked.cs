namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApproverQuestion_IsAsked : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestApprovers", "IsQuestionAsked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestApprovers", "IsQuestionAsked");
        }
    }
}
