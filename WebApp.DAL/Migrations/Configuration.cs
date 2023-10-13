namespace WebApp.DAL.Migrations
{
    using Data;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApp.DAL.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebApp.DAL.Data.ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }

            List<Models.NewUserViewModel> adminUsers = new List<Models.NewUserViewModel> {
                new Models.NewUserViewModel
                {
                    Name = "Administrator",
                    Email = "administrator@qlstyle.com",
                    Designation = "Administrator",
                    Password = "99QNeT#%"
                }
            };
            List<Models.NewUserViewModel> operators = new List<Models.NewUserViewModel>
            {
                new Models.NewUserViewModel
                {
                    Name = "Sameer Anand",
                    Email = "sameer.anand2711@gmail.com",
                    Designation = "Senior Software Engineer",
                    Password = "Sameer1@2"
                },
                new Models.NewUserViewModel
                {
                    Name = "Desmond Chan",
                    Email = "desmond.chan@qlstyle.com",
                    Designation = "System Analyst",
                    Password = "81051101"
                },
                new Models.NewUserViewModel
                {
                    Name = "Kelvin Simon",
                    Email = "it.support@qlstyle.com",
                    Designation = "IT Manager",
                    Password = "81051101"
                },
                new Models.NewUserViewModel
                {
                    Name = "Elvin Kong",
                    Email = "operations@tripsavr.com",
                    Designation = "Finance Manager",
                    Password = "81051101"
                },
                new Models.NewUserViewModel
                {
                    Name = "Shanthi Aravindan",
                    Email = "no_reply@tripsavr.com",
                    Designation = "Global Head",
                    Password = "81011105"
                }
            };

            if (!context.Roles.Any(w=>w.Name.Equals("viewer", StringComparison.OrdinalIgnoreCase)))
            {
                var role = new IdentityRole { Name = "Viewer" };
                var viewerRoleCreateResult = roleManager.Create(role);
                context.SaveChanges();
            }
            ApplicationUser user = null;
            if (!context.Roles.Any(w => w.Name.Equals("admin", StringComparison.OrdinalIgnoreCase)))
            {
                //var role = new IdentityRole
                //{
                //    Name = "Admin",
                //};
                var adminRoleCreateResult = roleManager.Create(new IdentityRole { Name = "Admin" });
                var operatorRoleCreateResult = roleManager.Create(new IdentityRole { Name = "Operator" });
            }




            user = userManager.FindByName("administrator@qlstyle.com");
            if (user == null)
            {
                foreach (var adminUser in adminUsers)
                {
                    var appUser = new ApplicationUser
                    {
                        UserName = adminUser.Email,
                        Email = adminUser.Email
                    };
                    var userCreateResult = userManager.CreateAsync(appUser, adminUser.Password).Result;
                    if (userCreateResult.Succeeded)
                    {
                        context.AppUsers.Add(new User
                        {
                            CreatedOn = DateTime.Now,
                            Designation = adminUser.Designation,
                            Email = adminUser.Email,
                            ExternalUser = appUser,
                            Name = adminUser.Name
                        });
                        context.SaveChanges();
                    }
                    userManager.AddToRole(appUser.Id, "Admin");
                }
                user = userManager.FindByName("administrator@qlstyle.com");
                foreach (var opUser in operators)
                {
                    var appUser = new ApplicationUser
                    {
                        UserName = opUser.Email,
                        Email = opUser.Email
                    };
                    var userCreateResult = userManager.CreateAsync(appUser, opUser.Password).Result;
                    if (userCreateResult.Succeeded)
                    {
                        context.AppUsers.Add(new User
                        {
                            CreatedOn = DateTime.Now,
                            Designation = opUser.Designation,
                            Email = opUser.Email,
                            ExternalUser = appUser,
                            Name = opUser.Name
                        });
                        context.SaveChanges();
                    }
                    userManager.AddToRole(appUser.Id, "Operator");
                }
            }

            if (context.ApplicationGroups.Count() == 0)
            {
                context.ApplicationGroups.Add(new ApplicationGroup("Originator", true));
                for (int i = 1; i < 9; i++)
                {
                    context.ApplicationGroups.Add(new ApplicationGroup("Approver " + i, true));
                }
                context.SaveChanges();
            }


            if (context.Departments.Count() == 0)
            {
                context.Departments.AddRange(new List<Department> {
                    new Department("Admin","125467") { CreatedBy = user },
                    new Department("BVI","456756"){ CreatedBy = user },
                    new Department("Comm","788045"){ CreatedBy = user },
                    new Department("CRT Int'l","119334"){ CreatedBy = user },
                    new Department("DC Office","444490"){ CreatedBy = user },
                    new Department("EDP","450623"){ CreatedBy = user },
                    new Department("EXEC","781912"){ CreatedBy = user },
                    new Department("Finance","113201"){ CreatedBy = user },
                    new Department("Hospitality","Hos"){ CreatedBy = user },
                    new Department("HR","107068"){ CreatedBy = user },
                    new Department("IDC","438357"){ CreatedBy = user },
                    new Department("Information Technology (IT)","769646"){ CreatedBy = user },
                    new Department("IPC","100935"){ CreatedBy = user },
                    new Department("LAD","432224"){ CreatedBy = user },
                    new Department("Legal","763513"){ CreatedBy = user },
                    new Department("Marketing","694802"){ CreatedBy = user },
                    new Department("MD Office","426091"){ CreatedBy = user },
                    new Department("NID","757380"){ CreatedBy = user },
                    new Department("Operations","188669"){ CreatedBy = user },
                    new Department("Others","419958"){ CreatedBy = user },
                    new Department("QI Ltd","751247"){ CreatedBy = user },
                    new Department("QNET","782536"){ CreatedBy = user },
                    new Department("Wellness","413825"){ CreatedBy = user }
                });
                context.SaveChanges();
            }
            if (context.Countries.Count() == 0)
            {
                context.Countries.AddRange(new List<Country> {
                    new Country("United Arab Emirates", "AE"),
                    new Country("Azerbaijan", "AZ"){IsEnabled=false },
                    new Country("Canada", "CA"){IsEnabled=false },
                    new Country("China", "CN"),
                    new Country("Egypt", "EG"){IsEnabled=false },
                    new Country("EU", "EU"),
                    new Country("Hong Kong", "HK"),
                    new Country("Indonesia", "ID"),
                    new Country("India", "IN"),
                    new Country("Japan", "JP"){IsEnabled=false },
                    new Country("Kazakhstan", "KZ"){IsEnabled=false },
                    new Country("Morocco", "MA"){IsEnabled=false },
                    new Country("Malaysia", "MY"),
                    new Country("Russia", "RU"),
                    new Country("Singapore", "SG"),
                    new Country("Thailand", "TH"),
                    new Country("Tunisia", "TN"){IsEnabled=false },
                    new Country("Turkey", "TR"){IsEnabled=false },
                    new Country("United States", "US"),
                    new Country("South Africa", "ZA"),
                    new Country("Philippines", "PH"){IsEnabled=false }
                });
                context.SaveChanges();
            }
            if (context.FrequencyTypes.Count() == 0)
            {
                string[] frequencies = { "Annually", "Daily", "Monthly", "One Time", "Quarterly", "Weekly", "Half Year", "Others" };
                context.FrequencyTypes.AddRange(frequencies.Select(s => new FrequencyType(s)).ToList());
                context.SaveChanges();
            }
            if (context.PaymentMethods.Count() == 0)
            {
                string[] paymentMethods = { "Bank Draft", "Cheque", "Cash", "GIRO", "Money Order", "Others" };
                context.PaymentMethods.AddRange(paymentMethods.Select(s => new PaymentMethod(s)).ToList());
                context.SaveChanges();
            }
            if (context.ExpenseTypes.Count() == 0)
            {
                string[] expenseTypes = { "Capital Expenditure", "Inventory", "Operational", "Others" };
                context.ExpenseTypes.AddRange(expenseTypes.Select(s => new ExpenseType(s)).ToList());
                context.SaveChanges();
            }
            if (context.TaxTypes.Count() == 0)
            {
                context.TaxTypes.AddRange(new List<TaxType> {
                    new TaxType("GBE 10%", 10m),
                    new TaxType("GST (SG) 7%", 7m),
                    new TaxType("VAT 13%", 13m),
                    new TaxType("VAT 13.70%", 13.7m),
                    new TaxType("GST (MY) 6%", 6m),
                    new TaxType("IGST (IN) 18%", 18m)
                });
                context.SaveChanges();
            }
            if (context.Currencies.Count() == 0)
            {
                context.Currencies.AddRange(new List<Currency> {
                    new Currency("Singapore Dollar", "SGD", 0.72m, 0.00m) { CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("Malaysian Ringgit", "MYR", 0.23m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("Philippine Peso", "PHP", 0.020m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("Indian Rupee", "INR", 0.015m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("South African Rand", "ZAR", 0.077m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("Thai Baht", "THB", 0.029m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("Turkish Lira", "TRY", 0.27m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("USD", "USD", 1m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user },
                    new Currency("Emirati Dirham", "AER", 0.272257m, 0.00m){ CreatedOn=DateTime.Now, CreatedBy=user }
                });
                context.SaveChanges();
            }
            if (context.PayingEntities.Count() == 0)
            {
                context.PayingEntities.AddRange(new List<PayingEntity> {
                    new PayingEntity("Bonvo Travel","BNVO") { CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Cosmos Pioneer (S) Pte Ltd","CSMS"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Gita International (S) Pte Ltd","GITA"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Marco Year Limited","MARCO"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle - Thailand","QLST"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle (S) Pte Ltd","QLS"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle (S) Pte Ltd (on behalf of Vanamala)","QLSVAN"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle Holdings Limited","QLH"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle Limited","QLL"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle Properties (M) Sdn Bhd","QLPM"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle Properties, Inc USA","QLPI"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Q Lifestyle Rewards (S) Pte Ltd","QLR"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("QI Group","QIG"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("QI Holdings (HK)","QIH"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("QNET Limited (HK)","QNHK"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("QNET Singapore Pte Ltd","QNSG"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Quest ION Limited","QION"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Quest Vacation International Co., Ltd-Thai","QVIT"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Quest Vacation International Pte Ltd","QVIS"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Questnet Limited (Singapore Branch)","QSNT"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Riverjaya Hotels, Travel & Tourism Services","RVJY"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("RYTHM Foundation","RTMF"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("RYTHM House Pte Ltd","RTMH"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("V Global Management Singapore Pte Ltd","VGM"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now },
                    new PayingEntity("Vanamala Hotels Travel and Tourism","VAN"){ CreatedOn = DateTime.Now, UpdatedOn = DateTime.Now }
                });
                context.SaveChanges();
            }
            if (context.Permissions.Count() == 0)
            {
                context.Permissions.AddRange(new List<Permission> { 
                    new Permission("Generate Report"),
                    new Permission("Close PO/PPRF")
                });
                context.SaveChanges();
            }

            if (context.RejectTypes.Count() == 0)
            {
                string[] rejectTypes = { "Quotation Price Not Satisfied", "Wrong Product", "Out of Stock" };
                context.RejectTypes.AddRange(rejectTypes.Select(s => new RejectTypes(s)).ToList());
                context.SaveChanges();
            }
            //List<PayeeJson> data = null;
            //if (context.Payees.Count() == 0)
            //{
            //    var s = AppDomain.CurrentDomain.BaseDirectory;
            //    data = LoadJson<List<PayeeJson>>("D:\\PaymentProcessingRequestForm\\WebApp.DAL\\CustomData\\InitialData\\payees.json");
            //    List<Payee> payees = PayeeJson.ToPayeeEntity(data);
            //    context.Payees.AddRange(payees);
            //    context.SaveChanges();
            //}
        }

        private T LoadJson<T>(string filepath)
        {
            T items;
            using (StreamReader r = new StreamReader(filepath))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<T>(json);
            }
            return items;
        }
    }

    class PayeeJson
    {
        [JsonProperty("Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("Address 3")]
        public string Address3 { get; set; }

        [JsonProperty("Address 1")]
        public string Address1 { get; set; }

        [JsonProperty("")]
        public string Empty { get; set; }

        [JsonProperty("Address 2")]
        public string Address2 { get; set; }

        [JsonProperty("Bank Account No")]
        public string BankAccountNo { get; set; }

        [JsonProperty("Bank Account Name")]
        public string BankAccountName { get; set; }

        [JsonProperty("Bank Address")]
        public string BankAddress { get; set; }

        [JsonProperty("Hidden")]
        public string Hidden { get; set; }

        [JsonProperty("Contact No")]
        public string ContactNo { get; set; }

        [JsonProperty("Bank Swift Code")]
        public string BankSwiftCode { get; set; }

        [JsonProperty("Fax No")]
        public string FaxNo { get; set; }

        [JsonProperty("IBAN Number")]
        public string IBANNumber { get; set; }

        [JsonProperty("Payee")]
        public string Payee { get; set; }

        [JsonProperty("Hotel")]
        public string Hotel { get; set; }

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("Sort Code")]
        public string SortCode { get; set; }

        [JsonProperty("Type of Account")]
        public string TypeOfAccount { get; set; }


        public static Payee ToPayeeEntity(PayeeJson model)
        {
            if (model == null) return null;
            return new Payee
            {
                Name = model.Payee,
                AddressLine1 = model.Address1,
                AddressLine2 = model.Address2,
                AddressLine3 = model.Address3,
                CountryId = null,
                Fax = model.FaxNo,
                HotelName = model.Hotel,
                IsEnabled = true,
                PayeeBankAccounts = new List<PayeeBankAccountDetail>() { 
                    new PayeeBankAccountDetail{
                        AccountName = model.BankAccountName,
                        AccountNumber = model.BankAccountNo,
                        AccountType = model.TypeOfAccount,
                        BankAddress = model.BankAddress,
                        BankName = model.BankName,
                        IBAN = model.IBANNumber,
                        IFSC = string.Empty,
                        IsEnabled = true,
                        Swift = model.BankSwiftCode
                    }
                },
                Phone = model.ContactNo
            };
        }
        public static List<Payee> ToPayeeEntity(List<PayeeJson> models)
        {
            if (models == null) return null;
            return models.Select(s => ToPayeeEntity(s)).ToList();
        }
    }
}
