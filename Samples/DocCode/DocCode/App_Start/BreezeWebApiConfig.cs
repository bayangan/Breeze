using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(DocCode.App_Start.BreezeWebApiConfig), "RegisterBreezePreStart")]
namespace DocCode.App_Start {
  ///<summary>
  /// Inserts the Breeze Web API controller route at the front of all Web API routes
  ///</summary>
  ///<remarks>
  /// This class is discovered and run during startup; see
  /// http://blogs.msdn.com/b/davidebb/archive/2010/10/11/light-up-your-nupacks-with-startup-code-and-webactivator.aspx
  ///</remarks>
  public static class BreezeWebApiConfig {

    public static void RegisterBreezePreStart() {
      GlobalConfiguration.Configuration.Routes.MapHttpRoute(
          name: "BreezeApi",
          routeTemplate: "breeze/{controller}/{action}"
      );

      WriteMetadataScriptFiles.WriteNorthwindMetadataScriptFile();

      #region Breeze MultiController routes

        // See controllers in the 'MultiControllers' directory
        // Simplistic. Prefer Darrel Miller's API Router for complex routing
        // https://github.com/tavis-software/ApiRouter#readme

        // Breeze Metadata call when "multi" is the manager's service name
        GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            name: "MultiControllerMetadataApi",
            routeTemplate: "multi/metadata",
            defaults: new {controller = "multi", action = "Metadata"}
            );

        // Breeze SaveChanges call when "multi" is the manager's service name
        GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            name: "MultiControllerSaveChangesApi",
            routeTemplate: "multi/savechanges",
            defaults: new {controller = "multi", action = "SaveChanges"}
            );

        //GET ~/multi/search/employees/nancy
        GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            name: "MultiControllerSearchApi",
            routeTemplate: "multi/search/{action}/{searchText}",
            defaults: new { controller = "search", searchText = string.Empty },
            constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

        //GET ~/multi/employees/1
        //GET ~/multi/customers/785efa04-cbf2-4dd7-a7de-083ee17b6ad2
        GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            name: "MultiControllerIdApi",
            routeTemplate: "multi/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional },
            constraints: new { id = @"(^\d+$)|(^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$)" }
            );

        //GET ~/multi/customers
        GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            name: "MultiControllerGetApi",
            routeTemplate: "multi/{controller}",
            defaults: new { },
            constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

        //GET ~/multi/orderprocess/customers
        GlobalConfiguration.Configuration.Routes.MapHttpRoute(
            name: "MultiControllerActionApi",
            routeTemplate: "multi/{controller}/{action}/{id}",
            defaults: new { id = RouteParameter.Optional }
            );

      #endregion

    }
  }
}