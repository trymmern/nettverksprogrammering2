using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        const int port = 8080;
        const string ServerIp = "127.0.0.1";
        static void Main(string[] args)
        {
            // Create TCP client object
            var client = new TcpClient(ServerIp, port);
            var stream = client.GetStream();

            // Data to send to server
            var textToSend = "";
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);

            while (textToSend != "exit") {
                // stream.Write(bytesToSend, 0, bytesToSend.Length);

                Console.WriteLine("Enter first number: ");
                textToSend = Console.ReadLine();
                bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                stream.Write(bytesToSend, 0 , bytesToSend.Length);

                Console.WriteLine("Enter operand (+/-): ");
                textToSend = Console.ReadLine();
                bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                stream.Write(bytesToSend, 0 , bytesToSend.Length);

                Console.WriteLine("Enter second number: ");
                textToSend = Console.ReadLine();
                bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                stream.Write(bytesToSend, 0 , bytesToSend.Length);

                // Read back text
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                Console.WriteLine($"Answer = {Encoding.UTF8.GetString(bytesToRead, 0, bytesRead)}");

                Console.WriteLine("\nReady to accept another expression...");
                // textToSend = Console.ReadLine();
                // bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            }

            client.Close();
        }
    }
}
