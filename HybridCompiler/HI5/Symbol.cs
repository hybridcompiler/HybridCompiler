using HILibrary;
using System.Collections.Generic;

namespace HI5
{
    internal class Symbol
    {
        public long IntegerValue { get; set; }
        public Token Token { get; private set; }

        public Symbol(Token token)
        {
            HIDebug.AssertEqual(token.Type, TT.Identifier);
            if (symbols.TryGetValue(token.Text, out _))
            {
                throw new HIException(ET.NameError, Id.NameAlreadyDefined, token);
            }
            Token = token;
            symbols[token.Text] = this;
        }

        public static bool Find(Token token, out Symbol symbol) =>
            symbols.TryGetValue(token.Text, out symbol);

        private static readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
    }
}