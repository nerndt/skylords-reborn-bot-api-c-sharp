﻿using Api;
using Example;
using System;
using System.Diagnostics;

namespace TryEverything
{
    //    /AI: add XanderAI TaintedFlora 4

    // Basic Strategies:

    public class TryMyBot : IAspWrapperImpl
    {
        string IAspWrapperImpl.Name => "XanderAI";

        private readonly Maps[] SUPPORTED_MAPS = new[] {
                    Maps.HaladurSpectator,
                    Maps.SimaiSpectator,
                    Maps.ElyonSpectator,
                    Maps.WazhaiSpectator,
                    Maps.LajeshSpectator,
                    Maps.UroSpectator,
                    Maps.YrmiaSpectator,
                    Maps.KoshanSpectator,
                    Maps.ZahaduneSpectator,
                    Maps.TuranSpectator,
                    Maps.FyreSpectator,
                    Maps.DanduilSpectator,
                    Maps.GorgashSpectator,
                    Maps.YshiaSpectator,
                    Maps.NadaiSpectator,
            };

        private BotState botState;

        #region Info to track
        private Team team = new Team();
        private Team opponents = new Team();
        #endregion Info to track

        public Deck[] DecksForMap(Maps map, string? name, ulong crc)
        {
            if (SUPPORTED_MAPS.Contains(map))
            {
                return new[] { TAINTED_FLORA_WITH_DEFENDER, BG1_DECK };
            }
            else
            {
                return Array.Empty<Deck>();
            }
        }

        public void PrepareForBattle(Maps map, string? name, ulong crc, Deck deck)
        {
            botState = new BotState()
            {
                selectedDeck = deck,
                myStart = Position2DExt.Zero(),
                oponents = new List<EntityId>(),
                team = new List<EntityId>(),
                tasks = new Tasks
                {
                    burrowerTasks = new Tasks.BurrowerTasks
                    {
                        burrower = new EntityId(0),
                        spawnBurrower = false,
                        getEnemyOffTheWall = false,
                        done = false,
                    },
                    swiftclawTasks = new Tasks.SwiftclawTasks { 
                        closestPowerSlotPosition = Position2DExt.Zero(),
                        closestTokenSlotPosition = Position2DExt.Zero(),
                        buildTokenSlot = false,
                        buildPowerSlot = false,
                        buildPrimalDefender = false,
                        changeMode = false, 
                        closestPowerSlot = new EntityId(0),
                        closestTokenSlot = new EntityId(0),
                        holdPosition = false,
                        produceSwiftclaw = false,
                        swiftclaw = new EntityId(0),
                        done = false,
                    },
                    windweaversTasks = new Tasks.WindweaversTasks {
                        closestWallPosition = Position2DExt.Zero(),
                        closeGateAt = 0,
                        closeGate = false,
                        closestWall = new EntityId(0),
                        buildBarrier = false,
                        closeGateId = new EntityId(0),
                        enterWall = false,
                        exitWall = false,
                        exitWallAt = 0,
                        exitWallId = new EntityId(0),
                        openGate = false,
                        spawnWindweavers = false,
                        spawnWindweaversOnBarrier = false,
                        windweavers = new EntityId(0),
                        done = false,
                    },
                    orbTasks = new Tasks.OrbTasks
                    {
                        getNearbyOrb = true,
                        getNearbyWell = true,
                        heal = false,
                        done = false
                    },
                    wellTasks = new Tasks.WellTasks
                    {
                        getNearbyOrb = true,
                        getNearbyWell = true,
                        heal = false,
                        done = false
                    },
                    cancelRepairBuilding = false,
                    cancelRepairWall = false,
                    castHeal = false,
                    castHurricane = false,
                    repairBuilding = false,
                    repairWall = false,
                    surrender = false,
                },
                canPlayCardAt = 0,
                myId = new EntityId(0),
                myTeam = 0,
            };
        }

