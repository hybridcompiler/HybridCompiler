﻿using HILibrary;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HI4
{
    // Token Type
    internal enum TT : byte
    {
        Invalid,

        LF = (byte)'\n',
        CR = (byte)'\r',
        Space = (byte)' ',

        Plus = (byte)'+',
        Minus = (byte)'-',
        Mul = (byte)'*',
        Div = (byte)'/',

        LeftParens = (byte)'(',
        RightParens = (byte)')',

        Assign = (byte)'=',

        BeginBlock = (byte)'{',
        EndBlock = (byte)'}',

        EOF = 127,

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

        [FieldOffset(8)] public readonly int IdentifierStringId;

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

        public Token(string identifier, byte column, int line)
        {
            Column = column;
            Line = line;

            if (Keyword.Find(identifier, out KT keywordType))
            {
                Type = TT.Keyword;
                KeywordType = keywordType;
            }
            else
            {
                Type = TT.Identifier;
                IdentifierStringId = strings.Count;
                strings.Add(identifier);
            }
        }

        public string Text
        {
            get
            {
                HIDebug.AssertEqual(Type, TT.Identifier);
                return strings[IdentifierStringId];
            }
        }

        private static List<string> strings = new List<string>();
    }
}