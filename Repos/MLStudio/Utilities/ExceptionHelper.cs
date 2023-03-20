
namespace Newtera.MLStudio.Utilities
{
    using System;
    using System.Text;

    public class ExceptionHelper
    {
        public static string FormatStackTrace(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(exception.Message);
            sb.Append(exception.StackTrace);

            if (exception.InnerException != null)
            {
                return sb.Append(FormatStackTrace(exception.InnerException)).ToString();
            }
            else
            {
                return sb.ToString();
            }
        }
    }
}
