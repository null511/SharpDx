using System;
using System.Text;

namespace SharpDX.Extensions
{
    static class ExceptionUtils
    {
        public static string UnfoldMessage(this Exception error) {
            var builder = new StringBuilder(error.Message);
            while (error.InnerException != null) {
                builder.Append(" > "+error.InnerException.Message);
                error = error.InnerException;
            }
            return builder.ToString();
        }
    }
}