        public void MatchStart(GameStartState state)
        {
            var yourPlayerId = state.YourPlayerId;
            var entities = state.Entities;
            botState.myId = yourPlayerId;
            Console.WriteLine($"My player ID is: {yourPlayerId}, I have deck: {botState.selectedDeck.Name}, and I own:");
#pragma warning disable CS8602 // Player must exist
            botState.myTeam = Array.Find(state.Players, p => p.Entity.Id == yourPlayerId).Entity.Team;
#pragma warning restore CS8602
            botState.oponents = new List<EntityId>();
            foreach (var player in state.Players)
            {
                if (player.Entity.Team != botState.myTeam)
                {
                    botState.oponents.Add(player.Entity.Id);
                    
                    opponents.Players.Add(player.Entity.Id, new Player(player.Entity.Id)); // Add all player to team
                }
                else
                {
                    botState.team.Add(player.Entity.Id);
                    team.Players.Add(player.Entity.Id, new Player(player.Entity.Id)); // Add all opponents to team
                }
                // break;
            }
            foreach (var s in entities.PowerSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    Console.WriteLine($"Power slot: {s.Entity.Id} at {s.Entity.Position.X}/{s.Entity.Position.Z}");
                    botState.myStart = s.Entity.Position.To2D();
                }
                if (s.Entity.PlayerEntityId != null)
                {
                    if (team.Players.ContainsKey(s.Entity.PlayerEntityId) == true)
                    {
                        team.Players[s.Entity.PlayerEntityId].StartPos = s.Entity.Position.To2D(); // Add player start pos
                    }
                    if (opponents.Players.ContainsKey(s.Entity.PlayerEntityId) == true)
                    {
                        opponents.Players[s.Entity.PlayerEntityId].StartPos = s.Entity.Position.To2D(); // Add player start pos
                    }
                }
            }

            #region Xander get other player info as well!

            #endregion Xander get other player info as well!

            var closestWall = new EntityId(0u);
            var closestWallDistanceSq = 0f;
            var closestWallPosition = Position2DExt.Zero();
            var closestTokenSlot = new EntityId(0u);
            var closestTokenSlotDistanceSq = 0f;
            var closestTokenSlotPosition = Position2DExt.Zero();
            var closestPowerSlot = new EntityId(0u);
            var closestPowerSlotDistanceSq = 0f;
            var closestPowerSlotPosition = Position2DExt.Zero();
            float d;
            foreach (var b in entities.BarrierSets)
            {
                d = DistanceSquared(botState.myStart, b.Entity.Position);
                if (closestWall.V == 0 || closestWallDistanceSq > d)
                {
                    closestWall = b.Entity.Id;
                    closestWallDistanceSq = d;
                    closestWallPosition = b.Entity.Position.To2D();
                }
            }
            foreach (var s in entities.TokenSlots)
            {
                if (s.Entity.PlayerEntityId != null)
                    continue;
                d = DistanceSquared(botState.myStart, s.Entity.Position);
                if (closestTokenSlot.V == 0 || closestTokenSlotDistanceSq > d)
                {
                    closestTokenSlot = s.Entity.Id;
                    closestTokenSlotDistanceSq = d;
                    closestTokenSlotPosition = s.Entity.Position.To2D();
                }
            }
            foreach (var s in entities.PowerSlots)
            {
                if (s.Entity.PlayerEntityId != null)
                    continue;
                d = DistanceSquared(botState.myStart, s.Entity.Position);
                if (closestPowerSlot.V == 0 || closestPowerSlotDistanceSq > d)
                {
                    closestPowerSlot = s.Entity.Id;
                    closestPowerSlotDistanceSq = d;
                    closestPowerSlotPosition = s.Entity.Position.To2D();
                }
            }
            botState.tasks.windweaversTasks.closestWall = closestWall;
            botState.tasks.windweaversTasks.closestWallPosition = closestWallPosition;
            botState.tasks.swiftclawTasks.closestTokenSlot = closestTokenSlot;
            botState.tasks.swiftclawTasks.closestTokenSlotPosition = closestTokenSlotPosition;
            botState.tasks.swiftclawTasks.closestPowerSlot = closestPowerSlot;
            botState.tasks.swiftclawTasks.closestPowerSlotPosition = closestPowerSlotPosition;
        }

