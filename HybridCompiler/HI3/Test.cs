using HILibrary;

namespace HI3
{
    internal static class Test
    {
        // 테스트를 실행한다.
        public static void Run()
        {
            TestLexer();
            TestInterpreter();
        }

        // Interpreter를 테스트한다.
        private static void TestInterpreter()
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
                new HIUnitTest("6 * (-3 - - 6) / 3 + 7", 13),
            };
            HIUnitTest.Run(tests, (test) =>
            {
                var tokens = new Lexer(test.Text).Run();
                var node = new Parser(tokens).Run();
                var result = node.Visit();
                return test.AssertEqual(result);
            });
        }

        // Lexer를 테스트한다.
        private static void TestLexer()
        {
            HIUnitTest[] tests =
            {
                new HIUnitTest("1234567890", TT.Integer),
                new HIUnitTest(" +", TT.Plus),
                new HIUnitTest("- ", TT.Minus),
                new HIUnitTest(" * ", TT.Mul),
                new HIUnitTest("/", TT.Div),
                new HIUnitTest("(", TT.LeftParens),
                new HIUnitTest(")", TT.RightParens),
                new HIUnitTest("a", HIUnitTest.Error)
            };
            HIUnitTest.Run(tests, (test) =>
            {
                var lexer = new Lexer(test.Text);
                var token = lexer.GetNextToken();
                return test.AssertEqual(token.Type);
            });
        }
    }
}