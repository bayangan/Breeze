using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;
using Zza.DataAccess.EF;

namespace Zza.Controllers 
{
    public class DevController : ApiController
    {
        public DevController() {
            _repository = new DevRepository();
        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            _repository.UserStoreId = GetUserStoreId();
        }

        // ~/breeze/ZzaEf/reset - clears the current user's changes
        // ~/breeze/ZzaEf/reset/?options=fullreset - clear out all user changes; back to base state.
        [HttpPost]
        public string Reset(string options = "")
        {
            return _repository.Reset(options);
        }

        [HttpPost]
        public HttpResponseMessage WriteScriptFile(string filename, JObject body, string path=null, string prefix=null, string postfix=null)
        {
            if (String.IsNullOrWhiteSpace(path)) { path = "~/test/testdata/"; }
            var scriptFilename = HostingEnvironment.MapPath(path + filename + ".js");
            return _writeFile(scriptFilename, body, prefix, postfix);
        }

        private HttpResponseMessage _writeFile(string filename, JObject body, string prefix=null, string postfix=null)
        {
            if (_devUserId != GetUserStoreId().ToString().ToUpper())
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            App_Start.ScriptWriter.WriteFile(filename, body.ToString(), prefix, postfix);

            return Request.CreateResponse(HttpStatusCode.OK, filename);          
        }

        /// <summary>
        /// Get the repository UserStoreId from the current request
        /// </summary>
        private Guid GetUserStoreId()
        {
            try {
                var id = Request.Headers.GetValues("X-UserSessionId").First();
                return Guid.Parse(id);
            } catch {
                return Guid.Empty;
            }
        }

        private const string _devUserId = "A5844F4B-CAC6-47F3-91B1-A94D53CF6000";
        private readonly DevRepository _repository;
    }
}