using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var client = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        var ip = IPAddress.Parse("127.0.0.1");
        var port = 4066;
         
        var server_end_point = new IPEndPoint(ip, port);

        try
        {
            client.Connect(server_end_point);

            if (client.Connected)
            { 
                Console.WriteLine("Client connected to Server... ");

                Task.Run(() =>
                {
                    var buffer = new byte[1024];
                    var len = 0;
                    var msg = string.Empty;

                    while (true)
                    {
                        len = client.Receive(buffer);
                        msg = Encoding.Default.GetString(buffer, 0, len);
                        Console.WriteLine($"Message: {msg}"); 
                    }
                });

                while (true)
                {
                    var msg = Console.ReadLine();

                    var bufer = Encoding.Default.GetBytes(msg);
                    client.Send(bufer);
                }
            }
            else
                Console.WriteLine("Server is not avaible");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured: {ex.Message}");
        }
    }
}