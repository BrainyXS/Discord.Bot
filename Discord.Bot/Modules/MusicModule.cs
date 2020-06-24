using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Discord.Bot.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _node;
        private readonly QueueManager _manager;

        public MusicModule(LavaNode node, QueueManager manager)
        {
            _node = node;
            _manager = manager;
        
    }

        [Command("Add")]
        [Alias("Play")]
        public async Task Add([Remainder] string args)
        {
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            var song = await _node.SearchYouTubeAsync(args);
            var track = song.Tracks.First();
            _manager.Songs.Add(track);
            _manager.Channel = Context.Channel;
            _manager.Guild = Context.Guild;
            await _node.JoinAsync(channel);
            if (_node.HasPlayer(Context.Guild))
            {
                var p = _node.GetPlayer(Context.Guild);
                if (p.PlayerState != PlayerState.Playing)
                {
                    await p.PlayAsync(track);
                }

                var e = new EmbedBuilder();
                e.WithDescription(track.Title);
                e.Color = Color.Gold;
                var msg = await ReplyAsync(Context.User.Username, false, e.Build());
                await Task.Delay(6000);
                await msg.DeleteAsync();
            }
        }

        [Command("Clear")]
        public async Task Clear()
        {
            _manager.Songs.Clear();
            await _node.GetPlayer(Context.Guild).StopAsync();
            var e = new EmbedBuilder();
            e.Description = "Warteschlange gelöscht";
            e.Color = Color.Red;

            var msg = await ReplyAsync("", false, e.Build());
            await Task.Delay(6000);
            await msg.DeleteAsync();
        }

        [Command("Skip")]
        public async Task Skip()
        {
            await _node.GetPlayer(Context.Guild).StopAsync();
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