using System.Net;

namespace CalendarClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new();
            Task.Run(() =>
            {
                var a = client.GetAsync("http://localhost:8080/neco");
                Console.WriteLine(a);
            });
            Task.Run(() =>
            {
                var a = client.GetAsync("http://localhost:8080/neco2");
                Console.WriteLine(a);
            });
            Task.Delay(500).Wait();
            //req.Method = "GET";
        }
    }
}
