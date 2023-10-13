namespace WebApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserGroups",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        ApplicationGroupId = c.Guid(nullable: false),
                        IsSelected = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationGroups", t => t.ApplicationGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ApplicationGroupId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        DepartmentId = c.Guid(),
                        Designation = c.String(),
                        ExternalUserId = c.String(maxLength: 128),
                        CreatedOn = c.DateTime(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.AspNetUsers", t => t.ExternalUserId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ExternalUserId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        CreatedById = c.String(maxLength: 128),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        USDValue = c.Decimal(nullable: false, precision: 18, scale: 6),
                        EuroValue = c.Decimal(nullable: false, precision: 18, scale: 6),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.ExpenseTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FrequencyTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GoodsAndServices",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PaymentRequestFormId = c.Guid(nullable: false),
                        Description = c.String(),
                        TaxTypeId = c.Guid(),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 6),
                        TaxAmountUSD = c.Decimal(nullable: false, precision: 18, scale: 6),
                        TaxAmountEuro = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Quantity = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 6),
                        AmountUSD = c.Decimal(nullable: false, precision: 18, scale: 6),
                        AmountEuro = c.Decimal(nullable: false, precision: 18, scale: 6),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentRequestForms", t => t.PaymentRequestFormId)
                .ForeignKey("dbo.TaxTypes", t => t.TaxTypeId)
                .Index(t => t.PaymentRequestFormId)
                .Index(t => t.TaxTypeId);
            
            CreateTable(
                "dbo.PaymentRequestForms",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PprfDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        PayingEntityId = c.Guid(nullable: false),
                        CountryId = c.Guid(nullable: false),
                        Month = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                        FrequencyTypeId = c.Guid(nullable: false),
                        PaymentMethodId = c.Guid(nullable: false),
                        ExpenseTypeId = c.Guid(nullable: false),
                        PayeeBankAccountDetailId = c.Guid(nullable: false),
                        Description = c.String(),
                        Remarks = c.String(),
                        ClarificationRequired = c.Boolean(nullable: false),
                        Clarifications = c.String(),
                        CurrencyId = c.Guid(nullable: false),
                        Tax2TypeId = c.Guid(),
                        Tax2Amount = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Tax2AmountUSD = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Tax2AmountEuro = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Tax3TypeId = c.Guid(),
                        Tax3Amount = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Tax3AmountUSD = c.Decimal(nullable: false, precision: 18, scale: 6),
                        Tax3AmountEuro = c.Decimal(nullable: false, precision: 18, scale: 6),
                        DocumentPath = c.String(),
                        DocumentName = c.String(),
                        OriginatorId = c.Guid(nullable: false),
                        CreatedById = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        AutoGeneratedSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedById, cascadeDelete: true)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.ExpenseTypes", t => t.ExpenseTypeId, cascadeDelete: true)
                .ForeignKey("dbo.FrequencyTypes", t => t.FrequencyTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.OriginatorId)
                .ForeignKey("dbo.PayeeBankAccountDetails", t => t.PayeeBankAccountDetailId, cascadeDelete: true)
                .ForeignKey("dbo.PayingEntities", t => t.PayingEntityId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodId, cascadeDelete: true)
                .ForeignKey("dbo.TaxTypes", t => t.Tax2TypeId)
                .ForeignKey("dbo.TaxTypes", t => t.Tax3TypeId)
                .Index(t => t.PayingEntityId)
                .Index(t => t.CountryId)
                .Index(t => t.DepartmentId)
                .Index(t => t.FrequencyTypeId)
                .Index(t => t.PaymentMethodId)
                .Index(t => t.ExpenseTypeId)
                .Index(t => t.PayeeBankAccountDetailId)
                .Index(t => t.CurrencyId)
                .Index(t => t.Tax2TypeId)
                .Index(t => t.Tax3TypeId)
                .Index(t => t.OriginatorId)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.RequestApprovers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ApproverId = c.Guid(nullable: false),
                        PaymentRequestFormId = c.Guid(nullable: false),
                        ApprovalStatus = c.Int(nullable: false),
                        ApprovalToken = c.String(),
                        SequenceNo = c.Int(nullable: false),
                        Remarks = c.String(),
                        ResponseDate = c.DateTime(),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUserGroups", t => t.ApproverId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentRequestForms", t => t.PaymentRequestFormId)
                .Index(t => t.ApproverId)
                .Index(t => t.PaymentRequestFormId);
            
            CreateTable(
                "dbo.PayeeBankAccountDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PayeeId = c.Guid(nullable: false),
                        BankName = c.String(),
                        BankAddress = c.String(),
                        AccountName = c.String(),
                        AccountNumber = c.String(),
                        AccountType = c.String(),
                        IBAN = c.String(),
                        Swift = c.String(),
                        IFSC = c.String(),
                        PreviousDetailsId = c.Guid(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Payees", t => t.PayeeId, cascadeDelete: true)
                .ForeignKey("dbo.PayeeBankAccountDetails", t => t.PreviousDetailsId)
                .Index(t => t.PayeeId)
                .Index(t => t.PreviousDetailsId);
            
            CreateTable(
                "dbo.Payees",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        Fax = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        PreviousDetailsId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Payees", t => t.PreviousDetailsId)
                .Index(t => t.PreviousDetailsId);
            
            CreateTable(
                "dbo.PayingEntities",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Abbreviation = c.String(),
                        LogoPath = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                        PreviousDetailsId = c.Guid(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PayingEntities", t => t.PreviousDetailsId)
                .Index(t => t.PreviousDetailsId);
            
            CreateTable(
                "dbo.PaymentMethods",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaxTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        PercentageValue = c.Decimal(nullable: false, precision: 18, scale: 6),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.GoodsAndServices", "TaxTypeId", "dbo.TaxTypes");
            DropForeignKey("dbo.PaymentRequestForms", "Tax3TypeId", "dbo.TaxTypes");
            DropForeignKey("dbo.PaymentRequestForms", "Tax2TypeId", "dbo.TaxTypes");
            DropForeignKey("dbo.PaymentRequestForms", "PaymentMethodId", "dbo.PaymentMethods");
            DropForeignKey("dbo.PaymentRequestForms", "PayingEntityId", "dbo.PayingEntities");
            DropForeignKey("dbo.PayingEntities", "PreviousDetailsId", "dbo.PayingEntities");
            DropForeignKey("dbo.PaymentRequestForms", "PayeeBankAccountDetailId", "dbo.PayeeBankAccountDetails");
            DropForeignKey("dbo.PayeeBankAccountDetails", "PreviousDetailsId", "dbo.PayeeBankAccountDetails");
            DropForeignKey("dbo.Payees", "PreviousDetailsId", "dbo.Payees");
            DropForeignKey("dbo.PayeeBankAccountDetails", "PayeeId", "dbo.Payees");
            DropForeignKey("dbo.PaymentRequestForms", "OriginatorId", "dbo.Users");
            DropForeignKey("dbo.GoodsAndServices", "PaymentRequestFormId", "dbo.PaymentRequestForms");
            DropForeignKey("dbo.PaymentRequestForms", "FrequencyTypeId", "dbo.FrequencyTypes");
            DropForeignKey("dbo.PaymentRequestForms", "ExpenseTypeId", "dbo.ExpenseTypes");
            DropForeignKey("dbo.PaymentRequestForms", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.PaymentRequestForms", "CurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.PaymentRequestForms", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.PaymentRequestForms", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.RequestApprovers", "PaymentRequestFormId", "dbo.PaymentRequestForms");
            DropForeignKey("dbo.RequestApprovers", "ApproverId", "dbo.ApplicationUserGroups");
            DropForeignKey("dbo.Currencies", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserGroups", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "ExternalUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Users", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Departments", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserGroups", "ApplicationGroupId", "dbo.ApplicationGroups");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PayingEntities", new[] { "PreviousDetailsId" });
            DropIndex("dbo.Payees", new[] { "PreviousDetailsId" });
            DropIndex("dbo.PayeeBankAccountDetails", new[] { "PreviousDetailsId" });
            DropIndex("dbo.PayeeBankAccountDetails", new[] { "PayeeId" });
            DropIndex("dbo.RequestApprovers", new[] { "PaymentRequestFormId" });
            DropIndex("dbo.RequestApprovers", new[] { "ApproverId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "CreatedById" });
            DropIndex("dbo.PaymentRequestForms", new[] { "OriginatorId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "Tax3TypeId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "Tax2TypeId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "CurrencyId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "PayeeBankAccountDetailId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "ExpenseTypeId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "PaymentMethodId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "FrequencyTypeId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "DepartmentId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "CountryId" });
            DropIndex("dbo.PaymentRequestForms", new[] { "PayingEntityId" });
            DropIndex("dbo.GoodsAndServices", new[] { "TaxTypeId" });
            DropIndex("dbo.GoodsAndServices", new[] { "PaymentRequestFormId" });
            DropIndex("dbo.Currencies", new[] { "CreatedById" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Departments", new[] { "CreatedById" });
            DropIndex("dbo.Users", new[] { "ExternalUserId" });
            DropIndex("dbo.Users", new[] { "DepartmentId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "ApplicationGroupId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "UserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TaxTypes");
            DropTable("dbo.PaymentMethods");
            DropTable("dbo.PayingEntities");
            DropTable("dbo.Payees");
            DropTable("dbo.PayeeBankAccountDetails");
            DropTable("dbo.RequestApprovers");
            DropTable("dbo.PaymentRequestForms");
            DropTable("dbo.GoodsAndServices");
            DropTable("dbo.FrequencyTypes");
            DropTable("dbo.ExpenseTypes");
            DropTable("dbo.Currencies");
            DropTable("dbo.Countries");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Departments");
            DropTable("dbo.Users");
            DropTable("dbo.ApplicationUserGroups");
            DropTable("dbo.ApplicationGroups");
        }
    }
}
