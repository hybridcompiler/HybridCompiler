using System;

namespace HI3
{
    internal enum ET
    {
        SyntaxError,
    }

    internal class HIException : Exception
    {
        public static string Text { private get; set; }

        public HIException(ET type, Id message, byte column) :
            base(string.Format(errorMessage, Text, "".PadLeft(column), type, message.Str()))
        {
            this.type = type;
            this.column = column;
            HResult = (int)message;
        }

        private const string errorMessage = "{0}\n{1}^\n{2}: {3}";

        private readonly byte column;
        private readonly ET type;
    }
}