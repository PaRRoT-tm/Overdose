using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System.Collections.Generic;

namespace Overdose.Handlers
{
    public class Server
    {
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Overdose.Singleton.mainCoroEnabled = false;
            foreach (CoroutineHandle handle in Overdose.Singleton.Coroutines)
            {
                Log.Debug($"Killed coro {handle}", Overdose.Singleton.Config.Debug);
                Timing.KillCoroutines(handle);
            }

            Overdose.Singleton.Coroutines = new List<CoroutineHandle>();

            Overdose.Singleton.player.medicalUsers = null;
            Overdose.Singleton.player.numOverdoses = null;
        }

        public void OnRoundStarted()
        {
            Overdose.Singleton.player.medicalUsers = new Dictionary<int, int>();
            Overdose.Singleton.player.numOverdoses = new Dictionary<int, int>();
        }
    }
}