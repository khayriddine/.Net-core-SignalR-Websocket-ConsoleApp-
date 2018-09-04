using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace SignalR_Client
{
    class Program
    {
        static HubConnection connection;
        static ConcurrentBag<string> messagesList = new ConcurrentBag<string>();
        static void Main(string[] args)
        {
            connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5050/chathub")
            .Build();


            ConnectToServer();
            string message = "Hello world!";
            SendMessage("Bob", message);

            while (true)
            {

                Console.WriteLine(Console.ReadLine());
            }
        }

        private static async void ConnectToServer()
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var newMessage = $"{user}: {message}";
                messagesList.Add(newMessage);
                Console.WriteLine(newMessage);
            });

            try
            {
                await connection.StartAsync();
                messagesList.Add("Connection started");
                Console.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                messagesList.Add(ex.Message);
            }
        }

        private static async void SendMessage(string user, string message)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", user, message);
            }
            catch (Exception ex)
            {
                messagesList.Add(ex.Message);
            }
        }
    }
}
