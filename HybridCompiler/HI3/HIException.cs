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

        public HIException(ET type, Id message, byte column)
        {
            this.type = type;
            this.message = message;
            this.column = column;
        }

        public override string ToString() => string.Format(errorMessage,
            Text, "".PadLeft(column), type, message.Str());

        private const string errorMessage = "{0}\n{1}^\n{2}: {3}";

        private readonly byte column;
        private readonly Id message;
        private readonly ET type;
    }
}