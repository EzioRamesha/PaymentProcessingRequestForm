using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using WebApp.DAL.Custom;

namespace WebApp.DAL.Data
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }




    public static class IdentityExtensions
    {

        public static string[] GetPermissions(this System.Security.Principal.IIdentity identity)
        {
            var _userBAL = new BAL.UserBAL();
            var userPermissions = _userBAL.GetUserPermissions(identity.Name);
            if (userPermissions != null)
            {
                return userPermissions.Where(w=>w.IsEnabled).Select(s => s.Name).ToArray();
            }
            return null;
        }
        public static string GetFullName(this System.Security.Principal.IIdentity identity)
        {
            string returnValue = string.Empty;
            var _userBAL = new BAL.UserBAL();
            var user = _userBAL.FindByEmail(identity.Name);
            if (user != null)
            {
                returnValue = user.Name;
            }
            return returnValue;
        }
    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(nameOrConnectionString: "PPRF", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<PayingEntity>().HasOptional(h => h.PreviousDetails).WithMany(w => w.PreviousEdits).WillCascadeOnDelete(false);
                modelBuilder.Entity<PaymentRequestForm>().HasRequired(p => p.Department).WithMany().WillCascadeOnDelete(false);
                modelBuilder.Entity<PaymentRequestForm>().HasRequired(p => p.Originator).WithMany().WillCascadeOnDelete(false);


                modelBuilder.Entity<PaymentRequestForm>().HasMany(p => p.GoodsAndServices).WithRequired(w => w.PaymentRequestForm).WillCascadeOnDelete(false);
                modelBuilder.Entity<PaymentRequestForm>().HasMany(p => p.Approvers).WithRequired(w => w.PaymentRequestForm).WillCascadeOnDelete(false);


                modelBuilder.Entity<GoodsAndService>().HasOptional(p => p.TaxType);


                foreach (Type classType in from t in Assembly.GetAssembly(typeof(DecimalPrecisionAttribute)).GetTypes()
                                           where t.IsClass && t.Namespace == "WebApp.DAL.Data"
                                           select t)
                {
                    foreach (var propAttr in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<DecimalPrecisionAttribute>() != null).Select(
                           p => new { prop = p, attr = p.GetCustomAttribute<DecimalPrecisionAttribute>(true) }))
                    {

                        var entityConfig = modelBuilder.GetType().GetMethod("Entity").MakeGenericMethod(classType).Invoke(modelBuilder, null);
                        ParameterExpression param = ParameterExpression.Parameter(classType, "c");
                        Expression property = Expression.Property(param, propAttr.prop.Name);
                        LambdaExpression lambdaExpression = Expression.Lambda(property, true,
                                                                                 new ParameterExpression[] { param });
                        DecimalPropertyConfiguration decimalConfig;
                        if (propAttr.prop.PropertyType.IsGenericType && propAttr.prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            MethodInfo methodInfo = entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[7];
                            decimalConfig = methodInfo.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                        }
                        else
                        {
                            MethodInfo methodInfo = entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[6];
                            decimalConfig = methodInfo.Invoke(entityConfig, new[] { lambdaExpression }) as DecimalPropertyConfiguration;
                        }

                        decimalConfig.HasPrecision(propAttr.attr.Precision, propAttr.attr.Scale);
                    }
                }
            }
            catch (Exception)
            {
           //     throw;
            }
            finally
            {

            }
           
        }

        public DbSet<ApplicationGroup> ApplicationGroups { get; set; }
        public DbSet<PayingEntity> PayingEntities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<FrequencyType> FrequencyTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<PayeeBankAccountDetail> PayeeBankAccountDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentsAccount> DepartmentsAccounts { get; set; }
        public DbSet<User> AppUsers { get; set; }
        public DbSet<TaxType> TaxTypes { get; set; }
        public DbSet<GoodsAndService> GoodsAndServices { get; set; }
        public DbSet<PaymentRequestForm> PaymentRequestForms { get; set; }
        public DbSet<PaymentRequestDocuments> PaymentRequestDocuments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<ApproverRequestQuestion> Queries { get; set; }

        public DbSet<EntityAmountRange> EntityAmountRanges { get; set; }
        public DbSet<AmountRangeEmail> AmountRangeEmails { get; set; }
        public DbSet<RejectTypes> RejectTypes { get; set; }
        public DbSet<CancelPPRFReasonTypes> CancelPPRFReasonType { get; set; }
        public DbSet<ClosePPRFReasonTypes> ClosePPRFReasonType { get; set; }
        //public DbSet<ApplicationRole> Roles { get; set; }
        //public DbSet<ApprovalStatus> ApprovalStatuses { get; set; }
    }
}