using Api;
using Bots;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SkylordsRebornAPI;
using SkylordsRebornAPI.Cardbase;
using static Bots.AIXanderBot;
using System.Runtime.InteropServices.Marshalling;
using System.Collections.Generic;
using System.IO;
using SkylordsRebornAPI.Auction;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bots
{
    // /AI: list

    // /AI: add XanderAI TaintedFlora 4
    // /AI: add XanderAI FireNature 4
    // /AI: add XanderAI TopDeck 4
    // Basic Strategies:

    /* // What UltraLord Does
     ##########################################################
     # Where do i get the json
     # 0 offline file
     # 1 online file
     SMJOnline=0
     
     ##########################################################
     # What function should be running
     WellKiller=1
     UnitEruption=1
     AvoidArea=1
     BattleTable=1
     ##########################################################
     # What start type should be used
     # 0:build 2 wells			Then 4
     # 1:wait for OP				Then 0
     # 2:Run to othere base
     # 3:Spawn Card on Slot 1	Then 4
     # 4:BattleMode
     StartType=1    
     ##########################################################
     */
    public class AIXanderBot : IAspWrapperImpl
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

        public enum OrbCost
        {
            Orb1 = 100,
            Orb2 = 150,
            Orb3 = 250,
            Orb4 = 300,
            Orb5 = 300,
            Orb6 = 300,
            Orb7 = 300,
            Orb8 = 300,
            Orb9 = 300,
            Orb10 = 300,
        };

        public enum Strategy
        {
            Attack = 0,
            Defend = 1,
            Build = 2, // Build wells first, then orbs
            BuildWells = 3,
            BuildOrbs = 4,
            HealOrbs = 5,
            HealWells = 6,
            HealStructure = 7,
            HoldPosition = 8,
        };

        public Dictionary<int, string> WhyCanNotPlayCardThere = new Dictionary<int, string>
        {
            { 0x10, "DoesNotHaveEnoughPower" },
            { 0x20, "InvalidPosition" }, // too close to (0,y), or (x,0)
            { 0x80, "CardCondition" },
            { 0x100, "ConditionPreventCardPlay" },
            { 0x200, "DoesNotHaveThatCard" },
            { 0x400,"DoesNotHaveEnoughOrbs" },
            { 0x10000,"CastingTooOften" },
        };

        #region SMJCards JSON info

        public readonly string[] UnitModel =
        { "Non-Units", "Amazon", "Atrocity", "Balrog", "Behemoth", "Bird", "Bow", "Canine", "Claw", "Dancer", "Dragon", "Fighter", "Floater", "Guardian", "Head", "Insect", "Mage", "Minion", "Quadraped", "Raptor", "Rider", "Rifle", "Roughneck", "Ruffian", "Serpent", "Ship", "Skyelf", "Spear", "Titan", "Wagon", "Worm", "Unique" };

        //public string category { get; set; }
        public readonly string[] UnitSpecies = { "Non-Units", "Amii", "Ancient", "Artifact", "Beast", "Demon", "Dragonkin", "Elemental", "Elf", "Forestkin", "Giant", "Human", "Kobold", "Ogre", "Orc", "Primordial", "Spirit", "Undead", "Special" };
        public readonly string[] UnitClass = { "Non-Units", "Archer", "Commander", "Corrupter", "Crusader", "Destroyer", "Dominatior", "Gladiatore", "Marauder", "Soldier", "Supporter", "Wizard", "Special" };
        public readonly string[] BuildingClass = { "Non-Buildings", "Artillary", "Barrier", "Device", "Fortress", "Hut", "Shrine", "Statue", "Tower" };
        public readonly string[] SpellClass = { "Non-Spells", "Arcane", "Enchantment", "Spell" };
        //public int gender { get; set; } // "Non-Units", "Unspecified", "Male", "Female"
        //public int movementType { get; set; } // "Non-Units", "Ground", "Flying"
        //public int attackType { get; set; } // "Non-Units", "Melee", "Ranged"
        //public int offenseType { get; set; } // "Non-Units", "Small", "Medium", "Large", "Extra Large", "Special"
        //public int defenseType { get; set; } // "Non-Units", "Small", "Medium", "Large", "Extra Large"
        //public int maxCharges { get; set; } // 4, 8, 12, 16, 20, 24
        //public int squadSize { get; set; } // 1, 2, 4, 6
        //public int starterCard { get; set; } // Non-Starter, Starter
        //public List<int> powerCost { get; set; }
        //public List<int> damage { get; set; }
        //public List<int> health { get; set; }
        //public List<int> boosters { get; set; } // None, "Mini, General, "Fire, Shadow, "Nature, Frost, "Bandits, Stonekin, "Twilight, Lost Souls, "Amii, Fire/Frost
        public readonly string[] UpgradeMaps = { "None", "Encounter With Twilight", "Siege of Hope", "Defending Hope", "The Soultree", "The Treasure Fleet", "Behind Enemy Lines", "Mo", "Ocean", "Oracle", "Crusade", "Sunbridge", "Nightmare Shard", "Nightmare's End", "The Insane God", "Slave master", "Convoy", "Bad Harvest", "King of the Giants", "Titan", "The Dwarven Riddle", "The guns of Lyr", "Blight", "Raven's End", "Empire" };

        #endregion SMJCards JSON info

        string botVersion = "0.0.0.6";

        string previousTargetMessage = string.Empty;

        Deck? currentDeck = null;

        int unitsNeededBeforeAttack = 1;
        int maxAttackUnits = 2;
        int defaultSquadSize = 2;
        int defaultTickUpdateRate = 1;
        int defaultAttackSquads = 4; // For now do not build more than X attack units
        int defaultDefendSquads = 2; // For now do not build more than X defend units
        int healOrbs = 3;
        bool buildNearbyWallAtStart = true;
        bool buildArcherOnWallAtStart = true;
        bool archerOnWallAtStartBuilt = false;
        EntityId archerOnWallAtStartBuiltId = new EntityId(0u);

        List<Strategy> strategyList = new List<Strategy>(); // This dictates what to build when in the process

        List<Entity> previousTargets = new List<Entity>(); // List of all previous targets
        List<string> previousTargetTypes = new List<string>(); // "Orb", "Well", "Squad", etc.

        List<RejectedCommand> myRejectedCommands = new List<RejectedCommand>();

        List<TokenSlot> myOrbs = new List<TokenSlot>(); // Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<PowerSlot> myWells = new List<PowerSlot>(); // Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<BarrierSet> myWalls = new List<BarrierSet>(); // Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<BarrierModule> myWallModules = new List<BarrierModule>(); // Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<Squad> mySquads = new List<Squad>(); // Array.FindAll(state.Entities.Squads, x => x.Entity.Id == botState.myId).ToList();
        List<EntityId> myUnits = new List<EntityId>();

        List<RejectedCommand> enemyRejectedCommands = new List<RejectedCommand>();

        List<TokenSlot> enemyOrbs = new List<TokenSlot>(); // Array.FindAll(state.Entities.TokenSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<PowerSlot> enemyWells = new List<PowerSlot>(); // Array.FindAll(state.Entities.PowerSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<BarrierModule> enemyWallModules = new List<BarrierModule>(); // Array.FindAll(state.Entities.BarrierSets, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<BarrierSet> enemyWalls = new List<BarrierSet>(); // Array.FindAll(state.Entities.BarrierModules, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<Squad> enemySquads = new List<Squad>(); // Array.FindAll(state.Entities.Squads, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<EntityId> enemyUnits = new List<EntityId>();

        List<TokenSlot> emptyOrbs = new List<TokenSlot>(); // Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == null).ToList();
        List<PowerSlot> emptyWells = new List<PowerSlot>(); // Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == null).ToList();
        List<BarrierSet> emptyWalls = new List<BarrierSet>(); // Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == null).ToList();
        List<BarrierModule> emptyWallModules = new List<BarrierModule>(); // Array.FindAll(state.Entities.BarrierSets, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();

        DeckOfficialCardIds? myCurrentDeckOfficialCardIds = null; // myDeckOfficialCardIds.FirstOrDefault(d => d.Name == botState.selectedDeck.Name) ?? myDeckOfficialCardIds[0];
        List<int>? archerCardPositions = null; // GetArcherCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which Cards in Deck are archers
        List<int>? towerCardPositions = null; // GetTowerCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1);
        List<int>? spellCardPositions = null; // GetSpellCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1);
        int wallBreakerCardPosition = -1; // Does deck have special wallbreaker card? 

        float nearestBarrierDistance = float.MaxValue; // Is there a wall by my first orb?
        float buildWallCost = 50f; // NGE05012024!!!!!! How can I get the cost to build the wall?

        EntityId closestWall = new EntityId(0u);
        float closestWallDistanceSq = 0f;
        Position2D closestWallPosition = Position2DExt.Zero();
        EntityId closestTokenSlot = new EntityId(0u);
        float closestTokenSlotDistanceSq = 0f;
        Position2D closestTokenSlotPosition = Position2DExt.Zero();
        EntityId closestPowerSlot = new EntityId(0u);
        float closestPowerSlotDistanceSq = 0f;
        Position2D closestPowerSlotPosition = Position2DExt.Zero();

        int previousArmyCount = 0;
        double previousSquadHealth = 0;
        Position2D previousAttackPos = Position2DExt.Zero();
        int previousSquadCount = 0;

        /// <summary>
        /// // This dictates what to build when in the process during the PvP battle
        /// </summary>
        /// <param name="strategySet"></param>
        private void SetStrategy(List<Strategy> strategySet)
        {
            if (strategyList == null || strategyList.Count == 0)
            {
                // Create the defualt strategy
                strategyList = new List<Strategy>() { Strategy.Attack, Strategy.Defend, Strategy.HealWells, Strategy.HealOrbs,
                                                      Strategy.HealStructure, Strategy.BuildWells, Strategy.BuildOrbs };
            }
            else
            {
                strategyList = strategySet;
            }
        }

        string previousMessageAttack = "";
        private Command? Attack(EntityId target, List<Squad> squads, Position2D pos, int unitsNeededBeforeAttack)
        {
            int squadSize = 0;
            if (squads.Count > 0)
            {
                Squad squad = squads[0];
                squadSize = squad.Figures.Length;
                if (squads.Count < unitsNeededBeforeAttack)
                {
                    Console.WriteLine("Army must have {0} squads to attack. Army has {1} squads", unitsNeededBeforeAttack, squads.Count);
                    return null;
                }
            }
            if (target.V == 0)
            {
                Console.WriteLine("Attack none since target at 0");
                return null;
            }
            else
            {
                List<EntityId> squadsIds = squads.Select(s => s.Entity.Id).ToList();
                string message = string.Format("Attack target:{0} at pos X,Y:{1},{2} with {3} squads", target.V, (int)pos.X, (int)pos.Y, squads.Count);
                if (message != previousMessageAttack)
                {
                    previousMessageAttack = message;
                    Console.WriteLine(message);
                }
                previousAttackPos = pos;
                previousSquadCount = squads.Count;
                return new CommandGroupAttack { Squads = squadsIds.ToArray(), TargetEntityId = target, ForceAttack = true }; // NGE04292024 ForceAttack = false
                //if (!((int)previousAttackPos.X == (int)pos.X && (int)previousAttackPos.Y == (int)pos.Y && previousSquadCount == squads.Count))
                //{
                //}
            }
        }

        string previousMessageSpawnUnit = "";
        private Command? SpawnUnit(List<Squad> myArmy, float myPower, Position2D pos, byte cardPosition, int unitPower, uint tick, ref float powerRemaining)
        {
            if (myPower >= unitPower && botState.canPlayCardAt < tick)
            {
                // botState.canPlayCardAt = uint.MaxValue;
                string message = string.Format("CommandProduceSquad CardPosition:{0} X,Y:{1},{2}", cardPosition, (int)pos.X, (int)pos.Y);
                if (message != previousMessageSpawnUnit)
                {
                    ConsoleWriteLine(true, message);
                    powerRemaining = myPower - unitPower;

                    return new CommandProduceSquad { CardPosition = cardPosition, Xy = pos };
                }
                else
                {
                    return null;
                }
            }
            else if (myPower < unitPower)
            {
                return null;
            }
            else
            {
                Console.WriteLine("CommandProduceSquad Failed");
                return null;
            }
        }

        private Command? SpawnUnitOnBarrier(float myPower, Position2D pos, byte cardPosition, EntityId barrierModuleId, int unitPower, uint tick, ref float powerRemaining)
        {
            if (myPower >= unitPower && botState.canPlayCardAt < tick)
            {
                // botState.canPlayCardAt = uint.MaxValue;
                string message = string.Format("CommandProduceSquadOnBarrier CardPosition:{0} X,Y:{1},{2}", cardPosition, (int)pos.X, (int)pos.Y);
                ConsoleWriteLine(true, message);
                powerRemaining = myPower - unitPower;

                return new CommandProduceSquadOnBarrier { CardPosition = cardPosition, Xy = pos, BarrierToMount = barrierModuleId };
            }
            else if (myPower < unitPower)
            {
                return null;
            }
            else
            {
                Console.WriteLine("CommandProduceSquadOnBarrier Failed");
                return null;
            }
        }

        private BotState botState;

        #region Info to track

        SMJCard[]? cardsSMJ; // Instances.CardService.GetSMJCardList();

        private Dictionary<EntityId, Orb> orbs = new Dictionary<EntityId, Orb>();
        private Dictionary<EntityId, Well> wells = new Dictionary<EntityId, Well>();

        #endregion Info to track

        public Deck[] DecksForMap(Maps map, string? name, ulong crc)
        {
            if (SUPPORTED_MAPS.Contains(map))
            {
                return new[] {
                    TopDeck, BattleGrounds, FireNature,
                    PvPCardDecks.TaintedDarkness, PvPCardDecks.GiftedDarkness, PvPCardDecks.BlessedDarkness, PvPCardDecks.InfusedDarkness,
                    PvPCardDecks.TaintedFlora, PvPCardDecks.GiftedFlora, PvPCardDecks.BlessedFlora, PvPCardDecks.InfusedFlora,
                    PvPCardDecks.TaintedIce, PvPCardDecks.GiftedIce, PvPCardDecks.BlessedIce, PvPCardDecks.InfusedIce,
                    PvPCardDecks.TaintedFlame, PvPCardDecks.GiftedFlame, PvPCardDecks.BlessedFlame, PvPCardDecks.InfusedFlame,
                 };
            }
            else
            {
                return Array.Empty<Deck>();
            }
        }

        private Dictionary<string, List<Task>> dictDeckTasks = new Dictionary<string, List<Task>>();
        private Tasks? currentDeckTasks = null;

        // TopDeck, TaintedFlora, BattleGrounds, FireNature
        private void SetUpDeckTasks()
        {
            List<Task> TaintedFloraTasks = new List<Task>();
            Tasks? taskTaintedFlora = GetTasksBasedOnDeck(PvPCardDecks.TaintedFlame);
            if (taskTaintedFlora != null)
            {
                dictDeckTasks.Add("TaintedFloraTasks", TaintedFloraTasks);
            }

            // List<Task> FireNature = new List<Task>();

        }

        private Tasks? GetTasksBasedOnDeck(Deck deck)
        {
            Tasks? tasks = null;
            // Get list of all deck names
            var deckNames = myDecks.Select(item => item.Name).ToList();
            switch (deck.Name)
            {
                case "TopDeck":
                    break;
                case "TaintedFlora":
                    tasks = new Tasks
                    {
                        burrowerTasks = new Tasks.BurrowerTasks
                        {
                            burrower = new EntityId(0),
                            spawnBurrower = false,
                            getEnemyOffTheWall = false,
                            done = false,
                        },
                        swiftclawTasks = new Tasks.SwiftclawTasks
                        {
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
                        windweaversTasks = new Tasks.WindweaversTasks
                        {
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
                        /*
                        orbTasks = new Tasks.OrbTasks
                        {
                            buildNearbyOrb = true,
                            buildNearbyWell = true,
                            heal = false,
                            done = false
                        },
                        wellTasks = new Tasks.WellTasks
                        {
                            buildNearbyOrb = true,
                            buildNearbyWell = true,
                            heal = false,
                            done = false
                        },
                        */
                        cancelRepairBuilding = false,
                        cancelRepairWall = false,
                        castHeal = false,
                        castHurricane = false,
                        repairBuilding = false,
                        repairWall = false,
                        surrender = false,
                    };
                    break;
                case "BattleGrounds":
                    break;
                case "FireNature":
                    break;
            }
            return tasks;
        }

        #region Main 3 methods for the Bot! PrepareForBattle, MatchStart and Tick
        public void PrepareForBattle(Maps map, string? name, ulong crc, Deck deck)
        {
            cardsSMJ = Instances.CardService.GetSMJCardList(); // Used to determine which cards have what features
            Tasks? tasks = GetTasksBasedOnDeck(deck);
            botState = new BotState()
            {
                selectedDeck = deck,
                myStart = Position2DExt.Zero(),
                oponents = new List<EntityId>(),
                team = new List<EntityId>(),
                //tasks,
                canPlayCardAt = 0,
                myId = new EntityId(0),
                myTeam = 0,
                isGameStart = true,
            };
        }

        public void MatchStart(GameStartState state)
        {
            var yourPlayerId = state.YourPlayerId;
            var entities = state.Entities;
            botState.myId = yourPlayerId;
            Console.WriteLine($"My PlayerID:{yourPlayerId.V} Deck:{botState.selectedDeck.Name}");
#pragma warning disable CS8602 // Player must exist
            botState.myTeam = Array.Find(state.Players, p => p.Entity.Id == yourPlayerId).Entity.Team;
#pragma warning restore CS8602
            botState.oponents = new List<EntityId>();
            foreach (var player in state.Players)
            {
                if (player.Entity.Team != botState.myTeam)
                {
                    botState.oponents.Add(player.Entity.Id);
                }
            }
            foreach (var s in entities.TokenSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    Console.WriteLine($"Orb:{s.Entity.Id.V} Pos:{(int)s.Entity.Position.X},{(int)s.Entity.Position.Z}");
                    botState.myStart = s.Entity.Position.To2D();
                }
            }
            foreach (var s in entities.PowerSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    Console.WriteLine($"Well:{s.Entity.Id.V} Pos:{(int)s.Entity.Position.X},{(int)s.Entity.Position.Z}");
                }
            }

            // Get the deck cardIds from the matching currentDeck.Name
            myCurrentDeckOfficialCardIds = myDeckOfficialCardIds.FirstOrDefault(d => d.Name == botState.selectedDeck.Name) ?? myDeckOfficialCardIds[0];
            archerCardPositions = GetArcherCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which Cards in Deck are archers            
            towerCardPositions = GetTowerCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which cards are defense Towers example Stranglehold, Primeval Defender
            spellCardPositions = GetTowerCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which cards are defense Spess example Mine, Wallbreaker

            for (int i = 0; i < myCurrentDeckOfficialCardIds.Ids.Length; i++)
            {
                if (myCurrentDeckOfficialCardIds.Ids[i] == (int)Api.CardTemplate.Wallbreaker)
                {
                    wallBreakerCardPosition = i;
                    ConsoleWriteLine(true, $"Deck has Wallbreaker spell at pos:{i}");
                    break;
                }
            }

            float d;
            foreach (var b in entities.BarrierSets)
            {
                d = DistanceSquared(botState.myStart, b.Entity.Position);
                if (closestWall.V == 0 || closestWallDistanceSq > d) // Closest empty wall 
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
                if (closestTokenSlot.V == 0 || closestTokenSlotDistanceSq > d) // Closest empty orb
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
                if (closestPowerSlot.V == 0 || closestPowerSlotDistanceSq > d) // Closest empty well
                {
                    closestPowerSlot = s.Entity.Id;
                    closestPowerSlotDistanceSq = d;
                    closestPowerSlotPosition = s.Entity.Position.To2D();
                }
            }
        }

        public Command[] Tick(GameState state)
        {
            List<Command> commands = new List<Command>();
            string deckName = "TopDeck";
            currentDeck = myDecks.FirstOrDefault(d => d.Name == deckName);
            if (currentDeck != null)
            {
                return SendCommands(state, (Deck)currentDeck);
            }
            return commands.ToArray();
        }

        #endregion Main 3 methods for the Bot! PrepareForBattle, MatchStart and Tick

        public void MatchStartOrig(GameStartState state)
        {
            // Get all players and their positions
            // Get all orbs (Tokens) and their positions
            // Get all wells (PowerSlots) and their positions

            var yourPlayerId = state.YourPlayerId;
            var entities = state.Entities;
            botState.myId = yourPlayerId;
            ConsoleWriteLine(true, $"My player ID is: {yourPlayerId}, I have deck: {botState.selectedDeck.Name}, and I own:");
#pragma warning disable CS8602 // Player must exist
            botState.myTeam = Array.Find(state.Players, p => p.Entity.Id == yourPlayerId).Entity.Team;
#pragma warning restore CS8602
            botState.oponents = new List<EntityId>();
            foreach (var player in state.Players)
            {
                if (player.Entity.Team != botState.myTeam)
                {
                    botState.oponents.Add(player.Entity.Id);

                    // opponents.Players.Add(player.Entity.Id, new Player(player.Entity.Id)); // Add all player to team
                }
                else
                {
                    botState.team.Add(player.Entity.Id);
                    //team.Players.Add(player.Entity.Id, new Player(player.Entity.Id)); // Add all opponents to team
                }
                // break;
            }
            foreach (var s in entities.PowerSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    ConsoleWriteLine(true, $"Well slot:{s.Entity.Id} at {(int)s.Entity.Position.X},{(int)s.Entity.Position.Z}");
                    botState.myStart = s.Entity.Position.To2D();
                }
                //if (s.Entity.PlayerEntityId != null)
                //{
                //    if (team.Players.ContainsKey(s.Entity.PlayerEntityId) == true)
                //    {
                //        team.Players[s.Entity.PlayerEntityId].StartPos = s.Entity.Position.To2D(); // Add player start pos
                //    }
                //    if (opponents.Players.ContainsKey(s.Entity.PlayerEntityId) == true)
                //    {
                //        opponents.Players[s.Entity.PlayerEntityId].StartPos = s.Entity.Position.To2D(); // Add player start pos
                //    }
                //}
            }
            foreach (var s in entities.TokenSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    Console.WriteLine($"Orb slot:{s.Entity.Id} at {s.Entity.Position.X},{s.Entity.Position.Z}");
                }
            }

            #region Xander get other player info as well!
            /*
            if (team != null)
            {
                List<EntityId> keyList = new List<EntityId>(team.Players.Keys);
                foreach (EntityId p in keyList)
                {
                    ConsoleWriteLine(true, $"Team: {p} at {(int)team.Players[p].StartPos.X}:{(int)team.Players[p].StartPos.Y}");
                }
            }
            if (opponents != null)
            {
                List<EntityId> keyList = new List<EntityId>(opponents.Players.Keys);
                foreach (EntityId p in keyList)
                {
                    ConsoleWriteLine(true, $"Opponent: {p} at {(int)opponents.Players[p].StartPos.X}:{(int)opponents.Players[p].StartPos.Y}");
                }
            }
            */
            #endregion Xander get other player info as well!

            /*
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
            */
        }

        public double GetUnitInSquadHealth(GameState state, Squad squad, EntityId unitId)
        {
            // Get health of units
            double health = 0;
            if (squad.Entity.PlayerEntityId != botState.myId)
                return health;
            var heath = 0.0;
            var figure = state.Entities.Figures.FirstOrDefault(f => f.Entity.Id == unitId);
            if (figure != null)
            {
                var healthAspect = figure.Entity.Aspects.First(a => a.Health != null);
                if (healthAspect.Health != null)
                {
                    health += healthAspect.Health.CurrentHp;
                    Console.WriteLine($"Added squad player");
                }
            }
            Console.WriteLine($"Unit:{unitId} has {heath}hp");

            return health;
        }

        public void GetSquadHealth(GameState state, Squad squad, out double health, out List<EntityId> unitsHealthy)
        {
            unitsHealthy = new List<EntityId>();
            health = 0; // Get health of units

            if (squad.Entity.PlayerEntityId != botState.myId)
                return;

            foreach (var figureId in squad.Figures)
            {
                var figure = state.Entities.Figures.FirstOrDefault(f => f.Entity.Id == figureId);
                if (figure != null)
                {
                    var healthAspect = figure.Entity.Aspects.First(a => a.Health != null);
                    if (healthAspect.Health != null)
                    {
                        health += healthAspect.Health.CurrentHp;
                        unitsHealthy.Add(figureId);
                        // Console.WriteLine($"Added squad player");
                    }
                }
            }
            // Console.WriteLine($"My squad: {squad.Entity.Id} has {heath} hp");
        }

        public bool BreachBarrierSetModules(GameState state, List<BarrierSet> barriers, List<BarrierModule> modules)
        {
            bool breachBarrierModule = false;

            // If gate is down or a barrier module has 0 health, then I can skip attacking the barrier
            // NOTE: For now just use the first barrier!!!!!!
            if (barriers.Count() > 0)
            {
                BarrierSet barrier = barriers[0];
                foreach (var module in modules)
                {
                    if (module.Set.V == barrier.Entity.Id.V) // Part of the same Barrier
                    {
                        var moduleAspectHealth = module.Entity.Aspects.FirstOrDefault(h => h.Health != null) ?? null;
                        if (moduleAspectHealth == null) // This should mean a module is fully broken so I can attack something else!
                        {
                            return true;
                        }
                        var gateAspect = module.Entity.Aspects.FirstOrDefault(g => g.BarrierGate != null) ?? null;
                        if (gateAspect != null) // Has a gate.  Now need to find a way to determine its state open/closed
                        {
                            if (gateAspect.BarrierGate.Open == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return breachBarrierModule;
        }

        public Command[] TickGood(GameState state) // NGE04292024 
        {
            var currentTick = state.CurrentTick;
            var entities = state.Entities;
            var myArmy = new List<EntityId>();
            var myArmySquads = new List<Squad>();
            var myOrbs = new List<EntityId>();
            var myWells = new List<EntityId>();
            var myBarrierModules = new List<EntityId>();
            var enemyOrbs = new List<EntityId>();
            var enemyWells = new List<EntityId>();
            var enemyBarrierModules = new List<EntityId>();
            var target = new EntityId(0u);
#pragma warning disable CS8602 // Player must exist
            var myPower = Array.Find(state.Players, p => p.Id == botState.myId).Power;
#pragma warning restore CS8602
            // Get health of units
            foreach (var squad in state.Entities.Squads)
            {
                double squadHealth = 0;
                if (squad.Entity.PlayerEntityId != botState.myId)
                    continue;
                GetSquadHealth(state, squad, out squadHealth, out List<EntityId> healthyUnits);
                if (squadHealth != 0)
                {
                    myArmy.AddRange(healthyUnits);
                    myArmySquads.Add(squad);
                }
                if (squadHealth != previousSquadHealth)
                {
                    Debug.WriteLine($"My squad:{squad.Entity.Id} has hp:{squadHealth}");
                }

                previousSquadHealth = squadHealth;

                //Console.WriteLine($"My squad:{squad.Entity.Id} has hp:{squadHealth}");
            }

            foreach (var s in entities.TokenSlots) // Orbs
            {
                if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                {
                    target = s.Entity.Id;
                    enemyOrbs.Add(s.Entity.Id);
                }
                else if (s.Entity.PlayerEntityId != null && s.Entity.PlayerEntityId == botState.myId)
                {
                    myOrbs.Add(s.Entity.Id);
                }
            }
            foreach (var s in entities.PowerSlots) // Wells
            {
                if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                {
                    enemyWells.Add(s.Entity.Id);
                }
                else if (s.Entity.PlayerEntityId != null && s.Entity.PlayerEntityId == botState.myId)
                {
                    myWells.Add(s.Entity.Id);
                }
            }
            foreach (var s in entities.BarrierModules) // Walls
            {
                if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                {
                    enemyBarrierModules.Add(s.Entity.Id);
                }
                else if (s.Entity.PlayerEntityId != null && s.Entity.PlayerEntityId == botState.myId)
                {
                    myBarrierModules.Add(s.Entity.Id);
                }
            }
            if (currentTick.V % defaultTickUpdateRate == 0 && myArmy.Count > 0 && myArmy.Count != previousArmyCount)
            {
                Console.WriteLine($"Tick: {currentTick} target: {target} my power: {(int)myPower} my army size: {myArmy.Count}");
            }
            previousArmyCount = myArmy.Count;

            string deckName = "TopDeck";
            Deck? currentDeck = myDecks.FirstOrDefault(d => d.Name == deckName);
            // Get the deck cardIds from the matching currentDeck.Name
            DeckOfficialCardIds myCurrentDeckOfficialCardIds = myDeckOfficialCardIds.FirstOrDefault(d => d.Name == currentDeck.Name) ?? myDeckOfficialCardIds[0];
            int unitPower = 75; // Nomad power cost - should be able to find info about card
            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[0]; // Starting unit for AI
            if (unitOfficialCardId != 0) // Not a card
            {
                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                if (card != null)
                {
                    unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                    if (botState.isGameStart == true)
                    {
                        Console.WriteLine("Found Card Info:{0}", card.cardName);
                        Console.WriteLine("My Power:{0} Unit Power:{1} Orbs:{2} Wells{3}", (int)myPower, unitPower, myOrbs.Count(), myWells.Count());
                        botState.isGameStart = false; // NGE04302024
                    }
                }
                else
                {
                    Console.WriteLine("Card Not Found:ID{0}", (int)SkylordsRebornAPI.Cardbase.CardTemplate.NomadANature);
                }
            }

            Position2D myArmyPos = botState.myStart;
            if (myArmySquads.Count > 0)
            {
                myArmyPos = PositionExtension.To2D(myArmySquads[0].Entity.Position);
            }
            Command? spawn = null;
            if (myArmy != null && myArmy.Count() < defaultAttackSquads)
            {
                spawn = SpawnUnit(myArmySquads, myPower, myArmyPos, 0, unitPower, currentTick.V, ref myPower); // Spawn unit in squad next to other units
            }

            //var spawn = SpawnUnit(myPower); // NGE04282024 
            var attack = Attack(target, myArmySquads);
            if (spawn == null && attack == null)
            {
                return Array.Empty<Command>();
            }
            else if (spawn != null && attack != null)
            {
                return [spawn, attack];
            }
            else if (spawn != null)
            {
                return [spawn];
            }
            else if (attack != null)
            {
                return [attack];
            }
            else
            {
                throw new InvalidOperationException("Unreachable, because all cases are handled above");
            }
        }

        private Command? Attack(EntityId target, List<Squad> squads)
        {
            if (target.V == 0)
            {
                return null;
            }
            else
            {
                List<EntityId> squadsIds = squads.Select(s => s.Entity.Id).ToList();
                return new CommandGroupAttack { Squads = squadsIds.ToArray(), TargetEntityId = target, ForceAttack = false }; 
            }
        }

        private Command? SpawnArcher(List<Squad> myArmy, float myPower, Position2D pos, int unitPower, uint tick)
        {
            if (archerCardPositions != null)
            {
                byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition]; // Starting unit for AI
                if (unitOfficialCardId != 0) // Not a card
                {
                    SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                    if (card != null)
                    {
                        unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!

                        return SpawnUnit(mySquads, myPower, pos, cardPosition, unitPower, tick, ref myPower); // Spawn unit in squad next to other units
                    }
                }
            }
            return null;
        }

        private Command? SpawnArcherOnBarrier(float myPower, Position2D pos, EntityId barrierModuleId, uint tick, ref float powerRemaining)
        {
            if (archerCardPositions != null)
            {
                byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition]; // Starting unit for AI
                if (unitOfficialCardId != 0) // Not a card
                {
                    SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                    if (card != null)
                    {
                        int unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!

                        return SpawnUnitOnBarrier(myPower, pos, cardPosition, barrierModuleId, unitPower, tick, ref powerRemaining); // Spawn unit in squad next to other units
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The Tick event happesn every 1/10 second and updates all entities (units, buildings, spells)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public Command[] TickOrig(GameState state)
        {
            bool showTickMessage = false;

            var currentTick = state.CurrentTick.V;
            var entities = state.Entities;
            var myArmy = new List<Squad>();      // XL04202024 
            var theirArmy = new List<Squad>();   // XL04202024 Need to make a List of a List when multiple opponents!
            var target = new EntityId(0u);          // XL04202024 

            if (botState.canPlayCardAt == uint.MaxValue)
            {
                foreach (var c in state.Commands)
                {
                    if (c.Player == botState.myId)
                    {
                        if (c.Command.CastSpellGod != null || c.Command.CastSpellGodMulti != null || c.Command.ProduceSquad != null || c.Command.BuildHouse != null)
                        {
                            if (c.Command.ProduceSquad != null)
                            {
                                Console.WriteLine("CommandProduceSquad canPlayCardAt");
                            }
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
                    myArmy.Add(s);
                }
                else
                {
                    theirArmy.Add(s);
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

            if (showTickMessage == true)
            {
                ConsoleWriteLine(true, $"Tick:{currentTick} opp:{target} pow:{(int)myPower} size:{myArmy.Count} oSize:{theirArmy.Count} oPow:{theirArmy}");
            }

            List<Command> commands = new List<Command>();

            var swiftclawTasks = SwiftclawTasks(currentTick, myPower, entities);
            if (swiftclawTasks != null)
            {
                commands.Add(swiftclawTasks);
                //return new[] { swiftclawTasks };
            }

            #region XanderLord new tasks
            /*
            var orbTasks = OrbTasks(currentTick, myPower, entities);
            if (orbTasks != null)
            {
                commands.Add(orbTasks);
                // return new[] { orbTasks };
            }

            var wellTasks = WellTasks(currentTick, myPower, entities);
            if (wellTasks != null)
            {
                commands.Add(wellTasks);
                //return new[] { wellTasks };
            }
            */
            #endregion XanderLord new tasks
            return commands.ToArray();
        }

        #region XanderLord Methods
        // If a well is next to an orb to own it, then get it

        // Example PvP Decks:
        // MgKaF-DiFVTNX6GQa_KDTiRuLMPsPZFlSCP0MrWMX GiftedFlame PvP
        // MgKaF-DiFVTNX6GQa_KDTuLMPsPZFlSCP0MMX PvPGiftedFlame
        // MgKaF-DiFVT3FTPuIkUFaNYTLwSCPGalS0MMLhYbY TaintedFlame
        // MaF1F-DgKEUhRVTqV4F3F TestDeck

        // Depending on the deck perform different strategies
        public Command[] Tick(GameState state, Deck currentDeck)
        {
            List<Command> commands = new List<Command>();
            switch (currentDeck.Name)
            {
                case "TopDeck":
                case "BattleGrounds":
                case "FireNature":
                case "TaintedDarkness":
                case "GiftedDarkness":
                case "BlessedDarkness":
                case "InfusedDarkness":
                case "TaintedFlora":
                case "GiftedFlora":
                case "BlessedFlora":
                case "InfusedFlora":
                case "TaintedIce":
                case "GiftedIce":
                case "BlessedIce":
                case "InfusedIce":
                case "TaintedFlame":
                case "GiftedFlame":
                case "BlessedFlame":
                case "InfusedFlame":
                    break;
            }
            return commands.ToArray();
        }

        public static SMJCard? GetCardFromOfficialCardId(SMJCard[]? cardsSMJ, int cardId)
        {
            if (cardsSMJ != null)
            {
                SMJCard[] cards = (SMJCard[])cardsSMJ;
                foreach (SMJCard card in cards)
                {
                    var result = card.officialCardIds.FirstOrDefault(i => i == cardId);
                    if (result != 0)
                    {
                        return card;
                    }
                }
            }
            return null;
        }

        private Command[] BuildStructure(Position pos, byte cardPosition, uint tick) // 1 through 20
        {
            List<Command> commands = new List<Command>();
            if (botState.canPlayCardAt < tick)
            {
                int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
                if (card != null)
                {
                    Console.WriteLine("build:{0}", card.cardName);
                    Command command = new CommandBuildHouse
                    {
                        CardPosition = cardPosition,
                        Xy = pos.To2D(),
                        Angle = float.Pi / 2
                    };
                    commands.Add(command);
                }
                else
                {
                    Console.WriteLine("Cannot build from deck card position:{0} - Not a card!", cardPosition);
                }
            }
            return commands.ToArray();
        }

        private Command[] CastSpell(Position2D pos, byte cardPosition, uint tick) // 1 through 20
        {
            List<Command> commands = new List<Command>();
            if (botState.canPlayCardAt < tick)
            {
                int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
                if (card != null)
                {
                    Console.WriteLine("cast spell:{card.cardName} at pos:{pos.X},{pos.Y}");
                    SingleTargetLocation stl = new SingleTargetLocation { Xy = pos };
                    Command command = new CommandCastSpellGod
                    {
                        CardPosition = cardPosition,
                        Target = new SingleTargetHolder { Location = stl },
                    };
                    commands.Add(command);
                }
                else
                {
                    Console.WriteLine("Cannot build from deck card position:{0} - Not a card!", cardPosition);
                }
            }
            return commands.ToArray();
        }

        private Command[] CastSpellMulti(Position pos1, Position pos2, byte cardPosition, uint tick) // 1 through 20
        {
            List<Command> commands = new List<Command>();
            if (botState.canPlayCardAt < tick)
            {
                int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
                if (card != null)
                {
                    Console.WriteLine("Cast spell multi:{0}", card.cardName);
                    Command command = new CommandCastSpellGodMulti
                    {
                        CardPosition = cardPosition,
                        Xy1 = pos1.To2D(),
                        Xy2 = pos2.To2D(),
                    };
                    commands.Add(command);
                }
                else
                {
                    Console.WriteLine("Cannot build from deck card position:{0} - Not a card!", cardPosition);
                }
            }
            return commands.ToArray();
        }

        private Command[] CastSpellEntity(Position pos, byte cardPosition, EntityId target, uint tick) // 1 through 20
        {
            List<Command> commands = new List<Command>();
            if (botState.canPlayCardAt < tick)
            {
                int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
                if (card != null)
                {
                    Console.WriteLine("cast spell {0}", card.cardName);
                    Command command = new CommandCastSpellEntity
                    {
                        Entity = target,
                        Spell = new SpellId((uint)officialCardId),
                        Target = new SingleTargetHolder { },
                    };
                    commands.Add(command);
                }
                else
                {
                    Console.WriteLine("Cannot cast entity spell from deck card position:{0} - Not a card!", cardPosition);
                }
            }
            return commands.ToArray();
        }


        private Command[] BuildOrb(float myPower, out float powerRemaining, EntityId orbId, CreateOrbColor color, Position pos, uint tick)
        {
            List<Command> commands = new List<Command>();
            powerRemaining = myPower;
            float orbCost = 150f;
            if (myPower > orbCost && botState.canPlayCardAt < tick)
            {
                Console.WriteLine("build OrbId:{orbId}");
                Command command = new CommandTokenSlotBuild
                {
                    SlotId = orbId,
                    Color = color
                };
                commands.Add(command);
                powerRemaining -= orbCost;
            }
            return commands.ToArray();
        }

        private Command[] BuildWell(float myPower, out float powerRemaining, EntityId wellId, Position pos, uint tick)
        {
            List<Command> commands = new List<Command>();
            powerRemaining = myPower;
            if (myPower > 100f && botState.canPlayCardAt < tick)
            {
                Console.WriteLine("build WellId:{wellId}");
                Command command = new CommandPowerSlotBuild
                {
                    SlotId = wellId
                };
                powerRemaining -= 100f;
                commands.Add(command);
            }
            return commands.ToArray();
        }

        private Command[] BuildWall(float myPower, out float powerRemaining, EntityId wallId, bool invertedDirection, uint tick)
        {
            List<Command> commands = new List<Command>();
            powerRemaining = myPower;
            // How to get the power cost to build structure?  API.Building.PowerCost or SMJCard.powerCost
            if (myPower > 100f && botState.canPlayCardAt < tick)
            {
                string message = string.Format("Build WallId:{0}", wallId.V);
                ConsoleWriteLine(true, message);
                Command command = new CommandBarrierBuild
                {
                    BarrierId = wallId,
                    InvertedDirection = invertedDirection
                };
                commands.Add(command);
                //Console.WriteLine("toggle WallId:{0} gate", wallId.V);
                //Command command2 = new CommandBarrierGateToggle
                //{
                //    BarrierId = wallId,
                //};
                //commands.Add(command2);
            }
            return commands.ToArray();
        }

        private string previousToggleWallGateMessage = "";
        private Command[] ToggleWallGate(EntityId wallId, bool open, uint tick)
        {
            List<Command> commands = new List<Command>();
            if (botState.canPlayCardAt < tick)
            {
                string message = string.Format("Toggle WallId:{0} from open:{1} to open:{2}", wallId.V, open, !open);
                if (message != previousToggleWallGateMessage)
                {
                    ConsoleWriteLine(true, message);
                    Command command = new CommandBarrierGateToggle
                    {
                        BarrierId = wallId,
                    };
                    commands.Add(command);
                    previousToggleWallGateMessage = message;
                }
            }
            return commands.ToArray();
        }


        // Get Unit Class 'Archer' cards so that I can put them on a wall
        public List<int> GetArcherCardPositionsFromDeck(DeckOfficialCardIds deckIds, int obsTotal = 1)
        {
            List<int> archerCardIdPositions = new List<int>(); // A deck has 20 card Ids ranging from position 0 (First card) to 19 (Last card)
            var queryCardsArchersT1OfficialIds = cardsSMJ.Where(item => item.orbsTotal == obsTotal && item.unitClass == "Archer").Select(s => s.officialCardIds[0]); // All T1 Archers CardIds

            for (int i = 0; i < deckIds.Ids.Length; i++)
            {
                if (queryCardsArchersT1OfficialIds.Contains(deckIds.Ids[i]) == true)
                {
                    archerCardIdPositions.Add(i);
                }
            }
            return archerCardIdPositions;
        }

        // Get Building Class 'Tower' cards so that I can put them on a wall
        public List<int> GetTowerCardPositionsFromDeck(DeckOfficialCardIds deckIds, int obsTotal = 1)
        {
            List<int> towerCardIdPositions = new List<int>(); // A deck has 20 card Ids ranging from position 0 (First card) to 19 (Last card)
            var queryCardsBuildingsOfficialIds = cardsSMJ.Where(item => item.orbsTotal == obsTotal && item.buildingClass == "Tower").Select(s => s.officialCardIds[0]); // All T1 Archers CardIds

            for (int i = 0; i < deckIds.Ids.Length; i++)
            {
                if (queryCardsBuildingsOfficialIds.Contains(deckIds.Ids[i]) == true)
                {
                    towerCardIdPositions.Add(i);
                }
            }
            return towerCardIdPositions;
        }

        // Get Spell Class 'Spell' cards so that I can put them on a wall
        public List<int> GetSpellCardPositionsFromDeck(DeckOfficialCardIds deckIds, int obsTotal = 1)
        {
            List<int> spellCardIdPositions = new List<int>(); // A deck has 20 card Ids ranging from position 0 (First card) to 19 (Last card)
            var queryCardsBuildingsOfficialIds = cardsSMJ.Where(item => item.orbsTotal == obsTotal && item.spellClass == "Spell").Select(s => s.officialCardIds[0]); // All T1 Archers CardIds

            for (int i = 0; i < deckIds.Ids.Length; i++)
            {
                if (queryCardsBuildingsOfficialIds.Contains(deckIds.Ids[i]) == true)
                {
                    spellCardIdPositions.Add(i);
                }
            }
            return spellCardIdPositions;
        }

        string presviousRejectedCommandMessage = "";
        public void ShowRejectedCommandsInfo(GameState state)
        {
            RejectedCommand[] RejectedCommands = state.RejectedCommands;
            if (RejectedCommands != null && RejectedCommands.Length > 0)
            {
                string commandSent = "";
                string commandFailReason = "";
                foreach (var command in RejectedCommands)
                {
                    if (command.Reason.CardRejected != null)
                    {
                        if (command.Command.ProduceSquad != null)
                        {
                            commandSent += "ProduceSquad ";
                        }
                        if (command.Command.ProduceSquadOnBarrier != null)
                        {
                            commandSent += "ProduceSquadOnBarrier ";
                        }
                        if (command.Command.GroupAttack != null)
                        {
                            commandSent += "GroupAttack ";
                        }
                        if (command.Command.CastSpellEntity != null)
                        {
                            commandSent += "CastSpellEntity ";
                        }
                        if (command.Command.CastSpellGod != null)
                        {
                            commandSent += "CastSpellGod ";
                        }
                        if (command.Command.CastSpellGodMulti != null)
                        {
                            commandSent += "CastSpellGodMulti ";
                        }

                        if (command.Reason.CardRejected != null)
                        {
                            commandFailReason = command.Reason.CardRejected.Reason.ToString();
                        }
                        if (command.Player == botState.myId) // Only my errors!
                        {
                            string message = string.Format("RejectedCommand Reason:{0}, Command{1}", commandFailReason, commandSent);
                            if (message != presviousRejectedCommandMessage)
                            {
                                Console.WriteLine(message);
                                presviousRejectedCommandMessage = message;
                            }
                        }
                        else
                        {
                            string message = string.Format("RejectedCommand PlayerId:{0} Reason:{1}, Command{2}", command.Player, commandFailReason, commandSent);
                            if (message != presviousRejectedCommandMessage)
                            {
                                Console.WriteLine(message);
                                presviousRejectedCommandMessage = message;
                            }
                        }
                    }
                }
            }
        }

        public Command[] SendCommands(GameState state, Deck currentDeck)
        {
            var currentTick = state.CurrentTick;
            var entities = state.Entities;
            if (botState.canPlayCardAt == uint.MaxValue)
            {
                foreach (var c in state.Commands)
                {
                    if (c.Player == botState.myId)
                    {
                        if (c.Command.CastSpellGod != null || c.Command.CastSpellGodMulti != null || c.Command.ProduceSquad != null || c.Command.ProduceSquadOnBarrier != null ||
                            c.Command.GroupAttack != null || c.Command.BuildHouse != null || c.Command.BarrierBuild != null || c.Command.BarrierGateToggle != null)
                        {
                            if (c.Command.ProduceSquad != null)
                            {
                                Console.WriteLine("CommandProduceSquad");
                            }
                            if (c.Command.ProduceSquadOnBarrier != null)
                            {
                                Console.WriteLine("CommandProduceSquadOnBarrier");
                            }
                            if (c.Command.GroupAttack != null)
                            {
                                Console.WriteLine("CommandGroupAttack");
                            }
                            if (c.Command.BuildHouse != null)
                            {
                                Console.WriteLine("CommandBuildHouse");
                            }
                            if (c.Command.CastSpellGod != null)
                            {
                                Console.WriteLine("CommandCastSpellGod");
                            }
                            if (c.Command.CastSpellGodMulti != null)
                            {
                                Console.WriteLine("CommandCastSpellGodMulti");
                            }
                            if (c.Command.BarrierBuild != null)
                            {
                                Console.WriteLine("CommandBarrierBuild");
                            }
                            if (c.Command.BarrierGateToggle != null)
                            {
                                Console.WriteLine("CommandBarrierGateToggle");
                            }
                            botState.canPlayCardAt = currentTick.V + 10; // Delay next command by 1 second (10 ticks per second)
                            break;
                        }
                    }
                }
            }

            ShowRejectedCommandsInfo(state);

            List<Command> commands = new List<Command>();
            // Get the deck cardIds from the matching currentDeck.Name
            DeckOfficialCardIds myCurrentDeckOfficialCardIds = myDeckOfficialCardIds.FirstOrDefault(d => d.Name == currentDeck.Name) ?? myDeckOfficialCardIds[0];

            switch (currentDeck.Name)
            {
                case "TopDeck":
                case "BattleGrounds":
                case "FireNature":
                case "TaintedDarkness":
                case "GiftedDarkness":
                case "BlessedDarkness":
                case "InfusedDarkness":
                case "TaintedFlora":
                case "GiftedFlora":
                case "BlessedFlora":
                case "InfusedFlora":
                case "TaintedIce":
                case "GiftedIce":
                case "BlessedIce":
                case "InfusedIce":
                case "TaintedFlame":
                case "GiftedFlame":
                case "BlessedFlame":
                case "InfusedFlame":
                    var player = Array.Find(state.Players, p => p.Id == botState.myId);
                    if (player == null) { return commands.ToArray(); }
                    var myPower = player.Power;

                    BarrierSet[] myBarriers = Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == botState.myId);
                    BarrierModule[] myWallsModules0 = Array.FindAll(state.Entities.BarrierModules, x => x.Entity.PlayerEntityId == botState.myId);
                    myWalls = myBarriers != null ? myBarriers.ToList() : new List<BarrierSet>();
                    myWallModules = myWallsModules0 != null ? myWallsModules0.ToList() : new List<BarrierModule>();

                    #region Get Squad Info
                    Dictionary<uint, Squad> myArmies = new Dictionary<uint, Squad>();
                    Dictionary<uint, Squad> enemyArmies = new Dictionary<uint, Squad>();
                    mySquads = new List<Squad>();
                    enemySquads = new List<Squad>();

                    myArmies.Clear();
                    enemySquads.Clear();

                    foreach (var s in state.Entities.Squads)
                    {
                        if (s.Entity.PlayerEntityId == botState.myId)
                        {
                            GetSquadHealth(state, s, out double squadHealth, out List<EntityId> healthyUnits);
                            if (squadHealth != 0)
                            {
                                myUnits.AddRange(healthyUnits);
                                myArmies[s.ResSquadId.V] = s;
                                mySquads.Add(s);
                            }
                        }
                        else if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                        {
                            GetSquadHealth(state, s, out double squadHealth, out List<EntityId> healthyUnits);
                            if (squadHealth != 0)
                            {
                                enemyUnits.AddRange(healthyUnits);
                                enemyArmies[s.ResSquadId.V] = s;
                                enemySquads.Add(s);
                            }
                        }
                    }
                    #endregion Get Squad Info

                    if (currentTick.V % defaultTickUpdateRate == 0) // Try to do stuff every 0.5 second instead of every 1/10 second
                    {
                        #region Build Wall at start
                        if (buildNearbyWallAtStart == true && closestWall.V != 0 && myWalls.Count() == 0)
                        {
                            if (closestWallDistanceSq < 200 && myPower > buildWallCost)
                            {
                                Command[] cmd = BuildWall(myPower, out float powerRemaining, closestWall, false, currentTick.V);
                                // Console.WriteLine("WallId:{0} built at:{1},{2}", closestWall, (int)closestWallPosition.X, (int)closestWallPosition.Y);
                                return cmd.ToArray();
                            }
                        }
                        if (buildNearbyWallAtStart == true && myWalls.Count() > 0 && myWallModules.Count() > 0) // Open Wall gate to let out squads
                        {
                            if (buildArcherOnWallAtStart == true)
                            {
                                if (archerCardPositions != null)
                                {
                                    Command? spawn = null;
                                    Command? spawnOnBarrier = null;

                                    byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                                    var archerSquad = mySquads.Where(a => a.CardId == currentDeck.Cards[cardPosition]).ToList(); // See if I have any archers

                                    int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition]; // Should be an archer

                                    int unitPower = 75; // Default power cost - should be able to find info about card
                                    if (unitOfficialCardId != 0) // Not a card
                                    {
                                        SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                                        if (card != null)
                                        {
                                            unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                            //if (botState.isGameStart == true)
                                            //{
                                            //    Console.WriteLine("Found Card Info:{0}", card.cardName);
                                            //    Console.WriteLine("My Power:{0} Unit Power:{1} Orbs:{2} Wells{3}", (int)myPower, unitPower, myOrbs.Count(), myWells.Count());
                                            //}
                                        }
                                        else
                                        {
                                            Console.WriteLine("Card Not Found:ID{0}", (int)SkylordsRebornAPI.Cardbase.CardTemplate.NomadANature);
                                        }
                                    }

                                    if (archerOnWallAtStartBuilt == false && myPower > unitPower)
                                    {
                                        Command? commandSpawnArcherOnWall = SpawnArcherOnWall(myWalls[0].Entity, myWallModules, currentTick.V, myPower, unitPower, ref myPower);
                                        if (commandSpawnArcherOnWall != null)
                                        {
                                            buildArcherOnWallAtStart = false;
                                            archerOnWallAtStartBuilt = true;

                                            commands.Add(commandSpawnArcherOnWall);
                                            return commands.ToArray();
                                        }
                                     }
                                    else if (myPower > unitPower) // Make an archer squad at starting orb location for now
                                    {
                                        Position2D wallPos = PositionExtension.To2D(myWalls[0].Entity.Position);
                                        Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };

                                        spawn = SpawnUnit(mySquads, myPower, pos, cardPosition, unitPower, currentTick.V, ref myPower); // Create Archer
                                                                                                                           
                                        // spawn = SpawnUnit(mySquads, myPower, botState.myStart, cardPosition, unitPower, currentTick.V); // Create Archer
                                        if (spawn != null)
                                        {
                                            commands.Add(spawn);
                                            return commands.ToArray();
                                        }
                                    }
                                    else
                                    {
                                        return commands.ToArray();
                                    }
                                }
                            }

                            List<Command> cmd = new List<Command>();
                            foreach (var module in myWallModules)
                            {
                                if (module.Set.V == myWalls[0].Entity.Id.V) // Part of the same Barrier
                                {
                                    var gateAspect = module.Entity.Aspects.FirstOrDefault(g => g.BarrierGate != null) ?? null;
                                    if (gateAspect != null)
                                    {
                                        if (gateAspect.BarrierGate.Open == false)
                                        {
                                            Command[] cmdModule = ToggleWallGate(module.Entity.Id, gateAspect.BarrierGate.Open, currentTick.V);
                                            cmd.AddRange(cmdModule);
                                            // Console.WriteLine("WallIModuleID:{0} gate open:{1}", module.Entity.Id, true);
                                        }
                                    }
                                }
                                if (cmd.Count() > 0)
                                {
                                    return cmd.ToArray();
                                }
                            }

                            buildNearbyWallAtStart = false; // This task is completed
                        }
                        #endregion Build Wall at start

                        TokenSlot[] myOrbs0 = Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == botState.myId);
                        myOrbs = myOrbs0 != null ? myOrbs0.ToList() : new List<TokenSlot>();
                        PowerSlot[] myWells0 = Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == botState.myId);
                        myWells = myWells0 != null ? myWells0.ToList() : new List<PowerSlot>();

                        // Squad[] mySquads0 = Array.FindAll(state.Entities.Squads, x => x.Entity.PlayerEntityId == botState.myId);
                        // mySquads = mySquads0 != null ? mySquads0.ToList() : new List<Squad>();

                        TokenSlot[] enemyOrbs0 = Array.FindAll(state.Entities.TokenSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        enemyOrbs = enemyOrbs0 != null ? enemyOrbs0.ToList() : new List<TokenSlot>();
                        PowerSlot[] enemyWells0 = Array.FindAll(state.Entities.PowerSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        enemyWells = enemyWells0 != null ? enemyWells0.ToList() : new List<PowerSlot>();
                        BarrierSet[] enemyBarriers0 = Array.FindAll(state.Entities.BarrierSets, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        BarrierModule[] enemyWalls0 = Array.FindAll(state.Entities.BarrierModules, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        enemyWalls = enemyBarriers0 != null ? enemyBarriers0.ToList() : new List<BarrierSet>();
                        enemyWallModules = enemyWalls0 != null ? enemyWalls0.ToList() : new List<BarrierModule>();
                        // Squad[] enemySquads0 = Array.FindAll(state.Entities.Squads, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        // enemySquads = enemySquads0 != null ? enemySquads0.ToList() : new List<Squad>();

                        TokenSlot[] emptyOrbs0 = Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == null);
                        emptyOrbs = emptyOrbs0 != null ? emptyOrbs0.ToList() : new List<TokenSlot>();
                        PowerSlot[] emptyWells0 = Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == null);
                        emptyWells = emptyWells0 != null ? emptyWells0.ToList() : new List<PowerSlot>();
                        BarrierSet[] emptyBarriers0 = Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == null);
                        BarrierModule[] emptyWalls0 = Array.FindAll(state.Entities.BarrierModules, x => x.Entity.PlayerEntityId == null);
                        emptyWalls = emptyBarriers0 != null ? emptyBarriers0.ToList() : new List<BarrierSet>();
                        float nearestEnemyObjectDistance = float.MaxValue;

                        if (botState.isGameStart == true || currentTick.V % defaultTickUpdateRate == 0) // Try to do stuff every 0.5 seconds instead of every 1/10 second
                        {
                            if (botState.isGameStart == true)
                            {
                                string message = string.Format("{0} Strategy start", currentDeck.Name);
                                ConsoleWriteLine(true, message);
                            }

                            // If we have power greater than the cost of the unit, we want to create the unit
                            // Deck CardId is not OfficialCardId!!!
                            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[0]; // Starting unit for AI

                            // Console.WriteLine("Tick CardID:{0}", unitOfficialCardId);
                            int unitPower = 75; // Nomad power cost - should be able to find info about card
                            if (unitOfficialCardId != 0) // Not a card
                            {
                                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                                if (card != null)
                                {
                                    unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Found Card Info:{0}", card.cardName);
                                        Console.WriteLine("My Power:{0} Unit Power:{1} Orbs:{2} Wells{3}", (int)myPower, unitPower, myOrbs.Count(), myWells.Count());
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Card Not Found:ID{0}", (int)SkylordsRebornAPI.Cardbase.CardTemplate.NomadANature);
                                }
                            }
                            if (myOrbs.Count > 0 && myPower > unitPower)
                            {
                                var myArmy = mySquads;
                                var theirArmy = enemySquads;

                                Position2D myArmyPos = botState.myStart;

                                #region Code to put a squad outside of a barrier near an orb
                                if (myWalls.Count() > 0 && mySquads.Count() == 0)
                                {
                                    Position2D wallPos = PositionExtension.To2D(myWalls[0].Entity.Position);
                                    Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };
                                    myArmyPos = pos;
                                    //Console.WriteLine("myArmyPos:{0},{1}", (int)myArmyPos.X, (int)myArmyPos.Y);
                                }
                                else if (myWalls.Count() > 0 && mySquads.Count() > 0) // Cannot build squad outside of wall unless another squad is inside wall
                                {
                                    // Turn all positions to int
                                    Position2D wallPos = PositionExtension.To2D(myWalls[0].Entity.Position);
                                    Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                    Position2D orbPos = PositionExtension.To2D(myOrbs[0].Entity.Position);
                                    Position2D orbPosInt = new Position2D { X = (int)orbPos.X, Y = (int)orbPos.Y };
                                    bool firstSquadInsideWall = false;
                                    Position2D mySqadPos = PositionExtension.To2D(myArmy[0].Entity.Position);
                                    Position2D mySqadPosInt = new Position2D { X = (int)mySqadPos.X, Y = (int)mySqadPos.Y };
                                    // Squad is NOT between orb and wall
                                    if (mySqadPosInt != wallPos && mySqadPosInt != orbPosInt &&
                                        mySqadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySqadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                                        mySqadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySqadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
                                    {
                                        firstSquadInsideWall = true;
                                    }

                                    if (firstSquadInsideWall == true)
                                    {
                                        // Get distance between Orb and Wall
                                        Position2D pos = new Position2D { X = wallPos.X + (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y + (wallPos.Y - botState.myStart.Y) / 2 };
                                        myArmyPos = pos;
                                    }
                                    else
                                    {
                                        myArmyPos = PositionExtension.To2D(myArmy[0].Entity.Position);
                                    }
                                    //Console.WriteLine("myArmyPos:{0},{1}", (int)myArmyPos.X, (int)myArmyPos.Y);
                                }
                                else if (myArmy.Count > 0)
                                {
                                    myArmyPos = PositionExtension.To2D(myArmy[0].Entity.Position);
                                }
                                #endregion Code to put a squad outside of a barrier near an orb

                                if (myArmy != null && myArmy.Count() > 0 && myWalls.Count() > 0)
                                {
                                    #region Open Gate when appropriate
                                    // If gate is closed and any attack squads are inside gate, open it
                                    {
                                        Position2D wallPos = PositionExtension.To2D(myWalls[0].Entity.Position);
                                        Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                        Position2D orbPos = PositionExtension.To2D(myOrbs[0].Entity.Position);
                                        Position2D orbPosInt = new Position2D { X = (int)myOrbs[0].Entity.Position.X, Y = (int)myOrbs[0].Entity.Position.Y };
                                        Position2D mySqadPos = new Position2D { X = 0, Y = 0 };
                                        Position2D mySqadPosInt = new Position2D { X = 0, Y = 0 };

                                        bool openGate = false;
                                        foreach (var squad in myArmy)
                                        {
                                            mySqadPos = PositionExtension.To2D(squad.Entity.Position);
                                            mySqadPosInt = new Position2D { X = (int)mySqadPos.X, Y = (int)mySqadPos.Y };
                                            // Squad is between orb and wall
                                            if (mySqadPosInt != wallPos && mySqadPosInt != orbPosInt &&
                                                mySqadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySqadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                                                mySqadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySqadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
                                            {
                                                openGate = true;
                                                string message = string.Format("SquadId{0} still inside, so open gate!", squad.Entity.Id);
                                                ConsoleWriteLine(true, message);
                                                break;
                                            }
                                        }
                                        if (openGate == true)
                                        {
                                            foreach (var module in myWallModules)
                                            {
                                                if (module.Set.V == myWalls[0].Entity.Id.V) // Part of the same Barrier
                                                {
                                                    var gateAspect = module.Entity.Aspects.FirstOrDefault(g => g.BarrierGate != null) ?? null;
                                                    if (gateAspect != null)
                                                    {
                                                        if (gateAspect.BarrierGate.Open == false)
                                                        {
                                                            Command[] cmdModule = ToggleWallGate(module.Entity.Id, gateAspect.BarrierGate.Open, currentTick.V);
                                                            commands.AddRange(cmdModule.ToArray());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion Open Gate when appropriate

                                    #region Close Gate when appropriate
                                    // If gate is open and all squads are outside gate, close it
                                    {
                                        // Turn all positions to int
                                        Position2D wallPos = PositionExtension.To2D(myWalls[0].Entity.Position);
                                        Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                        Position2D orbPos = PositionExtension.To2D(myOrbs[0].Entity.Position);
                                        Position2D orbPosInt = new Position2D { X = (int)orbPos.X, Y = (int)orbPos.Y };
                                        Position2D mySqadPos = new Position2D { X = 0, Y = 0 };
                                        Position2D mySqadPosInt = new Position2D { X = 0, Y = 0 };

                                        bool closeGate = true;
                                        foreach (var squad in myArmy)
                                        {
                                            mySqadPos = PositionExtension.To2D(squad.Entity.Position);
                                            mySqadPosInt = new Position2D { X = (int)mySqadPos.X, Y = (int)mySqadPos.Y };
                                            // Squad is NOT between orb and wall
                                            if (mySqadPosInt != wallPos && mySqadPosInt != orbPosInt &&
                                                mySqadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySqadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                                                mySqadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySqadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
                                            {
                                                closeGate = false;
                                                break;
                                            }
                                        }
                                        if (closeGate == true)
                                        {
                                            foreach (var module in myWallModules)
                                            {
                                                if (module.Set.V == myWalls[0].Entity.Id.V) // Part of the same Barrier
                                                {
                                                    var gateAspect = module.Entity.Aspects.FirstOrDefault(g => g.BarrierGate != null) ?? null;
                                                    if (gateAspect != null && gateAspect.BarrierGate.Open == true)
                                                    {
                                                        Command[] cmdModule = ToggleWallGate(module.Entity.Id, gateAspect.BarrierGate.Open, currentTick.V);
                                                        commands.AddRange(cmdModule.ToArray());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion Close Gate when appropriate

                                }

                                Command? spawn = null;
                                if (myArmy != null && myArmy.Count() < defaultAttackSquads)
                                {
                                    spawn = SpawnUnit(myArmy, myPower, myArmyPos, 0, unitPower, currentTick.V, ref myPower);
                                }
                                else if (myArmy != null && myArmy.Count() >= defaultAttackSquads && myArmy.Count() < defaultAttackSquads + defaultDefendSquads)
                                {
                                    if (myWalls.Count() > 0)
                                    {
                                        Position2D wallPos = PositionExtension.To2D(myWalls[0].Entity.Position); // Get nearest wall pos
                                        EntityId myWallId = myWalls[0].Entity.Id;
                                        //spawn = SpawnArcherOnBarrier(myArmy, myPower, botState.myStart, myWallId, unitPower, currentTick.V); // wallPos
                                        if (archerCardPositions != null)
                                        {
                                            byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                                            var archerSquad = myArmy.Where(a => a.CardId == currentDeck.Cards[cardPosition]).ToList(); // See if I have any archers

                                            Command? commandSpawnArcherOnWall = SpawnArcherOnWall(myWalls[0].Entity, myWallModules, currentTick.V, myPower, unitPower, ref myPower);
                                            if (commandSpawnArcherOnWall != null)
                                            { 
                                                commands.Add(commandSpawnArcherOnWall);
                                                return commands.ToArray();
                                            }
                                        }
                                    }
                                }
                                var target = new EntityId(0u);
                                EntityId previousTarget = new EntityId(0u);
                                if (previousTargets.Count > 0)
                                {
                                    if (previousTargets[0] != null)
                                    {
                                        previousTarget = previousTargets[0].Id;
                                    }
                                }
                                var targetOrb = new EntityId(0u);
                                var targetWell = new EntityId(0u);
                                var targetSquad = new EntityId(0u);
                                var targetWall = new EntityId(0u);
                                float nearestOrbDistance = float.MaxValue;
                                Entity targetEntityOrb = null;
                                Entity targetEntityWell = null;
                                Entity targetEntitySquad = null;
                                Entity targetEntityWall = null;
                                Entity targetEntity = null;
                                string targetType = "";

                                Squad attackSquad = null;
                                foreach (var s in enemyOrbs) // Attack enemy Orbs
                                {
                                    foreach (KeyValuePair<uint, Squad> kvp in myArmies)
                                    {
                                        attackSquad = kvp.Value;
                                        float squadDistanceToOrb = DistanceSquared(PositionExtension.To2D(attackSquad.Entity.Position), s.Entity.Position);
                                        if (squadDistanceToOrb < nearestOrbDistance)
                                        {
                                            nearestOrbDistance = squadDistanceToOrb;
                                            targetOrb = s.Entity.Id;
                                            targetEntityOrb = s.Entity;
                                        }
                                    }
                                }

                                float nearestWellDistance = float.MaxValue;
                                foreach (var s in enemyWells) // Attack enemy Wells
                                {
                                    foreach (KeyValuePair<uint, Squad> kvp in myArmies)
                                    {
                                        attackSquad = kvp.Value;
                                        float squadDistanceToWell = DistanceSquared(PositionExtension.To2D(attackSquad.Entity.Position), s.Entity.Position);
                                        if (squadDistanceToWell < nearestWellDistance)
                                        {
                                            nearestWellDistance = squadDistanceToWell;
                                            targetWell = s.Entity.Id;
                                            targetEntityWell = s.Entity;
                                        }
                                    }
                                }

                                float nearestOpponentSquadDistance = float.MaxValue;
                                foreach (var s in enemySquads) // if squad is near me attack it
                                {
                                    foreach (KeyValuePair<uint, Squad> kvp in myArmies)
                                    {
                                        attackSquad = kvp.Value;
                                        float squadDistanceToOpponent = DistanceSquared(PositionExtension.To2D(attackSquad.Entity.Position), s.Entity.Position);
                                        if (squadDistanceToOpponent < nearestOpponentSquadDistance)
                                        {
                                            nearestOpponentSquadDistance = squadDistanceToOpponent;
                                            targetSquad = s.Entity.Id;
                                            targetEntitySquad = s.Entity;
                                        }
                                    }
                                }

                                float nearestOpponentBarrierDistance = float.MaxValue;
                                Position nearestOpponentBarrierPosition;

                                bool isBarrierOpen = BreachBarrierSetModules(state, enemyWalls, enemyWallModules);
                                if (isBarrierOpen == false)
                                {
                                    foreach (var s in enemyWallModules) // if barrier module is near me attack it if it is not broken or the gate is open
                                    {
                                        nearestOpponentBarrierPosition = s.Entity.Position;
                                        //// Find the BarrierModule that has the same EntityId as the enemyWalls.EntityId
                                        //BarrierModule? barrier = state.Entities.BarrierModules.Where(x => x.Entity.Id == s.Entity.Id).FirstOrDefault();
                                        //if (barrier == null)
                                        //{
                                        //    continue;
                                        //}
                                        //else
                                        //{
                                        //    nearestOpponentBarrierPosition = barrier.Entity.Position;
                                        //}
                                        foreach (KeyValuePair<uint, Squad> kvp in myArmies)
                                        {
                                            attackSquad = kvp.Value;
                                            float squadDistanceToWall = DistanceSquared(PositionExtension.To2D(attackSquad.Entity.Position), nearestOpponentBarrierPosition);
                                            //float squadDistanceToWall = DistanceSquared(PositionExtension.To2D(attackSquad.Entity.Position), s.Entity.Position);
                                            if (squadDistanceToWall < nearestOpponentBarrierDistance)
                                            {
                                                nearestOpponentBarrierDistance = squadDistanceToWall;
                                                targetWall = s.Entity.Id;
                                                targetEntityWall = s.Entity;
                                            }
                                        }
                                    }
                                }

                                // Attack whatever is nearest to squad
                                if (targetOrb != null && targetOrb.V != 0 && nearestOrbDistance <= nearestOpponentSquadDistance && nearestOrbDistance <= nearestWellDistance && nearestOrbDistance <= nearestOpponentBarrierDistance)
                                {
                                    previousTarget = target;
                                    target = targetOrb;
                                    targetEntity = targetEntityOrb;
                                    targetType = "Orb";
                                }
                                else if (targetWell != null && targetWell.V != 0 && nearestWellDistance <= nearestOpponentSquadDistance && nearestWellDistance <= nearestOrbDistance && nearestWellDistance <= nearestOpponentBarrierDistance)
                                {
                                    previousTarget = target;
                                    target = targetWell;
                                    targetEntity = targetEntityWell;
                                    targetType = "Well";
                                }
                                else if (targetSquad != null && targetSquad.V != 0 && nearestOpponentSquadDistance <= nearestOrbDistance && nearestOpponentSquadDistance <= nearestWellDistance && nearestOpponentSquadDistance <= nearestOpponentBarrierDistance)
                                {
                                    previousTarget = target;
                                    target = targetSquad;
                                    targetEntity = targetEntitySquad;
                                    targetType = "Squad";
                                }
                                else if (targetWall != null && targetWall.V != 0 && nearestOpponentBarrierDistance <= nearestOrbDistance && nearestOpponentBarrierDistance <= nearestWellDistance && nearestOpponentBarrierDistance <= nearestOpponentSquadDistance)
                                {
                                    // CommandProduceSquadOnBarrier, CommandBarrierGateToggle, CommandBarrierBuild, CommandBarrierRepair, CommandBarrierCancelRepair 
                                    previousTarget = target; // A BarrierSet consists of BarrierModules.  The state of the Barrier Module determines if Barrier is fine 1, injured 2, gone 3
                                    target = targetWall;
                                    targetEntity = targetEntityWall;
                                    targetType = "Barrier";
                                }
                                if (previousTarget != target || previousTarget.V == 0)
                                {
                                    if (previousTargets.Count > 0)
                                    {
                                        if (previousTargets[0].Id != target && currentTick.V % defaultTickUpdateRate == 0)
                                        {
                                            string message = string.Format("Target:{0} Type:{1} Previous Target:{2} Type:{3}", target.V, targetType, previousTargets[0].Id.V, previousTargetTypes[0]);
                                            if (message != previousTargetMessage)
                                                Console.WriteLine(message);

                                            previousTargetMessage = message;

                                            if (targetEntity != null && targetType != null)
                                            {
                                                previousTargets.Insert(0, targetEntity);
                                                previousTargetTypes.Insert(0, targetType);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //if (currentTick.V % defaultTickUpdateRate == 0)
                                        //    Console.WriteLine("Target:{0} Type:{1}", target.V, targetType);
                                        if (targetEntity != null && targetType != null)
                                        {
                                            previousTargets.Insert(0, targetEntity);
                                            previousTargetTypes.Insert(0, targetType);
                                        }
                                    }
                                }
                                if (target.V == 0)
                                {
                                    var orb = enemyOrbs.Where(o => o.Entity.Id.V != 0).FirstOrDefault();
                                    if (orb != null)
                                    {
                                        target = orb.Entity.Id;
                                    }
                                    else
                                    {
                                        /*
                                        var well = enemyWells.Where(o => o.Entity.Id.V != 0).FirstOrDefault();
                                        if (well != null)
                                        {
                                            target = well.Entity.Id;
                                        }
                                        else
                                        {
                                            var squad = enemySquads.Where(o => o.Entity.Id.V != 0).FirstOrDefault();
                                            if (squad != null)
                                            {
                                                target = squad.Entity.Id;
                                            }
                                            else
                                            {
                                                var wall = enemyWalls.Where(o => o.Entity.Id.V != 0).FirstOrDefault();
                                                if (wall != null)
                                                {
                                                    target = wall.Entity.Id;
                                                }
                                            }
                                        }
                                        */
                                    }
                                    // Go attack the opponent's orb
                                    if (enemyOrbs.Count() > 0)
                                    {
                                        target = enemyOrbs[0].Entity.Id;
                                    }
                                    else if (enemyWells.Count() > 0)
                                    {
                                        target = enemyWells[0].Entity.Id;
                                    }
                                    else if (enemySquads.Count() > 0)
                                    {
                                        target = enemySquads[0].Entity.Id;
                                    }
                                    else if (enemyWallModules.Count() > 0)
                                    {
                                        target = enemyWallModules[0].Entity.Id;
                                    }
                                }
                                Command? attack = null;
                                List<Squad>? squads = null;
                                if (targetEntity != null && attackSquad != null)
                                {
                                    squads = new List<Squad>(); // { attackSquad };
                                    // Add squads to attackSquad until we reach maxAttackUnits // NGE04302024 
                                    int attackSquadCount = 0;

                                    for (int i = 0; i < mySquads.Count(); i++)
                                    {
                                        Squad squad = mySquads[i];
                                        if (!(archerOnWallAtStartBuilt == true && archerOnWallAtStartBuiltId == squad.Entity.Id))
                                        {
                                            squads.Add(squad);
                                            attackSquadCount++;
                                        }
                                        if (attackSquadCount >= maxAttackUnits)
                                            break;
                                    }

                                }
                                if (squads != null && squads.Count() > 0 && currentTick.V % defaultTickUpdateRate == 0) // NGE05062024 added  && currentTick.V % defaultTickUpdateRate == 0
                                {
                                    attack = Attack(target, squads, targetEntity.Position.To2D(), unitsNeededBeforeAttack);
                                }


                                if (squads != null && currentTick.V % defaultTickUpdateRate == 0)
                                {
                                    if (previousTarget != target && previousSquadCount != squads.Count())
                                    {
                                        Console.WriteLine($"Tick: {currentTick} Target:{target} My Power:{(int)myPower} Army:{myUnits.Count()} Squads:{squads.Count()} Enemy army:{theirArmy.Count} Squads:{enemySquads.Count()}");
                                    }
                                }

                                if (spawn == null && attack == null)
                                {
                                    // return Array.Empty<Command>();
                                }
                                else if (spawn != null && attack != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Spawn unit and attack");
                                    }
                                    commands.Add(spawn);
                                    commands.Add(attack);
                                }
                                else if (spawn != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Spawn unit");
                                    }
                                    commands.Add(spawn);
                                }
                                else if (attack != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Attack");
                                    }
                                    commands.Add(attack);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Unreachable, because all cases are handled above");
                                }
                            }
                        }
                        // Console.WriteLine("{0} AttackStrategy end", currentDeck.Name);
                    }
                    break;
            }
            botState.isGameStart = false;
            return commands.ToArray();
        }

        private Command? SpawnArcherOnWall(Entity wall, List<BarrierModule> barrierModules, uint currentTick, float myPower, int unitPower, ref float powerRemaining)
        {
            if (myWalls.Count() > 0)
            {
                Position2D wallPos = PositionExtension.To2D(wall.Position); // Get nearest wall pos
                EntityId myWallId = myWalls[0].Entity.Id;

                if (archerCardPositions != null)
                {
                    Command? spawnOnBarrier = null;
                    foreach (var b in barrierModules)
                    {
                        if (b.Entity.PlayerEntityId == botState.myId)
                        {
                            Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };

                            double distance = Math.Sqrt(DistanceSquared(wallPos, b.Entity.Position));
                            string message = string.Format("distance:{0:0.0}", distance);
                            ConsoleWriteLine(true, message);

                            if (b.FreeSlots >= 6 && myPower >= unitPower && botState.canPlayCardAt < currentTick && distance < 15)
                            {
                                spawnOnBarrier = SpawnArcherOnBarrier(myPower, b.Entity.Position.To2D(), b.Entity.Id, currentTick, ref myPower);
                                if (spawnOnBarrier != null)
                                {
                                    return spawnOnBarrier;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void SelectOrb()
        {

        }

        private Command? BuildWells(float myPower, out float powerRemaining)
        {
            List<EntityId> wellIds = wells.Keys.ToList();
            powerRemaining = myPower;
            int counter = 0;
            for (int i = 0; i < wellIds.Count; i++)
            {
                {
                    Position2D pos1 = wells[wellIds[i]].Pos;
                    counter++;
                    for (int j = counter; j < wellIds.Count; j++)
                    {
                        Position2D pos2 = wells[wellIds[j]].Pos;
                        if (MathF.Sqrt(DistanceSquared(pos1, pos2)) < 15)
                        {
                            if (powerRemaining >= 150) // 150 should depend on how many orbs I have
                            {
                                powerRemaining -= 150; // 150 should depend on how many orbs I have
                                ConsoleWriteLine(true, "Build Well");
                                //tasks.buildNearbyWell = true;
                                return new CommandPowerSlotBuild
                                {
                                    SlotId = wellIds[j],
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Command? BuildOrbs(float myPower, out float powerRemaining)
        {
            List<EntityId> obIds = orbs.Keys.ToList();
            powerRemaining = myPower;
            int counter = 0;
            for (int i = 0; i < obIds.Count; i++)
            {
                {
                    Position2D pos1 = orbs[obIds[i]].Pos;
                    counter++;
                    for (int j = counter; j < obIds.Count; j++)
                    {
                        Position2D pos2 = orbs[obIds[j]].Pos;
                        if (MathF.Sqrt(DistanceSquared(pos1, pos2)) < 15)
                        {
                            if (powerRemaining >= 150) // 150 should depend on how many orbs I have
                            {
                                powerRemaining -= 150; // 150 should depend on how many orbs I have
                                ConsoleWriteLine(true, "Build Orb");
                                //tasks.buildNearbyOrb = true;
                                return new CommandTokenSlotBuild
                                {
                                    SlotId = obIds[j],
                                    Color = CreateOrbColor.Shadow
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }

        /*
        // If another empty well or orb is next to an well to own it, then get it (as long as we have power and do not exceed 4 orbs)

        private Command? OrbTasks(uint tick, float myPower, MapEntities entities)
        {
            var tasks = currentDeckTasks; // botState.tasks.orbTasks;
            float powerRemaining = myPower;
            if (tasks != null && tasks.done)
                return null;

            // Always heal, then get wells, then get orbs
            int orbCost = (int)OrbCost.Orb1; // How many orbs do I have?
            int orbHealth = 0;
            int orbHealthLimit = 20;
            int wellCost = 100; 
            int wellHealth = 0;
            int wellHealthLimit = 20;
            if (tasks.buildNearbyWell == true)
            {
                BuildWells(myPower, out powerRemaining);
            }
            if (tasks.buildNearbyOrb == true)
            {
                BuildOrbs(powerRemaining, out powerRemaining);
            }

            //if (!tasks.heal && myPower >= (100 - wellHealth) && wellHealth < wellHealthLimit) // && botState.canPlayCardAt < tick)
            //{
            //}
            //if (!tasks.buildNearbyWell && myPower >= 100) // && botState.canPlayCardAt < tick)
            //{
            //}
            //// See how many orbs we currently have
            //if (!tasks.buildNearbyOrb && myPower >= orbCost) // && botState.canPlayCardAt < tick)
            //{
            //}
            return null;
        }

        // If another empty well or orb is next to an well to own it, then get it (as long as we have power and do not exceed 4 orbs)

        private Command? WellTasks(uint tick, float myPower, MapEntities entities)
        {
            var tasks = botState.tasks.orbTasks;
            float powerRemaining = myPower;
            if (tasks.done)
                return null;

            // Always heal, then get wells, then get orbs
            int orbCost = (int)OrbCost.Orb1; // How many orbs do I have?
            int orbHealth = 0;
            int orbHealthLimit = 20;
            int wellCost = 100;
            int wellHealth = 0;
            int wellHealthLimit = 20;
            if (tasks.buildNearbyWell == true)
            {
                BuildWells(myPower, out powerRemaining);
            }
            if (tasks.buildNearbyOrb == true)
            {
                BuildOrbs(powerRemaining, out powerRemaining);
            }

            //if (!tasks.heal && myPower >= (100 - wellHealth) && wellHealth < wellHealthLimit) // && botState.canPlayCardAt < tick)
            //{
            //}
            //if (!tasks.buildNearbyWell && myPower >= 100) // && botState.canPlayCardAt < tick)
            //{
            //}
            //// See how many orbs we currently have
            //if (!tasks.buildNearbyOrb && myPower >= orbCost) // && botState.canPlayCardAt < tick)
            //{
            //}
            return null;
        }
        */

        public string prevousMessage = "";
        public void ConsoleWriteLine(bool show, string text)
        {
            if (show == true && prevousMessage != text)
            {
                Console.WriteLine(text);
                prevousMessage = text;
            }
        }

        #endregion XanderLord Methods

        private Command? SwiftclawTasks(uint tick, float myPower, MapEntities entities)
        {
            var tasks = currentDeckTasks; // var tasks = botState.tasks.swiftclawTasks;
            if (tasks.done)
                return null;

            if (!tasks.swiftclawTasks.produceSwiftclaw && myPower >= 70 && botState.canPlayCardAt < tick)
            {
                botState.canPlayCardAt = uint.MaxValue;
                ConsoleWriteLine(true, "produce swiftclaw");
                tasks.swiftclawTasks.produceSwiftclaw = true;
                return new CommandProduceSquad
                {
                    CardPosition = 0,
                    Xy = botState.myStart
                };
            }
            else if (tasks.swiftclawTasks.produceSwiftclaw && tasks.swiftclawTasks.swiftclaw.V == 0)
            {
                var SEARCH_ID = CardIdCreator.New(Api.CardTemplate.Swiftclaw, Upgrade.U3);
                var swiftclaw = Array.Find(entities.Squads, s => s.CardId == SEARCH_ID);
                if (swiftclaw != null)
                {
                    tasks.swiftclawTasks.swiftclaw = swiftclaw.Entity.Id;
                    ConsoleWriteLine(true, $"Swiftclaw = {swiftclaw.Entity.Id}");
                    // We found him, but give him a task on next tick
                }
                return null;
            }
            else if (tasks.swiftclawTasks.produceSwiftclaw && tasks.swiftclawTasks.swiftclaw.V > 0)
            {
                var swiftclaw = entities.Squads.FirstOrDefault(s => s.Entity.Id == tasks.swiftclawTasks.swiftclaw);
                if (swiftclaw == null)
                {
                    ConsoleWriteLine(true, "Swiftclaw was killed");
                    // Swiftclaw was killed, give up on this task set
                    tasks.done = true;
                    return null;
                }
                else
                {
                    var pos = swiftclaw.Entity.Position;
                    if (!tasks.swiftclawTasks.buildPrimalDefender && myPower >= 60 && botState.canPlayCardAt < tick)
                    {
                        botState.canPlayCardAt = uint.MaxValue;
                        tasks.swiftclawTasks.buildPrimalDefender = true;
                        ConsoleWriteLine(true, "Build Primal Defender");
                        return new CommandBuildHouse
                        {
                            CardPosition = 4,
                            Xy = pos.To2D(),
                            Angle = float.Pi / 2
                        };
                    }
                    else if (!tasks.swiftclawTasks.buildTokenSlot
                        && MathF.Sqrt(DistanceSquared(tasks.swiftclawTasks.closestTokenSlotPosition, pos)) < 15)
                    {
                        if (myPower >= 150)
                        {
                            ConsoleWriteLine(true, "Token slot build");
                            tasks.swiftclawTasks.buildTokenSlot = true;
                            return new CommandTokenSlotBuild
                            {
                                SlotId = tasks.swiftclawTasks.closestTokenSlot,
                                Color = CreateOrbColor.Shadow
                            };
                        }
                        else if (!tasks.swiftclawTasks.holdPosition)
                        {
                            tasks.swiftclawTasks.holdPosition = true;
                            ConsoleWriteLine(true, "Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclawTasks.swiftclaw }
                            };
                        }
                        else
                        {
                            // Waiting for power
                            return null;
                        }
                    }
                    else if (!tasks.swiftclawTasks.buildPowerSlot
                        && MathF.Sqrt(DistanceSquared(tasks.swiftclawTasks.closestPowerSlotPosition, pos)) < 15)
                    {
                        if (myPower >= 100)
                        {
                            ConsoleWriteLine(true, "Power slot build");
                            tasks.swiftclawTasks.buildPowerSlot = true;
                            return new CommandPowerSlotBuild
                            {
                                SlotId = tasks.swiftclawTasks.closestPowerSlot
                            };
                        }
                        else if (!tasks.swiftclawTasks.holdPosition)
                        {
                            tasks.swiftclawTasks.holdPosition = true;
                            ConsoleWriteLine(true, "Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclawTasks.swiftclaw }
                            };
                        }
                        else
                        {
                            // Waiting for power
                            return null;
                        }
                    }
                    else if (tasks.swiftclawTasks.buildPrimalDefender && tasks.swiftclawTasks.buildTokenSlot && tasks.swiftclawTasks.buildPowerSlot)
                    {
                        if (!tasks.swiftclawTasks.holdPosition)
                        {
                            tasks.swiftclawTasks.holdPosition = true;
                            ConsoleWriteLine(true, "Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclawTasks.swiftclaw }
                            };
                        }
                        else if (!tasks.swiftclawTasks.changeMode)
                        {
                            tasks.swiftclawTasks.changeMode = true;
                            ConsoleWriteLine(true, "Swiftclaw change mode");
                            return new CommandModeChange
                            {
                                EntityId = tasks.swiftclawTasks.swiftclaw,
                                NewModeId = new ModeId(3000497)
                            };
                        }
                        else
                        {
                            ConsoleWriteLine(true, "Swiftclaw done");
                            tasks.done = true;
                            return null;
                        }
                    }
                    else
                    {
                        var figure = entities.Figures.FirstOrDefault(f => f.Entity.Id == swiftclaw.Figures[0]);
                        Debug.Assert(figure != null);
                        if (figure.CurrentSpeed > 0)
                        {
                            // already moving
                            return null;
                        }
                        else
                        {
                            ConsoleWriteLine(true, "Swiftclaw Patrol");
                            return new CommandGroupGoto
                            {
                                Squads = new[] { tasks.swiftclawTasks.swiftclaw },
                                Positions = new[]
                                {
                                    tasks.swiftclawTasks.closestTokenSlotPosition,
                                    tasks.swiftclawTasks.closestPowerSlotPosition,
                                    botState.myStart
                                },
                                WalkMode = WalkMode.Patrol,
                                Orientation = 0
                            };
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private static float DistanceSquared(Position2D from, Position to)
        {
            return (from.X - to.X) * (from.X - to.X) + (from.Y - to.Z) * (from.Y - to.Z);
        }

        private static float DistanceSquared(Position2D from, Position2D to)
        {
            return (from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y);
        }

        #region Special Decks
        private static readonly Deck BattleGrounds = new()
        {
            Name = "BattleGrounds",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
            {
                CardIdCreator.New(Api.CardTemplate.Windweavers, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Shaman, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Spearmen, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPaladins, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPhantom, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShadowPhoenix, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Tranquility, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CultistMaster, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Upgrade.U3),
            }
        };

        public static readonly DeckOfficialCardIds BattleGroundsCardIds = new()
        {
            Name = "BattleGrounds",
            Ids = new int[20]
    {
                (int)Api.CardTemplate.Windweavers,
                (int)Api.CardTemplate.DryadAFrost,
                (int)Api.CardTemplate.Swiftclaw,
                (int)Api.CardTemplate.Shaman,
                (int)Api.CardTemplate.Spearmen,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.NastySurprise,
                (int)Api.CardTemplate.DarkelfAssassins,
                (int)Api.CardTemplate.Nightcrawler,
                (int)Api.CardTemplate.AmiiPaladins,
                (int)Api.CardTemplate.AmiiPhantom,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.ShadowPhoenix,
                (int)Api.CardTemplate.AuraofCorruption,
                (int)Api.CardTemplate.Tranquility,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.CultistMaster,
                (int)Api.CardTemplate.AshbonePyro,
    }
        };

        private static readonly Deck TopDeck = new()
        {
            Name = "TopDeck",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
    {
                CardIdCreator.New(Api.CardTemplate.Scavenger, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Mine, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Wallbreaker, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Enforcer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Firedancer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BreedingGrounds, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SwampDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.MagmaHurler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StrangleholdAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thunderstorm, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
    }
        };

        public static readonly DeckOfficialCardIds TopDeckCardIds = new()
        {
            Name = "TopDeck",
            Ids = new int[20]
    {
                (int)Api.CardTemplate.Scavenger,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Mine,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.Wallbreaker,
                (int)Api.CardTemplate.Enforcer,
                (int)Api.CardTemplate.Firedancer,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.BreedingGrounds,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.MagmaHurler,
                (int)Api.CardTemplate.StrangleholdAShadow,
                (int)Api.CardTemplate.Thunderstorm,
                (int)Api.CardTemplate.DryadAFrost,
    }
        };

        private static readonly Deck FireNature = new()
        {
            Name = "FireNature",
            CoverCardIndex = 0,
            Cards = new Api.CardId[20]
     {
                CardIdCreator.New(Api.CardTemplate.NomadANature, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Mine, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunderer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Firedancer, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BreedingGrounds, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.MagmaHurler, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SwampDrake, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StrangleholdAShadow, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DeathgliderAFrost, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thunderstorm, Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Upgrade.U3),
     }
        };

        public static readonly DeckOfficialCardIds FireNatureCardIds = new()
        {
            Name = "FireNature",
            Ids = new int[20]
            {
                (int)Api.CardTemplate.NomadANature,
                (int)Api.CardTemplate.Mine,
                (int)Api.CardTemplate.Eruption,
                (int)Api.CardTemplate.Sunderer,
                (int)Api.CardTemplate.Firedancer,
                (int)Api.CardTemplate.SkyfireDrake,
                (int)Api.CardTemplate.Ravage,
                (int)Api.CardTemplate.CurseofOink,
                (int)Api.CardTemplate.EnsnaringRoots,
                (int)Api.CardTemplate.BreedingGrounds,
                (int)Api.CardTemplate.SurgeOfLight,
                (int)Api.CardTemplate.Sunstriders,
                (int)Api.CardTemplate.Hurricane,
                (int)Api.CardTemplate.Burrower,
                (int)Api.CardTemplate.MagmaHurler,
                (int)Api.CardTemplate.SwampDrake,
                (int)Api.CardTemplate.StrangleholdAShadow,
                (int)Api.CardTemplate.DeathgliderAFrost,
                (int)Api.CardTemplate.Thunderstorm,
                (int)Api.CardTemplate.DryadAFrost,
            }
        };

        #endregion Special Decks

        private static Dictionary<string, List<int>> dictDeckNameOfficialCardIds = new Dictionary<string, List<int>>();
        private void CreateDeckOfficialCardIds(string name, List<int> cardIds)
        {
            dictDeckNameOfficialCardIds[name] = cardIds;
        }

        private static List<Deck> myDecks = new List<Deck>() { TopDeck, BattleGrounds, FireNature,
            PvPCardDecks.TaintedDarkness, PvPCardDecks.GiftedDarkness, PvPCardDecks.BlessedDarkness, PvPCardDecks.InfusedDarkness,
            PvPCardDecks.TaintedFlora, PvPCardDecks.GiftedFlora, PvPCardDecks.BlessedFlora, PvPCardDecks.InfusedFlora,
            PvPCardDecks.TaintedIce, PvPCardDecks.GiftedIce, PvPCardDecks.BlessedIce, PvPCardDecks.InfusedIce,
            PvPCardDecks.TaintedFlame, PvPCardDecks.GiftedFlame, PvPCardDecks.BlessedFlame, PvPCardDecks.InfusedFlame,
        };

        private static List<DeckOfficialCardIds> myDeckOfficialCardIds = new List<DeckOfficialCardIds>() { TopDeckCardIds, BattleGroundsCardIds, FireNatureCardIds,
              PvPCardDecks.TaintedDarknessCardIds, PvPCardDecks.GiftedDarknessCardIds, PvPCardDecks.BlessedDarknessCardIds, PvPCardDecks.InfusedDarknessCardIds,
              PvPCardDecks.TaintedFloraCardIds, PvPCardDecks.GiftedFloraCardIds, PvPCardDecks.BlessedFloraCardIds, PvPCardDecks.InfusedFloraCardIds,
              PvPCardDecks.TaintedIceCardIds, PvPCardDecks.GiftedIceCardIds, PvPCardDecks.BlessedIceCardIds, PvPCardDecks.InfusedIceCardIds,
              PvPCardDecks.TaintedFlameCardIds, PvPCardDecks.GiftedFlameCardIds, PvPCardDecks.BlessedFlameCardIds, PvPCardDecks.InfusedFlameCardIds,
        };

        private struct BotState
        {
            public byte myTeam;
            public required List<EntityId> oponents;
            public required List<EntityId> team;
            public required Position2D myStart;
            public UInt32 canPlayCardAt;
            public required EntityId myId;
            //public required Tasks? tasks;
            public required Deck selectedDeck;
            public required bool isGameStart;
        }

        private class Tasks
        {
            public bool castHurricane;
            public bool castHeal;
            public bool repairWall;
            public bool cancelRepairWall;
            public bool repairBuilding;
            public bool cancelRepairBuilding;
            public bool done;

            public bool surrender;

            public SwiftclawTasks? swiftclawTasks;
            public WindweaversTasks? windweaversTasks;
            public BurrowerTasks? burrowerTasks;

            public OrbTasks? orbTasks;
            public WellTasks? wellTasks;

            public class OrbTasks
            {
                //public required EntityId closestTokenSlot;
                //public required Position2D closestTokenSlotPosition;
                //public required EntityId closestPowerSlot;
                //public required Position2D closestPowerSlotPosition;
                //public required EntityId well;

                public bool buildNearbyOrb;
                public bool buildNearbyWell;
                public bool heal;
                public bool done;
            }

            public class WellTasks
            {
                //public required EntityId closestTokenSlot;
                //public required Position2D closestTokenSlotPosition;
                //public required EntityId closestPowerSlot;
                //public required Position2D closestPowerSlotPosition;
                //public required EntityId well;

                public bool buildNearbyOrb;
                public bool buildNearbyWell;
                public bool heal;
                public bool done;
            }

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
