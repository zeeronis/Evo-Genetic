using PixelEngine;
using System;

namespace EvoGenome
{
    public class Unit : WorldObject
    {
        public static Pixel PhotosynthesisColor     = new Pixel(127, 255, 0);
        public static Pixel HarvestCrystalsColor    = new Pixel(30, 144, 255);
        public static Pixel AgressiveColor          = new Pixel(178, 34, 34);

        public static Pixel NewbieColor             = new Pixel(0, 255, 0);
        public static Pixel DieColor                = new Pixel(230, 230, 230);
        
        public static Pixel minEnergyColor          = new Pixel(255, 255, 224);
        public static Pixel maxEnergyColor          = new Pixel(255, 69, 0);


        public int genIndex;
        public byte[] genome;

        public bool IsDie;

        public float lifetimeLeft;
        public float energy;

        public float killsScore;
        public float photosynthesisScore;
        public float harvestCrystalsScore;

        public Pixel CurrentColor
        {
            get
            {
                if (IsDie)
                    return DieColor;

                float totalScore = killsScore + photosynthesisScore + harvestCrystalsScore;
                if (totalScore == 0)
                    return NewbieColor;

                return PixelEx.CombinedColors(averageColor: false,
                    PixelEx.Multiply(AgressiveColor, killsScore / totalScore),
                    PixelEx.Multiply(PhotosynthesisColor, photosynthesisScore / totalScore),
                    PixelEx.Multiply(HarvestCrystalsColor, harvestCrystalsScore / totalScore)
                    );
            }
        }

        public Pixel HeatColor
        {
            get
            {
                return PixelEx.Lerp(minEnergyColor, maxEnergyColor, energy / WorldRules.UNIT_MAX_ENERGY);
            }
        }


        public void Init(Func<int, int> Random, int x, int y)
        {
            var genome = new byte[WorldRules.UNIT_GENOME_SIZE];
            for (int i = 0; i < genome.Length; i++)
            {
                genome[i] = (byte)UnitCommand.Photosynthesis;
            }

            Init(Random, x, y, genome);
        }

        public void Init(Func<int, int> Random, int x, int y, byte[] genome)
        {
            this.x = x;
            this.y = y;

            genIndex = 0;
            killsScore = 0;
            photosynthesisScore = 0;
            harvestCrystalsScore = 0;

            IsDie = false;
            energy = WorldRules.UNIT_START_ENERGY;
            lifetimeLeft = WorldRules.UNIT_MIN_LIFETIME + Random((int)WorldRules.UNIT_MAX_LIFETIME - (int)WorldRules.UNIT_MIN_LIFETIME);

            if (this.genome == null)
            {
                this.genome = new byte[genome.Length];
            }
            
            genome.CopyTo(this.genome, 0);
        }


        public void MutateGenome(int mutateCounts, Func<int, int> Random)
        {
            for (int i = 0; i < mutateCounts; i++)
            {
                genome[Random(genome.Length)] = (byte)Random(WorldRules.UNIT_COMMANDS_COUNT);
            }
        }

        public void MoveCommandsPointer()
        {
            genIndex = (genIndex + 1) % genome.Length;
        }

        public UnitCommand GetCurrentCommand()
        {
            byte currCommand = genome[genIndex];
            var command = (UnitCommand)currCommand;

            return command;
        }

        public int CompareGenome(byte[] genome)
        {
            int matchCount = 0;

            for (int i = 0; i < genome.Length; i++)
            {
                if (this.genome[i] == genome[i])
                    matchCount++;
            }

            return matchCount;
        }
    }
}
