using PixelEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EvoGenome
{
    public class Evo1Sim : Game
    {
        private Pixel bgUpColor = new Pixel(0, 0, 220);
        private Pixel bgDownColor = new Pixel(0, 0, 0);

        private int worldDrawType = 0;
        private int ticksPerFrame = 1;

        private double worldTicks;

        private Cell[,] cells;
        private List<Unit> aliveUnits = new List<Unit>();
        private List<Unit> dieUnits = new List<Unit>();
        private List<Unit> createdUnits = new List<Unit>();

        private Cell[] bufferCells1 = new Cell[4];
        private Cell[] bufferCells2 = new Cell[4];


        #region Lifecircle

        public override void OnKeyPress(Key k)
        {
            if ((int)k >= 27 && (int)k <= 35)
            {
                ticksPerFrame = WorldRules.gameSpeedOptions[k];
            }

            if (k == Key.Q) worldDrawType = 0;
            else if (k == Key.W) worldDrawType = 1;
        }

        public override void OnCreate()
        {
            cells = new Cell[ScreenWidth, ScreenHeight];
            for (int x = 0; x < ScreenWidth; x++)
            {
                for (int y = 0; y < ScreenHeight; y++)
                {
                    cells[x, y] = new Cell(x, y)
                    {
                        sunLevel = GetResourceLevel(y, UnitCommand.Photosynthesis),
                        crystalsLevel = GetResourceLevel(y, UnitCommand.HarvestCrystals)
                    };
                }
            }

            for (int i = 0; i < WorldRules.WORLD_INIT_UNITS_COUNT; i++)
            {
                var cell = cells[Random(ScreenWidth), Random(ScreenHeight)];
                if (cell.IsEmpty)
                {
                    CreateUnitOnCell(cell);
                }
            }

            DrawMap();
        }

        public override void OnUpdate(float elapsed)
        {
            for (int i = 0; i < ticksPerFrame; i++)
            {
                HandleUnits();
            }

            DrawMap();
        }

        #endregion


        #region Drawing

        private void DrawMap()
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                for (int y = 0; y < ScreenHeight; y++)
                {
                    var cell = cells[x, y];
                    if (cell.IsEmpty)
                    {
                        SetDefaultColor(x, y);
                    }
                    else
                    {
                        DrawUnit(x, y, cell.unit);
                    }
                }
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"World ticks: {worldTicks}");
            Console.WriteLine($"*************************");
            Console.WriteLine($"Alive: {aliveUnits.Count + createdUnits.Count}");
            Console.WriteLine($"Poolled: {dieUnits.Count}");
        }

        private void SetDefaultColor(int x, int y)
        {
            Draw(x, y, PixelEx.Lerp(
                bgUpColor,
                bgDownColor,
                (float)y / ScreenHeight));
        }

        private void DrawUnit(int x, int y, Unit unit)
        {
            if (worldDrawType == 0)
            {
                Draw(x, y, unit.CurrentColor);
            }
            else if (worldDrawType == 1)
            {
                Draw(x, y, unit.HeatColor);
            }
        }

        #endregion


        #region Map Cells

        private Cell TryGetCell(int x, int y)
        {
            if (y < 0 || y >= ScreenHeight)
                return null;

           /* if (x < 0)
                x += ScreenWidth;*/

            return cells[(x + ScreenWidth) % ScreenWidth, y];
        }

        private void CollectNeighborsCells(int x, int y)
        {
            // Clear buffer
            for (int i = 0; i < bufferCells1.Length; i++)
            {
                bufferCells1[i] = null;
                bufferCells2[i] = null;
            }

            bufferCells1[0] = TryGetCell(x + 1, y);
            bufferCells1[1] = TryGetCell(x - 1, y);
            bufferCells1[2] = TryGetCell(x, y + 1);
            bufferCells1[3] = TryGetCell(x, y - 1);
        }

        private Cell GetRandomNeighbornCellEmpty(int x, int y)
        {
            CollectNeighborsCells(x, y);

            int foundCells = 0;
            for (int i = 0; i < bufferCells1.Length; i++)
            {
                if (bufferCells1[i] == null || bufferCells1[i].unit == null)
                {
                    bufferCells2[foundCells] = bufferCells1[i];
                    foundCells++;
                }
            }

           /* if (foundCells == 0)
                return null;*/

            return bufferCells2[Random(foundCells)];
        }

        private Cell GetRandomNeighbornCellWithAliveEnemy(int x, int y, bool isDie)
        {
            CollectNeighborsCells(x, y);

            int foundCells = 0;
            for (int i = 0; i < bufferCells1.Length; i++)
            {
                if (bufferCells1[i] != null && bufferCells1[i].unit != null && bufferCells1[i].unit.IsDie == isDie)
                {
                    bufferCells2[foundCells] = bufferCells1[i];
                    foundCells++;
                }
            }

           /* if (foundCells == 0)
                return null;*/

            return bufferCells2[Random(foundCells)];
        }

        #endregion


        #region World Handle

        private void UnitDie(Unit unit)
        {
            var cell = TryGetCell(unit.x, unit.y);
            cell.unit = null;

            unit.IsDie = true;
        }

        private void CreateUnitOnCell(Cell cell, byte[] genome = null)
        {
            if (cell.IsEmpty)
            {
                if (dieUnits.Count > 0)
                {
                    cell.unit = dieUnits[0];
                    dieUnits.RemoveAt(0);
                }
                else
                {
                    cell.unit = new Unit();
                }

                createdUnits.Add(cell.unit);

                if (genome == null)
                {
                    cell.unit.Init(Random, cell.x, cell.y);
                }
                else
                {
                    cell.unit.Init(Random, cell.x, cell.y, genome);
                }
            }
            else
            {
                Console.WriteLine("Attempted to create unit on not empty cell");
            }
        }

        private void HandleUnits()
        {
            // Activete created unit frim last frame
            foreach (var createdUnit in createdUnits)
            {
                aliveUnits.Add(createdUnit);
            }
            createdUnits.Clear();


            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Handle alive Units
            for (int i = 0; i < aliveUnits.Count; i++)
            {
                var aliveUnit = aliveUnits[i];
                if (!aliveUnit.IsDie)
                {
                    HandleUnit(cells[aliveUnit.x, aliveUnit.y]);
                }
            }

            sw.Stop();
            Console.SetCursorPosition(0, 10);
            Console.WriteLine(sw.ElapsedTicks);


            // Collect die units
            for (int i = 0; i < aliveUnits.Count; i++)
            {
                if (aliveUnits[i].IsDie)
                {
                    dieUnits.Add(aliveUnits[i]);
                    aliveUnits.RemoveAt(i);
                    i--;
                }
            }

            worldTicks++;
        }

        private void HandleUnit(Cell cell)
        {
            var unit = cell.unit;
            var command = unit.GetCurrentCommand();

            unit.MoveCommandsPointer();
            unit.lifetimeLeft -= 1;
            unit.energy -= WorldRules.UNIT_LOST_ENERGY_PER_TICK;

            switch (command)
            {
                case UnitCommand.Photosynthesis:
                    UnitPhotosynthesis(cell);
                    break;

                case UnitCommand.HarvestCrystals:
                    UnitHarvestCrystals(cell);
                    break;
              
                case UnitCommand.AttackNeighborn:
                    var cellWithEnemy = GetRandomNeighbornCellWithAliveEnemy(cell.x, cell.y, isDie: false);
                    if (cellWithEnemy != null)
                    {
                        if (cell.unit.CompareGenome(cellWithEnemy.unit.genome) < WorldRules.FRACTION_GENOME_MATCHES)
                        {
                            UnitAttack(cell, cellWithEnemy);
                        }
                        
                    }

                    break;
              
                case UnitCommand.MoveUp:
                case UnitCommand.MoveDown:
                case UnitCommand.MoveLeft:
                case UnitCommand.MoveRight:
                    MoveUnit(cell, command);
                    break;
              
              
                default:
                case UnitCommand.Unknow:
                    break;
            }

            if (unit.energy <= 0 || unit.lifetimeLeft <= 0)
            {
                UnitDie(unit);
            }
            else if (unit.energy >= WorldRules.UNIT_MIN_ENERGY_FOR_DUPLICATE)
            {
                var neighbornCell = GetRandomNeighbornCellEmpty(cell.x, cell.y);
                if (neighbornCell != null)
                {
                    unit.energy -= WorldRules.UNIT_DUPLICATE_ENERGY_COST;

                    CreateUnitOnCell(neighbornCell, unit.genome);
                    if (Random(101) <= WorldRules.UNIT_MUTATE_CHANCE)
                    {
                        neighbornCell.unit.MutateGenome(WorldRules.UNIT_MUTATES_COUNT, Random);
                    }
                }
            }
        }

        private Cell MoveUnit(Cell from, UnitCommand command)
        {
            int offsetX = 0;
            int offsetY = 0;

            switch (command)
            {
                case UnitCommand.MoveUp:
                    offsetY--;
                    break;
                case UnitCommand.MoveDown:
                    offsetY++;
                    break;
                case UnitCommand.MoveLeft:
                    offsetX--;
                    break;
                case UnitCommand.MoveRight:
                    offsetX++;
                    break;
            }

            var to = TryGetCell(from.x + offsetX, from.y + offsetY);
            if (to != null && to.unit == null)
            {
                to.unit = from.unit;
                to.unit.x = to.x;
                to.unit.y = to.y;
                from.unit = null;

                return to;
            }

            return from;
        }

        private float GetNormalizedHeight(float y)
        {
            return (float)y / ScreenHeight;
        }

        private float GetResourceLevel(int y, UnitCommand resource)
        {
            float topLevel = 0;
            float bottomLevel = 0;

            if (resource == UnitCommand.HarvestCrystals)
            {
                topLevel = WorldRules.WORLD_TOP_CRYSTALS_LEVEL;
                bottomLevel = WorldRules.WORLD_BOTTOM_CRYSTALS_LEVEL;
            }
            else if (resource == UnitCommand.Photosynthesis)
            {
                topLevel = WorldRules.WORLD_TOP_SUN_LEVEL;
                bottomLevel = WorldRules.WORLD_BOTTOM_SUN_LEVEL;
            }

            var normalizedHeight = GetNormalizedHeight(y);
            var value = Mathf.Lerp(topLevel, bottomLevel, normalizedHeight);
            value = Mathf.Clamp(value, 0, float.MaxValue);

            return value;
        }

        private void UnitPhotosynthesis(Cell cell)
        {
            var value = cell.sunLevel;

            cell.unit.photosynthesisScore += value;
            cell.unit.energy = Mathf.Clamp(cell.unit.energy + value, 0, WorldRules.UNIT_MAX_ENERGY);
        }

        private void UnitHarvestCrystals(Cell cell)
        {
            var value = cell.crystalsLevel;

            cell.unit.harvestCrystalsScore += value;
            cell.unit.energy = Mathf.Clamp(cell.unit.energy + value, 0, WorldRules.UNIT_MAX_ENERGY);
        }

        private void UnitAttack(Cell attacker, Cell defender)
        {
            var energy = defender.unit.energy * WorldRules.WORLD_KILL_STEAL_ENERGY_PERCENT;
            
            attacker.unit.energy += energy;
            attacker.unit.killsScore += energy;

            UnitDie(defender.unit);
        }

        #endregion
    }
}
