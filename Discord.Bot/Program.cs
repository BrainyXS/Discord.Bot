 using System;
 using System.Reflection;
 using System.Security.Authentication.ExtendedProtection;
 using System.Threading.Tasks;
 using Discord;
 using Discord.Commands;
 using Discord.WebSocket;
 using Discord.Audio;
 using Microsoft.Extensions.DependencyInjection;
 using Victoria;

 namespace Discord.Bot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _service;
        private string token = "NzI0NjY4OTU4ODIwNjYzNDIy.XvDlTw.9Lo2ob1x_07l5YdXmp_cCKpYMws";
        static void Main(string[] args)
        {
            new Program().RunBotAsync().GetAwaiter().GetResult();
            Console.WriteLine("Starting Bot!");
            
        }
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _service = ConfigureServices();
            
            _client.Log += ClientLog;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task ClientLog(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message != null && message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;
            if (message.HasStringPrefix("/", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _service);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<LavaNode>().BuildServiceProvider();
        }
        
    }
}