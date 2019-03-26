using System;

namespace HI1
{
    internal class Program
    {
        // 콘솔에서 문자열을 입력받는다.
        private static string Input()
        {
            Console.Write("]");
            return Console.ReadLine();
        }

        private static void Main(string[] args)
        {
            Test.Run();

            string text;
            // 입력받은 내용이 없으면 루프를 끝낸다.
            while (!string.IsNullOrWhiteSpace(text = Input()))
            {
                // 입력받은 내용을 숫자와 사칙 연산 토큰으로 변환한다.
                var tokens = new Lexer(text).Run();

                // 사칙 연산을 계산한다.
                var result = new Interpreter(tokens).Run();

                // 계산한 결과를 출력한다.
                Console.WriteLine(result);
            }
        }
    }
}