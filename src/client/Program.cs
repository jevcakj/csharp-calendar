using CalendarCommon;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CalendarClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Start();
        }
    }
}
