using CalendarCommon;

namespace CalendarServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new();
            server.StartLoop();
        }
    }
}
