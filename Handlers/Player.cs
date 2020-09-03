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
        public void OnMedicalItemUsed(UsedMedicalItemEventArgs ev)
        {
            if (ev.Item == ItemType.Adrenaline && Overdose.Instance.Config.AdrenalineEnabled == false) return;
            if (ev.Item == ItemType.Painkillers && Overdose.Instance.Config.PainkillerEnabled == false) return;
            if (ev.Item == ItemType.Medkit && Overdose.Instance.Config.MedKitEnabled == false) return;
            if (ev.Item == ItemType.SCP207 && Overdose.Instance.Config.SCP207Enabled == false) return;
            if (ev.Item == ItemType.SCP500 && Overdose.Instance.Config.CanBeCleansed)
            {
                Log.Debug($"Player with id {ev.Player.Id} has been cleansed by SCP-500.", Overdose.Instance.Config.Debug);
                if (medicalUsers.ContainsKey(ev.Player.Id)) medicalUsers.Remove(ev.Player.Id);
                if (numOverdoses.ContainsKey(ev.Player.Id)) numOverdoses.Remove(ev.Player.Id);
                return;
            }

            Log.Debug($"Player with id {ev.Player.Id} has used a medical item.");
            if (medicalUsers.ContainsKey(ev.Player.Id))
            {
                medicalUsers[ev.Player.Id] += 1;
                Log.Debug($"Medical items used: {medicalUsers[ev.Player.Id]} and min uses: {Overdose.Instance.Config.MinUses}", Overdose.Instance.Config.Debug);
                if (medicalUsers[ev.Player.Id] >= Overdose.Instance.Config.MinUses)
                {
                    Log.Debug($"Player with id {ev.Player.Id} could overdose.", Overdose.Instance.Config.Debug);
                    double chance = 0;
                    if(Overdose.Instance.Config.ChanceIncreaseExponential == false)
                    {
                        chance = Overdose.Instance.Config.BaseChance + (Overdose.Instance.Config.ChanceIncreasePer * (medicalUsers[ev.Player.Id] - Overdose.Instance.Config.MinUses));
                    }else
                    {
                        chance = Math.Pow(Overdose.Instance.Config.BaseChance, medicalUsers[ev.Player.Id] - Overdose.Instance.Config.MinUses);
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
                                Overdose.Instance.mainCoroEnabled = true;
                                co = Timing.RunCoroutine(HealthDrain());
                                Overdose.Instance.Coroutines.Add(co);
                            }
                        }
                        Log.Debug($"Player with id {ev.Player.Id} has overdosed {numOverdoses[ev.Player.Id]} times.", Overdose.Instance.Config.Debug);
                        ev.Player.Broadcast(5, Overdose.Instance.Config.OverdoseMessage);
                    }else
                    {
                        Log.Debug($"Player with id {ev.Player.Id} has failed to overdose chance: {chance} value: {val}", Overdose.Instance.Config.Debug);
                    }
                }
            }else
            {
                Log.Debug($"Player with id {ev.Player.Id} has been added to the medialUsers dictionary.", Overdose.Instance.Config.Debug);
                medicalUsers.Add(ev.Player.Id, 1);
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (medicalUsers.ContainsKey(ev.Target.Id)) medicalUsers.Remove(ev.Target.Id);
            if(numOverdoses.ContainsKey(ev.Target.Id)) numOverdoses.Remove(ev.Target.Id);
        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (medicalUsers.ContainsKey(ev.Player.Id)) medicalUsers.Remove(ev.Player.Id);
            if (numOverdoses.ContainsKey(ev.Player.Id)) numOverdoses.Remove(ev.Player.Id);
        }

        public void OnChangingRole(ChangingRoleEventArgs ev) 
        {
            if (medicalUsers.ContainsKey(ev.Player.Id)) medicalUsers.Remove(ev.Player.Id);
            if (numOverdoses.ContainsKey(ev.Player.Id)) numOverdoses.Remove(ev.Player.Id);
        }

        public IEnumerator<float> HealthDrain()
        {
            while (numOverdoses != null && numOverdoses.Count > 0)
            {
                double HealthPerSec = Overdose.Instance.Config.HealthDrainPerSecond;
                double HealthPerSecInc = Overdose.Instance.Config.HealthDrainPerSecondIncrease;
                foreach (var ent in numOverdoses)
                {
                    Log.Debug($"Player with id {ent.Key} has drained {HealthPerSec + (HealthPerSecInc * (ent.Value - 1))} health.", Overdose.Instance.Config.Debug);
                    EPlayer p = EPlayer.Get(ent.Key);
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
            Overdose.Instance.mainCoroEnabled = false;
            Log.Debug($"Stoping Coro {co}", Overdose.Instance.Config.Debug);
            Timing.KillCoroutines(co);
            yield break;
        }
    }
}
