using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var listener = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        var clients = new List<Socket>();

        var ip = IPAddress.Parse("127.0.0.1");
        var port = 4066;
        var end_point = new IPEndPoint(ip, port);

        listener.Bind(end_point);
        var backlog = 1;
        listener.Listen(backlog);

        Console.WriteLine($"Listener listen on {listener.LocalEndPoint}");

        while (true)
        { 
            var client = listener.Accept();
            clients.Add(client); 
            Console.WriteLine($"{listener.LocalEndPoint} connected ....");

            Task.Run(() =>
            {
                var buffer = new byte[1024];
                var len = 0;
                var msg = string.Empty;

                while (true)
                {
                    len = client.Receive(buffer);

                    msg = Encoding.Default.GetString(buffer, 0, len);

                    Console.WriteLine($"{listener.LocalEndPoint} message: {msg}");

                    if (msg == "end")
                    {
                        client.Shutdown(SocketShutdown.Both); 
                    }

                    foreach (var c in clients)
                    {
                        if (c != client)
                        {
                            c.Send(Encoding.Default.GetBytes(msg));
                        }
                    }
                }
            });
        }
    }
}