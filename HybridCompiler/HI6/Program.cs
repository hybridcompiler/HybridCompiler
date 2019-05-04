using HILibrary;
using System;

namespace HI6
{
    internal static class Program
    {
        public static string ToText(this Id id) => lang[id];

        private static readonly Texts lang = new Texts();

        private static void Main(string[] args)
        {
            HIConsole.Run(() => Test.Run(), (text) =>
            {
                var tokens = new Lexer(text).Run();
                var node = new Parser(tokens).ParseLine();
                var result = node.Visit;
                Console.WriteLine(result);
            });
        }
    }
}