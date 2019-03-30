using System;

namespace HI3
{
    internal enum ET
    {
        SyntaxError,
        ZeroDivisionError,
    }

    internal class HIException : Exception
    {
        public static string Text { private get; set; }

        public HIException(ET type, Id message, byte column) :
            base(string.Format(errorMessage, Text, "".PadLeft(column), type, message.ToText()))
        {
            HResult = (int)message;
        }

        private const string errorMessage = "{0}\n{1}^\n{2}: {3}";
    }
}