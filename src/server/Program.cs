using CalendarCommon;

namespace CalendarServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            if (args.Length != 0)
            {
                int port = -1;
                string path = "";
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-p":
                            ++i;
                            port = int.Parse(args[i]);
                            break;
                        case "-d":
                            ++i;
                            path = args[i];
                            break;
                        default:
                            break;
                    }
                }
                if(port > 0 && path != "" && Path.Exists(path))
                {
                    server = new Server(path, port);
                }
                else if(port > 0)
                {
                    server = new Server(port: port);
                }
                else if(path != "" && Path.Exists(path))
                {
                    server = new Server(path);
                }
            }       
            server.StartLoop();
        }
    }
}
