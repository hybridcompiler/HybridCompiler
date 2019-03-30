using System;
using System.Collections.Generic;
using System.Linq;

namespace HI3
{
    internal class Lexer
    {
        // 문자를 Token 타입으로 매핑해주는 맵을 설정한다.
        // static 필드들에 대한 설정이기 static 생성자에서 설정한다..
        static Lexer()
        {
            Enum.GetValues(typeof(TT)).Cast<TT>().Where(type => type <= TT.EOF).
                ToList().ForEach(type => charToTokenTypeMap[(int)type] = type);
            for (char chr = '0'; chr <= '9'; chr++) { charToTokenTypeMap[chr] = TT.Integer; }
        }

        public Lexer(string text)
        {
            this.text = text;
            HIException.Text = text;
        }

        // 문자열로부터 토큰을 가져온다.
        public Token GetNextToken()
        {
            while (true)
            {
                var type = CurrentTokenType;
                var column = currentColumn;
                switch (type)
                {
                    case TT.InvalidType:
                        throw new HIException(ET.SyntaxError, Id.InvalidCharacter, column);

                    case TT.Space: SkipSpace(); continue;

                    case TT.Integer:
                        var token = new Token(type, column) { Integer = GetInteger() };
                        return token;

                    default: MoveNext(); return new Token(type, column);
                }
            }
        }

        // 문자열을 토큰 목록으로 변환한다.
        public List<Token> Run()
        {
            var tokens = new List<Token>();
            Token token;
            do
            {
                token = GetNextToken();
                tokens.Add(token);
            } while (token.Type != TT.EOF);

            return tokens;
        }

        private const byte maxCharNumberPerLine = 100;

        private static readonly TT[] charToTokenTypeMap =
                    Enumerable.Repeat(TT.InvalidType, byte.MaxValue + 1).ToArray();

        private readonly string text;

        private byte currentColumn;
        private int currentPos;

        private char CurrentChar => text.Length > currentPos ? text[currentPos] : (char)TT.EOF;

        private TT CurrentTokenType => charToTokenTypeMap.Length > CurrentChar ?
            charToTokenTypeMap[CurrentChar] : TT.EOF;

        // 숫자를 가져온다.
        private long GetInteger()
        {
            long integer = 0;
            do
            {
                integer = integer * 10 + CurrentChar - '0';
                MoveNext();
            } while (CurrentTokenType == TT.Integer);

            return integer;
        }

        // 문자열의 다음 문자로 이동한다.
        private void MoveNext()
        {
            if (currentPos > maxCharNumberPerLine)
            {
                throw new HIException(ET.SyntaxError, Id.TooManyCharacters, maxCharNumberPerLine);
            }
            currentPos++;
            currentColumn++;
        }

        // 공백을 제거한다.
        private void SkipSpace()
        {
            for (; CurrentTokenType == TT.Space; MoveNext()) ;
        }
    }
}