        /// <summary>
        /// The Tick event happesn every 1/10 second and updates all entities (units, buildings, spells)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public Command[] Tick(GameState state)
        {
            var currentTick = state.CurrentTick.V;
            var entities = state.Entities;
            var myArmy = new List<EntityId>();      // XL04202024 
            var theirArmy = new List<EntityId>();   // XL04202024 Need to make a List of a List when multiple opponents!
            var target = new EntityId(0u);          // XL04202024 

            if (botState.canPlayCardAt == uint.MaxValue)
            {
                foreach (var c in state.Commands)
                {
                    if (c.Player == botState.myId)
                    {
                        if (c.Command.CastSpellGod != null || c.Command.CastSpellGodMulti != null || c.Command.ProduceSquad != null || c.Command.BuildHouse != null)
                        {
                            botState.canPlayCardAt = currentTick + 10;
                            break;
                        }
                    }
                }
            }
            // Player must exist
            var me = Array.Find(state.Players, p => p.Id == botState.myId);
            Debug.Assert(me != null);
            var myPower = me.Power;
            var myOrbs = me.Orbs;
            var wells = entities.PowerSlots;
            List<PowerSlot> myWells = new List<PowerSlot>();
            List<PowerSlot> theirWells = new List<PowerSlot>();
            List<PowerSlot> emptyWells = new List<PowerSlot>();

            foreach (var s in entities.Squads)
            {
                if (s.Entity.PlayerEntityId == botState.myId) // myArmy
                {
                    myArmy.Add(s.Entity.Id);
                }
                else
                {
                    theirArmy.Add(s.Entity.Id);
                }
            }
            foreach (var s in entities.TokenSlots)
            {
                if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                {
                    target = s.Entity.Id;
                }
            }
            foreach (var s in entities.PowerSlots)
            {
                if (s.Entity.PlayerEntityId == null) // Empty well
                {
                    emptyWells.Add(s); 
                }
                else if (s.Entity.PlayerEntityId == botState.myId) // My well
                {
                    myWells.Add(s);
                }
                else if (s.Entity.PlayerEntityId == target) // Their well
                {
                    theirWells.Add(s);
                }
            }

            Console.WriteLine($"Tick:{currentTick} opp:{target} pow:{(int)myPower} size:{myArmy.Count} oSize:{theirArmy.Count} oPow:{theirArmy}");

            var swiftclawTasks = SwiftclawTasks(currentTick, myPower, entities);
            if (swiftclawTasks != null)
            {
                return new[] { swiftclawTasks };
            } else // TODO other tasks
            {
                return Array.Empty<Command>();
            }

            #region XanderLord new tasks
            var orbTasks = OrbTasks(currentTick, myPower, entities);
            if (orbTasks != null)
            {
                return new[] { orbTasks };
            }
            else // TODO other tasks
            {
                return Array.Empty<Command>();
            }

            var wellTasks = WellTasks(currentTick, myPower, entities);
            if (wellTasks != null)
            {
                return new[] { wellTasks };
            }
            else // TODO other tasks
            {
                return Array.Empty<Command>();
            }
            #endregion XanderLord new tasks
        }

        #region XanderLord Methods
        // If a well is next to an orb to own it, then get it
        private Command? OrbTasks(uint tick, float myPower, MapEntities entities)
        {
            return null;
        }

        // If another empty well or orb is next to an well to own it, then get it (as long as we have power and do not exceed 4 orbs)
        private Command? WellTasks(uint tick, float myPower, MapEntities entities)
        {
            return null;
        }
        #endregion XanderLord Methods

