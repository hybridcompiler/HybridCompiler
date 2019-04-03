using System;
using System.Diagnostics;

namespace HILibrary
{
    public static class HIDebug
    {
        // 확장된 Assert 메소드
        //
        // Type     Fail
        // ==================
        // all      null
        // bool     false
        // string   empty("")
        // number   0
        [Conditional("DEBUG")]
        public static void Assert(object value, string message = null)
        {
            bool condition = value != null;
            if (condition)
            {
                switch (value)
                {
                    case bool boolValue: condition = boolValue; break;
                    case string text: condition = !string.IsNullOrEmpty(text); break;

                    default:
                        if (byte.TryParse(value.ToString(), out byte number)) { condition = number != 0; }
                        break;
                }
            }
            Debug.Assert(condition, string.Format(assertFailed, GetMethodName(), Str(value)), message);
        }

        [Conditional("DEBUG")]
        public static void AssertEqual<T>(T value1, T value2, string message = null) =>
            Debug.Assert(value1.TEquals(value2), string.Format(assertEqualFailed,
                GetMethodName(), Str(value1), Str(value2)), message);

        [Conditional("DEBUG")]
        public static void Fail(object message, string detailedMessage = null) =>
            Debug.Fail(message.ToString(), detailedMessage);

        // 객체명과 메소드명을 가져온다.
        public static string GetMethodName(int stackFrame = 2)
        {
            var method = new StackFrame(stackFrame).GetMethod();
            return $"{method.DeclaringType.Name}.{method.Name}";
        }

        public static void PrintMethodName() => Console.WriteLine(GetMethodName());

        public static bool TEquals<T>(this T object1, T object2)
        {
            if (object1 == null || object2 == null) { return object1 == null && object2 == null; }
            return object1.ToString() == object2.ToString();
        }

        private const string assertEqualFailed = "{0}() assertion failed. '{1}' != '{2}'";
        private const string assertFailed = "{0}() assertion failed. value: {1}";

        private static string Str(object obj) => (obj ?? "null").ToString();
    }
}