using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HI6
{
    internal class Lexer
    {
        // static 필드들에 대한 초기화는 static 생성자에서 실행해야 한다.
        static Lexer()
        {
            // 문자를 Token 타입으로 매핑해주는 맵을 설정한다.

            Enum.GetValues(typeof(TT)).Cast<TT>().Where(type => type <= TT.EOF).ToList().
                ForEach(type => charToTokenTypeMap[(int)type] = type);

            // EBNF : digit = "0" | "1" | ...... | "9" ;
            for (char chr = '0'; chr <= '9'; chr++) { charToTokenTypeMap[chr] = TT.Integer; }

            // EBNF : alphabet = "A" | "B" | ...... | "z" ;
            for (char chr = 'a'; chr <= 'z'; chr++)
            {
                charToTokenTypeMap[chr] = TT.Identifier;
                charToTokenTypeMap[chr + 'A' - 'a'] = TT.Identifier;
            }
            charToTokenTypeMap['_'] = TT.Identifier;
        }

        public Lexer(string text)
        {
            sourceCode = text;
            HIException.SourceCode = text;
        }

        public Token GetNextToken()
        {
            while (true)
            {
                var type = CurrentTokenType;
                var column = currentColumn;
                switch (type)
                {
                    case TT.Invalid:
                        throw new HIException(ET.SyntaxError, Id.InvalidCharacter, column, currentLine);

                    // 줄바꿈은 CR+LF, CR, LF 중에 하나가 사용되는데 CR 단독은 과거 8비트 시절 사용되었고
                    // 현재는 CR+LF 또는 LF가 사용된다. CR은 공백처럼 스킵하고 LF를 줄바꿈으로 처리한다.
                    // CR은 실제 화면에 출력되는 문자가 아니기 때문에 currentColumn은 증가시키지 않기 위해
                    // MoveNext()를 사용하지 않는다.
                    case TT.LF: currentPos++; currentLine++; currentColumn = 0; continue;
                    case TT.CR: currentPos++; continue;

                    case TT.Space: SkipSpace(); continue;

                    case TT.Integer: return new Token(GetInteger(), column, currentLine);

                    case TT.Identifier: return new Token(TT.Identifier, GetIdentifier(), column, currentLine);

                    case TT.String: return new Token(TT.String, GetString(), column, currentLine);

                    case TT.Assign:
                    case TT.Not:
                    case TT.LessThan:
                    case TT.GreaterThan:
                    case TT.BitAnd:
                    case TT.BitOr:
                        MoveNext();
                        var nextType = CurrentTokenType;
                        for(int i = 0;i <= twoCharOperators.GetUpperBound(0);i++)
                        {
                            if (twoCharOperators[i,0] == type && twoCharOperators[i, 1] == nextType)
                            {
                                type = twoCharOperators[i, 2];
                                MoveNext();
                                break;
                            }
                        }
                        return new Token(type, column, currentLine);

                    default: MoveNext(); return new Token(type, column, currentLine);
                }
            }
        }

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
            Enumerable.Repeat(TT.Invalid, byte.MaxValue + 1).ToArray();

        private readonly string sourceCode;

        private readonly TT[,] twoCharOperators =
        {
            { TT.Assign, TT.Assign, TT.Equal },
            { TT.Not, TT.Assign, TT.NotEqual },
            { TT.LessThan, TT.Assign, TT.LessThanEqual },
            { TT.GreaterThan, TT.Assign, TT.GreaterThanEqual },
            { TT.BitAnd, TT.BitAnd, TT.And },
            { TT.BitOr, TT.BitOr, TT.Or },
            { TT.LessThan, TT.LessThan, TT.LeftShift },
            { TT.GreaterThan, TT.GreaterThan, TT.RightShift },
        };

        private byte currentColumn;
        private int currentLine;
        private int currentPos;

        private char CurrentChar => sourceCode.Length > currentPos ?
            sourceCode[currentPos] : (char)TT.EOF;

        private TT CurrentTokenType => charToTokenTypeMap.Length > CurrentChar ?
            charToTokenTypeMap[CurrentChar] : TT.EOF;

        // EBNF : identifier = ( alphabet | "_" ), { alphabet | "_" | integer } ;
        private string GetIdentifier()
        {
            var identifier = new StringBuilder();
            do
            {
                identifier.Append(CurrentChar);
                MoveNext();
            } while (CurrentTokenType == TT.Identifier || CurrentTokenType == TT.Integer);

            return identifier.ToString();
        }

        // EBNF : integer = digit, { digit } ;
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

        // EBNF : string = '"', { all characters }, '"' ;
        private string GetString()
        {
            MoveNext();
            var str = new StringBuilder();
            while(CurrentTokenType != TT.String)
            {
                str.Append(CurrentChar);
                MoveNext();
            }
            MoveNext();
            return str.ToString();
        }

        private void MoveNext()
        {
            if (currentColumn > maxCharNumberPerLine)
            {
                throw new HIException(ET.SyntaxError, Id.TooManyCharacters,
                    maxCharNumberPerLine, currentLine);
            }
            currentPos++;
            currentColumn++;
        }

        private void SkipSpace()
        {
            while (CurrentTokenType == TT.Space) { MoveNext(); }
        }
    }
}