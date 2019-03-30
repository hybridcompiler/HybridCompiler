using System.Collections.Generic;

namespace HI3
{
    internal class Parser
    {
        public Parser(List<Token> tokens) => this.tokens = tokens;

        public Node Run()
        {
            var node = ParseExpression();
            Match(TT.EOF);
            return node;
        }

        private readonly List<Token> tokens;
        private int currentPos;

        private Token CurrentToken => tokens[currentPos];

        private void Match(TT type)
        {
            if (CurrentToken.Type != type)
            {
                throw new HIException(ET.SyntaxError, Id.InvalidSyntax, CurrentToken.Column);
            }
            MoveNext();
        }

        private void MoveNext() => currentPos++;

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

        // EBNF : factor = ( [ "+" | "-" ], integer ) | ( "(", expression, ")" ) ;
        private Node ParseFactor()
        {
            var token = CurrentToken;
            MoveNext();

            switch (token.Type)
            {
                case TT.Plus:
                case TT.Minus:
                    var integerToken = CurrentToken;
                    Match(TT.Integer);
                    if (token.Type == TT.Minus) { integerToken.Integer *= -1; }
                    return new IntegerNode(integerToken);

                case TT.Integer:
                    return new IntegerNode(token);

                case TT.LeftParens:
                    var node = ParseExpression();
                    Match(TT.RightParens);
                    return node;

                default: throw new HIException(ET.SyntaxError, Id.InvalidSyntax, token.Column);
            }
        }

        // term = factor, { "*" | "/", factor } ;
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
    }
}