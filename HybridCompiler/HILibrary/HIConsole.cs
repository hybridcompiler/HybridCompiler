using System;

namespace HILibrary
{
    public static class HIConsole
    {
        public static void Run(Action test, Action<string> main)
        {
            while (true)
            {
                Console.Write(prompot);
                var text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text)) { continue; }

                switch (text)
                {
                    case commandTest: test(); continue;
                    case commandExit: return;
                }
                try
                {
                    main(text);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private const string commandExit = "exit";
        private const string commandTest = "test";
        private const string prompot = "]";
    }
}