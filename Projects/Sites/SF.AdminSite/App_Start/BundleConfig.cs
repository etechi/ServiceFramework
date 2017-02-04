using System.Web;
using System.Web.Optimization;

namespace SF.AdminSite
{
	public class BundleConfig
	{
		class file
		{
			public string js { get; set; }
		}

		static string GetScriptAsset(string name)
		{
#if DEBUG
			return "~/scripts/" + name + ".js";
#else
			var text = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/scripts/assets.js"));
			var i = text.IndexOf('{');
			var e = text.LastIndexOf('}');
			var cfgs = ServiceProtocol.Json.Decode<Dictionary<string, file>>(text.Substring(i, e - i + 1));
			var f = cfgs.Get(name);
			return f == null ? "~/scripts/" + name + ".js" : "~" + f.js;
#endif
		}

		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/site.css"));



			bundles.Add(new Bundle("~/bundles/admin/js").Include(
				"~/vender/js/kindeditor/kindeditor-all-min.js",
				"~/vender/js/clipboard.min.js",
				GetScriptAsset("admin")
				));


			bundles.Add(new StyleBundle("~/bundles/admin/css").Include(
				"~/vender/massets/global/plugins/font-awesome/css/font-awesome.min.css",
				"~/vender/massets/global/plugins/simple-line-icons/simple-line-icons.min.css",
				"~/vender/massets/global/plugins/bootstrap/css/bootstrap.min.css",
				"~/vender/massets/global/plugins/uniform/css/uniform.default.css",
				"~/vender/massets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",


				"~/vender/massets/global/css/components.css",
				"~/vender/massets/global/css/plugins.min.css",
				"~/vender/massets/layouts/layout/css/layout.css",

				"~/vender/massets/layouts/layout/css/themes/default.min.css",
				"~/vender/massets/layouts/layout/css/custom.css",
				"~/vender/js/kindeditor/themes/default/default.css",

				//"~/Content/bootstrap.css",
				//"~/Content/defaultStyle.css",
				"~/Content/index.css"
				 ));
		}
	}
}
