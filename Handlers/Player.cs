using Exiled.Events.EventArgs;
using EPlayer = Exiled.API.Features.Player;
using MEC;

using System.Collections.Generic;
using System;
using Exiled.API.Features;
using Exiled.API.Extensions;

namespace Overdose.Handlers
{
    public class Player
    {
        public Dictionary<int, int> medicalUsers = new Dictionary<int, int>();
        public Dictionary<int, int> numOverdoses = new Dictionary<int, int>();

        Random rnd = new Random();

        CoroutineHandle co;
        public void OnUsedItem(UsedItemEventArgs ev)
        {
            if (medicalUsers == null || numOverdoses == null) return;
            if (ev.Player.IsGodModeEnabled) return;
            if (ev.Item.Type == ItemType.SCP268) return;
            if (ev.Item.Type == ItemType.Adrenaline && Overdose.Singleton.Config.AdrenalineEnabled == false) return;
            if (ev.Item.Type == ItemType.Painkillers && Overdose.Singleton.Config.PainkillerEnabled == false) return;
            if (ev.Item.Type == ItemType.Medkit && Overdose.Singleton.Config.MedKitEnabled == false) return;
            if (ev.Item.Type == ItemType.SCP207 && Overdose.Singleton.Config.SCP207Enabled == false) return;
            if (ev.Item.Type == ItemType.SCP500 && Overdose.Singleton.Config.CanBeCleansed)
            {
                Log.Debug($"Player with id {ev.Player.Id} has been cleansed by SCP-500.", Overdose.Singleton.Config.Debug);
                if (medicalUsers != null && medicalUsers.ContainsKey(ev.Player.Id)) medicalUsers.Remove(ev.Player.Id);
                if (numOverdoses != null && numOverdoses.ContainsKey(ev.Player.Id)) numOverdoses.Remove(ev.Player.Id);
                return;
            }

            Log.Debug($"Player with id {ev.Player.Id} has used a medical item.", Overdose.Singleton.Config.Debug);
            if (medicalUsers.ContainsKey(ev.Player.Id))
            {
                medicalUsers[ev.Player.Id] += 1;
                Log.Debug($"Medical items used: {medicalUsers[ev.Player.Id]} and min uses: {Overdose.Singleton.Config.MinUses}", Overdose.Singleton.Config.Debug);
                if (medicalUsers[ev.Player.Id] >= Overdose.Singleton.Config.MinUses)
                {
                    Log.Debug($"Player with id {ev.Player.Id} could overdose.", Overdose.Singleton.Config.Debug);
                    double chance = 0;
                    if(Overdose.Singleton.Config.ChanceIncreaseExponential == false)
                    {
                        chance = Overdose.Singleton.Config.BaseChance + (Overdose.Singleton.Config.ChanceIncreasePer * (medicalUsers[ev.Player.Id] - Overdose.Singleton.Config.MinUses));
                    }else
                    {
                        chance = Math.Pow(Overdose.Singleton.Config.BaseChance, medicalUsers[ev.Player.Id] - Overdose.Singleton.Config.MinUses);
                    }
                    double val = (rnd.NextDouble() * 99) + 1;
                    if (val <= chance)
                    {
                        if(numOverdoses.ContainsKey(ev.Player.Id))
                        {
                            numOverdoses[ev.Player.Id] += 1;
                        } else
                        {
                            numOverdoses.Add(ev.Player.Id, 1);
                            if(numOverdoses.Count == 1)
                            {
                                Overdose.Singleton.mainCoroEnabled = true;
                                co = Timing.RunCoroutine(HealthDrain());
                                Overdose.Singleton.Coroutines.Add(co);
                            }
                        }
                        Log.Debug($"Player with id {ev.Player.Id} has overdosed {numOverdoses[ev.Player.Id]} times.", Overdose.Singleton.Config.Debug);
                        ev.Player.Broadcast(5, Overdose.Singleton.Config.OverdoseMessage);
                    }else
                    {
                        Log.Debug($"Player with id {ev.Player.Id} has failed to overdose chance: {chance} value: {val}", Overdose.Singleton.Config.Debug);
                    }
                }
            }else
            {
                Log.Debug($"Player with id {ev.Player.Id} has been added to the medialUsers dictionary.", Overdose.Singleton.Config.Debug);
                medicalUsers.Add(ev.Player.Id, 1);
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (medicalUsers != null && medicalUsers.ContainsKey(ev.Target.Id)) medicalUsers.Remove(ev.Target.Id);
            if (numOverdoses != null && numOverdoses.ContainsKey(ev.Target.Id)) numOverdoses.Remove(ev.Target.Id);
        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (medicalUsers != null && medicalUsers.ContainsKey(ev.Player.Id)) medicalUsers.Remove(ev.Player.Id);
            if (numOverdoses != null && numOverdoses.ContainsKey(ev.Player.Id)) numOverdoses.Remove(ev.Player.Id);
        }

        public void OnChangingRole(ChangingRoleEventArgs ev) 
        {
            if (medicalUsers != null && medicalUsers.ContainsKey(ev.Player.Id)) medicalUsers.Remove(ev.Player.Id);
            if (numOverdoses != null && numOverdoses.ContainsKey(ev.Player.Id)) numOverdoses.Remove(ev.Player.Id);
        }

        public IEnumerator<float> HealthDrain()
        {
            while (numOverdoses != null && numOverdoses.Count > 0)
            {
                double HealthPerSec = Overdose.Singleton.Config.HealthDrainPerSecond;
                double HealthPerSecInc = Overdose.Singleton.Config.HealthDrainPerSecondIncrease;
                foreach (var ent in numOverdoses)
                {
                    Log.Debug($"Player with id {ent.Key} has drained {HealthPerSec + (HealthPerSecInc * (ent.Value - 1))} health.", Overdose.Singleton.Config.Debug);
                    EPlayer p = EPlayer.Get(ent.Key);
                    if (p.IsGodModeEnabled) 
                    {
                        numOverdoses.Remove(ent.Key);
                    }
                    if (p.Health - HealthPerSec + (HealthPerSecInc * (ent.Value - 1)) <= 0)
                    {
                        numOverdoses.Remove(ent.Key);
                        medicalUsers.Remove(ent.Key);
                        p.Kill(DamageTypes.Asphyxiation);
                        continue;
                    }
                    p.Health -= (float) (HealthPerSec + (HealthPerSecInc * (ent.Value - 1)));
                }
                yield return Timing.WaitForSeconds(1f);
            }
            Overdose.Singleton.mainCoroEnabled = false;
            Log.Debug($"Stoping Coro {co}", Overdose.Singleton.Config.Debug);
            Timing.KillCoroutines(co);
            yield break;
        }
    }
}