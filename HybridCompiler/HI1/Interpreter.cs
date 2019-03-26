using System;
using System.Collections.Generic;

namespace HI1
{
    internal class Interpreter
    {
        public Interpreter(List<Token> tokens) => this.tokens = tokens;

        // 토큰목록으로 사칙 연산을 계산하고 결과를 반환한다.
        public long Run()
        {
            if (tokens.Count != 3) { throw new Exception($"{tokens.Count} : 3개의 토큰이 필요합니다."); }

            var integer1 = tokens[0];
            if (integer1.type != TokenType.Integer) { throw new Exception($"{integer1.type} : 정수가 필요합니다."); }

            var integer2 = tokens[2];
            if (integer2.type != TokenType.Integer) { throw new Exception($"{integer2.type} : 정수가 필요합니다."); }

            var op = tokens[1].type;
            switch (op)
            {
                case TokenType.Plus: return integer1.integer + integer2.integer;
                case TokenType.Minus: return integer1.integer - integer2.integer;
                case TokenType.Mul: return integer1.integer * integer2.integer;
                case TokenType.Div: return integer1.integer / integer2.integer;
            }
            throw new Exception($"{op} : 사칙 연산자가 필요합니다.");
        }

        private readonly List<Token> tokens;
    }
}