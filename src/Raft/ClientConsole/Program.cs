using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var client = new HttpClient();

            for (var i = 1; i <= 3; i++)
                Console.WriteLine($"R{i} state: " +
                    await (await client.GetAsync($"http://localhost:1000{i}/RaftApi/GetState"))
                    .Content.ReadAsStringAsync());

            for (var i = 2; i < 5; i++)
            {
                var req = new Dictionary<string, string>{
                        {"command", $"{i}" }
                    };

                var content = new FormUrlEncodedContent(req);
                Console.WriteLine($"Posting i={i}");
                var response = await client.PostAsync("http://localhost:10001/RaftApi/MakeRequest", content);
                
                Task.Delay(5000).Wait();
            }
            for(var i = 1; i <= 3; i++) 
                Console.WriteLine( $"R{i} state: "+
                    await(await client.GetAsync($"http://localhost:1000{i}/RaftApi/GetState"))
                    .Content.ReadAsStringAsync());

            Console.ReadLine();
        }
    }
}
