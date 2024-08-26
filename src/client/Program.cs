using System.Net;
using System.Text;

namespace CalendarClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new();
            Task.Run(async () =>
            {
                var a = await client.GetAsync("http://localhost:8080/neco");
                Console.WriteLine($"Status: {a.StatusCode.ToString()}");
                Console.WriteLine($"content: \n{await a.Content.ReadAsStringAsync()}");
            });
            //Task.Run(() =>
            //{
            //    var a = client.GetAsync("http://localhost:8080/neco2");
            //    Console.WriteLine(a);
            //});
            Task.Delay(500).Wait();
            //req.Method = "GET";
        }
    }
}
