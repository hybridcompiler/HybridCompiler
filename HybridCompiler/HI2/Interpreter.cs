using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HI2
{
    internal class Interpreter
    {
        public Interpreter(List<Token> tokens) => this.tokens = tokens;

        public long Run()
        {
            return InterpreteExpression();
        }

        private readonly List<Token> tokens;

        private int currentPos;

        private Token CurrentToken => tokens.Count > currentPos ? tokens[currentPos] : new Token { type = TokenType.EOF };

        // EBNF : expression = term, { "+" | "-", term } ;
        private long InterpreteExpression()
        {
            using (var tracer = new Tracer("expression"))
            {
                long expression = InterpreteTerm();

                while (CurrentToken.type == TokenType.Plus || CurrentToken.type == TokenType.Minus)
                {
                    var op = CurrentToken;
                    MoveNext();
                    var term = InterpreteTerm();
                    if (op.type == TokenType.Plus)
                    {
                        tracer.Print($"{expression} + {term}");
                        expression += term;
                    }
                    else
                    {
                        tracer.Print($"{expression} - {term}");
                        expression -= term;
                    }
                }
                tracer.Print(expression);
                return expression;
            }
        }

        // EBNF : factor = ( [ "+" | "-" ], integer ) | ( "(", expression, ")" ) ;
        private long InterpreteFactor()
        {
            using (var tracer = new Tracer("factor"))
            {
                var token = CurrentToken;
                MoveNext();

                long factor;
                switch (token.type)
                {
                    case TokenType.Plus:
                    case TokenType.Minus:
                        var integer = CurrentToken;
                        Match(TokenType.Integer);
                        factor = token.type == TokenType.Plus ? integer.integer : -integer.integer;
                        break;

                    case TokenType.Integer:
                        factor = token.integer;
                        break;

                    case TokenType.LeftParens:
                        var expression = InterpreteExpression();
                        Match(TokenType.RightParens);
                        factor = expression;
                        break;

                    default: throw new Exception($"{token.type} : 알 수 없는 토큰 타입입니다.");
                }
                tracer.Print(factor);
                return factor;
            }
        }

        // term = factor, { "*" | "/", factor } ;
        private long InterpreteTerm()
        {
            using (var tracer = new Tracer("term"))
            {
                long term = InterpreteFactor();

                while (CurrentToken.type == TokenType.Mul || CurrentToken.type == TokenType.Div)
                {
                    var op = CurrentToken;
                    MoveNext();
                    var factor = InterpreteFactor();
                    if (op.type == TokenType.Mul)
                    {
                        tracer.Print($"{term} * {factor}");
                        term *= factor;
                    }
                    else
                    {
                        tracer.Print($"{term} / {factor}");
                        term /= factor;
                    }
                }
                tracer.Print(term);
                return term;
            }
        }

        private void Match(TokenType type)
        {
            if (CurrentToken.type != type) { throw new Exception($"{CurrentToken.type} : {type} 타입 토큰이 필요합니다."); }
            MoveNext();
        }

        private void MoveNext() => currentPos++;
    }

    internal class Tracer : IDisposable
    {
        public Tracer(string text)
        {
            name = text;
            Console.WriteLine($"{indent}Begin {name}");
            indent += "    ";
        }

        public void Dispose()
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

            indent = indent.Remove(0, 4);
            Console.WriteLine($"{indent}End {name}");
            name = null;
        }

        public void Print(object text) => Console.WriteLine($"{indent}{name} = {text}");

        private static string indent;

        private string name;
    }
}