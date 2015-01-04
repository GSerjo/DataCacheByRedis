using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using StackExchange.Redis;

namespace BookService
{
    internal class Program
    {
        private static ConnectionMultiplexer _redis;

        private static ConnectionMultiplexer CreateRedisConnection()
        {
            var config = new ConfigurationOptions
            {
                AllowAdmin = true,
                EndPoints = { { "localhost", 6379 } },
                AbortOnConnectFail = false
            };
            return ConnectionMultiplexer.Connect(config);
        }

        private static void InsertTestData()
        {
            IDatabase db = _redis.GetDatabase();

            Enumerable.Range(1, 1000)
                      .ToList()
                      .ForEach(x => db.SetAdd("TestKey", Guid.NewGuid().ToString()));
        }

        private static void Main()
        {
            try
            {
                StartRedis();

                _redis = CreateRedisConnection();
                InsertTestData();

                Console.WriteLine("Press any Key to Exit");
                Console.ReadKey();

                StopRedis();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private static void StartRedis()
        {
            string redisDirectory = Path.Combine(Environment.CurrentDirectory, "redis");
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(redisDirectory, "redis-server.exe"),
                Arguments = Path.Combine(redisDirectory, "redis.windows.conf"),
                UseShellExecute = false
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();
        }

        private static void StopRedis()
        {
            EndPoint[] endPoints = _redis.GetEndPoints();
            IServer server = _redis.GetServer(endPoints.First());
            server.Shutdown();
        }
    }
}
