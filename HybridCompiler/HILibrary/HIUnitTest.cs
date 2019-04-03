using System;

namespace HILibrary
{
    public class HIUnitTest
    {
        public static Exception Error => new Exception();

        public string TestCode { get; private set; }

        public HIUnitTest(string text, object value, object errorId = null)
        {
            TestCode = text;
            targetValue = value;
            targetErrorId = errorId;
        }

        public static void Run(HIUnitTest[] tests, Func<HIUnitTest, int> testCode)
        {
            var testMethodName = HIDebug.GetMethodName();
            Console.WriteLine(testBegin, testMethodName);

            int failCount = 0;
            foreach (var test in tests)
            {
                try
                {
                    failCount += testCode(test);
                }
                catch (Exception e)
                {
                    if (test.targetValue.GetType() != Error.GetType())
                    {
                        Console.WriteLine(testNotEqual, test.TestCode, test.targetValue, e.Data["Id"]);
                        failCount++;
                    }
                    else
                    {
                        failCount += test.EqualError(e.Data["Id"]);
                    }
                }
            }
            Console.WriteLine(testEnd, testMethodName, tests.Length - failCount, failCount);
        }

        public int AssertEqual(object resultValue)
        {
            int failCount = 0;
            if (targetValue.GetType() == Error.GetType() || !targetValue.TEquals(resultValue))
            {
                Console.WriteLine(testNotEqual, TestCode, targetErrorId, resultValue);
                failCount = 1;
            }
            return failCount;
        }

        public int EqualError(object resultErrorId)
        {
            int failCount = 0;
            if (targetErrorId != null && !targetErrorId.TEquals(resultErrorId))
            {
                Console.WriteLine(testNotEqual, TestCode, targetErrorId, resultErrorId);
                failCount = 1;
            }
            return failCount;
        }

        private const string testBegin = "---------- {0} 시작";
        private const string testEnd = "========== {0} 완료: 성공 {1}, 실패 {2}";
        private const string testNotEqual = "{0}\n- 예상: {1}, 결과: {2}";

        private readonly object targetErrorId;
        private readonly object targetValue;
    }
}