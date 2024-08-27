using System.Net.Sockets;
using System.Net;
using System.Text;

namespace _23._08
{
    internal class Program
    {
        private static Dictionary<string, double> currencyRates = new Dictionary<string, double>
        {
        { "USD_EUR", 0.91 },
        { "EUR_USD", 1.10 },
        { "USD_RUB", 100.0 },
        { "RUB_USD", 0.01 },
        { "EUR_RUB", 110.0 },
        { "RUB_EUR", 0.0091 }
        };

        private static string logFilePath = "server_log.txt";

        static void Main()
        {
            StartServer();
        }

        public static void StartServer()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8888;
            TcpListener listener = new TcpListener(ipAddress, port);

            try
            {
                listener.Start();
                Console.WriteLine("Сервер запущен...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    string clientEndpoint = client.Client.RemoteEndPoint.ToString();
                    DateTime connectionTime = DateTime.Now;

                    Console.WriteLine($"Новый клиент подключен: {clientEndpoint} в {connectionTime}");
                    LogConnection(clientEndpoint, connectionTime, "Подключение");

                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    string response = ProcessRequest(request);
                    byte[] data = Encoding.ASCII.GetBytes(response);
                    stream.Write(data, 0, data.Length);

                    DateTime disconnectionTime = DateTime.Now;
                    LogConnection(clientEndpoint, disconnectionTime, "Отключение");

                    client.Close();
                    Console.WriteLine($"Клиент {clientEndpoint} отключился в {disconnectionTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            finally
            {
                listener.Stop();
            }
        }

        private static string ProcessRequest(string request)
        {
            if (currencyRates.ContainsKey(request))
            {
                string response = currencyRates[request].ToString();
                Console.WriteLine($"Курс валют для {request}: {response}");
                return response;
            }
            else
            {
                Console.WriteLine($"Некорректный запрос: {request}");
                return "Invalid currency pair";
            }
        }

        private static void LogConnection(string clientEndpoint, DateTime time, string action)
        {
            string logEntry = $"{time}: {action} клиента {clientEndpoint}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            Console.WriteLine(logEntry);
        }
    }
}

/* // Client
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace TcpClient2
{
    internal class Program
    {
        static void Main()
        {
            ConnectToServer();
        }

        static void ConnectToServer()
        {
            try
            {
                while (true)
                {
                    Console.Write("Введите запрос (например, USD_EUR или EUR_USD): ");
                    string userInput = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(userInput))
                    {
                        break;
                    }

                    TcpClient client = new TcpClient("127.0.0.1", 8888);
                    NetworkStream stream = client.GetStream();

                    byte[] data = Encoding.ASCII.GetBytes(userInput);
                    stream.Write(data, 0, data.Length);

                    data = new byte[256];
                    StringBuilder responseData = new StringBuilder();
                    int bytes;

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        responseData.Append(Encoding.ASCII.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine("Курс валют: " + responseData.ToString());

                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
}
*/