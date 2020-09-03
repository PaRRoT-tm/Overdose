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
            Overdose.Instance.mainCoroEnabled = false;
            foreach (CoroutineHandle handle in Overdose.Instance.Coroutines)
            {
                //Log.Debug($"Killed coro {handle}");
                Timing.KillCoroutines(handle);
            }

            Overdose.Instance.Coroutines = new List<CoroutineHandle>();

            Overdose.Instance.player.medicalUsers = null;
            Overdose.Instance.player.numOverdoses = null;
        }

        public void OnRoundStarted()
        {
            Overdose.Instance.player.medicalUsers = new Dictionary<int, int>();
            Overdose.Instance.player.numOverdoses = new Dictionary<int, int>();
        }
    }
}
