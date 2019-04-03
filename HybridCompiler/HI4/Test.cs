using HILibrary;

namespace HI4
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
                new HIUnitTest(" +", TT.Plus),
                new HIUnitTest("- ", TT.Minus),
                new HIUnitTest(" * ", TT.Mul),
                new HIUnitTest("/", TT.Div),
                new HIUnitTest("(", TT.LeftParens),
                new HIUnitTest(")", TT.RightParens),
                new HIUnitTest("=", TT.Assign),
                new HIUnitTest("{", TT.BeginBlock),
                new HIUnitTest("}", TT.EndBlock),

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
                new HIUnitTest("{ dim b = 5\ndim c = 6\nb = b + c }", 11),
                new HIUnitTest("{ dim d = 5\ndim d = 4 }", HIUnitTest.Error, Id.NameAlreadyDefined),
            };
            HIUnitTest.Run(tests, (test) =>
            {
                var tokens = new Lexer(test.TestCode).Run();
                var node = new Parser(tokens).Run();
                var result = node.Visit;
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
                new HIUnitTest("3", 3),
                new HIUnitTest("+ 5", 5),
                new HIUnitTest("2 + 5 - 3", 4),
                new HIUnitTest("3 * (2 + 6) - 3", 21),
                new HIUnitTest("dim a = 6 * (-3 - - 6) / 3 + 7", 13),

                new HIUnitTest("6 *", HIUnitTest.Error, Id.InvalidSyntax),
                new HIUnitTest("5 / 0", HIUnitTest.Error, Id.DivisionByZero),
                new HIUnitTest("b = 10", HIUnitTest.Error, Id.NameNotDefined),
            };
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