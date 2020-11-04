using Exiled.API.Enums;
using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using MEC;

using System;
using System.Collections.Generic;

namespace Overdose
{
    public class Overdose : Plugin<Config>
    {
        private static readonly Lazy<Overdose> LazyInstance = new Lazy<Overdose>(() => new Overdose());
        public static Overdose Instance => LazyInstance.Value;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public override string Name { get; } = "Overdose";
        public override string Author { get; } = "Steven4547466";
        public override Version Version { get; } = new Version(1, 0, 6);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 3);
        public override string Prefix { get; } = "Overdose";

        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        private Overdose() {}

        public Handlers.Player player { get; set; }
        public Handlers.Server server { get; set; }

        public bool mainCoroEnabled { get; set; }


        public override void OnEnabled()
        {
            if (Overdose.Instance.Config.IsEnabled == false) return;
            base.OnEnabled();
            Log.Info("Overdose enabled.");
            RegisterEvents();
            Coroutines = new List<CoroutineHandle>();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Log.Info("Overdose disabled.");
            UnregisterEvents();
        }

        public override void OnReloaded()
        {
            base.OnReloaded();
            Log.Info("Overdose reloading.");
        }

        public void RegisterEvents()
        {
            player = new Handlers.Player();

            Player.MedicalItemUsed += player.OnMedicalItemUsed;
            Player.Died += player.OnDied;
            Player.ChangingRole += player.OnChangingRole;
            Player.Left += player.OnLeft;

            server = new Handlers.Server();
            Server.RoundEnded += server.OnRoundEnded;
            Server.RoundStarted += server.OnRoundStarted;
        }

        public void UnregisterEvents()
        {
            Log.Info("Events unregistered");
            mainCoroEnabled = false;
            Player.MedicalItemUsed -= player.OnMedicalItemUsed;
            Player.Died -= player.OnDied;
            Player.ChangingRole -= player.OnChangingRole;
            Player.Left -= player.OnLeft;

            player = null;

            Server.RoundEnded -= server.OnRoundEnded;
            Server.RoundStarted -= server.OnRoundStarted;

            server = null;

            if (Coroutines != null)
            {
                foreach (CoroutineHandle handle in Coroutines)
                {
                    Log.Debug($"Killed coro {handle}", Overdose.Instance.Config.Debug);
                    Timing.KillCoroutines(handle);
                }

                Coroutines = null;
            }
        }
    }
}
