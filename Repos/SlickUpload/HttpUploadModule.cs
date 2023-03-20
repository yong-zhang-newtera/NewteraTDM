using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// The HttpUploadModule is the core of NetUpload. 
	/// </summary>
	public sealed class HttpUploadModule : IHttpModule
	{
		void IHttpModule.Dispose()
		{

		}

		void IHttpModule.Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(context_BeginRequest);
			context.Error += new EventHandler(context_Error);
			context.EndRequest += new EventHandler(context_EndRequest);
		}

		byte[] ExtractBoundary(string contentType)
		{
			int pos = contentType.IndexOf("boundary=");

			if (pos > 0)
				return Encoding.ASCII.GetBytes("--" + contentType.Substring(pos + 9));
			else
				return null;
		}

		HttpWorkerRequest GetWorkerRequest(HttpContext context)
		{
/*			Type c = context.GetType();

			PropertyInfo p = c.GetProperty("WorkerRequest",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (p != null)
				return (HttpWorkerRequest)p.GetValue(context, null);
			else
				return null;
*/
			IServiceProvider provider = (IServiceProvider)HttpContext.Current;

			return (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));

		}

		private void context_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;

			if (app.Request.ContentType.ToLower().StartsWith("multipart/form-data"))
			{
				HttpWorkerRequest worker = GetWorkerRequest(app.Context);

				if (worker != null)
				{
					byte[] boundary = ExtractBoundary(app.Request.ContentType);
					
					string uploadId = app.Request.QueryString["uploadId"];

					MimeUploadHandler handler = new MimeUploadHandler(new RequestStream(worker),
						boundary,
						uploadId,
						app.Request.ContentEncoding,
						app.Request.PhysicalApplicationPath);

					if (uploadId != null)
						RegisterIn(app.Context, handler);

					try
					{
						handler.Parse();

						InjectTextParts(worker, handler.TextParts);
					}
					catch (Exception ex)
					{
						CleanupFiles(app.Context, true);

						// TODO: display error
                        string url = "./MessageBox.aspx?Message=Data Transmitting failed due to " + ex.Message + ", please try again.";
                        app.Response.Redirect(url);
					}
				}
			}
			else
			{
				// TODO: also detect progress requests coming in
			}
		}

		void InjectTextParts(HttpWorkerRequest request, string textParts)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic; 
			
			Type type = request.GetType(); 
			
			while ((type != null) && (type.FullName != "System.Web.Hosting.ISAPIWorkerRequest"))
			{ 
				type = type.BaseType; 
			}

			if (type != null)
			{
				byte[] data = Encoding.ASCII.GetBytes(textParts);

				type.GetField("_contentAvailLength", bindingFlags).SetValue(request, textParts.Length); 
				type.GetField("_contentTotalLength", bindingFlags).SetValue(request, textParts.Length);
				type.GetField("_preloadedContent", bindingFlags).SetValue(request, data); 
				type.GetField("_preloadedContentRead", bindingFlags).SetValue(request, true);
			}
		}

		void RegisterIn(HttpContext context, MimeUploadHandler handler)
		{
			context.Items["_uploadHandler"] = handler;
			context.Application["_UploadStatus_" + handler.UploadStatus.UploadId] = handler.UploadStatus;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>A collection containing all files uploaded in the current context.</returns>
		public static UploadedFileCollection GetUploadedFiles()
		{
			return GetUploadedFiles(HttpContext.Current);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context">A context to get the up</param>
		/// <returns>A collection containing all files uploaded in the specified context.</returns>
		public static UploadedFileCollection GetUploadedFiles(HttpContext context)
		{
			UploadedFileCollection parts = ((MimeUploadHandler)context.Items["_uploadHandler"]).UploadedFiles;

			return UploadedFileCollection.ReadOnly(parts);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uploadId"></param>
		/// <returns></returns>
		public static UploadStatus GetUploadStatus(string uploadId)
		{
			return GetUploadStatus(HttpContext.Current.Application, uploadId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="application"></param>
		/// <param name="uploadId"></param>
		/// <returns></returns>
		public static UploadStatus GetUploadStatus(HttpApplicationState application, string uploadId)
		{
			UploadStatus status = (UploadStatus)application["_UploadStatus_" + uploadId];

			return status;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uploadId"></param>
		public static void RemoveFrom(string uploadId)
		{
			RemoveFrom(HttpContext.Current.Application, uploadId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="application"></param>
		/// <param name="uploadId"></param>
		public static void RemoveFrom(HttpApplicationState application, string uploadId)
		{
			application.Remove("_UploadStatus_" + uploadId);
		}

		private void context_Error(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;

			CleanupFiles(app.Context, true);
		}

		private void context_EndRequest(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;

			CleanupFiles(app.Context, false);
		}

		void CleanupFiles(HttpContext context, bool remove)
		{
			MimeUploadHandler handler = context.Items["_uploadHandler"] as MimeUploadHandler;

			if (handler != null)
			{
				// Delete all files
				//foreach (UploadedFile file in handler.UploadedFiles)
				//	File.Delete(file.ServerPath);

				if (remove)
					RemoveFrom(context.Application, handler.UploadStatus.UploadId);
			}
		}
	}
}