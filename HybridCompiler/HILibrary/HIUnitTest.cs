using System;

namespace HILibrary
{
    public class HIUnitTest
    {
        public static Exception Error => new Exception();

        public string Text { get; private set; }

        public HIUnitTest(string text, object targetValue)
        {
            Text = text;
            this.targetValue = targetValue;
        }

        // 테스트를 실행한다.
        public static void Run(HIUnitTest[] tests, Func<HIUnitTest, int> testCode)
        {
            var testMethodName = HIDebug.GetMethodName();
            Print(testBegin, testMethodName);

            int failed = 0;
            foreach (var test in tests)
            {
                try
                {
                    failed += testCode(test);
                }
                catch
                {
                    if (test.targetValue.GetType() != Error.GetType())
                    {
                        Print(testNotEqual, test.Text, test.targetValue, nameof(Exception));
                        failed++;
                    }
                }
            }
            Print(testEnd, testMethodName, tests.Length - failed, failed);
        }

        // 목표값과 결과값이 다를 경우 콘솔창에 출력한다.
        public int AssertEqual(object result)
        {
            if (targetValue.GetType() == Error.GetType())
            {
                Print(testNotEqual, Text, nameof(Exception), result);
                return 1;
            }
            if (!targetValue.TEquals(result))
            {
                Print(testNotEqual, Text, targetValue, result);
                return 1;
            }
            return 0;
        }

        private const string testBegin = "---------- {0} 시작";
        private const string testEnd = "========== {0} 완료: 성공 {1}, 실패 {2}";
        private const string testNotEqual = "{0}\n- 예상: {1}, 결과: {2}";

        private readonly object targetValue;    // 유닛 테스트 실행시 예상되는 결과값

        private static void Print(string text, params object[] objects) =>
            Console.WriteLine(string.Format(text, objects));
    }
}