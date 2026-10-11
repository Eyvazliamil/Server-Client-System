using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static string ReadMessage()
    {
        Console.Write("Message...");

        var message = new StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (message.Length > 0)
                {
                    message.Length--;

                    Console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            { 
                if (message.Length == 0)
                {
                    Console.Write("\r");
                    Console.Write(new string(' ', Console.WindowWidth - 1));
                    Console.Write("\r");
                }

                message.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }

        return message.ToString();
    }
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
                    string msg = ReadMessage();

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
