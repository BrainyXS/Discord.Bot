using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;

namespace Discord.Bot.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _node;

        public MusicModule(LavaNode node)
        {
            _node = node;
        }
        

        [Command("play")]
        public async Task Play(string args)
        {
            var i = _node.IsConnected;

            await Context.Message.DeleteAsync();
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel != null)
            {
                await _node.JoinAsync(channel);
                var search = args;


                try
                {
                    var response = await _node.SearchYouTubeAsync("Apache 207 Roller");
                    var song = response.Tracks.First();
                    if (song == null)
                    {
                        await ReplyAsync("Kein passender Song gefunden");
                        return;
                    }

                    await ReplyAsync("Starte song: " + song.Title);
                    var player = _node.GetPlayer(Context.Guild);
                    await player.PlayAsync(song);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await _node.LeaveAsync(channel);
                }
            }
        }


        [Command("leave")]
        [Alias("stop")]
        public async Task stop()
        {
            await Context.Message.DeleteAsync();
            await _node.LeaveAsync((Context.User as IGuildUser)?.VoiceChannel);
        }
    }
}