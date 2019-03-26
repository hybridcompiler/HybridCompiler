namespace HI1
{
    internal enum TokenType
    {
        Integer,

        // 사칙 연산

        Plus,
        Minus,
        Mul,
        Div,

        EOF,    // 토큰이 더 이상 없을 때 반환하는 타입
    }

    internal class Token
    {
        public long integer;    // 정수 토큰의 값
        public TokenType type;
    }
}