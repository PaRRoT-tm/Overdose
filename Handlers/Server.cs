using Exiled.Events.EventArgs;
using MEC;
using System.Collections.Generic;

namespace Overdose.Handlers
{
    public class Server
    {
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle handle in Overdose.Instance.Coroutines)
                Timing.KillCoroutines(handle);

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
