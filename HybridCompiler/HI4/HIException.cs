﻿using System;

namespace HI4
{
    internal class HIException : Exception
    {
        public static string Text { private get; set; }

        public HIException(ET type, Id messageId, byte column) :
            base(string.Format(errorMessage, Text, "".PadLeft(column), type, messageId.Str()))
        {
            this.type = type;
            this.column = column;
            HResult = (int)messageId;
        }

        private static readonly string errorMessage = "{0}\n{1}^\n{2}: {3}";

        private readonly byte column;
        private readonly ET type;
    }
}