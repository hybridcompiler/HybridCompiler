using System;

namespace HI1
{
    public class InterpreterTest
    {
        public object targetValue;  // 유닛 테스트 실행시 예상되는 결과값
        public string text;         // 문자열

        public InterpreterTest(string text, object targetValue)
        {
            this.text = text;
            this.targetValue = targetValue;
        }

        // 목표값과 결과값이 다를 경우 콘솔창에 출력한다.
        public void AssertEqual(object result)
        {
            if (targetValue.GetType() == typeof(Exception))
            {
                Console.Error.WriteLine($"{text}\n실행결과: {result}, 에러가 발생해야 합니다.");
            }
            else if (!CheckEqual(result))
            {
                Console.Error.WriteLine($"{text}\n실행결과({result})와 목표값({targetValue})이 일치하지 않습니다.");
            }
        }

        // 목표값과 결과값이 동일한지 검토한다.
        public bool CheckEqual(object result)
        {
            // object는 = 연산자를 지원하지 않기 때문에 ToString을 이용하여 비교한다.
            // 값이 null인 객체는 ToString을 사용하면 에러를 발생시키기 때문에 따로 처리한다.
            if (result == null || targetValue == null) { return result == null && targetValue == null; }
            return targetValue.ToString() == result.ToString();
        }

        // 예외가 발생시 예외가 발생하지 않아야 하는 테스트의 경우 콘솔창에 출력한다.
        public void CheckException()
        {
            if (targetValue.GetType() != typeof(Exception))
            {
                Console.WriteLine($"{text}\n목표값 : {targetValue}, 에러가 발생하지 않아야 합니다.");
            }
        }
    }

    internal static class Test
    {
        // 테스트를 실행한다.
        public static void Run()
        {
            Console.WriteLine("----- 테스트 시작");
            TestLexer();
            TestInterpreter();
            Console.WriteLine("----- 테스트 완료");
        }

        // Interpreter를 테스트한다.
        private static void TestInterpreter()
        {
            InterpreterTest[] tests =
            {
                new InterpreterTest("5 + 8", 13),
                new InterpreterTest("6 - 9", -3),
                new InterpreterTest("7 * 6", 42),
                new InterpreterTest("12 / 3", 4),
                new InterpreterTest("3", new Exception()),
                new InterpreterTest("+ 5", new Exception()),
                new InterpreterTest("2 + 5 - 3", new Exception())
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.text);
                try
                {
                    var tokens = lexer.Run();
                    var result = new Interpreter(tokens).Run();
                    test.AssertEqual(result);
                }
                catch (Exception)
                {
                    test.CheckException();
                }
            }
        }

        // Lexer를 테스트한다.
        private static void TestLexer()
        {
            InterpreterTest[] tests =
            {
                new InterpreterTest("1234567890", TokenType.Integer),
                new InterpreterTest(" +", TokenType.Plus),
                new InterpreterTest("- ", TokenType.Minus),
                new InterpreterTest(" * ", TokenType.Mul),
                new InterpreterTest("/", TokenType.Div),
                new InterpreterTest("a", new Exception())
            };

            foreach (var test in tests)
            {
                var lexer = new Lexer(test.text);
                try
                {
                    var token = lexer.GetNextToken();
                    test.AssertEqual(token.type);
                }
                catch (Exception)
                {
                    test.CheckException();
                }
            }
        }
    }
}