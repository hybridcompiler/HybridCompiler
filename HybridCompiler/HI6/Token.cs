using HILibrary;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HI6
{
    // Token Type
    internal enum TT : byte
    {
        Invalid,

        LF = (byte)'\n',
        CR = (byte)'\r',
        Space = (byte)' ',

        LeftParens = (byte)'(',
        RightParens = (byte)')',

        Plus = (byte)'+',
        Minus = (byte)'-',
        Mul = (byte)'*',
        Div = (byte)'/',
        Remainder = (byte)'%',

        BitAnd = (byte)'&',

        BitOr = (byte)'|',
        BitXor = (byte)'^',

        LessThan = (byte)'<',
        GreaterThan = (byte)'>',

        Assign = (byte)'=',
        Not = (byte)'!',

        BeginBlock = (byte)'{',
        EndBlock = (byte)'}',

        Comma = (byte)',',

        String = (byte)'"',

        EOF = 127,

        Equal,
        NotEqual,

        LessThanEqual,
        GreaterThanEqual,

        And,
        Or,

        LeftShift,
        RightShift,

        Integer,
        Identifier,
        Keyword,
    }

    //
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal class Token
    {
        // 공통 필드

        [FieldOffset(0)] public readonly TT Type;
        [FieldOffset(1)] public readonly byte Column;
        [FieldOffset(4)] public readonly int Line;

        [FieldOffset(8)] public readonly long IntegerValue;

        [FieldOffset(8)] public readonly int StringId;

        [FieldOffset(8)] public readonly KT KeywordType;

        public Token(TT type, byte column, int line)
        {
            Type = type;
            Column = column;
            Line = line;
        }

        public Token(long value, byte column, int line)
        {
            Type = TT.Integer;
            IntegerValue = value;
            Column = column;
            Line = line;
        }

        public Token(TT type, string identifier, byte column, int line)
        {
            HIDebug.Assert(type == TT.Identifier || type == TT.String);
            Type = type;
            Column = column;
            Line = line;

            if (type == TT.Identifier && Keyword.Find(identifier, out KT keywordType))
            {
                Type = TT.Keyword;
                KeywordType = keywordType;
            }
            else
            {
                StringId = strings.Count;
                strings.Add(identifier);
            }
        }

        public string Text
        {
            get
            {
                HIDebug.Assert(Type == TT.Identifier || Type == TT.String);
                return strings[StringId];
            }
        }

        private static readonly List<string> strings = new List<string>();
    }
}