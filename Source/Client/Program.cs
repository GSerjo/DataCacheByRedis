using System;
using System.Linq;
using StackExchange.Redis;

namespace Client
{
    internal class Program
    {
        private static ConnectionMultiplexer _redis;

        private static void GetData()
        {
            IDatabase database = _redis.GetDatabase();
            RedisValue[] values = database.SetMembers(GetKey());
            values.Where(x => x.HasValue)
                  .Select(x => (string)x)
                  .ToList()
                  .ForEach(Console.WriteLine);
        }

        private static RedisKey GetKey()
        {
            return "TestKey";
        }

        private static void Main()
        {
            _redis = ConnectionMultiplexer.Connect("localhost");
            bool isRun = true;
            while (isRun)
            {
                Console.WriteLine("Press <Esc> to Exit or <Enter> to Get data");

                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        isRun = false;
                        break;
                    case ConsoleKey.Enter:
                        GetData();
                        break;
                }
            }

            _redis.Dispose();
            Console.WriteLine("Bye");
        }
    }
}
