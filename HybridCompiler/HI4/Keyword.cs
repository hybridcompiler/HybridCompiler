using System;
using System.Collections.Generic;
using System.Linq;

namespace HI4
{
    // Keyword Type
    internal enum KT
    {
        dim,
    }

    internal static class Keyword
    {
        static Keyword()
        {
            Enum.GetValues(typeof(KT)).Cast<KT>().ToList().
                ForEach(type => keywords[type.ToString()] = type);
        }

        public static bool Find(string identifier, out KT keywordType) =>
            keywords.TryGetValue(identifier, out keywordType);

        private static Dictionary<string, KT> keywords = new Dictionary<string, KT>();
    }
}