        private Command? SwiftclawTasks(uint tick, float myPower, MapEntities entities)
        {
            var tasks = botState.tasks.swiftclawTasks;
            if (tasks.done)
                return null;

            if (!tasks.produceSwiftclaw && myPower >= 70 && botState.canPlayCardAt < tick)
            {
                botState.canPlayCardAt = uint.MaxValue;
                Console.WriteLine("produce swiftclaw");
                tasks.produceSwiftclaw = true;
                return new CommandProduceSquad
                {
                    CardPosition = 0,
                    Xy = botState.myStart
                };
            } else if (tasks.produceSwiftclaw && tasks.swiftclaw.V == 0)
            {
                var SEARCH_ID = CardIdCreator.New(CardTemplate.Swiftclaw, Upgrade.U3);
                var swiftclaw = Array.Find(entities.Squads, s => s.CardId == SEARCH_ID);
                if (swiftclaw != null)
                {
                    tasks.swiftclaw = swiftclaw.Entity.Id;
                    Console.WriteLine($"Swiftclaw = {swiftclaw.Entity.Id}");
                    // We found him, but give him a task on next tick
                }
                return null;
            } else if (tasks.produceSwiftclaw && tasks.swiftclaw.V > 0)
            {
                var swiftclaw = entities.Squads.FirstOrDefault(s => s.Entity.Id == tasks.swiftclaw);
                if (swiftclaw == null)
                {
                    Console.WriteLine("Swiftclaw was killed");
                    // Swiftclaw was killed, give up on this task set
                    tasks.done = true;
                    return null;
                } else
                {
                    var pos = swiftclaw.Entity.Position;
                    if (!tasks.buildPrimalDefender && myPower >= 60 && botState.canPlayCardAt < tick)
                    {
                        botState.canPlayCardAt = uint.MaxValue;
                        tasks.buildPrimalDefender = true;
                        Console.WriteLine("build Primal Defender");
                        return new CommandBuildHouse{
                            CardPosition = 4,
                            Xy = pos.To2D(),
                            Angle = float.Pi / 2
                        };
                    } else if (!tasks.buildTokenSlot
                        && MathF.Sqrt(DistanceSquared(tasks.closestTokenSlotPosition, pos)) < 15)
                    {
                        if (myPower >= 150)
                        {
                            Console.WriteLine("Token slot build");
                            tasks.buildTokenSlot = true;
                            return new CommandTokenSlotBuild
                            {
                                SlotId = tasks.closestTokenSlot,
                                Color = CreateOrbColor.Shadow
                            };
                        } else if (!tasks.holdPosition)
                        {
                            tasks.holdPosition = true;
                            Console.WriteLine("Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclaw }
                            };
                        } else
                        {
                            // Waiting for power
                            return null;
                        }
                    }
                    else if (!tasks.buildPowerSlot
                        && MathF.Sqrt(DistanceSquared(tasks.closestPowerSlotPosition, pos)) < 15)
                    {
                        if (myPower >= 100)
                        {
                            Console.WriteLine("Power slot build");
                            tasks.buildPowerSlot = true;
                            return new CommandPowerSlotBuild
                            {
                                SlotId = tasks.closestPowerSlot
                            };
                        }
                        else if (!tasks.holdPosition)
                        {
                            tasks.holdPosition = true;
                            Console.WriteLine("Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclaw }
                            };
                        }
                        else
                        {
                            // Waiting for power
                            return null;
                        }
                    } else if (tasks.buildPrimalDefender && tasks.buildTokenSlot && tasks.buildPowerSlot)
                    {
                        if (!tasks.holdPosition)
                        {
                            tasks.holdPosition = true;
                            Console.WriteLine("Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclaw }
                            };
                        } else if (!tasks.changeMode)
                        {
                            tasks.changeMode = true;
                            Console.WriteLine("Swiftclaw change mode");
                            return new CommandModeChange
                            {
                                EntityId = tasks.swiftclaw,
                                NewModeId = new ModeId(3000497)
                            };
                        } else
                        {
                            Console.WriteLine("Swiftclaw done");
                            tasks.done = true;
                            return null;
                        }
                    } else
                    {
                        var figure = entities.Figures.FirstOrDefault(f => f.Entity.Id == swiftclaw.Figures[0]);
                        Debug.Assert(figure != null);
                        if (figure.CurrentSpeed > 0)
                        {
                            // already moving
                            return null;
                        } else
                        {
                            Console.WriteLine("Swiftclaw Patrol");
                            return new CommandGroupGoto
                            {
                                Squads = new[] { tasks.swiftclaw },
                                Positions = new[]
                                {
                                    tasks.closestTokenSlotPosition,
                                    tasks.closestPowerSlotPosition,
                                    botState.myStart
                                },
                                WalkMode = WalkMode.Patrol,
                                Orientation = 0
                            };
                        }
                    }
                }
            } else
            {
                return null;
            }
        }

        private static float DistanceSquared(Position2D from, Position to)
        {
            return (from.X - to.X) * (from.X - to.X) + (from.Y - to.Z) * (from.Y - to.Z);
        }

