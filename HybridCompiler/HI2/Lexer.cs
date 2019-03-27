using System;
using System.Collections.Generic;

namespace HI2
{
    internal class Lexer
    {
        public Lexer(string text) => this.text = text;

        // 문자열로부터 토큰을 가져온다.
        public Token GetNextToken()
        {
            while (CurrentChar != (char)0)
            {
                if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    return new Token { type = TokenType.Integer, integer = GetInteger() };
                }
                switch (CurrentChar)
                {
                    case ' ': SkipSpace(); continue;
                    case '+': MoveNext(); return new Token { type = TokenType.Plus };
                    case '-': MoveNext(); return new Token { type = TokenType.Minus };
                    case '*': MoveNext(); return new Token { type = TokenType.Mul };
                    case '/': MoveNext(); return new Token { type = TokenType.Div };
                    case '(': MoveNext(); return new Token { type = TokenType.LeftParens };
                    case ')': MoveNext(); return new Token { type = TokenType.RightParens };
                }
                throw new Exception($"{CurrentChar} : 유효하지 않은 문자입니다.");
            }
            return new Token { type = TokenType.EOF };
        }

        // 문자열을 토큰 목록으로 변환한다.
        public List<Token> Run()
        {
            Token token;
            var tokens = new List<Token>();
            while ((token = GetNextToken()).type != TokenType.EOF)
            {
                tokens.Add(token);
            }
            return tokens;
        }

        private readonly string text;

        private int currentPos;

        private char CurrentChar => text.Length > currentPos ? text[currentPos] : (char)0;

        // 숫자를 가져온다.
        private long GetInteger()
        {
            long integer = 0;
            do
            {
                integer = integer * 10 + CurrentChar - '0';
                MoveNext();
            } while (CurrentChar >= '0' && CurrentChar <= '9');
            return integer;
        }

        private void MoveNext() => currentPos++;

        // 공백을 제거한다.
        private void SkipSpace()
        {
            do
            {
                MoveNext();
            } while (CurrentChar == ' ');
        }
    }
}