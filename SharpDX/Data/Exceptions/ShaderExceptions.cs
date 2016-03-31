using System;

namespace SharpDX.Data.Exceptions
{
    class ShaderCompilerException : ApplicationException
    {
        public ShaderCompilerException(string message, Exception innerException) : base(message, innerException) {}
    }
}
