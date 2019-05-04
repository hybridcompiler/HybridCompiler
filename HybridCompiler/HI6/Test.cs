using HILibrary;

namespace HI6
{
    internal static class Test
    {
        public static void Run()
        {
            TestLexer();
            TestParserLine();
            TestParser();
        }

        private static void TestLexer()
        {
            HIUnitTest[] tests =
            {
                new HIUnitTest("(", TT.LeftParens),
                new HIUnitTest(")", TT.RightParens),

                new HIUnitTest("+", TT.Plus),
                new HIUnitTest("-", TT.Minus),
                new HIUnitTest("*", TT.Mul),
                new HIUnitTest("/", TT.Div),
                new HIUnitTest("%", TT.Remainder),

                new HIUnitTest("&", TT.BitAnd),

                new HIUnitTest("|", TT.BitOr),
                new HIUnitTest("^", TT.BitXor),

                new HIUnitTest("<", TT.LessThan),
                new HIUnitTest(">", TT.GreaterThan),

                new HIUnitTest("=", TT.Assign),
                new HIUnitTest("!", TT.Not),

                new HIUnitTest("{", TT.BeginBlock),
                new HIUnitTest("}", TT.EndBlock),

                new HIUnitTest("<<", TT.LeftShift),
                new HIUnitTest(">>", TT.RightShift),

                new HIUnitTest("<=", TT.LessThanEqual),
                new HIUnitTest(">=", TT.GreaterThanEqual),

                new HIUnitTest("==", TT.Equal),
                new HIUnitTest("!=", TT.NotEqual),

                new HIUnitTest("&&", TT.And),
                new HIUnitTest("||", TT.Or),

                new HIUnitTest(",", TT.Comma),

                new HIUnitTest("1234567890", TT.Integer),
                new HIUnitTest("abcdz", TT.Identifier),
                new HIUnitTest("dim", TT.Keyword),

                new HIUnitTest("`", HIUnitTest.Error, Id.InvalidCharacter)
            };
            HIUnitTest.Run(tests, (test) =>
            {
                var lexer = new Lexer(test.TestCode);
                var token = lexer.GetNextToken();
                return test.AssertEqual(token.Type);
            });
        }

        private static void TestParser()
        {
            HIUnitTest[] tests =
            {
                new HIUnitTest("def main() { dim b = 5\ndim c = 6\nb += c }", 11),
                new HIUnitTest("def main() { dim d = 5\ndim d = 4 }", HIUnitTest.Error, Id.NameAlreadyDefined),
                new HIUnitTest("def main() { if(0 == 0) { 1 } elif(0 == 1) { 2 } else { 3 } }", 1),
                new HIUnitTest("def main() { if(1 == 0) { 1 } elif(1 == 1) { 2 } else { 3 } }", 2),
                new HIUnitTest("def main() { if(2 == 0) { 1 } elif(2 == 1) { 2 } else { 3 } }", 3),
                new HIUnitTest("def main() { dim e = 0\ndim f = 0\nwhile(e < 10) { e += 1\nf += e } }", 55),
                new HIUnitTest("def main() { dim g = 0\nfor range(10) { g += 1 } }", 10),
                new HIUnitTest("def main() { dim h = 0\nfor range(0, 10) { h += 1 } }", 10),
                new HIUnitTest("def main() { dim i = 0\nfor range(0, 10, 2) { i += 1 } }", 5),
                new HIUnitTest("def main() { dim j = 0\ndim k\nfor k in range(1, 11) { j += k } }", 55),
                new HIUnitTest("def main() { dim l = 0\nfor dim m in range(1, 11) { l += m } }", 55),
                new HIUnitTest(@"def test() { 5 } def main() { test() }", 5),
                new HIUnitTest(@"
def main() {
    dim result = factorial(5)
}
def factorial(number) {
    if (number < 2) { 1 }
    else { number * factorial(number - 1) }
}", 120),
                new HIUnitTest(@"
def main() {
    print(""구구단 예제"")
    for dim i in range(2, 10) {
        for dim j in range(2, 10) {
            print(i, "" * "", j, "" = "", i * j)
        }
    }
}", 81),
            };
            HIUnitTest.Run(tests, (test) =>
            {
                var tokens = new Lexer(test.TestCode).Run();
                var parser = new Parser(tokens);
                parser.Run();
                var result = parser.EntryFunction.Function.Visit;
                return test.AssertEqual(result);
            });
        }

        private static void TestParserLine()
        {
            HIUnitTest[] tests =
            {
                new HIUnitTest("5 + 8", 13),
                new HIUnitTest("6 - 9", -3),
                new HIUnitTest("7 * 6", 42),
                new HIUnitTest("12 / 3", 4),
                new HIUnitTest("10 % 6", 4),
                new HIUnitTest("2 << 6", 128),
                new HIUnitTest("256 >> 6", 4),
                new HIUnitTest("6 & 10", 2),
                new HIUnitTest("6 | 10", 14),
                new HIUnitTest("6 ^ 10", 12),
                new HIUnitTest("6 < 10", 1),
                new HIUnitTest("6 > 10", 0),
                new HIUnitTest("6 <= 10", 1),
                new HIUnitTest("6 >= 10", 0),
                new HIUnitTest("6 == 10", 0),
                new HIUnitTest("6 != 10", 1),
                new HIUnitTest("6 && 0", 0),
                new HIUnitTest("6 || 0", 1),

                new HIUnitTest("3", 3),
                new HIUnitTest("+ 5", 5),
                new HIUnitTest("!5", 0),
                new HIUnitTest("2 + 5 - 3", 4),
                new HIUnitTest("3 * (2 + 6) - 3", 21),
                new HIUnitTest("dim a = 6 * (-3 - - 6) / 3 + 7", 13),

                new HIUnitTest("6 *", HIUnitTest.Error, Id.InvalidSyntax),
                new HIUnitTest("5 / 0", HIUnitTest.Error, Id.DivisionByZero),
                new HIUnitTest("b = 10", HIUnitTest.Error, Id.NameNotDefined),
            };
            SymbolTable.Init();
            HIUnitTest.Run(tests, (test) =>
            {
                var tokens = new Lexer(test.TestCode).Run();
                var node = new Parser(tokens).ParseLine();
                var result = node.Visit;
                return test.AssertEqual(result);
            });
        }
    }
}