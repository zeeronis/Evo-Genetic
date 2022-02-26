using PixelEngine;
using System.Collections.Generic;

namespace EvoGenome
{
    public static class WorldRules
    {
        //Energy
        public const float UNIT_LOST_ENERGY_PER_TICK = 5;

        public const float UNIT_START_ENERGY = 100;
        public const float UNIT_MAX_ENERGY = 550;
        public const float UNIT_DUPLICATE_ENERGY_COST = 110;
        public const float UNIT_MIN_ENERGY_FOR_DUPLICATE = 200;

        //EVO
        public const int UNIT_MUTATE_CHANCE = 25;
        public const int UNIT_MUTATES_COUNT = 1;

        //COMMANDS
        public const int UNIT_GENOME_SIZE = 64;
        public const int UNIT_COMMANDS_COUNT = 64;

        //LIFETIME
        public const float UNIT_MIN_LIFETIME = 60 * 4f;
        public const float UNIT_MAX_LIFETIME = 60 * 10f;


        //FRACTION
        public const float FRACTION_GENOME_MATCHES = 60;


        //WORLD
        public const int WORLD_INIT_UNITS_COUNT = 50;

        public const float WORLD_TOP_SUN_LEVEL = 25;
        public const float WORLD_BOTTOM_SUN_LEVEL = -5;

        public const float WORLD_TOP_CRYSTALS_LEVEL = -5;
        public const float WORLD_BOTTOM_CRYSTALS_LEVEL = 30;

        public const float WORLD_KILL_STEAL_ENERGY_PERCENT = 0.2f;


        //WORLD
        public static Dictionary<Key, int> gameSpeedOptions = new Dictionary<Key, int>()
        { 
            [Key.K1] = 1, 
            [Key.K2] = 2, 
            [Key.K3] = 4, 
            [Key.K4] = 8, 
            [Key.K5] = 16, 
            [Key.K6] = 32, 
            [Key.K7] = 64, 
            [Key.K8] = 100, 
            [Key.K9] = 1000, 
        };
    }
}
