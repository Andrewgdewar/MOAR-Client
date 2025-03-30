// ConfigSettings.cs
using System;

namespace MOAR
{
    /// <summary>
    /// Represents all server-configurable parameters for AI and spawn behavior.
    /// Used for syncing between client/server and preset application.
    /// </summary>
    public class ConfigSettings
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public double pmcDifficulty { get; set; }
        public double scavDifficulty { get; set; }

        public double scavWaveDistribution { get; set; }
        public double scavWaveQuantity { get; set; }

        public bool zombiesEnabled { get; set; }
        public double zombieWaveDistribution { get; set; }
        public double zombieWaveQuantity { get; set; }
        public double zombieHealth { get; set; }

        public bool startingPmcs { get; set; }
        public bool spawnSmoothing { get; set; }
        public bool randomSpawns { get; set; }

        public double pmcWaveDistribution { get; set; }
        public double pmcWaveQuantity { get; set; }

        public int maxBotCap { get; set; }
        public int maxBotPerZone { get; set; }

        public double scavGroupChance { get; set; }
        public double pmcGroupChance { get; set; }
        public double sniperGroupChance { get; set; }

        public int pmcMaxGroupSize { get; set; }
        public int scavMaxGroupSize { get; set; }
        public double sniperMaxGroupSize { get; set; }

        public bool bossOpenZones { get; set; }

        public bool randomRaiderGroup { get; set; }
        public int randomRaiderGroupChance { get; set; }

        public bool randomRogueGroup { get; set; }
        public int randomRogueGroupChance { get; set; }

        public bool disableBosses { get; set; }
        public int mainBossChanceBuff { get; set; }

        public bool bossInvasion { get; set; }
        public int bossInvasionSpawnChance { get; set; }
        public bool gradualBossInvasion { get; set; }

        public bool debug { get; set; }
    }
}
