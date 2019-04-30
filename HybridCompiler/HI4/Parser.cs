using System.Collections.Generic;

namespace HI4
{
    internal class Parser
    {
        public Parser(List<Token> tokens) => this.tokens = tokens;

        public Node ParseLine()
        {
            var node = ParseStatement();
            Match(TT.EOF);
            return node;
        }

        public Node Run()
        {
            var node = ParseBlock();
            Match(TT.EOF);
            return node;
        }

        private readonly List<Token> tokens;
        private int currentPos;

        private Token CurrentToken => tokens[currentPos];

        private Token NextToken =>
            tokens.Count > currentPos + 1 ? tokens[currentPos + 1] : tokens[tokens.Count - 1];

        private Symbol GetSymbol(Token token)
        {
            if (Symbol.Find(token, out Symbol symbol)) { return symbol; }
            throw new HIException(ET.NameError, Id.NameNotDefined, token);
        }

        private void Match(TT type)
        {
            if (CurrentToken.Type != type)
            {
                throw new HIException(ET.SyntaxError, Id.InvalidSyntax, CurrentToken);
            }
            MoveNext();
        }

        private void MoveNext() => currentPos++;

        // EBNF : assignment_statement = identifier, "=", expression ;
        private Node ParseAssignmentStatement(Symbol symbol)
        {
            var token = CurrentToken;
            Match(TT.Assign);
            return new AssignNode(symbol, token, ParseExpression());
        }

        // EBNF : block = "{", { statement }, "}"
        private Node ParseBlock()
        {
            Match(TT.BeginBlock);
            var node = new BlockNode();
            while (CurrentToken.Type != TT.EndBlock) { node.Nodes.Add(ParseStatement()); }
            MoveNext();
            return node;
        }

        // EBNF : expression = term, { "+" | "-", term } ;
        private Node ParseExpression()
        {
            var leftNode = ParseTerm();
            var token = CurrentToken;
            while (token.Type == TT.Plus || token.Type == TT.Minus)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseTerm());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : factor =  ( [ "+" | "-" ], factor ) | integer | identifier |
        //                  ( "(", expression, ")" ) ;
        private Node ParseFactor()
        {
            var token = CurrentToken;
            MoveNext();

            switch (token.Type)
            {
                case TT.Plus:
                case TT.Minus: return new UnaryNode(ParseFactor(), token);

                case TT.Integer: return new IntegerNode(token);
                case TT.Identifier: return new IdentifierNode(GetSymbol(token));

                case TT.LeftParens:
                    var node = ParseExpression();
                    Match(TT.RightParens);
                    return node;

                default: throw new HIException(ET.SyntaxError, Id.InvalidSyntax, token);
            }
        }

        // EBNF : keyword_statement = variable_declaration ;
        private Node ParseKeywordStatement()
        {
            var token = CurrentToken;
            MoveNext();

            switch (token.KeywordType)
            {
                case KT.dim: return ParseVariableDeclaration();
            }
            throw new HIException(ET.SyntaxError, Id.InvalidSyntax, token);
        }

        // EBNF : statement = expression | keyword_statement | assignment_statement ;
        private Node ParseStatement()
        {
            var token = CurrentToken;
            switch (token.Type)
            {
                case TT.Keyword: return ParseKeywordStatement();

                case TT.Identifier:
                    if (NextToken.Type == TT.Assign)
                    {
                        MoveNext();
                        return ParseAssignmentStatement(GetSymbol(token));
                    }
                    break;
            }
            return ParseExpression();
        }

        // EBNF : term = factor, { "*" | "/", factor } ;
        private Node ParseTerm()
        {
            var leftNode = ParseFactor();
            var token = CurrentToken;
            while (token.Type == TT.Mul || token.Type == TT.Div)
            {
                MoveNext();
                leftNode = new BinaryNode(leftNode, token, ParseFactor());
                token = CurrentToken;
            }
            return leftNode;
        }

        // EBNF : variable_declaration = "dim", ( identifier | assignment_statement ) ;
        private Node ParseVariableDeclaration()
        {
            var symbol = new Symbol(CurrentToken);
            Match(TT.Identifier);
            if (CurrentToken.Type == TT.Assign) { return ParseAssignmentStatement(symbol); }
            return new Node();
        }
    }
}