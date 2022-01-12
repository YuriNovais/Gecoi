using System.Web.Optimization;

namespace Protocolo
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymask").Include(
                        "~/Scripts/jquery.maskedinput.js",
                        "~/Scripts/maskedinput-binder.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/metisMenu").Include(
                      "~/Scripts/metisMenu.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                      "~/Scripts/moment.js",
                      "~/Scripts/pt-br.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                      "~/Scripts/raphael.js",
                      "~/Scripts/morris.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                      "~/Scripts/jquery.dataTables.js",
                      "~/Scripts/dataTables.bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/sbAdmin2").Include(
                      "~/Scripts/sb-admin-2.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/metisMenu.css",
                      "~/Content/dataTables.bootstrap.css",
                      "~/Content/dataTables.responsive.css",
                      "~/Content/sb-admin-2.css",
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap-datetimepicker.css"));
        }
    }
}
