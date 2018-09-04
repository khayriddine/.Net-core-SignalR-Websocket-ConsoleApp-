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

            var str = "{\r\n    \"glossary\": {\r\n        \"title\": \"example glossary\",\r\n\t\t\"GlossDiv\": {\r\n            \"title\": \"S\",\r\n\t\t\t\"GlossList\": {\r\n                \"GlossEntry\": {\r\n                    \"ID\": \"SGML\",\r\n\t\t\t\t\t\"SortAs\": \"SGML\",\r\n\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\r\n\t\t\t\t\t\"Acronym\": \"SGML\",\r\n\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\r\n\t\t\t\t\t\"GlossDef\": {\r\n                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\r\n\t\t\t\t\t\t\"GlossSeeAlso\": [\"GML\", \"XML\"]\r\n                    },\r\n\t\t\t\t\t\"GlossSee\": \"markup\"\r\n                }\r\n            }\r\n        }\r\n    }\r\n}";


            SendMessage("Bob", str);



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
