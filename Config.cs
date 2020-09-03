
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Overdose
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Sets the min uses before possible chance of getting poisoned (It's impossible to be poisoned before this number). Must be greater than 1.")]
        public int MinUses { get; set; } = 2;

        [Description("Sets the initial starting chance of getting poisoned. 0-100")]
        public double BaseChance { get; set; } = 5;

        [Description("Sets whether the chance increase is exponential or not (Exponential meaning BaseChance^(Medical items used - MinUses)).")]
        public bool ChanceIncreaseExponential { get; set; } = false;

        [Description("Sets the amount of increase each use (Medical item used - MinUses). Disregarded if exponential.")]
        public double ChanceIncreasePer { get; set; } = 5;

        [Description("Sets the amount of health to drain per second.")]
        public double HealthDrainPerSecond { get; set; } = 0.08;

        [Description("Sets the amount of increase to health drain on multiple poison.")]
        public double HealthDrainPerSecondIncrease { get; set; } = 0.2;

        [Description("Sets whether this plugin works on painkillers.")]
        public bool PainkillerEnabled { get; set; } = true;

        [Description("Sets whether this plugin works on adrenaline.")]
        public bool AdrenalineEnabled { get; set; } = true;

        [Description("Sets whether this plugin works on medkits.")]
        public bool MedKitEnabled { get; set; } = false;

        [Description("Sets whether SCP-500 cleanses the effect.")]
        public bool CanBeCleansed { get; set; } = true;

        [Description("Sets the message the player sees when they begin to drain health.")]
        public string OverdoseMessage { get; set; } = "You begin to feel sick";

        [Description("Show debug messages?")]
        public bool Debug { get; set; } = false;
    }
}
