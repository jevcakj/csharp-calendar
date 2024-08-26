using CalendarCommon;

namespace CalendarServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            server server = new();
            server.StartLoop();
        }
    }
}
