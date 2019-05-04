using System.Collections.Generic;

namespace HI6
{
    internal class Symbol
    {
        public long IntegerValue { get; set; }
        public Token Token { get; private set; }
        public FunctionNode Function { get; set; }

        public Symbol(Token token) => Token = token;
    }

    internal class SymbolTable
    {
        public static void Init()
        {
            blockNodes.Clear();
            Open();
        }

        public static BlockNode Open()
        {
            var blockNode = new BlockNode();
            blockNodes.Push(blockNode);
            return blockNode;
        }

        public static void Close() => blockNodes.Pop();

        public static Symbol AddSymbol(Token token)
        {
            if (token.Type != TT.Identifier)
            {
                throw new HIException(ET.SyntaxError, Id.InvalidSyntax, token);
            }
            var symbol = new Symbol(token);
            blockNodes.Peek().AddSymbol(token, symbol);
            return symbol;
        }

        public static bool Find(Token token, out Symbol symbol) => Find(token.Text, out symbol);

        public static bool Find(string name, out Symbol symbol)
        {
            foreach(var blockNode in blockNodes)
            {
                if (blockNode.GetSymbol(name, out symbol)) { return true; }
            }
            symbol = null;
            return false;
        }

        public static Symbol GetSymbol(Token token)
        {
            if (Find(token, out Symbol symbol)) { return symbol; }
            throw new HIException(ET.NameError, Id.NameNotDefined, token);
        }

        public static Symbol GetVariable(Token token)
        {
            var symbol = GetSymbol(token);
            if (symbol.Function != null) { throw new HIException(ET.TypeError, Id.FunctionIdentifier, token); }
            return symbol;
        }

        public static Symbol GetFunction(Token token)
        {
            var symbol = GetSymbol(token);
            if (symbol.Function == null) { throw new HIException(ET.TypeError, Id.NotFunctionIdentifier, token); }
            return symbol;
        }

        private static readonly Stack<BlockNode> blockNodes = new Stack<BlockNode>();
    }
}