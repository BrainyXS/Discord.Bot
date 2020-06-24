using System.Collections.Generic;
using Discord.WebSocket;
using Victoria;

namespace Discord.Bot.Modules
{
    public class QueueManager
    {
        public IList<LavaTrack> Songs { get; set; }
        public SocketGuild Guild { get; set; }
        public ISocketMessageChannel Channel { get; set; }

        public QueueManager()
        {
            Songs = new List<LavaTrack>();
        }
    }
}