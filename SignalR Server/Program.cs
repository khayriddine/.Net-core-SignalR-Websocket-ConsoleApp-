using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace SignalR_Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            var ConsOut = Console.Out;
            Console.SetOut(new StreamWriter(Stream.Null));

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5050")
                .Build();

            host.Start();

            Console.SetOut(ConsOut);

            while (true)
            {
                Console.WriteLine(Console.ReadLine());
            }
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void ConfigureProduction(IApplicationBuilder app)
        {
            app.UseSignalR(route =>
            {
                route.MapHub<MyHub>("/stream");
            });
        }
    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class MyHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            Console.WriteLine($"User: {user} says {message}");
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            Console.WriteLine($"Connected user: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            Console.WriteLine($"Disconnected user: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
