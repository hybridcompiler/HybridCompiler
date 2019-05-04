using HILibrary;
using System;

namespace HI6
{
    // Error Type
    internal enum ET
    {
        SyntaxError,
        ZeroDivisionError,
        NameError,
        TypeError,
    }

    internal class HIException : Exception
    {
        public static string SourceCode { private get; set; }

        // Lexer에서 예외 발생시 사용하는 생성자
        public HIException(ET errorType, Id errorId, byte errorColumn, int errorLine) :
            base(CreateErrorMessage(errorType, errorId, errorColumn, errorLine))
        {
            HResult = (int)errorId;
            Data["Id"] = errorId;
        }

        // Parser에서 예외 발생시 사용하는 생성자
        public HIException(ET errorType, Id errorId, Token token) :
            base(CreateErrorMessage(errorType, errorId, token.Column, token.Line))
        {
            HResult = (int)errorId;
            Data["Id"] = errorId;
        }

        // 코드에서 발생한 예외가 아닐시 사용하는 생성자
        public HIException(ET errorType, Id errorId) :
            base(string.Format("{0}: {1}",errorType, errorId.ToText()))
        {
            HResult = (int)errorId;
            Data["Id"] = errorId;
        }

        private static string CreateErrorMessage(ET errorType, Id errorId,
            byte errorColumn, int errorLine)
        {
            string[] codes = SourceCode.Split((char)TT.LF);
            HIDebug.Assert(codes.Length > errorLine);
            return string.Format(Id.ErrorFormat.ToText(), codes[errorLine],
                "".PadLeft(errorColumn), errorType, errorId.ToText(), errorLine + 1);
        }
    }
}