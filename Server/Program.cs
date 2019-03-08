using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Program
    {
        const int port = 8080;
        const string ServerIp = "127.0.0.1";
        List<TcpClient> clients = new List<TcpClient>();

        static void Main(string[] args)
        {
            // Create and start a TCP listener on port 8080
            var localAddress = IPAddress.Parse(ServerIp);
            var tcpListener = new TcpListener(localAddress, port);
            TcpClient clientSocket;

            tcpListener.Start();
            Console.WriteLine("Server started...");

            int counter = 0;
            while(true) {
                counter += 1;
                clientSocket = tcpListener.AcceptTcpClient();
                Console.WriteLine($"Client No: {counter} started.");
                HandleClient client = new HandleClient();
                client.StartClient(clientSocket, Convert.ToString(counter));
            }

            clientSocket.Close();
            tcpListener.Stop();
            Console.ReadLine();
        }

        private void AddClient(TcpClient client) {
            clients.Add(client);
        }
    }

    public class HandleClient
    {
        TcpClient client;
        string clientNo;

        public void StartClient(TcpClient inClient, string clientNo) {
            this.client = inClient;
            this.clientNo = clientNo;
            Thread clientThread = new Thread(DoChat);
            clientThread.Start();
        }

        private void DoChat() {
            byte[] bytesFrom = new byte[10025];
            string rCount = null;
            var buffer = new byte[client.ReceiveBufferSize];

            int requestCount = 0;
            while ((true)) {
                try {
                    requestCount += 1;
                    var stream = client.GetStream();

                    // Read stream
                    var bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

                    var answer = "";
                    try {
                        // Convert received data to string
                        var num1 = Int32.Parse(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                        Console.WriteLine($"Client > First number: {num1}");
                        
                        bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
                        var operand = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Client > Operand: {operand}");

                        bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
                        var num2 = Int32.Parse(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                        Console.WriteLine($"Client > Second number: {num2}");

                        rCount = Convert.ToString(requestCount);


                        if (operand == "+") {
                            answer = $"{num1} {operand} {num2} = {Add(num1, num2).ToString()}";
                        }
                        else if (operand == "-") {
                            answer = $"{num1} {operand} {num2} = {Subtr(num1, num2).ToString()}";
                        }
                    }
                    catch (Exception e) {
                        answer = "Operator must be either + or -.";
                        Console.WriteLine($"Client > Operator must be either + or -. {e}");
                        break;
                    }

                    var response = Encoding.UTF8.GetBytes(answer);

                    // Write back to client
                    Console.WriteLine($"Client > Sending back to client");
                    stream.Write(response, 0, response.Length);
                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private static int Add(int num1, int num2) {
            Console.WriteLine($"{num1} + {num2}");
            return num1+num2;
        }

        private static int Subtr(int num1, int num2) {
            Console.WriteLine($"{num1} - {num2}");
            return num1-num2;
        }
    }
}
