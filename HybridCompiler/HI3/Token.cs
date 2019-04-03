namespace HI3
{
    internal enum TT
    {
        InvalidType,    // 유효하지 않은 Token 타입

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
        public long Integer { get; set; }
        public TT Type { get; private set; }

        public Token(TT type, byte column)
        {
            Type = type;
            Column = column;
        }
    }
}