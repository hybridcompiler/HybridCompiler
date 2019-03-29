using HILibrary;

namespace HI3
{
    internal enum Id
    {
        Culture,    // 현재 선택된 언어의 국가 코드

        // Assert

        InvalidOperator,

        // 에러

        InvalidSyntax,
        InvalidCharacter,
        TooManyCharacters,
    }

    internal class Texts : HIText
    {
        public Texts() : base(typeof(Id), texts)
        {
        }

        private static readonly string[,] texts =
        {
            // 지원 언어 목록

            { "en", "ko-KR" },

            // Assert

            { "invalid operatpr", "유효하지 않은 연산자입니다." },

            // 에러

            { "invalid syntax", "유효하지 않은 구문입니다." },
            { "invalid character", "유효하지 않은 문자입니다." },
            { "too many characters per line", "한 줄에 너무 많은 문자가 있습니다." },
        };
    }
}