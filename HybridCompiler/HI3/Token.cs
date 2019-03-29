namespace HI3
{
    internal enum TT
    {
        InvalidType,    // 유효하지 않은 토큰 타입

        Space = ' ',

        Plus = '+',
        Minus = '-',
        Mul = '*',
        Div = '/',

        LeftParens = '(',
        RightParens = ')',

        EOF = 127,  // 토큰이 없을 때 반환하는 타입

        Integer,
    }

    internal class Token
    {
        public byte column;
        public long integer;    // 정수 토큰의 값
        public TT type;

        public Token(TT type, byte column)
        {
            this.type = type;
            this.column = column;
        }
    }
}