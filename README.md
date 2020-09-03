This is a simple plugin that adds overdosing by use of medical items to SCP: Secret Lab. It's highly customizable with the config. Works with MedKits, Adrenaline, and Painkillers.


You can download the dll [here](https://github.com/steven4547466/Overdose/releases/latest). This plugin requires EXILED, which you can find [here](https://github.com/galaxy119/EXILED).

Please report any issues you find or feature requests!

## Config options

- MinUses (2) - The minimum uses of medical items before an overdose has the chance of happening.
- BaseChance (5) - The base chance of a player becoming sick.
- ChanceIncreaseExponential (false) - Sets whether the chance increase should be exponential or linear.
- ChanceIncreasePer (5) - Chance increase after each medical item use (subtracting MinUses). Disregarded if exponential.
- HealthDrainPerSecond (0.08) - Sets the health drain per second.
- HealthDrainPerSecondIncrease (0.2) - Sets the increase in health drain when overdosing more than once.
- PainKillerEnabled (true) - Sets whether a player can get sick from using painkillers.
- AdrenalineEnabled (true) - Sets whether a player can get sick from using adrenaline.
- MedKitEnabled (false) - Sets whether a player can get sick from using medkits.
- CanBeCleansed (true) - Sets whether SCP-500 cleanses the effect.
- OverdoseMessage ("You begin to feel sick") - Sets the message the player sees when they begin to drain health.
