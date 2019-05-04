using System;
using System.Collections.Generic;
using System.Linq;

namespace HI6
{
    // Keyword Type
    internal enum KT
    {
        Print,
        Dim,
        If,
        Elif,
        Else,
        While,
        For,
        In,
        Range,
        Def,
    }

    internal static class Keyword
    {
        static Keyword()
        {
            Enum.GetValues(typeof(KT)).Cast<KT>().ToList().
                ForEach(type => keywords[type.ToString().ToLower()] = type);
        }

        public static bool Find(string identifier, out KT keywordType) =>
            keywords.TryGetValue(identifier, out keywordType);

        private static readonly Dictionary<string, KT> keywords = new Dictionary<string, KT>();
    }
}