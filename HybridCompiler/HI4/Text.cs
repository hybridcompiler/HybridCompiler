using HILibrary;

namespace HI4
{
    internal enum Id
    {
        // 기타

        Culture,
        ErrorFormat,

        // Assert

        InvalidOperator,

        // SyntaxError

        InvalidCharacter,
        InvalidSyntax,
        TooManyCharacters,

        // ZeroDivisionError

        DivisionByZero,

        // NameError

        NameAlreadyDefined,
        NameNotDefined,
    }

    internal class Texts : HIText
    {
        public Texts() : base(typeof(Id), texts)
        {
        }

        private static readonly string[,] texts =
        {
            // 기타

            { "en", "ko-KR" },
            { "line {4}\n{0}\n{1}^\n{2}: {3}", "행 {4}\n{0}\n{1}^\n{2}: {3}" },

            // Assert

            { "invalid operator", "유효하지 않은 연산자입니다." },

            // SyntaxError

            { "invalid character", "유효하지 않은 문자입니다." },
            { "invalid syntax", "유효하지 않은 구문입니다." },
            { "too many characters per line", "한 줄에 너무 많은 문자가 있습니다." },

            // ZeroDivisionError

            { "division by zero", "0으로 나누려 했습니다." },

            // NameError

            { "name is already defined","이름이 이미 정의되어 있습니다." },
            { "name is not defined","이름이 정의되지 않았습니다." },
        };
    }
}