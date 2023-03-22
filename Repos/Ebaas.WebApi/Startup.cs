using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Configuration;
using System.Net.Http.Formatting;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Dispatcher;
using System.Web.Http.Results;
using System.Web.Http.Filters;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Assembly = System.Reflection.Assembly;

using Swashbuckle.Application;

using Newtera.Registry;
using Ebaas.WebApi.Infrastructure;
using Ebaas.WebApi.Providers;
using Newtera.Common.Core;
using Newtera.Server.UsrMgr;
using Newtera.Server.Timer;
using Newtera.Server.FullText;
using Newtera.Server.Engine.Cache;
using Newtera.ElasticSearchIndexer;
//using Newtera.MasterServerClient;

namespace Ebaas.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigStaticFiles(app);

            ConfigureOAuthTokenGeneration(app);

            ConfigureOAuthTokenConsumption(app);

            ConfigureWebApi(httpConfig);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(httpConfig);

            var idProvider = new CustomUserIdProvider();

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            app.MapSignalR();

            InitializeServices();
        }


        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            string baseUrl = ConfigurationManager.AppSettings[NewteraNameSpace.BASE_URL];

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(baseUrl)
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            // Set our own assembly resolver where we add the assemblies we need
            //AssembliesResolver assemblyResolver = new AssembliesResolver();
            //config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

            config.Filters.Add(new CustomExceptionFilter());

            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // There can be multiple exception loggers. (By default, no exception loggers are registered.)
            config.Services.Add(typeof(IExceptionLogger), new ApiExceptionLogger());

            config.EnableSwagger(c =>
                    {
                        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                        var commentsFile = Path.Combine(baseDirectory, commentsFileName);


                        c.SingleApiVersion("v1", "Ebaas REST API");
                        c.IncludeXmlComments(commentsFile);
                    }
                )
                .EnableSwaggerUi();
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            string issuer = ConfigurationManager.AppSettings[NewteraNameSpace.BASE_URL];
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityKeyProviders = new IIssuerSecurityKeyProvider[] {
                        new SymmetricKeyIssuerSecurityKeyProvider(issuer, audienceSecret)
                    }
                });
        }

        private void InitializeServices()
        {
            Newtera.Common.MetaData.XmlDataSourceListHandler.XmlDataSourceService = new Newtera.Server.Util.XmlDataSourceService();
            TimerManager.Instance.Start();
            IndexingManager.Instance.Start();

            if (UserDataCache.Instance.AllUsers == null)
            {
                // initialize the user data caceh
                UserDataCache.Instance.Initialize(); // this take a lot of time
            }

            if (Newtera.Common.Config.ElasticsearchConfig.Instance.IsElasticsearchEnabled)
            {
                // initialize IndexEventManager singleton with an SingleDocumentRunner
                IndexEventManager.Instance.IndexingRunner = new SingleDocumentRunner();
            }
        }

        private void ConfigStaticFiles(IAppBuilder app)
        {
            XmlRegistryKey rootKey;
            XmlRegistry theRegistry = XmlRegistryManager.Instance;

            rootKey = theRegistry.RootKey;

            XmlRegistryKey staticFilesRootKey = rootKey.GetSubKey(NewteraNameSpace.STATIC_FILES_ROOT, false);

            String rootFolder = (String)staticFilesRootKey.GetStringValue();
            PhysicalFileSystem fileSystem = new PhysicalFileSystem(rootFolder);

            var options = new FileServerOptions();

            options.EnableDirectoryBrowsing = true;
            options.FileSystem = fileSystem;
            options.StaticFileOptions.ContentTypeProvider = new CustomContentTypeProvider();

            app.UseFileServer(options);
        }

        private bool HasMultipleServers()
        {
            return false;
        }
    }

    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
        }
    }

    // get user id from the query string
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            string userId = request.QueryString["user"];

            return userId;
        }
    }

    public class AssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            //var controllersAssembly = Assembly.LoadFrom(@".\custom");
            //baseAssemblies.Add(controllersAssembly);
            return assemblies;
        }
    }

    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }

            ErrorLog.Instance.WriteLine(exceptionMessage);

            //We can log this exception message to the file or database.  
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(exceptionMessage),  
                    ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
            };
            actionExecutedContext.Response = response;
        }
    }
}