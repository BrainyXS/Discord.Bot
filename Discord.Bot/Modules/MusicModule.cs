using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MySql.Data.MySqlClient;
using Victoria;
using Victoria.Enums;

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

        [Command("ShowPlaylists")]
        public async Task SaveQueue()
        {
            using (var connection = new MySqlConnection(Secret.GetSqlConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand("SELECT * FROM Playlist", connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            await ReplyAsync(
                                $"{reader[0]}) \t Name: {reader[1]} \t User: {Context.Guild.GetUser(ulong.Parse(reader[2].ToString())).Username}");
                        }
                    }
                }
            }
        }

        [Command("SaveQueue")]
        public async Task SaveCurrentQueue(string name)
        {
            IList<ulong> userWithPlaylist = new List<ulong>();
            using (var connection = new MySqlConnection(Secret.GetSqlConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand("SELECT * FROM Playlist", connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           userWithPlaylist.Add(ulong.Parse(reader[2].ToString()));
                        }
                    }
                }
            }
            
            
            var user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Ehrenbürger");
            var lid = 0;
            if (Context.User.Username == "BrainyXS" || (user.Roles.Contains(role) && userWithPlaylist.Count(x => x == user.Id) < 3) || userWithPlaylist.All(x => x != user.Id))
            {
                using (var connection = new MySqlConnection(Secret.GetSqlConnectionString()))
                {
                    var id = Context.User.Id;
                    using (MySqlCommand command =
                        new MySqlCommand($"INSERT INTO Playlist (PlaylistName, User_ID) VALUES ('{name}', {id})",
                            connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();
                        }
                    }


                    using (MySqlCommand command =
                        new MySqlCommand($"SELECT PlaylistID FROM Playlist WHERE PlaylistName = '{name}'",
                            connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();
                            lid = int.Parse(reader[0].ToString());
                        }

                        ReplyAsync("Gespeichert!");
                    }
                }

                using (var connection = new MySqlConnection(Secret.GetSqlConnectionString()))
                {
                    connection.Open();
                    foreach (var lavaTrack in _manager.Songs)
                    {
                        using (var command =
                            new MySqlCommand(
                                $"INSERT INTO Song (Title, FK_PlaylistID) VALUES ('{lavaTrack.Title}', {lid})",
                                connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                reader.Read();
                            }
                        }
                    }
                }
            }
            else
            {
                var e = new EmbedBuilder();
                e.WithColor(Color.Red);
                e.WithDescription("Du hast bereits die Maximale Anzahl an Playlists erreicht!");
                await ReplyAsync("", false, e.Build());
            }
        }


        [Command("LoadList")]
        [Alias("LoadPlaylist")]
        public async Task AddPlaylistToQueue(int id)
        {
            using (var connection = new MySqlConnection(Secret.GetSqlConnectionString()))
            {
                using (MySqlCommand command =
                    new MySqlCommand($"SELECT * FROM Song WHERE FK_PlaylistID = {id}", connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            await Add(reader[1].ToString());
                        }
                    }
                }
            }
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
                var msg = await ReplyAsync("", false, e.Build());
            }
        }

        [Command("pause")]
        public async Task Pause()
        {
            if (_node.GetPlayer(Context.Guild).PlayerState == PlayerState.Paused)
            {
                await _node.GetPlayer(Context.Guild).ResumeAsync();
            }
            else
            {
                await _node.GetPlayer(Context.Guild).PauseAsync();
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
            var e = new EmbedBuilder();
            e.WithColor(Color.Red);
            e.WithDescription("Channel verlassen");
            var m = await ReplyAsync("", false, e.Build());
            await Task.Delay(5000);
            await m.DeleteAsync();
        }
    }
}