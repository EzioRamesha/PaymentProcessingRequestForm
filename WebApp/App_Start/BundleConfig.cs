using System.Web;
using System.Web.Optimization;

namespace WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/jquery-migrate-1.2.1.min.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/jquery.metisMenu.js",
                        "~/Scripts/jquery.slimscroll.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.menu.js",
                        "~/Scripts/jquery.moneymask.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/bootstrap-hover-dropdown.js",
                        "~/Scripts/bootstrap-datepicker.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-resource.min.js",
                    "~/Scripts/angular-animate.min.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                    "~/Scripts/ng-file-upload-shim.min.js",
                    "~/Scripts/ng-file-upload.min.js"
                    //"~/Scripts/PageScripts/app.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/angular/HomeController")
                    .Include("~/Scripts/PageScripts/NewRequest/RequestService.js")
                    .Include("~/Scripts/PageScripts/Home/homeController.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular/generalService")
                    .Include("~/Scripts/PageScripts/Settings/GeneralService.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular/pprfNew")
                    .Include("~/Scripts/PageScripts/Settings/PayeeManager.js")
                    .Include("~/Scripts/PageScripts/NewRequest/RequestService.js")
                    .Include("~/Scripts/PageScripts/NewRequest/newRequestController.js"));

            bundles.Add(new ScriptBundle("~/bundles/settings/country")
                    .Include("~/Scripts/PageScripts/Settings/CountriesController.js"));

            bundles.Add(new ScriptBundle("~/bundles/settings/currency")
                .Include("~/Scripts/PageScripts/Settings/CurrenciesController.js"));

            bundles.Add(new ScriptBundle("~/bundles/settings/payee")
                    .Include("~/Scripts/PageScripts/Settings/PayeeManager.js")
                    .Include("~/Scripts/PageScripts/Settings/PayeesController.js"));

            bundles.Add(new ScriptBundle("~/bundles/settings/payingEntity")
                    .Include("~/Scripts/PageScripts/Settings/PayingEntitiesController.js"));

            bundles.Add(new ScriptBundle("~/bundles/settings/user")
                    .Include("~/Scripts/PageScripts/Settings/UsersController.js"));

            bundles.Add(new ScriptBundle("~/bundles/common")
                    .Include("~/Scripts/html5shiv.js")
                    .Include("~/Scripts/respond.min.js")
                    .Include("~/Scripts/icheck.min.js")
                    .Include("~/Scripts/holder.js")
                    .Include("~/Scripts/responsive-tabs.js")
                    .Include("~/Scripts/main.js"));



            bundles.Add(new StyleBundle("~/Content/styles")
                    .Include("~/Content/jquery-ui-1.10.4.custom.min.css")
                    .Include("~/Content/font-awesome.min.css")
                    .Include("~/Content/bootstrap.min.css")
                    .Include("~/Content/animate.css")
                    .Include("~/Content/all.css")
                    .Include("~/Content/main.css")
                    .Include("~/Content/style-responsive.css")
                    .Include("~/Content/bootstrap-datepicker.min.css"));



            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                    .Include("~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
        }
    }
}
