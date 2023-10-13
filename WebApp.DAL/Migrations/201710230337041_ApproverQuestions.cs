namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApproverQuestions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApproverRequestQuestions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ApprovalId = c.Guid(nullable: false),
                        Question = c.String(),
                        Answer = c.String(),
                        AskedOn = c.DateTime(nullable: false),
                        AnsweredOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RequestApprovers", t => t.ApprovalId, cascadeDelete: true)
                .Index(t => t.ApprovalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApproverRequestQuestions", "ApprovalId", "dbo.RequestApprovers");
            DropIndex("dbo.ApproverRequestQuestions", new[] { "ApprovalId" });
            DropTable("dbo.ApproverRequestQuestions");
        }
    }
}
