using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Threading;
using System.Threading.Tasks;

using Newtera.Common.Core;

namespace Ebaas.WebApi.Infrastructure
{
    public class ApiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            /*
            if (context.Exception != null)
            {
                ErrorLog.Instance.WriteLine("Request URI:" + context.Request.RequestUri.ToString() + "; \n error message: " + context.Exception.Message + "\n" + context.Exception.StackTrace);
            }
            */
        }
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            if (context.Exception != null)
            {
                ErrorLog.Instance.WriteLine("Request URI:" + context.Request.RequestUri.ToString() + "; \n error message: " +  context.Exception.Message + "\n" + context.Exception.StackTrace);
            }

            return base.LogAsync(context, cancellationToken);
        }
    }
}