        private static readonly Deck TAINTED_FLORA_WITH_DEFENDER = new()
        {
            Name = "TaintedFlora",
            CoverCardIndex = 0,
            Cards = new CardId[20]
            {
                CardIdCreator.New(CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(CardTemplate.PrimalDefender, Upgrade.U3),
                CardIdCreator.New(CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AmiiPaladins, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AmiiPhantom, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(CardTemplate.ShadowPhoenix, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Tranquility, Upgrade.U3),
                CardIdCreator.New(CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(CardTemplate.CultistMaster, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AshbonePyro, Upgrade.U3),
            }
        };

        private static readonly Deck BG1_DECK = new()
        {
            Name = "BG1",
            CoverCardIndex = 0,
            Cards = new CardId[20]
            {
                CardIdCreator.New(CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Spearmen, Upgrade.U3),
                CardIdCreator.New(CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AmiiPaladins, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AmiiPhantom, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(CardTemplate.ShadowPhoenix, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Tranquility, Upgrade.U3),
                CardIdCreator.New(CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(CardTemplate.CultistMaster, Upgrade.U3),
                CardIdCreator.New(CardTemplate.AshbonePyro, Upgrade.U3),
            }
        };

        private struct BotState
        {
            public byte myTeam;
            public required List<EntityId> oponents;
            public required List<EntityId> team;
            public required Position2D myStart;
            public UInt32 canPlayCardAt;
            public required EntityId myId;
            public required Tasks tasks;
            public required Deck selectedDeck;
        }

        private class Tasks
        {
            public required SwiftclawTasks swiftclawTasks;
            public required WindweaversTasks windweaversTasks;
            public required BurrowerTasks burrowerTasks;

            public required OrbTasks orbTasks;
            public required WellTasks wellTasks;

            public class OrbTasks
            {
                public bool getNearbyOrb;
                public bool getNearbyWell;
                public bool heal;
                public bool done;
            }

            public class WellTasks
            {
                public bool getNearbyOrb;
                public bool getNearbyWell;
                public bool heal;
                public bool done;
            }

            public bool castHurricane;
            public bool castHeal;
            public bool repairWall;
            public bool cancelRepairWall;
            public bool repairBuilding;
            public bool cancelRepairBuilding;

            public bool surrender;
            public class BurrowerTasks
            {
                public bool spawnBurrower;
                public required EntityId burrower;
                public bool getEnemyOffTheWall;

                public bool done;
            }

            public class WindweaversTasks
            {
                public required Position2D closestWallPosition;
                public required EntityId closestWall;
                public bool spawnWindweavers;
                public required EntityId windweavers;
                public bool buildBarrier;
                public bool enterWall;
                public required EntityId exitWallId;
                public UInt32 exitWallAt;
                public bool exitWall;
                public bool spawnWindweaversOnBarrier;
                public bool openGate;
                public UInt32 closeGateAt;
                public required EntityId closeGateId;
                public bool closeGate;

                public bool done;
            }

            public class SwiftclawTasks
            {
                public required EntityId closestTokenSlot;
                public required Position2D closestTokenSlotPosition;
                public required EntityId closestPowerSlot;
                public required Position2D closestPowerSlotPosition;
                public required EntityId swiftclaw;

                public bool produceSwiftclaw;
                public bool buildPrimalDefender;
                public bool buildTokenSlot;
                public bool buildPowerSlot;
                public bool holdPosition;
                public bool changeMode;

                public bool done;
            }
        }

        #region New Commands


        /*
        Notes from UltraLord:
        ATM:
            Spam units in deck slot 1 and move to Opponent (like the demo example)
            Erupt a well while it is build (5ms time window)
            Erupt when 3 Units are close (and one is <300 HP)
            Scan all units on the map (or an area) and find the best counter (based on HP)
            Find closest well und build it

        XanderLord Notes:
        // MapEntities is key to know where everything is!
        // Basic Strategies are attack opponent or defend base
        // Select best cards to attack or defend
        // Try going to all non occupied wells wells and turning them on
        // Try going to nearest 3 orbs (after main orb)
        // Build an army of units based on a certain size (power cost)
        */
        public void FindNearestOrb(bool onlyEmpyOrbs = true)
        {

        }

        public void FindNearestWell(bool onlyEmpyOrbs = true)
        {

        }

        // When in attack mode find the nearest enemy relative to the player
        public void FindNearestEnemy()
        {

        }

        #endregion New Commands
    }
}
