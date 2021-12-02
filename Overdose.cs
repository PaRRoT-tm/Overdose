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
        public static Overdose Singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public override string Name { get; } = "Overdose";
        public override string Author { get; } = "Steven4547466";
        public override Version Version { get; } = new Version(1, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(4, 1, 2);
        public override string Prefix { get; } = "Overdose";

        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        public Handlers.Player player { get; set; }
        public Handlers.Server server { get; set; }

        public bool mainCoroEnabled { get; set; }


        public override void OnEnabled()
        {
            Singleton = this;

            if (Overdose.Singleton.Config.IsEnabled == false) return;
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

            Player.ItemUsed += player.OnUsedItem;
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
            Player.ItemUsed -= player.OnUsedItem;
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
                    Log.Debug($"Killed coro {handle}", Overdose.Singleton.Config.Debug);
                    Timing.KillCoroutines(handle);
                }

                Coroutines = null;
            }
        }
    }
}