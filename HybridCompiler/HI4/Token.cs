namespace HI4
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
        public byte Column { get; private set; }
        public long Integer { get; set; }       // 정수 토큰의 값
        public TT Type { get; private set; }

        public Token(TT type, byte column)
        {
            Type = type;
            Column = column;
        }

        public Token(TT type, byte column, long integer)
        {
            Type = type;
            Column = column;
            Integer = integer;
        }
    }
}