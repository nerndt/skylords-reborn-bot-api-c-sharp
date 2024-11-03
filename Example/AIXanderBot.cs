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
using SkylordsRebornAPI.Cardbase.Cards;
using System.Runtime.ConstrainedExecution;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

// TODO Put archers on wall modules nearest to enemy.
// Check code of when to attack enemy instead of target (typically enemy orb)
namespace Bots
{
    // /AI: list
    // Tainted Gifted Blessed Infused   Darkenss Flora Ice Flame GiftedFLora - all nature, BlessedIce - all frost, TaintedDarness - all shadow InfusedFlame - all fire
    // /AI: add XanderAI TopDeck 4
    // /AI: add XanderAI GiftedFlameNew 4
    // /AI: add XanderAI GiftedFlame 4
    // /AI: add XanderAI TaintedFlora 4
    // /AI: add XanderAI GiftedIce 4
    // /AI: add XanderAI InfusedFlame 4
    // Basic Strategies:

    /*
     07292024 Currently the bot should start with 
     "/AI: add XanderAI GiftedFlame 4" or "/AI: add XanderAI GiftedFlame 1" and will make a scavenger first and go to nearest well and make well.  It will then make 
    another scavenger.  If the opponent gets more than halfway over te map to my orb I then make a wall and put 4 archers on the wall to defend.
     */

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

        //public Dictionary<int, string> WhyCanNotPlayCardThere = new Dictionary<int, string>
        //{
        //    { 0x10, "DoesNotHaveEnoughPower" },
        //    { 0x20, "InvalidPosition" }, // too close to (0,y), or (x,0)
        //    { 0x80, "CardCondition" },
        //    { 0x100, "ConditionPreventCardPlay" },
        //    { 0x200, "DoesNotHaveThatCard" },
        //    { 0x400,"DoesNotHaveEnoughOrbs" },
        //    { 0x10000,"CastingTooOften" },
        //};

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
        //public List<int> damage { get; set; } // 4 levels attack power
        //public List<int> health { get; set; } // 4 levels health at start
        //public List<int> boosters { get; set; } // None, "Mini, General, "Fire, Shadow, "Nature, Frost, "Bandits, Stonekin, "Twilight, Lost Souls, "Amii, Fire/Frost
        public readonly string[] UpgradeMaps = { "None", "Encounter With Twilight", "Siege of Hope", "Defending Hope", "The Soultree", "The Treasure Fleet", "Behind Enemy Lines", "Mo", "Ocean", "Oracle", "Crusade", "Sunbridge", "Nightmare Shard", "Nightmare's End", "The Insane God", "Slave master", "Convoy", "Bad Harvest", "King of the Giants", "Titan", "The Dwarven Riddle", "The guns of Lyr", "Blight", "Raven's End", "Empire" };

        #endregion SMJCards JSON info

        string botVersion = "0.0.2.5";

        bool consoleWriteline = false; // Flag to track issues - when competition, set to false to try and improve 

        string previousTargetMessage = string.Empty;

        Deck? currentDeck = null;

        int unitPower = 75; // Power needed to build a specific game card

        int unitsNeededBeforeAttack = 6; // Must be <= defaultAttackSquads
        bool enemyBuildingOrb = false; // If enemy building second orb, go attack right away!
        int defaultTickUpdateRate = 2;
        int defaultAttackSquads = 6; // For now do not build more than X attack units
        int defaultDefendSquads = 6; // For now do not build more than X defend units

        bool buildNearbyWallAtStartWasTrue = false;
        bool buildNearbyWallAtStart = false;
        bool buildArcherOnWallAtStart = true;
        bool buildArcherOnWallAtStartWasTrue = false;
        bool archerOnWallAtStartBuilt = false;

        bool buildNearbyWellAtStart = false; // NGE11022024 true;
        bool buildNearbyOrbAtStart = false;

        bool attackClosestSquad = true; // NGE11022024 false;
        bool attackClosestWell = false;

        bool isEnemyNearBase = false; // Is there an enemy near my Orb and am I already attacking it?
        bool isEnemyOrbNearBase = false; // Is there an enemy orb near my Orb and am I already attacking it?
        bool isEnemyWellNearBase = false; // Is there an enemy well near my Orb and am I already attacking it?
        bool isEnemyBuildingNearBase = false; // Is there an enemy building near my Orb and am I already attacking it?

        bool attackingEnemyNearBase = true; // Is there is an enemy squad near my Orb and am I already attacking it, do so!

        List<Strategy> strategyList = new List<Strategy>(); // This dictates what to build when in the process

        List<Entity> previousTargets = new List<Entity>(); // List of all previous targets
        List<string> previousTargetTypes = new List<string>(); // "Orb", "Well", "Squad", etc.

        List<RejectedCommand> myRejectedCommands = new List<RejectedCommand>();

        List<TokenSlot> myOrbs = new List<TokenSlot>(); // Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<PowerSlot> myWells = new List<PowerSlot>(); // Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<BarrierSet> myWalls = new List<BarrierSet>(); // Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<Building> myBuildings = new List<Building>(); // Array.FindAll(state.Entities.Building, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<BarrierModule> myWallModules = new List<BarrierModule>(); // Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<Squad> mySquads = new List<Squad>(); // Array.FindAll(state.Entities.Squads, x => x.Entity.Id == botState.myId).ToList();
        List<EntityId> myUnits = new List<EntityId>();
        int attackSquadCount = 0;
        List<Squad>? myAttackSquads = null;
        int defendSquadCount = 0;
        List<Squad>? myDefendSquads = null;

        // List<Squad> attackingEnemyNearBaseSquads = new List<Squad>(); // List of squads I have sent to attack enemy near my base
        List<Squad>? enemyAttackingSquads = null; // Enemies I need to fight right away
        List<Squad>? engagingEnemyAttackingSquads = null; // The squds I will use to fight the enemyAttackingSquad

        Dictionary<string, Squad> mySquadTeams = new Dictionary<string, Squad>(); // Squads are grouped by what they are doing Attack1, Attack2, Defend1, Defend2, etc.

        List<RejectedCommand> enemyRejectedCommands = new List<RejectedCommand>();

        List<TokenSlot> enemyOrbs = new List<TokenSlot>(); // Array.FindAll(state.Entities.TokenSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<PowerSlot> enemyWells = new List<PowerSlot>(); // Array.FindAll(state.Entities.PowerSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<BarrierModule> enemyWallModules = new List<BarrierModule>(); // Array.FindAll(state.Entities.BarrierSets, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<BarrierSet> enemyWalls = new List<BarrierSet>(); // Array.FindAll(state.Entities.BarrierModules, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<Building> enemyBuildings = new List<Building>(); // Array.FindAll(state.Entities.Building, x => x.Entity.PlayerEntityId == botState.myId).ToList();
        List<Squad> enemySquads = new List<Squad>(); // Array.FindAll(state.Entities.Squads, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();
        List<EntityId> enemyUnits = new List<EntityId>();

        object enemySquadsLock = new object(); // Puts a lock around enemySquads while checking them

        List<TokenSlot> emptyOrbs = new List<TokenSlot>(); // Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == null).ToList();
        List<PowerSlot> emptyWells = new List<PowerSlot>(); // Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == null).ToList();
        List<BarrierSet> emptyWalls = new List<BarrierSet>(); // Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == null).ToList();
        List<BarrierModule> emptyWallModules = new List<BarrierModule>(); // Array.FindAll(state.Entities.BarrierSets, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId))).ToList();

        DeckOfficialCardIds? myCurrentDeckOfficialCardIds = null; // myDeckOfficialCardIds.FirstOrDefault(d => d.Name == botState.selectedDeck.Name) ?? myDeckOfficialCardIds[0];
        List<SMJCard?> myCurrentSMJCards = new List<SMJCard>(); // List of all card info for current chosen deck

        int? wreckerCardPosition = null; // Is there a wrecker card in the deck?
        List<int>? swiftCardPositions = null; // GetCardPositionsBasedOnAbilityFromDeck(myCurrentDeckOfficialCardIds, 1); // Which Cards in Deck are ability swift
        List<int>? archerCardPositions = null; // GetArcherCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which Cards in Deck are archers
        List<int>? towerCardPositions = null; // GetTowerCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1);
        List<int>? spellCardPositions = null; // GetSpellCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1);
        List<int>? longRangeCardPositions = null; // GetLongRangeCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1);
        int wallBreakerCardPosition = -1; // Does deck have special wallbreaker card? 

        float nearestBarrierDistance = float.MaxValue; // Is there a wall by my first orb?
        float buildWallCost = 50f; // NGE05012024!!!!!! How can I get the cost to build the wall?
        float enemyNearOrbDistance = 200; // 170; // Distance to use to decide when to build a wall near my orb - 170 allowed swift squad to get to orb before I built wall
        float engageEnemyNearOrbDistance = 80; // Distance to use to decide when to attack an enemy near my orb
        float maxPowerToDoMore = 200;

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

        TokenSlot? startingOrb = null;
        TokenSlot? startingOrbEnemy = null;

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

        string previousMessageAttackNearbyEnemyType = "";
        private Command? AttackNearbyEnemy(EntityId target, string targetType, List<Squad> squads, Position2D pos, int unitsNeededBeforeAttack)
        {
            int squadSize = 0;
            if (squads.Count > 0)
            {
                Squad squad = squads[0];
                squadSize = squad.Figures.Length;
                if (squads.Count < unitsNeededBeforeAttack)
                {
                    string msg = string.Format("Army must have {0} squads to attack. Army has {1} squads", unitsNeededBeforeAttack, squads.Count);
                    ConsoleWriteLine(consoleWriteline, msg);
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
                string message = string.Format("Attack target:{0} type:{1} with {2} squad(s)", target.V, targetType, squads.Count);
                // string message = string.Format("Attack target:{0} type:{1} at pos X,Y:{2},{3} with {4} squad(s)", target.V, targetType, (int)pos.X, (int)pos.Y, squads.Count);
                if (message != previousMessageAttackNearbyEnemyType)
                {
                    previousMessageAttackNearbyEnemyType = message;
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

        private void GetAttackingSquadsCloserToEnemyThanTarget(List<Squad> enemyAttackingSquads, List<Squad> myAttackingSquads, Entity target, out List<Squad>? engageEnemyAttackingSquads, out List<Squad>? myAttackingSquadsUpdated)
        {
            engageEnemyAttackingSquads = null;
            myAttackingSquadsUpdated = null;
            float dTarget = float.MaxValue;
            float dEnemy = float.MaxValue;
            foreach (Squad sq in myAttackingSquads)
            {
                foreach (Squad sqEnemy in enemyAttackingSquads)
                {
                    dTarget = DistanceSquared(sq.Entity.Position.To2D(), target.Position.To2D());
                    dEnemy = DistanceSquared(sq.Entity.Position.To2D(), sqEnemy.Entity.Position.To2D());
                    if (dTarget < dEnemy)
                    {
                        if (myAttackingSquadsUpdated == null)
                        {
                            myAttackingSquadsUpdated = new List<Squad>();
                        }
                        if (myAttackingSquadsUpdated.Contains(sq) == false)
                        {
                            myAttackingSquadsUpdated.Add(sq);
                        }
                    }
                    else
                    {
                        if (engageEnemyAttackingSquads == null)
                        {
                            engageEnemyAttackingSquads = new List<Squad>();
                        }
                        if (engageEnemyAttackingSquads.Contains(sq) == false)
                        {
                            engageEnemyAttackingSquads.Add(sq);
                        }
                    }
                }
            }
            if (myAttackingSquadsUpdated != null && engageEnemyAttackingSquads != null) // Exclude overlapping squads between the groups!
            {
                myAttackingSquadsUpdated = myAttackingSquadsUpdated.Except(engageEnemyAttackingSquads).ToList();
            }
        }

        private void GetAttackingSquadsCloserToWellThanTarget(List<PowerSlot> enemyWells, List<Squad> myAttackingSquads, Entity target, out List<Squad>? engageEnemyAttackingSquads, out List<Squad>? myAttackingSquadsUpdated)
        {
            engageEnemyAttackingSquads = null;
            myAttackingSquadsUpdated = null;
            float dTarget = float.MaxValue;
            float dEnemy = float.MaxValue;
            foreach (Squad sq in myAttackingSquads)
            {
                foreach (PowerSlot sqEnemy in enemyWells)
                {
                    dTarget = DistanceSquared(sq.Entity.Position.To2D(), target.Position.To2D());
                    dEnemy = DistanceSquared(sq.Entity.Position.To2D(), sqEnemy.Entity.Position.To2D());
                    if (dTarget < dEnemy)
                    {
                        if (myAttackingSquadsUpdated == null)
                        {
                            myAttackingSquadsUpdated = new List<Squad>();
                        }
                        if (myAttackingSquadsUpdated.Contains(sq) == false)
                        {
                            myAttackingSquadsUpdated.Add(sq);
                        }
                    }
                    else
                    {
                        if (engageEnemyAttackingSquads == null)
                        {
                            engageEnemyAttackingSquads = new List<Squad>();
                        }
                        if (engageEnemyAttackingSquads.Contains(sq) == false)
                        {
                            engageEnemyAttackingSquads.Add(sq);
                        }
                    }
                }
            }
            if (myAttackingSquadsUpdated != null && engageEnemyAttackingSquads != null) // Exclude overlapping squads between the groups!
            {
                myAttackingSquadsUpdated = myAttackingSquadsUpdated.Except(engageEnemyAttackingSquads).ToList();
            }
        }

        private void GetAttackingSquadsCloserToWellThanTarget(List<TokenSlot> enemyOrbs, List<Squad> myAttackingSquads, Entity target, out List<Squad>? engageEnemyAttackingSquads, out List<Squad>? myAttackingSquadsUpdated)
        {
            engageEnemyAttackingSquads = null;
            myAttackingSquadsUpdated = null;
            float dTarget = float.MaxValue;
            float dEnemy = float.MaxValue;
            foreach (Squad sq in myAttackingSquads)
            {
                foreach (TokenSlot sqEnemy in enemyOrbs)
                {
                    dTarget = DistanceSquared(sq.Entity.Position.To2D(), target.Position.To2D());
                    dEnemy = DistanceSquared(sq.Entity.Position.To2D(), sqEnemy.Entity.Position.To2D());
                    if (dTarget < dEnemy)
                    {
                        if (myAttackingSquadsUpdated == null)
                        {
                            myAttackingSquadsUpdated = new List<Squad>();
                        }
                        if (myAttackingSquadsUpdated.Contains(sq) == false)
                        {
                            myAttackingSquadsUpdated.Add(sq);
                        }
                    }
                    else
                    {
                        if (engageEnemyAttackingSquads == null)
                        {
                            engageEnemyAttackingSquads = new List<Squad>();
                        }
                        if (engageEnemyAttackingSquads.Contains(sq) == false)
                        {
                            engageEnemyAttackingSquads.Add(sq);
                        }
                    }
                }
            }
            if (myAttackingSquadsUpdated != null && engageEnemyAttackingSquads != null) // Exclude overlapping squads between the groups!
            {
                myAttackingSquadsUpdated = myAttackingSquadsUpdated.Except(engageEnemyAttackingSquads).ToList();
            }
        }

        public Command? OpenGateWhenNeeded(Tick currentTick)
        {
            Command? cmd = null;
            if (myAttackSquads != null && myAttackSquads.Count() > 0 && myWalls.Count() > 0)
            {
                #region Open Gate when appropriate
                // If gate is closed and any attack squads are inside gate, open it
                {
                    Position2D wallPos = myWalls[0].Entity.Position.To2D();
                    Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };

                    Position2D orbPos = startingOrb.Entity.Position.To2D(); // myOrbs[0].Entity.Position.To2D();

                    // Position2D orbPosInt = new Position2D { X = (int)myOrbs[0].Entity.Position.X, Y = (int)myOrbs[0].Entity.Position.Y };
                    Position2D orbPosInt = new Position2D { X = (int)startingOrb.Entity.Position.X, Y = (int)startingOrb.Entity.Position.Y };
                    Position2D mySqadPos = new Position2D { X = 0, Y = 0 };
                    Position2D mySqadPosInt = new Position2D { X = 0, Y = 0 };

                    bool openGate = false;
                    foreach (var squad in myAttackSquads)
                    {
                        mySqadPos = squad.Entity.Position.To2D();
                        mySqadPosInt = new Position2D { X = (int)mySqadPos.X, Y = (int)mySqadPos.Y };
                        // Squad is between orb and wall
                        if (mySqadPosInt != wallPos && mySqadPosInt != orbPosInt &&
                            mySqadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySqadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                            mySqadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySqadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
                        {
                            openGate = true;
                            string message = string.Format("SquadId{0} still inside, so open gate!", squad.Entity.Id);
                            ConsoleWriteLine(consoleWriteline, message);
                            break;
                        }
                    }

                    if (openGate == true)
                    {
                        cmd = ToggleGate(true, currentTick.V);
                        //if (cmd != null)
                        //{
                        //    commands.Add(cmd); // return new Command[] { cmd };
                        //}
                    }
                }
                #endregion Open Gate when appropriate

                #region Close Gate when appropriate
                // If gate is open and all squads are outside gate, close it
                {
                    // Turn all positions to int
                    Position2D wallPos = myWalls[0].Entity.Position.To2D();
                    Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                    Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                    Position2D orbPosInt = new Position2D { X = (int)orbPos.X, Y = (int)orbPos.Y };
                    Position2D mySqadPos = new Position2D { X = 0, Y = 0 };
                    Position2D mySqadPosInt = new Position2D { X = 0, Y = 0 };

                    bool closeGate = true;
                    foreach (var squad in myAttackSquads)
                    {
                        mySqadPos = squad.Entity.Position.To2D();
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
                        cmd = ToggleGate(false, currentTick.V);
                        //if (cmd != null)
                        //{
                        //    commands.Add(cmd); // return new Command[] { cmd };
                        //}
                    }
                }
                #endregion Close Gate when appropriate

            }
            return cmd;
        }

        string previousMessageAttackType = "";
        private Command? Attack(EntityId target, string targetType, List<Squad> squads, Position2D pos, int unitsNeededBeforeAttack)
        {
            int squadSize = 0;
            if (squads.Count > 0)
            {
                Squad squad = squads[0];
                squadSize = squad.Figures.Length;
                if (squads.Count < unitsNeededBeforeAttack)
                {
                    string msg = string.Format("Army must have {0} squads to attack. Army has {1} squads", unitsNeededBeforeAttack, squads.Count);
                    ConsoleWriteLine(consoleWriteline, msg);
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
                string message = string.Format("Attack target:{0} type:{1} with {2} squad(s)", target.V, targetType, squads.Count);
                // string message = string.Format("Attack target:{0} type:{1} at pos X,Y:{2},{3} with {4} squad(s)", target.V, targetType, (int)pos.X, (int)pos.Y, squads.Count);
                if (message != previousMessageAttackType)
                {
                    previousMessageAttackType = message;
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
                    string msg = string.Format("Army must have {0} squads to attack. Army has {1} squads", unitsNeededBeforeAttack, squads.Count);
                    ConsoleWriteLine(consoleWriteline, msg);
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
                string message = string.Format("Attack target:{0}  with {1} squads", target.V, squads.Count);
                // string message = string.Format("Attack target:{0}  at pos X,Y:{1},{2} with {3} squads", target.V, (int)pos.X, (int)pos.Y, squads.Count);
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
        private Command? SpawnUnit(float myPower, Position2D pos, byte cardPosition, int unitPower, uint tick, ref float powerRemaining)
        {
            if (myPower >= unitPower && botState.canPlayCardAt < tick)
            {
                //string message = string.Format("CommandProduceSquad CardPosition:{0} X,Y:{1},{2} Power:{3} UnitPower:{4}", cardPosition, (int)pos.X, (int)pos.Y, (int)myPower, unitPower);
                // NGE05222024 string message = string.Format("CommandProduceSquad CardPosition:{0} X,Y:{1},{2} UnitPower:{3}", cardPosition, (int)pos.X, (int)pos.Y, unitPower);
                string message = string.Format("CommandProduceSquad CardPosition:{0} UnitPower:{1}", cardPosition, unitPower);
                if (message != previousMessageSpawnUnit)
                {
                    ConsoleWriteLine(consoleWriteline, message);
                }
                powerRemaining = myPower - unitPower;

                botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
                return new CommandProduceSquad { CardPosition = cardPosition, Xy = pos };
                // NGE05222024!!!!!}
                // NGE05222024!!!!!else
                // NGE05222024!!!!!{
                // NGE05222024!!!!!    return null;
                // NGE05222024!!!!!}
            }
            else if (myPower < unitPower)
            {
                return null;
            }
            else if (botState.canPlayCardAt >= tick)
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
                string message = string.Format("CommandProduceSquadOnBarrier CardPosition:{0} X,Y:{1},{2}", cardPosition, (int)pos.X, (int)pos.Y);
                ConsoleWriteLine(consoleWriteline, message);
                powerRemaining = myPower - unitPower;

                botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
                return new CommandProduceSquadOnBarrier { CardPosition = cardPosition, Xy = pos, BarrierToMount = barrierModuleId };
            }
            else if (myPower < unitPower)
            {
                return null;
            }
            else if (botState.canPlayCardAt >= tick)
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

        private SMJCard[]? cardsSMJ; // Instances.CardService.GetSMJCardList();

        private List<CreateOrbColor> orbColorBuildOrder = new List<CreateOrbColor>(); // Determine orb build order based on current deck at PrepareForBattle
        private Dictionary<EntityId, Orb> orbs = new Dictionary<EntityId, Orb>();
        private Dictionary<EntityId, Well> wells = new Dictionary<EntityId, Well>();

        #endregion Info to track
        public List<CreateOrbColor> GetOrbColorBuildOrder(SMJCard? card)
        {
            List<CreateOrbColor> orbColorTierOrder = new List<CreateOrbColor>();
            int orbsFire = 0;
            int orbsShadow = 0;
            int orbsNature = 0;
            int orbsFrost = 0;

            if (card != null)
            {
                if (card.type == 0 && card.orbsTotal == 1 && orbColorTierOrder.Count() == 0) // // 0 - Unit, 1 - Building, 2 - Spell
                {
                    if (card.orbsFire == 1)
                    {
                        orbsFire++;
                        orbColorTierOrder.Add(CreateOrbColor.Fire);
                    }
                    else if (card.orbsShadow == 1)
                    {
                        orbsShadow++;
                        orbColorTierOrder.Add(CreateOrbColor.Shadow);
                    }
                    else if (card.orbsNature == 1)
                    {
                        orbsNature++;
                        orbColorTierOrder.Add(CreateOrbColor.Nature);
                    }
                    else if (card.orbsFrost == 1)
                    {
                        orbsFrost++;
                        orbColorTierOrder.Add(CreateOrbColor.Frost);
                    }
                    //else if (card.orbsNeutral == 1)
                    //{
                    //    //orbsNature++;
                    //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                    //}
                    if (orbColorTierOrder.Count() > 0)
                    {
                        Console.WriteLine("Tier1 Orb color:{0}", orbColorTierOrder[0]);
                    }
                }
                else if (card.orbsTotal == 2 && orbColorTierOrder.Count() < 2 && orbColorTierOrder.Count() <= 1)
                {
                    if (card.orbsFire >= 1 && orbsFire < card.orbsFire)
                    {
                        orbsFire++;
                        orbColorTierOrder.Add(CreateOrbColor.Fire);
                    }
                    else if (card.orbsShadow >= 1 && orbsShadow < card.orbsShadow)
                    {
                        orbsShadow++;
                        orbColorTierOrder.Add(CreateOrbColor.Shadow);
                    }
                    else if (card.orbsNature >= 1 && orbsNature < card.orbsNature)
                    {
                        orbsNature++;
                        orbColorTierOrder.Add(CreateOrbColor.Nature);
                    }
                    else if (card.orbsFrost >= 1 && orbsFrost < card.orbsFrost)
                    {
                        orbsFrost++;
                        orbColorTierOrder.Add(CreateOrbColor.Frost);
                    }
                    //else if (card.orbsNeutral >= 1)
                    //{
                    //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                    //}
                    if (orbColorTierOrder.Count() > 1)
                    {
                        Console.WriteLine("Tier2 Orb color:{0}", orbColorTierOrder[1]);
                    }
                }
                else if (card.orbsTotal == 3 && orbColorTierOrder.Count() < 3 && orbColorTierOrder.Count() <= 2)
                {
                    if (card.orbsFire >= 1 && orbsFire < card.orbsFire)
                    {
                        orbsFire++;
                        orbColorTierOrder.Add(CreateOrbColor.Fire);
                    }
                    else if (card.orbsShadow >= 1 && orbsShadow < card.orbsShadow)
                    {
                        orbsShadow++;
                        orbColorTierOrder.Add(CreateOrbColor.Shadow);
                    }
                    else if (card.orbsNature >= 1 && orbsNature < card.orbsNature)
                    {
                        orbsNature++;
                        orbColorTierOrder.Add(CreateOrbColor.Nature);
                    }
                    else if (card.orbsFrost >= 1 && orbsFrost < card.orbsFrost)
                    {
                        orbsFrost++;
                        orbColorTierOrder.Add(CreateOrbColor.Frost);
                    }
                    //else if (card.orbsNeutral >= 1)
                    //{
                    //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                    //}
                    if (orbColorTierOrder.Count() > 2)
                    {
                        Console.WriteLine("Tier3 Orb color:{0}", orbColorTierOrder[2]);
                    }
                }
                else if (card.orbsTotal == 4 && orbColorTierOrder.Count() < 4 && orbColorTierOrder.Count() <= 3)
                {
                    if (card.orbsFire >= 1 && orbsFire < card.orbsFire)
                    {
                        orbsFire++;
                        orbColorTierOrder.Add(CreateOrbColor.Fire);
                    }
                    else if (card.orbsShadow >= 1 && orbsShadow < card.orbsShadow)
                    {
                        orbsShadow++;
                        orbColorTierOrder.Add(CreateOrbColor.Shadow);
                    }
                    else if (card.orbsNature >= 1 && orbsNature < card.orbsNature)
                    {
                        orbsNature++;
                        orbColorTierOrder.Add(CreateOrbColor.Nature);
                    }
                    else if (card.orbsFrost >= 1 && orbsFrost < card.orbsFrost)
                    {
                        orbsFrost++;
                        orbColorTierOrder.Add(CreateOrbColor.Frost);
                    }
                    //else if (card.orbsNeutral >= 1)
                    //{
                    //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                    //}
                    if (orbColorTierOrder.Count() > 3)
                    {
                        Console.WriteLine("Tier3 Orb color:{0}", orbColorTierOrder[3]);
                    }
                }
            }

            return orbColorTierOrder;
        }

        public Squad? GetSquadNearestToEnemyOrb(List<Squad>? attackingSquad)
        {
            Squad? nearestSquadToOrb = null;
            if (attackingSquad != null && attackingSquad.Count > 0 && closestTokenSlotPosition != null)
            {
                float closestSquadToOrb = float.MaxValue;
                float d = float.MaxValue;
                foreach (Squad squad in attackingSquad)
                {
                    d = DistanceSquared(closestTokenSlotPosition, squad.Entity.Position);
                    if (closestTokenSlotDistanceSq > d)
                    {
                        closestTokenSlotDistanceSq = d;
                        nearestSquadToOrb = squad;
                    }
                }
            }
            return nearestSquadToOrb;
        }

        public List<CreateOrbColor> GetOrbColorBuildOrder()
        {
            List<CreateOrbColor> orbColorTierOrder = new List<CreateOrbColor>();
            // Get all T1 cards from deck and make sure at least 1 is a squad. If in deck position 0, then use that orb color
            // Get all T2 cards from deck
            // Get all T3 cards from deck
            // Get all T4 cards from deck

            int orbsFire = 0;
            int orbsShadow = 0;
            int orbsNature = 0;
            int orbsFrost = 0;

            for (int c = 0; c < myCurrentDeckOfficialCardIds.Ids.Length; c++)
            {
                int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[c];
                if (unitOfficialCardId != 0) // Not a card
                {
                    SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                    if (card != null)
                    {
                        if (card.type == 0 && card.orbsTotal == 1 && orbColorTierOrder.Count() == 0) // // 0 - Unit, 1 - Building, 2 - Spell
                        {
                            if (card.orbsFire == 1)
                            {
                                orbsFire++;
                                orbColorTierOrder.Add(CreateOrbColor.Fire);
                            }
                            else if (card.orbsShadow == 1)
                            {
                                orbsShadow++;
                                orbColorTierOrder.Add(CreateOrbColor.Shadow);
                            }
                            else if (card.orbsNature == 1)
                            {
                                orbsNature++;
                                orbColorTierOrder.Add(CreateOrbColor.Nature);
                            }
                            else if (card.orbsFrost == 1)
                            {
                                orbsFrost++;
                                orbColorTierOrder.Add(CreateOrbColor.Frost);
                            }
                            //else if (card.orbsNeutral == 1)
                            //{
                            //    //orbsNature++;
                            //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                            //}
                            if (orbColorTierOrder.Count() > 0)
                            {
                                Console.WriteLine("Tier1 Orb color:{0}", orbColorTierOrder[0]);
                            }
                        }
                        else if (card.orbsTotal == 2 && orbColorTierOrder.Count() < 2 && orbColorTierOrder.Count() <= 1)
                        {
                            if (card.orbsFire >= 1 && orbsFire < card.orbsFire)
                            {
                                orbsFire++;
                                orbColorTierOrder.Add(CreateOrbColor.Fire);
                            }
                            else if (card.orbsShadow >= 1 && orbsShadow < card.orbsShadow)
                            {
                                orbsShadow++;
                                orbColorTierOrder.Add(CreateOrbColor.Shadow);
                            }
                            else if (card.orbsNature >= 1 && orbsNature < card.orbsNature)
                            {
                                orbsNature++;
                                orbColorTierOrder.Add(CreateOrbColor.Nature);
                            }
                            else if (card.orbsFrost >= 1 && orbsFrost < card.orbsFrost)
                            {
                                orbsFrost++;
                                orbColorTierOrder.Add(CreateOrbColor.Frost);
                            }
                            //else if (card.orbsNeutral >= 1)
                            //{
                            //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                            //}
                            if (orbColorTierOrder.Count() > 1)
                            {
                                Console.WriteLine("Tier2 Orb color:{0}", orbColorTierOrder[1]);
                            }
                        }
                        else if (card.orbsTotal == 3 && orbColorTierOrder.Count() < 3 && orbColorTierOrder.Count() <= 2)
                        {
                            if (card.orbsFire >= 1 && orbsFire < card.orbsFire)
                            {
                                orbsFire++;
                                orbColorTierOrder.Add(CreateOrbColor.Fire);
                            }
                            else if (card.orbsShadow >= 1 && orbsShadow < card.orbsShadow)
                            {
                                orbsShadow++;
                                orbColorTierOrder.Add(CreateOrbColor.Shadow);
                            }
                            else if (card.orbsNature >= 1 && orbsNature < card.orbsNature)
                            {
                                orbsNature++;
                                orbColorTierOrder.Add(CreateOrbColor.Nature);
                            }
                            else if (card.orbsFrost >= 1 && orbsFrost < card.orbsFrost)
                            {
                                orbsFrost++;
                                orbColorTierOrder.Add(CreateOrbColor.Frost);
                            }
                            //else if (card.orbsNeutral >= 1)
                            //{
                            //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                            //}
                            if (orbColorTierOrder.Count() > 2)
                            {
                                Console.WriteLine("Tier3 Orb color:{0}", orbColorTierOrder[2]);
                            }
                        }
                        else if (card.orbsTotal == 4 && orbColorTierOrder.Count() < 4 && orbColorTierOrder.Count() <= 3)
                        {
                            if (card.orbsFire >= 1 && orbsFire < card.orbsFire)
                            {
                                orbsFire++;
                                orbColorTierOrder.Add(CreateOrbColor.Fire);
                            }
                            else if (card.orbsShadow >= 1 && orbsShadow < card.orbsShadow)
                            {
                                orbsShadow++;
                                orbColorTierOrder.Add(CreateOrbColor.Shadow);
                            }
                            else if (card.orbsNature >= 1 && orbsNature < card.orbsNature)
                            {
                                orbsNature++;
                                orbColorTierOrder.Add(CreateOrbColor.Nature);
                            }
                            else if (card.orbsFrost >= 1 && orbsFrost < card.orbsFrost)
                            {
                                orbsFrost++;
                                orbColorTierOrder.Add(CreateOrbColor.Frost);
                            }
                            //else if (card.orbsNeutral >= 1)
                            //{
                            //    orbColorTierOrder.Add(CreateOrbColor.Nature); // ??????
                            //}
                            if (orbColorTierOrder.Count() > 3)
                            {
                                Console.WriteLine("Tier3 Orb color:{0}", orbColorTierOrder[3]);
                            }
                        }
                    }
                }
            }
            return orbColorTierOrder;
        }

        public Deck[] DecksForMap(Maps map, string? name, ulong crc)
        {
            if (SUPPORTED_MAPS.Contains(map))
            {
                return new[] {
                    PvPCardDecks.GiftedFlameNew, PvPCardDecks.GiftedFlame, TopDeck, BattleGrounds, FireNature,
                    PvPCardDecks.TaintedDarkness, PvPCardDecks.GiftedDarkness, PvPCardDecks.BlessedDarkness, PvPCardDecks.InfusedDarkness,
                    PvPCardDecks.TaintedFlora, PvPCardDecks.GiftedFlora, PvPCardDecks.BlessedFlora, PvPCardDecks.InfusedFlora,
                    PvPCardDecks.TaintedIce, PvPCardDecks.GiftedIce, PvPCardDecks.BlessedIce, PvPCardDecks.InfusedIce,
                    PvPCardDecks.TaintedFlame, PvPCardDecks.BlessedFlame, PvPCardDecks.InfusedFlame,
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
            MatchPlayer? yourPlayer = null;
            foreach (var player in state.Players)
            {
                if (player.Entity.Team != botState.myTeam)
                {
                    botState.oponents.Add(player.Entity.Id);
                }
                if (player.Entity.Id == yourPlayerId)
                {
                    yourPlayer = player;
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

            buildArcherOnWallAtStartWasTrue = buildArcherOnWallAtStart;

            // Get the deck cardIds from the matching currentDeck.Name
            myCurrentDeckOfficialCardIds = myDeckOfficialCardIds.FirstOrDefault(d => d.Name == botState.selectedDeck.Name) ?? myDeckOfficialCardIds[0];
            wreckerCardPosition = GetWreckerCardPositionFromDeck(myCurrentDeckOfficialCardIds, 1); // Which Card in Deck is a wrecker            
            swiftCardPositions = GetCardPositionsBasedOnAbilityFromDeck(myCurrentDeckOfficialCardIds, "Swift", 1); // Which Cards in Deck are archers            
            archerCardPositions = GetArcherCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which Cards in Deck are archers            
            towerCardPositions = GetTowerCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which cards are defense Towers example Stranglehold, Primeval Defender
            spellCardPositions = GetSpellCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which cards are defense Spess example Mine, Wallbreaker
            longRangeCardPositions = GetLongRangeCardPositionsFromDeck(myCurrentDeckOfficialCardIds, 1); // Which cards are defense Spess example Mine, Wallbreaker

            myCurrentSMJCards = new List<SMJCard?>();
            for (int i = 0; i < myCurrentDeckOfficialCardIds.Ids.Length; i++)
            {
                myCurrentSMJCards.Add(GetCardFromOfficialCardId(cardsSMJ, myCurrentDeckOfficialCardIds.Ids[i]));
            }
            orbColorBuildOrder = GetOrbColorBuildOrder(); // Based on the deck, what orb should I build next

            for (int i = 0; i < myCurrentDeckOfficialCardIds.Ids.Length; i++)
            {
                if (myCurrentDeckOfficialCardIds.Ids[i] == (int)Api.CardTemplate.Wallbreaker)
                {
                    wallBreakerCardPosition = i;
                    ConsoleWriteLine(consoleWriteline, $"Deck has Wallbreaker spell at pos:{i}");
                    break;
                }
            }

            for (int i = 0; i < myCurrentDeckOfficialCardIds.Ids.Length; i++)
            {
                if (myCurrentDeckOfficialCardIds.Ids[i] == (int)Api.CardTemplate.Wrecker)
                {
                    wreckerCardPosition = i;
                    ConsoleWriteLine(consoleWriteline, $"Deck has Wrecker unit at pos:{i}");
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

            var myPower = yourPlayer.Entity.Power;
            TokenSlot[] myOrbs = Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == botState.myId);
            startingOrb = myOrbs[0];

            TokenSlot[] enemyOrbs = Array.FindAll(state.Entities.TokenSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
            if (enemyOrbs != null)
                startingOrbEnemy = enemyOrbs[0];

            #region Get power cost of first unit in deck
            // If we have power greater than the cost of the unit, we want to create the unit
            // Deck CardId is not OfficialCardId!!!
            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[0]; // Starting unit for AI

            // Console.WriteLine("Tick CardID:{0}", unitOfficialCardId);
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
            #endregion Get power cost of first unit in deck
        }

        public Command[] Tick(GameState state)
        {
            List<Command> commands = new List<Command>();
            string deckName = botState.selectedDeck.Name;
            currentDeck = myDecks.FirstOrDefault(d => d.Name == deckName);
            if (currentDeck != null)
            {
                Command[] tempCommands = SendCommands(state, (Deck)currentDeck);
                if (botState.canPlayCardAt == uint.MaxValue && tempCommands.Length == 0)
                {
                    var player = Array.Find(state.Players, p => p.Id == botState.myId);
                    if (player == null) { return commands.ToArray(); }
                    var myPower = player.Power;
                    ConsoleWriteLine(true, $"Tick {state.CurrentTick} commands: {tempCommands.Length} myPower: {(int)myPower} botState.canPlayCardAt == uint.MaxValue");
                    botState.canPlayCardAt = 0; // NGE07042024 Reset the timer to try and continue
                }
                return tempCommands;
                // NGE07042024 return SendCommands(state, (Deck)currentDeck);
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
            ConsoleWriteLine(consoleWriteline, $"My player ID is: {yourPlayerId}, I have deck: {botState.selectedDeck.Name}, and I own:");
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
                    ConsoleWriteLine(consoleWriteline, $"Well slot:{s.Entity.Id} at {(int)s.Entity.Position.X},{(int)s.Entity.Position.Z}");
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
                    ConsoleWriteLine(consoleWriteline, $"Team: {p} at {(int)team.Players[p].StartPos.X}:{(int)team.Players[p].StartPos.Y}");
                }
            }
            if (opponents != null)
            {
                List<EntityId> keyList = new List<EntityId>(opponents.Players.Keys);
                foreach (EntityId p in keyList)
                {
                    ConsoleWriteLine(consoleWriteline, $"Opponent: {p} at {(int)opponents.Players[p].StartPos.X}:{(int)opponents.Players[p].StartPos.Y}");
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

        public List<int>? GetCardHealth(int cardPositionInDeck)
        {
            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPositionInDeck];
            if (unitOfficialCardId != 0) // Not a card
            {
                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                if (card != null)
                {
                    return card.health;
                }
            }
            return null;
        }

        public List<int>? GetCardDamage(int cardPositionInDeck)
        {
            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPositionInDeck];
            if (unitOfficialCardId != 0) // Not a card
            {
                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                if (card != null)
                {
                    return card.damage;
                }
            }
            return null;
        }

        public void GetSquadHealth(GameState state, Squad squad, out double health, out List<EntityId> unitsHealthy)
        {
            unitsHealthy = new List<EntityId>();
            health = 0; // Get health of units

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

        private void GetClosestEnemy(Position2D pos, out Squad? nearestEnemy, out float enemyDistance)
        {
            float d;
            float enemyDistanceSq = float.MaxValue;
            nearestEnemy = null;

            try
            {
                lock (enemySquadsLock)
                {
                    foreach (var s in enemySquads)
                    {
                        d = DistanceSquared(pos, s.Entity.Position);
                        if (enemyDistanceSq > d) // Closest empty wall 
                        {
                            enemyDistanceSq = d;
                            nearestEnemy = s;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
            }
            enemyDistance = MathF.Sqrt(enemyDistanceSq);
        }

        // If gate is down or a barrier module has 0 health, then I can skip attacking the barrier
        public bool BreachBarrierSetModules(GameState state, List<BarrierSet> barriers, List<BarrierModule> modules)
        {
            bool breachBarrierModule = false;

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

        // If gate is down or a barrier module has 0 health, then I can skip attacking the barrier
        public bool BreachBarrierSetModules(GameState state, BarrierSet barrier, List<BarrierModule> modules)
        {
            bool breachBarrierModule = false;

            // NOTE: For now just use the first barrier!!!!!!
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

            return breachBarrierModule;
        }


        // NGE05222024 For now if squad is an archer, do not include it - since they are typically on a wall !!!!!
        public Squad? NearestSquadToEnemy(Entity enemy)
        {
            Squad nearestSquad = null;
            if (myAttackSquads != null && myAttackSquads.Count > 0)
            {
                float nearestSquadDistance = float.MaxValue;
                foreach (var s in myAttackSquads) // if squad is near Entity, attack it
                {
                    if (archerCardPositions != null)
                    {
                        if (s.CardId == currentDeck.Cards[(byte)archerCardPositions[0]])
                        {
                            continue;
                        }
                    }
                    float squadDistanceToEntity = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), enemy.Position.To2D()));
                    if (squadDistanceToEntity < nearestSquadDistance)
                    {
                        nearestSquadDistance = squadDistanceToEntity;
                        nearestSquad = s;
                    }
                }
            }
            return nearestSquad;
        }

        // NGE05222024 For now if squad is an archer, do not include it - since they are typically on a wall !!!!!
        public Squad? NearestSquadToEnemy(List<Squad> enemy)
        {
            Squad nearestSquad = null;
            if (myAttackSquads != null && myAttackSquads.Count > 0)
            {
                float nearestSquadDistance = float.MaxValue;
                foreach (var s in myAttackSquads) // if squad is near Entity, attack it
                {
                    if (archerCardPositions != null)
                    {
                        if (s.CardId == currentDeck.Cards[(byte)archerCardPositions[0]])
                        {
                            continue;
                        }
                    }
                    foreach (var e in enemy) // if squad is near Entity, attack it
                    {
                        float squadDistanceToEntity = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), e.Entity.Position.To2D()));
                        if (squadDistanceToEntity < nearestSquadDistance)
                        {
                            nearestSquadDistance = squadDistanceToEntity;
                            nearestSquad = s;
                        }
                    }
                }
            }
            return nearestSquad;
        }

        public List<Squad>? NearestSquadsToEnemy(List<Squad> enemy, float distanceMax = 150f)
        {
            List<Squad>? nearbySquads = null;
            if (myAttackSquads != null && myAttackSquads.Count > 0)
            {
                foreach (var s in myAttackSquads) // if squad is near Entity, attack it
                {
                    if (archerCardPositions != null)
                    {
                        if (s.CardId == currentDeck.Cards[(byte)archerCardPositions[0]])
                        {
                            continue;
                        }
                    }
                    foreach (var e in enemy) // if squad is near Entity, attack it
                    {
                        float squadDistanceToEntity = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), e.Entity.Position.To2D()));
                        if (squadDistanceToEntity < distanceMax)
                        {
                            if (nearbySquads == null)
                            {
                                nearbySquads = new List<Squad>();
                            }
                            if (nearbySquads.Contains(s) == false)
                            {
                                nearbySquads.Add(s);
                            }
                        }
                    }
                }
            }
            return nearbySquads;
        }

        // See if a squad, orb, well, or building is near my base; if so, attack it!
        public bool IsEnemyNearBase(float distanceFromBase)
        {
            Squad? enemySquad = null;
            if (myOrbs.Count > 0)
            {
                Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                try
                {
                    lock (enemySquadsLock)
                    {
                        foreach (var s in enemySquads) // if squad is near me attack it
                        {
                            float enemySquadDistanceToOrb = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), orbPos));
                            if (enemySquadDistanceToOrb < distanceFromBase)
                            {
                                enemySquad = s;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                }
            }
            return false;
        }

        public bool IsEnemyNearBase(float distanceFromBase, out Squad? enemySquad)
        {
            enemySquad = null;
            if (myOrbs.Count > 0)
            {
                Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                try
                {
                    lock (enemySquadsLock)
                    {
                        foreach (var s in enemySquads) // if squad is near me attack it
                        {
                            float enemySquadDistanceToOrb = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), orbPos));
                            if (enemySquadDistanceToOrb < distanceFromBase)
                            {
                                enemySquad = s;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                }
            }
            return false;
        }

        public bool IsEnemyNearBase(float distanceFromBase, out List<Squad>? enemySquad)
        {
            enemySquad = null;
            bool isEnemyNearBase = false;
            if (myOrbs.Count > 0)
            {
                Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                try
                {
                    lock (enemySquadsLock)
                    {
                        foreach (var s in enemySquads) // if squad is near me attack it
                        {
                            float enemySquadDistanceToOrb = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), orbPos));
                            if (enemySquadDistanceToOrb < distanceFromBase)
                            {
                                if (enemySquad == null)
                                {
                                    enemySquad = new List<Squad>();
                                }
                                enemySquad.Add(s);
                                isEnemyNearBase = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                }
            }
            return isEnemyNearBase;
        }

        public bool IsEnemyNearBase(float distanceFromBase, out PowerSlot? enemyOrb)
        {
            enemyOrb = null;
            if (myOrbs.Count > 0)
            {
                Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                foreach (var s in enemyWells) // if squad is near me attack it
                {
                    float enemySquadDistanceToOrb = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), orbPos));
                    if (enemySquadDistanceToOrb < distanceFromBase)
                    {
                        enemyOrb = s;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsEnemyNearBase(float distanceFromBase, out TokenSlot? enemyOrb)
        {
            enemyOrb = null;
            if (myOrbs.Count > 0)
            {
                Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                foreach (var s in enemyOrbs) // if squad is near me attack it
                {
                    float enemySquadDistanceToOrb = MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), orbPos));
                    if (enemySquadDistanceToOrb < distanceFromBase)
                    {
                        enemyOrb = s;
                        return true;
                    }
                }
            }
            return false;
        }

        float wellHealLimit = 200;

        string previousMessageRepairWell = "";
        public Command? RepairWell(PowerSlot well, float myPower, ref float powerRemaining)
        {
            // If a well has less than a certain % health, then repair the well
            float wellPowerCost = 100;

            var wellAspectHealth = well.Entity.Aspects.FirstOrDefault(h => h.Health != null) ?? null;
            // if (wellAspectHealth != null && wellAspectHealth.Health != null && wellAspectHealth.Health.CurrentHp < maxModuleHealth) // Not at 100% health
            if (wellAspectHealth != null && wellAspectHealth.Health != null && wellAspectHealth.Health.CurrentHp < wellAspectHealth.Health.CapCurrentMax) // Not at 100% health
            {
                string message = string.Format("Repair Well BuildingId:{0}", well.Entity.Id);
                if (previousMessageRepairWell != message)
                {
                    Console.WriteLine(message);
                    previousMessageRepairWell = message;
                }
                // botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
                powerRemaining = MathF.Max(0, myPower - wellPowerCost * (wellAspectHealth.Health.CapCurrentMax - wellAspectHealth.Health.CurrentHp) / wellAspectHealth.Health.CapCurrentMax); // NGE05222024 2000 Max health = 100 powerCost

                Command command = new CommandRepairBuilding
                {
                    BuildingId = well.Entity.Id,
                };

                return command;
            }
            else if (wellAspectHealth == null) // Rebuild well
            {
                Command command = new CommandPowerSlotBuild
                {
                    SlotId = well.Entity.Id
                };
                return command;
            }
            return null;
        }

        string previousMessageRepairOrb = "";
        public Command? RepairOrb(TokenSlot orb, float myPower, ref float powerRemaining)
        {
            // If a orb has less than a certain % health, then repair the orb
            float orbCost = GetOrbCost();

            var orbAspectHealth = orb.Entity.Aspects.FirstOrDefault(h => h.Health != null) ?? null;
            if (orbAspectHealth != null && orbAspectHealth.Health != null && orbAspectHealth.Health.CurrentHp < orbAspectHealth.Health.CapCurrentMax) // Not at 100% health
            {
                string message = string.Format("Repair Orb BuildingId:{0}", orb.Entity.Id);
                if (previousMessageRepairOrb != message)
                {
                    Console.WriteLine(message);
                    previousMessageRepairOrb = message;
                }
                // botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
                powerRemaining = MathF.Max(0, myPower - (orbCost * (orbAspectHealth.Health.CapCurrentMax - orbAspectHealth.Health.CurrentHp) / orbAspectHealth.Health.CapCurrentMax)); // NGE05222024 
                Command command = new CommandRepairBuilding
                {
                    BuildingId = orb.Entity.Id,
                };
                return command;
            }
            else if (orbAspectHealth == null) // Rebuild Orb
            {
                // float orbCost = GetOrbCost(); // Note: this may be 1 less since the orb is lost (now an empty orb anyone can claim)
                CreateOrbColor orbColor = orbColorBuildOrder[myOrbs.Count]; // CreateOrbColor.Fire;  

                Command command = new CommandTokenSlotBuild
                {
                    SlotId = orb.Entity.Id,
                    Color = orbColor
                };
                return command;
            }
            return null;
        }

        public SMJCard? GetCardFromCardId(Api.CardId cardId)
        {
            SMJCard? card = null;
            int position = GetCardPositionFromCardID(cardId);
            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[position];
            if (unitOfficialCardId != 0) // Not a card
            {
                card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
            }
            return card;
        }

        public int GetCardPositionFromCardID(Api.CardId cardId)
        {
            int position = 0;
            for (position = 0; position < currentDeck.Cards.Length; position++)
            {
                if (cardId == currentDeck.Cards[position])
                {
                    break;
                }
            }
            return position;
        }

        string previousMessageRepairBuilding = "";
        public Command? RepairBuilding(Building building, float myPower, ref float powerRemaining)
        {
            // If a building has less than a certain % health, then repair the building
            float buildingCost = 100; // !!!!!! Get info from card_id
            SMJCard? card = GetCardFromCardId(building.CardId);
            if (card != null)
            {
                buildingCost = card.powerCost[3];
            }
            var buildingAspectHealth = building.Entity.Aspects.FirstOrDefault(h => h.Health != null) ?? null;
            if (buildingAspectHealth != null && buildingAspectHealth.Health != null && buildingAspectHealth.Health.CurrentHp < buildingAspectHealth.Health.CapCurrentMax) // Not at 100% health
            {
                string message = string.Format("Repair BuildingId:{0}", building.Entity.Id);
                if (previousMessageRepairBuilding != message)
                {
                    Console.WriteLine(message);
                    previousMessageRepairBuilding = message;
                }
                // botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
                powerRemaining = MathF.Max(0, myPower - buildingCost * (buildingAspectHealth.Health.CapCurrentMax - buildingAspectHealth.Health.CurrentHp) / buildingAspectHealth.Health.CapCurrentMax); // NGE05222024 
                Command command = new CommandRepairBuilding
                {
                    BuildingId = building.Entity.Id,
                };

                return command;
            }
            return null;
        }

        public Command? RepairWall(BarrierSet barrier, List<BarrierModule> modules, float myPower, ref float powerRemaining)
        {
            // If a barrier module has less than a certain % health, then repair the barrier
            // NOTE: For now just use the first barrier!!!!!!
            // Barrier hit points per module is 200, so anything less than that is damaged
            //float maxModuleHealth = 200;

            var barrierAspectRepairBarrierSet = barrier.Entity.Aspects.FirstOrDefault(r => r.RepairBarrierSet != null) ?? null;

            if (barrierAspectRepairBarrierSet == null) // See if the barrier needs to be repaired
            {
                int barrierModuleCost = 10; // Not sure???
                foreach (var module in modules)
                {
                    if (module.Set.V == barrier.Entity.Id.V) // Part of the same Barrier
                    {
                        var moduleAspectHealth = module.Entity.Aspects.FirstOrDefault(h => h.Health != null) ?? null;
                        if (moduleAspectHealth == null) // This should mean a module is fully broken so I can attack something else!
                        {
                            string message = string.Format("Repair WallId:{0}", barrier.Entity.Id);
                            Console.WriteLine(message);
                            powerRemaining = MathF.Max(0, myPower - barrierModuleCost); // (moduleAspectHealth.Health.CapCurrentMax - moduleAspectHealth.Health.CurrentHp); // NGE05222024 
                            Command command = new CommandBarrierRepair
                            {
                                BarrierId = barrier.Entity.Id,
                            };

                            return command;
                        }
                    }
                }
            }

            return null;
        }

        /*
        public Command[] TickGood(GameState state) // NGE04292024 
        {
            var currentTick = state.CurrentTick;
            var entities = state.Entities;
            //var myArmy = new List<EntityId>();
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

            #region determine which units are healthy and add to myArmy // and perhaps check enemyArmy health???
            foreach (var squad in state.Entities.Squads)
            {
                double squadHealth = 0;
                if (squad.Entity.PlayerEntityId != botState.myId)
                    continue;
                GetSquadHealth(state, squad, out squadHealth, out List<EntityId> healthyUnits);
                if (squadHealth != 0)
                {
                    //myArmy.AddRange(healthyUnits);
                    myArmySquads.Add(squad);
                }
                if (squadHealth != previousSquadHealth)
                {
                    Debug.WriteLine($"My squad:{squad.Entity.Id} has hp:{squadHealth}");
                }

                previousSquadHealth = squadHealth;

                //Console.WriteLine($"My squad:{squad.Entity.Id} has hp:{squadHealth}");
            }
            #endregion determine which units are healthy and add to myArmy // and perhaps check enemyArmy health???

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
            if (currentTick.V % defaultTickUpdateRate == 0 && myArmySquads.Count > 0 && myArmySquads.Count != previousArmyCount)
            {
                Console.WriteLine($"Tick: {currentTick} target: {target} my power: {(int)myPower} my army size: {myArmySquads.Count}");
            }
            previousArmyCount = myArmySquads.Count;

            string deckName = "TopDeck";
            Deck? currentDeck = myDecks.FirstOrDefault(d => d.Name == deckName);
            // Get the deck cardIds from the matching currentDeck.Name
            DeckOfficialCardIds myCurrentDeckOfficialCardIds = myDeckOfficialCardIds.FirstOrDefault(d => d.Name == currentDeck.Name) ?? myDeckOfficialCardIds[0];
            // int unitPower = 75; // Nomad power cost - should be able to find info about card
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

            Position2D myArmyPos = botState.myStart; // Ideally place the army as close to center of map as possible to get places faster
            if (myArmySquads.Count > 0)
            {
                myArmyPos = myArmySquads[0].Entity.Position.To2D();
            }
            Command? spawn = null;
            if (myArmySquads.Count() < defaultAttackSquads)
            {
                spawn = SpawnUnit(myPower, myArmyPos, 0, unitPower, currentTick.V, ref myPower); // Spawn unit in squad next to other units
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
        */

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

        private Command? SpawnArcher(List<Squad> myArmy, float myPower, Position2D pos, uint tick)
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

                        return SpawnUnit(myPower, pos, cardPosition, unitPower, tick, ref myPower); // Spawn unit in squad next to other units
                    }
                }
            }
            return null;
        }

        private Command? SpawnArcherOnBarrier(float myPower, Position2D pos, EntityId barrierModuleId, uint tick, ref float powerRemaining)
        {
            /*
             // Units on wall:
             //   First i get all barriers i own and find out the gate, since you cant place units there
                    std::vector < capi::Entity> myBarrier = entitiesTOentity(myId, state.entities.barrier_modules);
                    capi::Position GatePos;
                    //Remove Gate from Vector
                    for (unsigned int i = 0; i < myBarrier.size() ;i++)
                        for(auto A: myBarrier[i].aspects)
                            if (A.variant_case == capi::AspectCase::BarrierGate)
                            {
                                GatePos = myBarrier[i].position;
                                myBarrier.erase(myBarrier.begin() + i);
                                goto GateLoopOut;
                            }    
                    GateLoopOut:
                
                
            //    and then i loop over my units which are need the wall
            //    Check if they are are "unmounted"
            //    And if not send them on the wall with CommandGroupEnterWall
                for (auto S : Bro->U->pointsInRadius(entitiesTOentity(myId, state.entities.squads), capi::to2D(myOrb.position), 50))
                    for (auto A : S.aspects)
                        if (A.variant_case == capi::AspectCase::MountBarrier)        
                            if (A.variant_union.mount_barrier.state.variant_case == capi::MountStateCase::Unmounted)
                            {
                                capi::Entity A;
                                capi::Entity B;
                
                                Bro->U->CloseCombi({ S }, myBarrier, A, B);
                                vReturn.push_back(MIS_CommandGroupEnterWall({ S.id }, B.id));
                            }

             */

            if (archerCardPositions != null)
            {
                byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition]; // Starting unit for AI
                if (unitOfficialCardId != 0) // Not a card
                {
                    SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                    if (card != null)
                    {
                        int powerCost = card.powerCost[3]; // Assume unit fully upgraded for now!!!!

                        return SpawnUnitOnBarrier(myPower, pos, cardPosition, barrierModuleId, powerCost, tick, ref powerRemaining); // Spawn unit in squad next to other units
                    }
                }
            }
            return null;
        }

        /*
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
                ConsoleWriteLine(consoleWriteline, $"Tick:{currentTick} opp:{target} pow:{(int)myPower} size:{myArmy.Count} oSize:{theirArmy.Count} oPow:{theirArmy}");
            }

            List<Command> commands = new List<Command>();

            var swiftclawTasks = SwiftclawTasks(currentTick, myPower, entities);
            if (swiftclawTasks != null)
            {
                commands.Add(swiftclawTasks);
                //return new[] { swiftclawTasks };
            }

            #region XanderLord new tasks
            //var orbTasks = OrbTasks(currentTick, myPower, entities);
            //if (orbTasks != null)
            //{
            //    commands.Add(orbTasks);
            //    // return new[] { orbTasks };
            //}

            //var wellTasks = WellTasks(currentTick, myPower, entities);
            //if (wellTasks != null)
            //{
            //    commands.Add(wellTasks);
            //    //return new[] { wellTasks };
            //}
            #endregion XanderLord new tasks
            return commands.ToArray();
        }
        */

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
                case "GiftedFlameNew":
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
                    botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
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

        private Command[] CastSpell(float myPower, Position2D pos, byte cardPosition, uint tick, ref float powerRemaining) // 1 through 20
        {
            List<Command> commands = new List<Command>();
            int spellCost = 100;
            int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

            SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
            if (card != null)
            {
                spellCost = card.powerCost[3];
                if (myPower >= spellCost && botState.canPlayCardAt < tick)
                {
                    Console.WriteLine("cast spell:{card.cardName} at pos:{pos.X},{pos.Y}");
                    SingleTargetLocation stl = new SingleTargetLocation { Xy = pos };
                    botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
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

        private Command[] CastSpellMulti(float myPower, Position pos1, Position pos2, byte cardPosition, uint tick, ref float powerRemaining) // 1 through 20
        {
            List<Command> commands = new List<Command>();
            int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

            SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
            if (card != null)
            {
                int spellCost = 100;
                if (myPower >= spellCost && botState.canPlayCardAt < tick)
                {
                    spellCost = card.powerCost[3];
                    Console.WriteLine("Cast spell multi:{0}", card.cardName);
                    botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
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
            // NGE05222024 if (botState.canPlayCardAt < tick)
            {
                int officialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition];

                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, officialCardId);
                if (card != null)
                {
                    Console.WriteLine("cast spell {0}", card.cardName);
                    // botState.canPlayCardAt = uint.MaxValue; // NGE05222024!!!!! 
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

        float GetOrbCost()
        {
            if (myOrbs.Count == 0)
            {
                return 100f;
            }
            else if (myOrbs.Count == 1)
            {
                return 150f;
            }
            else if (myOrbs.Count == 2)
            {
                return 250f;
            }
            else // if (myOrbs.Count >= 3)
            {
                return 300f;
            }
        }

        string previousBuildNearestOrbByPosMessage = "";

        private Command? BuildNearestOrbByPos(float myPower, out float powerRemaining, Position2D pos, CreateOrbColor color, uint tick, out bool orbBuilt)
        {
            Command? command = null;
            powerRemaining = myPower;
            float orbCost = GetOrbCost();
            if (myOrbs.Count == 0) { }
            orbBuilt = false;
            if (myPower > orbCost) // NGE05222024 if (botState.canPlayCardAt < tick) // if (myPower > orbCost && botState.canPlayCardAt < tick)
            {
                EntityId nearestOrb = new EntityId(0u);
                float nearestOrbDistanceSq = 0f;
                float d = 0;
                Position2D nearestOrbPosition = Position2DExt.Zero();
                foreach (var s in emptyOrbs)
                {
                    if (s.Entity.PlayerEntityId != null)
                        continue;
                    d = DistanceSquared(pos, s.Entity.Position);
                    if (nearestOrb.V == 0 || nearestOrbDistanceSq > d) // Closest empty orb
                    {
                        nearestOrb = s.Entity.Id;
                        nearestOrbDistanceSq = d;
                        nearestOrbPosition = s.Entity.Position.To2D();
                    }
                }
                // See if enemyAttackingSquads are near orb
                bool enemyNearOrb = false;
                if (enemyAttackingSquads != null && enemyAttackingSquads.Count() > 0)
                {
                    foreach (var s in enemyAttackingSquads)
                    {
                        if (MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), nearestOrbPosition)) < 15)
                        {
                            enemyNearOrb = true;
                            break;
                        }
                    }
                }

                if (myPower > orbCost && MathF.Sqrt(DistanceSquared(pos, nearestOrbPosition)) < 15 && enemyNearOrb == false) // Close enough to build orb and no enemy units by orb!
                {
                    string message = string.Format("Build nearest OrbId:{0} at pos:{1},{2} with Power:{3}", nearestOrb, (int)nearestOrbPosition.X, (int)nearestOrbPosition.Y, (int)myPower);
                    if (message != previousBuildNearestOrbByPosMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestOrbByPosMessage = message;
                    }
                    command = new CommandTokenSlotBuild
                    {
                        SlotId = nearestOrb,
                        Color = color
                    };
                    powerRemaining -= orbCost;
                    orbBuilt = true;
                }
            }
            return command;
        }

        string previousBuildNearestOrbMessage = "";
        private Command? BuildNearestOrb(float myPower, out float powerRemaining, Entity squad, CreateOrbColor color, uint tick, out bool orbBuilt)
        {
            Command? command = null;
            powerRemaining = myPower;
            float orbCost = GetOrbCost();
            if (myOrbs.Count == 0) { }
            orbBuilt = false;

            {
                Position2D pos = squad.Position.To2D();
                EntityId nearestOrb = new EntityId(0u);
                float nearestOrbDistanceSq = 0f;
                float d = 0;
                Position2D nearestOrbPosition = Position2DExt.Zero();
                foreach (var s in emptyOrbs)
                {
                    if (s.Entity.PlayerEntityId != null)
                        continue;
                    d = DistanceSquared(pos, s.Entity.Position);
                    if (nearestOrb.V == 0 || nearestOrbDistanceSq > d) // Closest empty orb
                    {
                        nearestOrb = s.Entity.Id;
                        nearestOrbDistanceSq = d;
                        nearestOrbPosition = s.Entity.Position.To2D();
                    }
                }
                if (myPower > orbCost && MathF.Sqrt(DistanceSquared(pos, nearestOrbPosition)) < 15) // Close enough to build orb
                {
                    string message = string.Format("Build nearest OrbId:{0} at pos:{1},{2}", nearestOrb, (int)nearestOrbPosition.X, (int)nearestOrbPosition.Y);
                    if (message != previousBuildNearestOrbMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestOrbMessage = message;
                    }
                    command = new CommandTokenSlotBuild
                    {
                        SlotId = nearestOrb,
                        Color = color
                    };
                    powerRemaining -= orbCost;
                    orbBuilt = true;
                }
                else // Have the squad get closer to the target
                {
                    string message = string.Format("Go to nearest OrbId:{0} at pos:{1},{2}", nearestOrb, (int)nearestOrbPosition.X, (int)nearestOrbPosition.Y);
                    //string message = string.Format("Go to nearest OrbId:{0} at pos:{1},{2} from pos{3},{4}", nearestOrb, (int)nearestOrbPosition.X, (int)nearestOrbPosition.Y, (int)pos.X, (int)pos.Y);
                    if (message != previousBuildNearestOrbMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestOrbMessage = message;
                    }
                    return new CommandGroupGoto
                    {
                        Squads = new[] { squad.Id },
                        Positions = new[]
                     {
                                    nearestOrbPosition,
                                },
                        WalkMode = WalkMode.Scout,
                        Orientation = 0
                    };
                }
            }
            return command;
        }

        private Command? BuildOrb(float myPower, out float powerRemaining, EntityId orbId, CreateOrbColor color, Position pos, uint tick)
        {
            Command? command = null;
            powerRemaining = myPower;
            float orbCost = GetOrbCost();
            if (myPower > orbCost && botState.canPlayCardAt < tick)
            {
                Console.WriteLine("build OrbId:{orbId}");
                command = new CommandTokenSlotBuild
                {
                    SlotId = orbId,
                    Color = color
                };
                powerRemaining -= orbCost;
            }
            return command;
        }

        string previousBuildNearestWellMessage = "";
        private Command? BuildNearestWell(float myPower, out float powerRemaining, Entity squad, uint tick, out bool wellBuilt)
        {
            Command? command = null;
            powerRemaining = myPower;
            float wellCost = 100f;
            wellBuilt = false;

            {
                Position2D pos = squad.Position.To2D();
                EntityId nearestWell = new EntityId(0u);
                float nearestWellDistanceSq = 0f;
                float d = 0;
                Position2D nearestWellPosition = Position2DExt.Zero();
                foreach (var s in emptyWells)
                {
                    if (s.Entity.PlayerEntityId != null)
                        continue;
                    d = DistanceSquared(pos, s.Entity.Position);
                    if (nearestWell.V == 0 || nearestWellDistanceSq > d) // Closest empty well
                    {
                        nearestWell = s.Entity.Id;
                        nearestWellDistanceSq = d;
                        nearestWellPosition = s.Entity.Position.To2D();
                    }
                }
                // See if enemyAttackingSquads are near well
                bool enemyNearWell = false;
                if (enemyAttackingSquads != null && enemyAttackingSquads.Count() > 0)
                {
                    foreach (var s in enemyAttackingSquads)
                    {
                        if (MathF.Sqrt(DistanceSquared(s.Entity.Position.To2D(), nearestWellPosition)) < 15)
                        {
                            enemyNearWell = true;
                            break;
                        }
                    }
                }
                if (myPower > wellCost && MathF.Sqrt(DistanceSquared(pos, nearestWellPosition)) < 15 && enemyNearWell == false) // Close enough to build well
                {
                    string message = string.Format("Build nearest WellId:{0} at pos:{1},{2}", nearestWell, (int)nearestWellPosition.X, (int)nearestWellPosition.Y);
                    if (message != previousBuildNearestWellMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestWellMessage = message;
                    }
                    command = new CommandPowerSlotBuild
                    {
                        SlotId = nearestWell,
                    };
                    powerRemaining -= wellCost;
                    wellBuilt = true;
                }
                else // Have the squad get closer to the target
                {
                    string message = string.Format("Go to nearest WellId:{0} at pos:{1},{2}", nearestWell, (int)nearestWellPosition.X, (int)nearestWellPosition.Y);
                    //string message = string.Format("Go to nearest WellId:{0} at pos:{1},{2} from pos:{3},{4}", nearestWell, (int)nearestWellPosition.X, (int)nearestWellPosition.Y, (int)pos.X, (int)pos.Y);
                    if (message != previousBuildNearestWellMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestWellMessage = message;
                    }
                    command = new CommandGroupGoto
                    {
                        Squads = new[] { squad.Id },
                        Positions = new[]
                     {
                                    nearestWellPosition,
                                },
                        WalkMode = WalkMode.Scout, //Crusade works as well!
                        Orientation = 0
                    };
                }
            }
            return command;
        }


        private Command? BuildWell(float myPower, out float powerRemaining, EntityId wellId, Position pos, uint tick)
        {
            Command? command = null;
            powerRemaining = myPower;
            if (myPower > 100f && botState.canPlayCardAt < tick)
            {
                Console.WriteLine("build WellId:{wellId}");
                command = new CommandPowerSlotBuild
                {
                    SlotId = wellId
                };
                powerRemaining -= 100f;
            }
            return command;
        }

        string previousBuildNearestWallMessage = "";
        private Command? BuildNearestWall(float myPower, out float powerRemaining, Entity squad, bool invertedDirection, uint tick, out bool wallBuilt)
        {
            Command? command = null;
            powerRemaining = myPower;
            float wallCost = 50f; // ????
            wallBuilt = false;
            // if (botState.canPlayCardAt < tick) // if (myPower > wallCost && botState.canPlayCardAt < tick)
            {
                Position2D pos = squad.Position.To2D();
                EntityId nearestWall = new EntityId(0u);
                float nearestWallDistanceSq = 0f;
                float d = 0;
                Position2D nearestWallPosition = Position2DExt.Zero();
                foreach (var s in emptyWalls)
                {
                    if (s.Entity.PlayerEntityId != null)
                        continue;
                    d = DistanceSquared(pos, s.Entity.Position);
                    if (nearestWall.V == 0 || nearestWallDistanceSq > d) // Closest empty wall
                    {
                        nearestWall = s.Entity.Id;
                        nearestWallDistanceSq = d;
                        nearestWallPosition = s.Entity.Position.To2D();
                    }
                }
                if (myPower > wallCost) // && MathF.Sqrt(DistanceSquared(pos, nearestWallPosition)) < 15) // Close enough to build wall
                {
                    string message = string.Format("Build nearest BarrierSet:{0} at pos:{1},{2}", nearestWall, (int)nearestWallPosition.X, (int)nearestWallPosition.Y);
                    if (message != previousBuildNearestWallMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestWallMessage = message;
                    }
                    command = new CommandBarrierBuild
                    {
                        BarrierId = nearestWall,
                        InvertedDirection = invertedDirection
                    };
                    powerRemaining -= wallCost;
                    wallBuilt = true;
                }
                else // Have the squad get closer to the target
                {
                    string message = string.Format("Go to nearest Wall:{0} at pos:{1},{2}", nearestWall, (int)nearestWallPosition.X, (int)nearestWallPosition.Y);
                    //string message = string.Format("Go to nearest WallId:{0} at pos:{1},{2} from pos:{3},{4}", nearestWall, (int)nearestWallPosition.X, (int)nearestWallPosition.Y, (int)pos.X, (int)pos.Y);
                    if (message != previousBuildNearestWallMessage)
                    {
                        Console.WriteLine(message);
                        previousBuildNearestWallMessage = message;
                    }
                    command = new CommandGroupGoto
                    {
                        Squads = new[] { squad.Id },
                        Positions = new[]
                     {
                                    nearestWallPosition,
                                },
                        WalkMode = WalkMode.Scout, //Crusade works as well for quicker speed!
                        Orientation = 0
                    };
                }
            }
            return command;
        }

        private Command[] BuildWall(float myPower, out float powerRemaining, EntityId wallId, bool invertedDirection, uint tick)
        {
            List<Command> commands = new List<Command>();
            powerRemaining = myPower;
            // How to get the power cost to build structure?  API.Building.PowerCost or SMJCard.powerCost
            if (myPower > 50f && botState.canPlayCardAt < tick) // 05222024 Changed from 100f to 50f !!!!!
            {
                string message = string.Format("Build WallId:{0}", wallId.V);
                ConsoleWriteLine(consoleWriteline, message);
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
        private Command? ToggleWallGate(EntityId wallId, bool open, uint tick)
        {
            // NGE05222024 if (botState.canPlayCardAt < tick)
            {
                string message = string.Format("Toggle WallId:{0} from open:{1} to open:{2}", wallId.V, open, !open);
                if (message != previousToggleWallGateMessage)
                {
                    ConsoleWriteLine(consoleWriteline, message);
                } // NGE06282024 
                Command command = new CommandBarrierGateToggle
                {
                    BarrierId = wallId,
                };
                previousToggleWallGateMessage = message;
                return command;
                // } // NGE06282024 
            }
            return null;
        }

        // Get Unit 'Wrecker' card 
        public int? GetWreckerCardPositionFromDeck(DeckOfficialCardIds deckIds, int obsTotal = 1)
        {
            int? wreckerCardIdPosition = null; // A deck has 20 card Ids ranging from position 0 (First card) to 19 (Last card)
            var queryCardsWreckerT1OfficialId = cardsSMJ.Where(item => item.orbsTotal == obsTotal && item.cardName == "Wrecker").Select(s => s.officialCardIds[0]); // All T1 Archers CardIds

            for (int i = 0; i < deckIds.Ids.Length; i++)
            {
                if (queryCardsWreckerT1OfficialId.Contains(deckIds.Ids[i]) == true)
                {
                    wreckerCardIdPosition = i;
                    break;
                }
            }
            return wreckerCardIdPosition;
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

        // Get Unit Ability 'Swift' cards so that I can put them on a wall
        public List<int> GetCardPositionsBasedOnAbilityFromDeck(DeckOfficialCardIds deckIds, string abilityName, int obsTotal = 1)
        {
            List<int> swiftCardIdPositions = new List<int>(); // A deck has 20 card Ids ranging from position 0 (First card) to 19 (Last card)
            for (int i = 0; i < deckIds.Ids.Length; i++)
            {
                int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[i];
                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                if (card != null && card.abilities != null && card.abilities.Count > 0)
                {
                    for (int a = 0; a < card.abilities.Count; a++)
                    {
                        if (card.orbsTotal <= obsTotal && card.abilities[a].abilityName == abilityName)
                        {
                            swiftCardIdPositions.Add(i);
                        }
                    }
                }
            }
            return swiftCardIdPositions;
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

        // Get cards thsat can attack long range so that I can use them effectively // NGE05112024!!!!!!!
        public List<int> GetLongRangeCardPositionsFromDeck(DeckOfficialCardIds deckIds, int obsTotal = 1)
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
                        if (command.Command.BuildHouse != null)
                        {
                            botState.canPlayCardAt = 0; // NGE05222024!!!!!
                            commandSent += "BuildHouse ";
                        }
                        if (command.Command.ProduceSquad != null)
                        {
                            botState.canPlayCardAt = 0; // NGE05222024!!!!!
                            commandSent += "ProduceSquad ";
                        }
                        if (command.Command.ProduceSquadOnBarrier != null)
                        {
                            botState.canPlayCardAt = 0; // NGE05222024!!!!!
                            commandSent += "ProduceSquadOnBarrier ";
                        }
                        if (command.Command.GroupAttack != null)
                        {
                            commandSent += "GroupAttack ";
                        }
                        if (command.Command.CastSpellEntity != null)
                        {
                            //botState.canPlayCardAt = 0; // NGE05222024!!!!!
                            commandSent += "CastSpellEntity ";
                        }
                        if (command.Command.CastSpellGod != null)
                        {
                            botState.canPlayCardAt = 0; // NGE05222024!!!!!
                            commandSent += "CastSpellGod ";
                        }
                        if (command.Command.CastSpellGodMulti != null)
                        {
                            botState.canPlayCardAt = 0; // NGE05222024!!!!!
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
                            botState.canPlayCardAt = 0; // NGE07042024 Reset the timer to try and continue
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

        private string previousToggleGateMessage = "";
        private Command? ToggleGate(bool open, uint tick)
        {
            foreach (var module in myWallModules)
            {
                if (module.Set.V == myWalls[0].Entity.Id.V) // Part of the same Barrier
                {
                    var gateAspect = module.Entity.Aspects.FirstOrDefault(g => g.BarrierGate != null) ?? null;
                    if (gateAspect != null)
                    {
                        if (open == true)
                        {
                            if (gateAspect.BarrierGate.Open == false)
                            {
                                Command? cmdModule = ToggleWallGate(module.Entity.Id, gateAspect.BarrierGate.Open, tick);
                                // Console.WriteLine("WallIModuleID:{0} gate open:{1}", module.Entity.Id, true);
                                return cmdModule;
                            }
                        }
                        else if (open == false)
                        {
                            if (gateAspect.BarrierGate.Open == true)
                            {
                                Command? cmdModule = ToggleWallGate(module.Entity.Id, gateAspect.BarrierGate.Open, tick);
                                // Console.WriteLine("WallIModuleID:{0} gate open:{1}", module.Entity.Id, true);
                                return cmdModule;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void AttackBarrierModules(List<BarrierSet>? wallsToAttack, Api.GameState state, List<BarrierModule>? enemyBarrierModules, ref float nearestOpponentBarrierDistance, ref bool isBarrierOpen, ref Entity targetEntityWall, ref EntityId targetWall, ref Squad? attackSquad)
        {
            //Entity targetEntityWall = null;
            Position nearestOpponentBarrierPosition;
            // if (wallsToAttack != null && wallsToAttack.Count() > 0)
            {
                foreach (var wall in wallsToAttack)
                {
                    isBarrierOpen = BreachBarrierSetModules(state, wall, enemyWallModules);
                    if (isBarrierOpen == false)
                    {
                        if (enemyBarrierModules == null)
                        {
                            enemyBarrierModules = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                        }
                        else
                        {
                            List<BarrierModule>? barrierModuleTemp = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                            if (barrierModuleTemp != null && barrierModuleTemp.Count() > 0)
                            {
                                enemyBarrierModules.AddRange(barrierModuleTemp);
                            }
                        }

                        if (isBarrierOpen == false && enemyBarrierModules != null)
                        {
                            foreach (var s in enemyBarrierModules) // NGE05222024 enemyWallModules) // if barrier module is near me attack it if it is not broken or the gate is open
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
                                foreach (Squad squad in myAttackSquads)
                                {
                                    float squadDistanceToWall = DistanceSquared(squad.Entity.Position.To2D(), nearestOpponentBarrierPosition);
                                    //float squadDistanceToWall = DistanceSquared(attackSquad.Entity.Position.To2D(), s.Entity.Position);
                                    if (squadDistanceToWall < nearestOpponentBarrierDistance)
                                    {
                                        nearestOpponentBarrierDistance = squadDistanceToWall;
                                        targetWall = s.Entity.Id;
                                        targetEntityWall = s.Entity;
                                        attackSquad = squad;
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        public Command[] TickDebug(GameState state)
        {
            Command[] commands = null;
            string deckName = botState.selectedDeck.Name;
            currentDeck = myDecks.FirstOrDefault(d => d.Name == deckName);
            if (currentDeck != null)
            {
                commands = SendCommands(state, (Deck)currentDeck);
            }
            else
            {
                commands = Array.Empty<Command>();
            }

            ConsoleWriteLine(true, $"Tick {state.CurrentTick} commands: {commands.Length}");
            return commands;
        }

        public Command[] SendCommands(GameState state, Deck currentDeck)
        {
            /*
                From Kubik: In C# you can use List<EntityId>s, or a Dictionary<TaskEnum, List<EntityId>> in your bot state
                list's name or Dictionary's key will be the "task" for the squad (Attack, Defend, Scout, etc.)
                whenever you try to spawn a new squad you will have a booley trigger, or just check every tick, if there is new squad owned by you (new = have higher ID, than any squad in you Lists/Dictionary) and decide to which List/key in Dictionary you want to add it
                then you also should run from time to time a check, that these squads still exist, and if not remove them
             */
            var currentTick = state.CurrentTick;
            var entities = state.Entities;

            #region Delay next command by 1 second (10 ticks per second) // Note: Cards can only be played 1 per second!!!
            if (botState.canPlayCardAt == uint.MaxValue)
            {
                foreach (var c in state.Commands)
                {
                    if (c.Player == botState.myId)
                    {
                        // NGE05222024!!!!! if (c.Command.CastSpellGod != null || c.Command.CastSpellGodMulti != null || c.Command.ProduceSquad != null || c.Command.ProduceSquadOnBarrier != null ||
                        // NGE05222024!!!!!     c.Command.GroupAttack != null || c.Command.BuildHouse != null || c.Command.BarrierBuild != null || c.Command.BarrierGateToggle != null)

                        if (c.Command.CastSpellGod != null || c.Command.CastSpellGodMulti != null || c.Command.ProduceSquad != null || c.Command.ProduceSquadOnBarrier != null || c.Command.BuildHouse != null)
                        {
                            if (c.Command.ProduceSquad != null)
                            {
                                Console.WriteLine("CommandProduceSquad");
                            }
                            if (c.Command.ProduceSquadOnBarrier != null)
                            {
                                Console.WriteLine("CommandProduceSquadOnBarrier");
                            }
                            //if (c.Command.GroupAttack != null)
                            //{
                            //    Console.WriteLine("CommandGroupAttack");
                            //}
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
                            //if (c.Command.BarrierBuild != null)
                            //{
                            //    Console.WriteLine("CommandBarrierBuild");
                            //}
                            //if (c.Command.BarrierGateToggle != null)
                            //{
                            //    Console.WriteLine("CommandBarrierGateToggle");
                            //}
                            botState.canPlayCardAt = currentTick.V + 10; // Delay next command by 1 second (10 ticks per second) // Note: Cards can only be played 1 per second!!!
                            break;
                        }
                    }
                }
            }
            #endregion Delay next command by 1 second (10 ticks per second) // Note: Cards can only be played 1 per second!!!

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
                case "GiftedFlameNew":
                case "BlessedFlame":
                case "InfusedFlame":
                    var player = Array.Find(state.Players, p => p.Id == botState.myId);
                    if (player == null) { return commands.ToArray(); }
                    var myPower = player.Power;

                    BarrierSet[] myBarriers = Array.FindAll(state.Entities.BarrierSets, x => x.Entity.PlayerEntityId == botState.myId);
                    BarrierModule[] myWallsModules0 = Array.FindAll(state.Entities.BarrierModules, x => x.Entity.PlayerEntityId == botState.myId);
                    myWalls = myBarriers != null ? myBarriers.ToList() : new List<BarrierSet>();
                    myWallModules = myWallsModules0 != null ? myWallsModules0.ToList() : new List<BarrierModule>();

                    #region Get Squad Info based on health
                    mySquads = new List<Squad>();
                    enemySquads = new List<Squad>();

                    foreach (var s in state.Entities.Squads)
                    {
                        if (s.Entity.PlayerEntityId == botState.myId)
                        {
                            GetSquadHealth(state, s, out double squadHealth, out List<EntityId> healthyUnits);
                            if (squadHealth != 0)
                            {
                                myUnits.Add(s.Entity.Id); // myUnits.AddRange(healthyUnits);
                                //myArmies[s.ResSquadId.V] = s;
                                mySquads.Add(s);
                            }
                        }
                        else if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                        {
                            GetSquadHealth(state, s, out double squadHealth, out List<EntityId> healthyUnits);
                            if (squadHealth != 0)
                            {
                                enemyUnits.Add(s.Entity.Id); // enemyUnits.AddRange(healthyUnits);
                                //enemyArmies[s.ResSquadId.V] = s;
                                enemySquads.Add(s);
                            }
                        }
                    }
                    #endregion Get Squad Info based on health

                    if (currentTick.V % defaultTickUpdateRate == 0) // defaultTickUpdateRate can slow down update rate - default tick rate is 1/10 second
                    {
                        #region Collect critical game stats

                        TokenSlot[] myOrbs0 = Array.FindAll(state.Entities.TokenSlots, x => x.Entity.PlayerEntityId == botState.myId);
                        myOrbs = myOrbs0 != null ? myOrbs0.ToList() : new List<TokenSlot>();
                        PowerSlot[] myWells0 = Array.FindAll(state.Entities.PowerSlots, x => x.Entity.PlayerEntityId == botState.myId);
                        myWells = myWells0 != null ? myWells0.ToList() : new List<PowerSlot>();
                        Building[] myBuildings0 = Array.FindAll(state.Entities.Buildings, x => x.Entity.PlayerEntityId == botState.myId);
                        myBuildings = myBuildings0 != null ? myBuildings0.ToList() : new List<Building>();

                        // Code below does not take into account the health of the squads
                        // Squad[] mySquads0 = Array.FindAll(state.Entities.Squads, x => x.Entity.PlayerEntityId == botState.myId);
                        // mySquads = mySquads0 != null ? mySquads0.ToList() : new List<Squad>();

                        TokenSlot[] enemyOrbs0 = Array.FindAll(state.Entities.TokenSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        enemyOrbs = enemyOrbs0 != null ? enemyOrbs0.ToList() : new List<TokenSlot>();
                        PowerSlot[] enemyWells0 = Array.FindAll(state.Entities.PowerSlots, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        enemyWells = enemyWells0 != null ? enemyWells0.ToList() : new List<PowerSlot>();
                        Building[] enemyBuildings0 = Array.FindAll(state.Entities.Buildings, x => x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId));
                        enemyBuildings = enemyBuildings0 != null ? enemyBuildings0.ToList() : new List<Building>();
                        BarrierSet[] enemyBarriers0 = Array.FindAll(state.Entities.BarrierSets, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        BarrierModule[] enemyWalls0 = Array.FindAll(state.Entities.BarrierModules, x => (x.Entity.PlayerEntityId != null && botState.oponents.Contains(x.Entity.PlayerEntityId)));
                        enemyWalls = enemyBarriers0 != null ? enemyBarriers0.ToList() : new List<BarrierSet>();
                        enemyWallModules = enemyWalls0 != null ? enemyWalls0.ToList() : new List<BarrierModule>();

                        // Code below does not take into account the health of the squads
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

                        #endregion Collect critical game stats

                        float enemyDistance = float.MaxValue;

                        if (enemyBuildingOrb == true)
                        {
                            unitsNeededBeforeAttack = 1;
                        }
                        else
                        {
                            if (unitsNeededBeforeAttack > defaultAttackSquads) // Must be <= defaultAttackSquads
                            {
                                unitsNeededBeforeAttack = defaultAttackSquads;
                            }
                        }

                        Squad? nearestEnemy = null;

                        // NGE11022024 
                        GetClosestEnemy(myOrbs[0].Entity.Position.To2D(), out nearestEnemy, out enemyDistance);
                        if (nearestEnemy != null && enemyDistance < 2 * enemyNearOrbDistance) // Want enough time to build power to build wall
                        {
                            unitsNeededBeforeAttack = 1;
                        }
                        else
                        {
                            unitsNeededBeforeAttack = defaultAttackSquads;
                        }


                        #region Buid/Rebuild Orb if none exist!
                        if (myOrbs.Count == 0) // Rebuild Orb at start position!!
                        {
                            Command? cmd = BuildNearestOrbByPos(myPower, out myPower, botState.myStart, orbColorBuildOrder[0], currentTick.V, out bool orbBuilt);
                            if (cmd != null)
                            {
                                ConsoleWriteLine(true, $"Build/Rebuild orb at botState.myStart Pos: {(int)botState.myStart.X},{(int)botState.myStart.Y} with Power:{(int)myPower}");
                                commands.Add(cmd);
                                if (myPower < 150)
                                {
                                    return commands.ToArray();
                                }
                            }
                            if (myPower >= 150) // Try to build an orb by a nearby well
                            {
                                if (myWells.Count > 0)
                                {
                                    //int counter = myWells.Count - 1;
                                    //for (int i = counter; i >= 0; i--) // Start with last well made since probably farthest away
                                    //{
                                    //    PowerSlot well = myWells[i];
                                    //    Command? cmdNewOrb = BuildNearestOrbByPos(myPower, out myPower, well.Entity.Position.To2D(), orbColorBuildOrder[0], currentTick.V, out bool orbBuiltNew);
                                    //    if (cmdNewOrb != null)
                                    //    {
                                    //        ConsoleWriteLine(true, $"Building new orb by wellID {well.Entity.Id} Pos: {(int)well.Entity.Position.To2D().X},{(int)well.Entity.Position.To2D().Y} with Power:{(int)myPower}");
                                    //        commands.Add(cmdNewOrb);
                                    //        return commands.ToArray();
                                    //    }
                                    //}
                                    foreach (var well in myWells)
                                    {
                                        Command? cmdNewOrb = BuildNearestOrbByPos(myPower, out myPower, well.Entity.Position.To2D(), orbColorBuildOrder[0], currentTick.V, out bool orbBuiltNew);
                                        if (cmdNewOrb != null)
                                        {
                                            ConsoleWriteLine(true, $"Building new orb by wellID {well.Entity.Id} Pos: {(int)well.Entity.Position.To2D().X},{(int)well.Entity.Position.To2D().Y} with Power:{(int)myPower}");
                                            commands.Add(cmdNewOrb);
                                            // NGE07042024 return commands.ToArray();
                                        }
                                    }
                                    if (commands.Count() > 0) // NGE07042024 
                                    {
                                        return commands.ToArray();
                                    }
                                }
                            }
                        }
                        #endregion Buid/Rebuild Orb if none exist!

                        #region If enemy builds new orb, attack it!
                        TokenSlot? enemyOrbToAttack = null;
                        if (enemyOrbs != null && enemyOrbs.Count > 1)
                        {
                            for (int i = 0; i < enemyOrbs.Count; i++)
                            {
                                if (enemyOrbs[i] != startingOrbEnemy)
                                {
                                    enemyOrbToAttack = enemyOrbs[i];
                                    break;
                                }
                            }
                        }
                        if (enemyOrbToAttack != null)
                        {
                            enemyBuildingOrb = false; // NGE110222024 !!!! Not working yet!! true;
                            string message = string.Format("Enemy build another orb! EntityId={0}", enemyOrbToAttack.Entity.Id);
                            ConsoleWriteLine(consoleWriteline, message);
                            // Attack the orb with any army units
                            /*
                                                        
                            int cardID = 0; // Default card ID
                            // bool armyHasSwiftUnit = false; // If army does not have a swift unit, make one!
                            int unitPower = 75; // Default power cost - should be able to find info about card
                            if (swiftCardPositions != null && swiftCardPositions.Count > 0)
                            {
                                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, swiftCardPositions[0]);
                                if (card != null)
                                {
                                    unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                }
                            }
                            else
                            {
                                SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, myCurrentDeckOfficialCardIds.Ids[0]);
                                if (card != null)
                                {
                                    unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                }
                            }

                            Position2D wallPos = myWalls[0].Entity.Position.To2D();
                            Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };

                            Command? spawn = SpawnUnit(myPower, pos, (byte)cardID, unitPower, currentTick.V, ref myPower); // Create Archer
                            if (spawn != null)
                            {
                                commands.Add(spawn);
                                Command? cmdOpenGate = ToggleGate(true, currentTick.V);
                                if (cmdOpenGate != null)
                                {
                                    commands.Add(cmdOpenGate);
                                }

                                return commands.ToArray();
                            }

                            */
                        }
                        else
                        {
                            enemyBuildingOrb = false;
                        }
                        #endregion

                        // Squad? nearestEnemy = null;
                        if (enemyBuildingOrb == false)
                        {
                            #region Build/Repair wall if enemy near my orb
                            if (enemySquads.Count() > 0 && myOrbs.Count() > 0 && myWalls.Count() == 0)
                            {
                                GetClosestEnemy(myOrbs[0].Entity.Position.To2D(), out nearestEnemy, out enemyDistance);
                                if (nearestEnemy != null && enemyDistance < enemyNearOrbDistance) // Want enough time to build power to build wall
                                {
                                    buildNearbyWallAtStart = true;
                                }
                            }
                            else if (enemySquads.Count() > 0 && myOrbs.Count() > 0 && myWalls.Count() > 0)
                            {
                                GetClosestEnemy(myOrbs[0].Entity.Position.To2D(), out nearestEnemy, out enemyDistance);
                                if (nearestEnemy != null && enemyDistance < engageEnemyNearOrbDistance) // Make sure gate is closed!
                                {
                                    Command? cmdCloseGate = ToggleGate(false, currentTick.V);
                                    if (cmdCloseGate != null)
                                    {
                                        Console.WriteLine("Close Gate!");
                                        return new Command[] { cmdCloseGate };
                                    }
                                }
                            }
                            #endregion Build/Repair wall if enemy near my orb

                            #region Build Wall at start
                            if (buildNearbyWallAtStart == true && closestWall.V != 0 && myWalls.Count() == 0 && buildNearbyWallAtStartWasTrue == false)
                            {
                                if (closestWallDistanceSq < enemyNearOrbDistance && myPower > buildWallCost)
                                {
                                    Command[] cmd = BuildWall(myPower, out float powerRemaining, closestWall, false, currentTick.V);
                                    // Console.WriteLine("WallId:{0} built at:{1},{2}", closestWall, (int)closestWallPosition.X, (int)closestWallPosition.Y);
                                    return cmd.ToArray();
                                }
                                else
                                {
                                    return Array.Empty<Command>(); // Need more power!!
                                }
                            }
                            if (buildNearbyWallAtStart == true && myWalls.Count() > 0 && myWallModules.Count() > 0) // Open Wall gate to let out squads
                            {
                                buildNearbyWallAtStartWasTrue = true;
                                if (buildArcherOnWallAtStart == true)
                                {
                                    if (archerCardPositions != null)
                                    {
                                        Command? spawn = null;
                                        Command? spawnOnBarrier = null;

                                        byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                                        var archerSquads = mySquads.Where(a => a.CardId == currentDeck.Cards[cardPosition]).ToList(); // See if I have any archers

                                        int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition]; // Should be an archer

                                        int unitPowerArcher = 75; // Default power cost - should be able to find info about card
                                        if (unitOfficialCardId != 0) // Not a card
                                        {
                                            SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                                            if (card != null)
                                            {
                                                unitPowerArcher = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
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

                                        if (archerOnWallAtStartBuilt == false && myPower > unitPowerArcher)
                                        {
                                            Command? commandSpawnArcherOnWall = SpawnArcherOnWall(myWalls[0].Entity, myWallModules, currentTick.V, myPower, unitPowerArcher, ref myPower);
                                            if (commandSpawnArcherOnWall != null)
                                            {
                                                buildArcherOnWallAtStart = false;
                                                archerOnWallAtStartBuilt = true;

                                                commands.Add(commandSpawnArcherOnWall);
                                                return commands.ToArray();
                                            }
                                        }
                                        else if (myPower > unitPowerArcher) // Make an archer squad at starting near orb location for now
                                        {
                                            Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                            Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };

                                            spawn = SpawnUnit(myPower, pos, cardPosition, unitPowerArcher, currentTick.V, ref myPower); // Create Archer
                                            if (spawn != null)
                                            {
                                                commands.Add(spawn);
                                                Command? cmdOpenGate = ToggleGate(true, currentTick.V);
                                                if (cmdOpenGate != null)
                                                {
                                                    commands.Add(cmdOpenGate);
                                                }

                                                return commands.ToArray();
                                            }
                                        }
                                        else
                                        {
                                            return commands.ToArray();
                                        }
                                    }
                                }

                                buildNearbyWallAtStart = false; // This task is completed
                            }
                            #endregion Build Wall at start

                            #region Build Nearest Empty Well at Start
                            if (buildNearbyWellAtStart == true && emptyWells.Count() > 0)
                            {
                                int wellBuildingSquad = 0;
                                if (buildArcherOnWallAtStartWasTrue == true)
                                {
                                    wellBuildingSquad = 1; // Use the second squad
                                }
                                if (mySquads.Count() > wellBuildingSquad && mySquads[wellBuildingSquad] != null)
                                {
                                    Command? buildNearestWellCommand = BuildNearestWell(myPower, out myPower, mySquads[wellBuildingSquad].Entity, currentTick.V, out bool isWellBuilt);
                                    if (buildNearestWellCommand != null && isWellBuilt == true)
                                    {
                                        buildNearbyWellAtStart = false;
                                        commands.Add(buildNearestWellCommand); // NGE05122024!!!!!! 
                                        return commands.ToArray(); // NGE05122024!!!!!! 
                                    }
                                    else if (buildNearestWellCommand != null)
                                    {
                                        commands.Add(buildNearestWellCommand); // NGE05122024!!!!!! 
                                        return commands.ToArray(); // NGE05122024!!!!!! 
                                    }
                                    else
                                    {
                                        myPower = 0; // Set Power to 0 so nothing else happens until a well is built
                                    }
                                }
                            }
                            #endregion Build Nearest Empty Well at Start

                            #region Build Nearest Empty Orb at Start
                            if (myPower != 0 && buildNearbyOrbAtStart == true && emptyOrbs.Count() > 0)
                            {
                                int orbBuildingSquad = 0;
                                if (buildArcherOnWallAtStartWasTrue == true)
                                {
                                    orbBuildingSquad = 1; // Use the second squad
                                }
                                if (mySquads.Count() > orbBuildingSquad)
                                {
                                    CreateOrbColor orbColor = orbColorBuildOrder[myOrbs.Count]; // CreateOrbColor.Fire;  
                                    int myOrbCount = myOrbs.Count();
                                    if (orbColorBuildOrder.Count() > myOrbCount)
                                    {
                                        orbColor = orbColorBuildOrder[myOrbs.Count()];
                                    }

                                    Command? buildNearestOrbCommand = BuildNearestOrb(myPower, out myPower, mySquads[orbBuildingSquad].Entity, orbColor, currentTick.V, out bool isOrbBuilt);
                                    if (buildNearestOrbCommand != null && isOrbBuilt == true)
                                    {
                                        buildNearbyOrbAtStart = false;
                                        commands.Add(buildNearestOrbCommand); // NGE05122024!!!!!! 
                                        return commands.ToArray(); // NGE05122024!!!!!! 
                                    }
                                    else if (buildNearestOrbCommand != null)
                                    {
                                        commands.Add(buildNearestOrbCommand); // NGE05122024!!!!!! 
                                        return commands.ToArray(); // NGE05122024!!!!!! 
                                    }
                                    else
                                    {
                                        myPower = 0; // Set Power to 0 so nothing else happens until a orb is built
                                    }
                                }
                            }
                            #endregion Build Nearest Empty Orb at Start
                        }

                        if (botState.isGameStart == true || currentTick.V % defaultTickUpdateRate == 0) // Try to do stuff every 0.5 seconds instead of every 1/10 second
                        {
                            if (botState.isGameStart == true)
                            {
                                string message = string.Format("{0} Strategy start", currentDeck.Name);
                                ConsoleWriteLine(consoleWriteline, message);
                            }

                            #region Buid/Rebuild Orb if none exist!
                            if (myOrbs.Count == 0) // Rebuild Orb at start position!!
                            {
                                Command? cmd = BuildNearestOrbByPos(myPower, out myPower, botState.myStart, orbColorBuildOrder[0], currentTick.V, out bool orbBuilt);
                                if (cmd != null)
                                {
                                    ConsoleWriteLine(true, $"Build/Rebuild orb at botState.myStart Pos: {(int)botState.myStart.X},{(int)botState.myStart.Y} with Power:{(int)myPower}");
                                    commands.Add(cmd);
                                    if (myPower < 150)
                                    {
                                        return commands.ToArray();
                                    }
                                }
                                if (myPower >= 150) // Try to build an orb by a nearby well
                                {
                                    if (myWells.Count > 0)
                                    {
                                        //int counter = myWells.Count - 1;
                                        //for (int i = counter; i >= 0; i--) // Start with last well made since probably farthest away
                                        //{
                                        //    PowerSlot well = myWells[i];
                                        //    Command? cmdNewOrb = BuildNearestOrbByPos(myPower, out myPower, well.Entity.Position.To2D(), orbColorBuildOrder[0], currentTick.V, out bool orbBuiltNew);
                                        //    if (cmdNewOrb != null)
                                        //    {
                                        //        ConsoleWriteLine(true, $"Building new orb by wellID {well.Entity.Id} Pos: {(int)well.Entity.Position.To2D().X},{(int)well.Entity.Position.To2D().Y} with Power:{(int)myPower}");
                                        //        commands.Add(cmdNewOrb);
                                        //        return commands.ToArray();
                                        //    }
                                        //}
                                        foreach (var well in myWells)
                                        {
                                            Command? cmdNewOrb = BuildNearestOrbByPos(myPower, out myPower, well.Entity.Position.To2D(), orbColorBuildOrder[0], currentTick.V, out bool orbBuiltNew);
                                            if (cmdNewOrb != null)
                                            {
                                                ConsoleWriteLine(true, $"Building new orb by wellID {well.Entity.Id} Pos: {(int)well.Entity.Position.To2D().X},{(int)well.Entity.Position.To2D().Y} with Power:{(int)myPower}");
                                                commands.Add(cmdNewOrb);
                                                // NGE07042024 return commands.ToArray();
                                            }
                                        }
                                        if (commands.Count() > 0) // NGE07042024 
                                        {
                                            return commands.ToArray();
                                        }
                                    }
                                }
                            }
                            #endregion Buid/Rebuild Orb if none exist!

                            #region Repair Walls
                            if (myWalls.Count > 0)
                            {
                                foreach (var wall in myWalls)
                                {
                                    Command? cmdRepairWall = RepairWall(wall, myWallModules, myPower, ref myPower);
                                    if (cmdRepairWall != null)
                                    {
                                        commands.Add(cmdRepairWall);
                                    }
                                }
                            }
                            #endregion Repair Walls

                            #region Repair Orbs
                            if (myOrbs.Count > 0)
                            {
                                foreach (var orb in myOrbs)
                                {
                                    Command? cmdRepairOrb = RepairOrb(orb, myPower, ref myPower);
                                    if (cmdRepairOrb != null)
                                    {
                                        commands.Add(cmdRepairOrb);
                                    }
                                }
                            }
                            #endregion Repair Orbs

                            #region Repair Wells
                            if (myWells.Count > 0)
                            {
                                foreach (var well in myWells)
                                {
                                    Command? cmdRepairWell = RepairWell(well, myPower, ref myPower);
                                    if (cmdRepairWell != null)
                                    {
                                        commands.Add(cmdRepairWell);
                                    }
                                }
                            }
                            #endregion Repair Wells

                            #region Repair Buildings
                            if (myBuildings.Count > 0)
                            {
                                foreach (var myBuilding in myBuildings)
                                {
                                    Command? cmdRepairBuilding = RepairBuilding(myBuilding, myPower, ref myPower);
                                    if (cmdRepairBuilding != null)
                                    {
                                        commands.Add(cmdRepairBuilding);
                                    }
                                }
                            }
                            #endregion Repair Buildings

                            if (myOrbs.Count > 0 && myPower > unitPower)
                            {
                                var theirArmy = enemySquads;

                                float nearestOrbDistance = float.MaxValue;
                                float armyPosDistanceToOrb = float.MaxValue;

                                #region Determine Attack and Defend Squads
                                // How many defense units do I have?
                                if (mySquads.Count() > 0 && archerCardPositions != null)
                                {
                                    byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                                    myDefendSquads = mySquads.Where(a => a.CardId == currentDeck.Cards[cardPosition]).ToList(); // See if I have any archers
                                    defendSquadCount = myDefendSquads.Count();
                                }

                                if (mySquads.Count() > 0 && archerCardPositions != null)
                                {
                                    byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                                    myAttackSquads = mySquads.Where(a => a.CardId != currentDeck.Cards[cardPosition]).ToList(); // See if I have any archers
                                    attackSquadCount = myAttackSquads.Count();
                                }
                                #endregion Determine Attack and Defend Squads

                                Position2D myArmyPos = botState.myStart; // Place unit as close to middle of map as possible near orb

                                #region Code to put a squad outside of a barrier near an orb
                                if (myWalls.Count() == 0 && mySquads.Count() == 0)
                                {
                                    if (MathF.Sqrt(DistanceSquared(botState.myStart, closestWallPosition)) < 15)
                                    {
                                        myArmyPos = closestWallPosition;
                                    }
                                    else if (enemyOrbs.Count() > 0)
                                    {
                                        Position2D enemyOrbPos = enemyOrbs[0].Entity.Position.To2D();
                                        float distance = MathF.Sqrt(DistanceSquared(enemyOrbPos, botState.myStart));
                                        float deltaX = (enemyOrbPos.X - botState.myStart.X) * 15 / distance;
                                        float deltaY = (enemyOrbPos.Y - botState.myStart.Y) * 15 / distance;
                                        myArmyPos = new Position2D { X = deltaX, Y = deltaY };
                                    }
                                }
                                else if (myWalls.Count() > 0 && mySquads.Count() == 0)
                                {
                                    Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                    Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };
                                    myArmyPos = pos;
                                    //Console.WriteLine("myArmyPos:{0},{1}", (int)myArmyPos.X, (int)myArmyPos.Y);
                                }
                                else if (myWalls.Count() > 0 && mySquads.Count() > 0) // Cannot build squad outside of wall unless another squad is inside wall
                                {
                                    // Turn all positions to int
                                    Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                    Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                    Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                                    Position2D orbPosInt = new Position2D { X = (int)orbPos.X, Y = (int)orbPos.Y };
                                    bool firstSquadInsideWall = false;
                                    Position2D mySquadPos = mySquads[0].Entity.Position.To2D();
                                    Position2D mySquadPosInt = new Position2D { X = (int)mySquadPos.X, Y = (int)mySquadPos.Y };
                                    // Squad is NOT between orb and wall
                                    if (mySquadPosInt != wallPos && mySquadPosInt != orbPosInt &&
                                        mySquadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySquadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                                        mySquadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySquadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
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
                                        Squad? nearestSquad = GetSquadNearestToEnemyOrb(myAttackSquads); // position new squad with other squad that is nearest to enemy orb!
                                        if (nearestSquad != null)
                                        {
                                            myArmyPos = nearestSquad.Entity.Position.To2D();
                                        }
                                        else
                                        {
                                            myArmyPos = mySquads[0].Entity.Position.To2D();
                                        }
                                    }
                                    //Console.WriteLine("myArmyPos:{0},{1}", (int)myArmyPos.X, (int)myArmyPos.Y);
                                }
                                else if (mySquads.Count > 0)
                                {
                                    Squad? nearestSquad = GetSquadNearestToEnemyOrb(myAttackSquads); // position new squad with other squad that is nearest to enemy orb!
                                    if (nearestSquad != null)
                                    {
                                        myArmyPos = nearestSquad.Entity.Position.To2D();
                                    }
                                    else
                                    {
                                        myArmyPos = mySquads[0].Entity.Position.To2D();
                                    }
                                }
                                #endregion Code to put a squad outside of a barrier near an orb

                                if (myAttackSquads != null && myAttackSquads.Count() > 0 && myWalls.Count() > 0)
                                {
                                    #region Open Gate when appropriate
                                    // If gate is closed and any attack squads are inside gate, open it
                                    {
                                        Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                        Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                        Position2D orbPos = startingOrb.Entity.Position.To2D(); // myOrbs[0].Entity.Position.To2D();

                                        // Position2D orbPosInt = new Position2D { X = (int)myOrbs[0].Entity.Position.X, Y = (int)myOrbs[0].Entity.Position.Y };
                                        Position2D orbPosInt = new Position2D { X = (int)startingOrb.Entity.Position.X, Y = (int)startingOrb.Entity.Position.Y };
                                        Position2D mySqadPos = new Position2D { X = 0, Y = 0 };
                                        Position2D mySqadPosInt = new Position2D { X = 0, Y = 0 };

                                        bool openGate = false;
                                        foreach (var squad in myAttackSquads)
                                        {
                                            mySqadPos = squad.Entity.Position.To2D();
                                            mySqadPosInt = new Position2D { X = (int)mySqadPos.X, Y = (int)mySqadPos.Y };
                                            // Squad is between orb and wall
                                            if (mySqadPosInt != wallPos && mySqadPosInt != orbPosInt &&
                                                mySqadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySqadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                                                mySqadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySqadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
                                            {
                                                openGate = true;
                                                string message = string.Format("SquadId{0} still inside, so open gate!", squad.Entity.Id);
                                                ConsoleWriteLine(consoleWriteline, message);
                                                break;
                                            }
                                        }

                                        if (openGate == true)
                                        {
                                            Command? cmd = ToggleGate(true, currentTick.V);
                                            if (cmd != null)
                                            {
                                                commands.Add(cmd); // return new Command[] { cmd };
                                            }
                                        }
                                    }
                                    #endregion Open Gate when appropriate

                                    #region Close Gate when appropriate
                                    // If gate is open and all squads are outside gate, close it
                                    {
                                        // Turn all positions to int
                                        Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                        Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                        Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                                        Position2D orbPosInt = new Position2D { X = (int)orbPos.X, Y = (int)orbPos.Y };
                                        Position2D mySqadPos = new Position2D { X = 0, Y = 0 };
                                        Position2D mySqadPosInt = new Position2D { X = 0, Y = 0 };

                                        bool closeGate = true;
                                        foreach (var squad in myAttackSquads)
                                        {
                                            mySqadPos = squad.Entity.Position.To2D();
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
                                            Command? cmd = ToggleGate(false, currentTick.V);
                                            if (cmd != null)
                                            {
                                                commands.Add(cmd); // return new Command[] { cmd };
                                            }
                                        }
                                    }
                                    #endregion Close Gate when appropriate

                                }

                                var targetOrb = new EntityId(0u);
                                var targetWell = new EntityId(0u);
                                var targetSquad = new EntityId(0u);
                                var targetWall = new EntityId(0u);
                                Entity targetEntityOrb = null;
                                Entity targetEntityWell = null;
                                Entity targetEntitySquad = null;
                                Entity targetEntityWall = null;
                                Entity targetEntity = null;
                                string targetType = "";

                                Squad? attackSquad = null;
                                if (attackSquadCount > 0)
                                {
                                    foreach (var s in enemyOrbs) // Attack enemy Orbs
                                    {
                                        float armyPosDistanceToOrbTemp = DistanceSquared(myArmyPos, s.Entity.Position);
                                        if (armyPosDistanceToOrb > armyPosDistanceToOrbTemp)
                                        {
                                            armyPosDistanceToOrb = armyPosDistanceToOrbTemp;
                                        }
                                        foreach (Squad squad in myAttackSquads)
                                        {
                                            float squadDistanceToOrb = DistanceSquared(squad.Entity.Position.To2D(), s.Entity.Position);
                                            if (squadDistanceToOrb < nearestOrbDistance)
                                            {
                                                nearestOrbDistance = squadDistanceToOrb;
                                                targetOrb = s.Entity.Id;
                                                targetEntityOrb = s.Entity;
                                                attackSquad = squad;
                                            }
                                        }
                                    }
                                }

                                Command? spawn = null;
                                Command? attack = null;
                                Command? attackEnemy = null;
                                Command? cmdOpenGate = null;

                                if (isEnemyNearBase == true) // Keep attacking enemies near my orb base until gone!
                                {
                                    int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[0]; // Starting unit for AI

                                    // Console.WriteLine("Tick CardID:{0}", unitOfficialCardId);
                                    if (unitOfficialCardId != 0) // Not a card
                                    {
                                        SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                                        if (card != null)
                                        {
                                            unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                        }
                                    }
                                    if (myWalls.Count() > 0 && mySquads.Count() == 0)
                                    {
                                        Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                        Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 };
                                        myArmyPos = pos;
                                        //Console.WriteLine("myArmyPos:{0},{1}", (int)myArmyPos.X, (int)myArmyPos.Y);
                                    }
                                    else if (myWalls.Count() > 0 && mySquads.Count() > 0) // Cannot build squad outside of wall unless another squad is inside wall
                                    {
                                        // Turn all positions to int
                                        Position2D wallPos = myWalls[0].Entity.Position.To2D();
                                        Position2D wallPosInt = new Position2D { X = (int)wallPos.X, Y = (int)wallPos.Y };
                                        Position2D orbPos = myOrbs[0].Entity.Position.To2D();
                                        Position2D orbPosInt = new Position2D { X = (int)orbPos.X, Y = (int)orbPos.Y };
                                        bool firstSquadInsideWall = false;
                                        Position2D mySquadPos = mySquads[0].Entity.Position.To2D();
                                        Position2D mySquadPosInt = new Position2D { X = (int)mySquadPos.X, Y = (int)mySquadPos.Y };
                                        // Squad is NOT between orb and wall
                                        if (mySquadPosInt != wallPos && mySquadPosInt != orbPosInt &&
                                            mySquadPosInt.X >= Math.Min(orbPosInt.X, wallPosInt.X) && mySquadPosInt.X <= Math.Max(orbPosInt.X, wallPosInt.X) &&
                                            mySquadPosInt.Y >= Math.Min(orbPosInt.Y, wallPosInt.Y) && mySquadPosInt.Y <= Math.Max(orbPosInt.Y, wallPosInt.Y))
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
                                            Squad? nearestSquad = GetSquadNearestToEnemyOrb(mySquads); // position new squad with other squad that is nearest to enemy orb!
                                            if (nearestSquad != null)
                                            {
                                                myArmyPos = nearestSquad.Entity.Position.To2D();
                                            }
                                            else
                                            {
                                                myArmyPos = mySquads[0].Entity.Position.To2D();
                                            }
                                        }
                                        //Console.WriteLine("myArmyPos:{0},{1}", (int)myArmyPos.X, (int)myArmyPos.Y);
                                    }
                                    else
                                    {
                                        myArmyPos = botState.myStart;
                                    }

                                    // NGE05222024 !!!!!!! spawn = SpawnUnit(myPower, myArmyPos, 0, unitPower, currentTick.V, ref myPower);
                                    // NGE05222024 !!!!!!! // attack = Attack(enemyAttackingSquad.Entity.Id, "squad", new List<Squad> { enemyAttackingSquad }, enemyAttackingSquad.Entity.Position.To2D(), 1);
                                    // NGE05222024 !!!!!!! if (spawn != null) // && attack != null)
                                    // NGE05222024 !!!!!!! {
                                    // NGE05222024 !!!!!!!     commands.Add(spawn);
                                    // NGE05222024 !!!!!!!     // commands.Add(attack);
                                    // NGE05222024 !!!!!!!     // return commands.ToArray();
                                    // NGE05222024 !!!!!!! }
                                }

                                if (attackSquad != null)
                                {
                                    if (nearestOrbDistance < armyPosDistanceToOrb)
                                    {
                                        myArmyPos = attackSquad.Entity.Position.To2D();
                                    }
                                }

                                float nearestWellDistance = float.MaxValue;

                                // NGE05232024 Below !!!!!!
                                // if (isEnemyNearBase == false) // NGE06282024 
                                {
                                    isEnemyNearBase = IsEnemyNearBase(100f, out List<Squad> enemySquadAttackingNearBase); // 75 seems to include nearby well or orb
                                    if (isEnemyNearBase == true)
                                    {
                                        enemyAttackingSquads = enemySquadAttackingNearBase;
                                    }
                                }
                                if (isEnemyNearBase == false) // NGE06282024  // else
                                {
                                    enemyAttackingSquads = null;
                                    isEnemyNearBase = false;
                                }
                                // NGE05232024 Above !!!!!!

                                // NGE06252024!!!!!! If there are more enemies than defenders, create more defenders
                                // NGE06252024!!!!!! if ((myWalls != null && myWalls.Count() > 0) && defendSquadCount < defaultDefendSquads ||
                                // NGE06252024!!!!!!     (enemyAttackingSquads != null && enemyAttackingSquads.Count() > defendSquadCount))
                                if ((myWalls != null && myWalls.Count() > 0) && defendSquadCount < defaultDefendSquads ||
                                    (enemyAttackingSquads != null && enemyAttackingSquads.Count() > defendSquadCount))
                                // NGE06252024 OKAY if ((myWalls != null && myWalls.Count() > 0) && defendSquadCount < defaultDefendSquads)
                                {
                                    string message = string.Format("defendSquadCount={0} defaultDefendSquads={1}", defendSquadCount, defaultDefendSquads);
                                    //ConsoleWriteLine(true, message); // ConsoleWriteLine(consoleWriteline, message);
                                    if ((enemyAttackingSquads != null && enemyAttackingSquads.Count() > defendSquadCount))
                                    {
                                        message = string.Format("enemyAttackingSquads={0}", enemyAttackingSquads.Count());
                                        //ConsoleWriteLine(true, message); // ConsoleWriteLine(consoleWriteline, message);
                                    }

                                    // NGE11022024 Position2D wallPos = myWalls[0].Entity.Position.To2D(); // Get nearest wall pos
                                    // NGE11022024 EntityId myWallId = myWalls[0].Entity.Id;

                                    //spawn = SpawnArcherOnBarrier(myArmy, myPower, botState.myStart, myWallId, unitPower, currentTick.V); // wallPos
                                    if (archerCardPositions != null)
                                    {
                                        byte cardPosition = (byte)archerCardPositions[0]; // Get first archer for now
                                        int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[cardPosition]; // Should be an archer

                                        int unitPowerArcher = 75; // Default power cost - should be able to find info about card
                                        if (unitOfficialCardId != 0) // Not a card
                                        {
                                            SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                                            if (card != null)
                                            {
                                                unitPowerArcher = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                            }
                                        }

                                        if (myWalls.Count > 0)
                                        {
                                            Command? commandSpawnArcherOnWall = SpawnArcherOnWall(myWalls[0].Entity, myWallModules, currentTick.V, myPower, unitPowerArcher, ref myPower);
                                            if (commandSpawnArcherOnWall != null)
                                            {
                                                commands.Add(commandSpawnArcherOnWall);
                                                return commands.ToArray();// NGE05122024!!!!!! return commands.ToArray();
                                            }
                                        }
                                    }
                                }

                                if (attackSquadCount < defaultAttackSquads || myPower > maxPowerToDoMore) // NGE08272024!!!!! else if (attackSquadCount < defaultAttackSquads || myPower > maxPowerToDoMore)
                                {
                                    //string message = string.Format("SpawnUnit Attack with {0} army < {1}", attackSquads.Count(), defaultAttackSquads);
                                    //ConsoleWriteLine(consoleWriteline, message);

                                    // NGE08272024 if the army contains a Wrecker in it, turn on its ability so that newly spawned units will not be dazed
                                    bool test = false;
                                    bool doNotCastSpell = IsPosNearWellOrOrb(myArmyPos, myOrbs, myWells, 15);
                                    if (test == true && doNotCastSpell == false)
                                    {
                                        CallRallyingCrySpellWhenAppropriate(test, myArmyPos, commands, ref myPower);
                                        //if (test == true && attackSquadCount > 0 && wreckerCardPosition != null)
                                        //{
                                        //    // See if a unit in the squad is a wrecker; if so, activate the rallying cry
                                        //    List<Squad> wreckerSquads = myAttackSquads.Where(a => a.CardId == currentDeck.Cards[(int)wreckerCardPosition]).ToList(); // See if I have any wreckers
                                        //    if (wreckerSquads != null && wreckerSquads.Count > 0)
                                        //    {
                                        //        // NGE08272024 !!!!!!! wreckerSquads[0] // Call the rallying cry
                                        //        int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[(int)wreckerCardPosition];
                                        //        SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                                        //        if (card.abilities != null && card.abilities.Count > 0)
                                        //            Console.WriteLine("cast card {0} spell {1} id {2}", card.cardName, card.abilities[0].abilityName, 3001274);
                                        //        Command command = new CommandCastSpellEntity
                                        //        {
                                        //            Entity = wreckerSquads[0].Entity.Id,
                                        //            Spell = new SpellId((uint)3001274),
                                        //            Target = new SingleTargetHolder(new SingleTargetSingleEntity { Id = wreckerSquads[0].Entity.Id }),
                                        //        };
                                        //        commands.Add(command);
                                        //    }
                                        //}
                                    }

                                    int cardID = 0; // Default card ID
                                                    
                                    bool armyHasSwiftUnit = false; // If army does not have a swift unit, make one!
                                    if (myAttackSquads != null)
                                    {
                                        foreach (var s in myAttackSquads) // if squad is near Entity, attack it
                                        {
                                            if (swiftCardPositions != null && swiftCardPositions.Count > 0)
                                            {
                                                armyHasSwiftUnit = true;
                                                break;
                                            }

                                        }
                                        if (armyHasSwiftUnit == false)
                                        {
                                            if (swiftCardPositions != null && swiftCardPositions.Count > 0)
                                            {
                                                cardID = swiftCardPositions[0];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (armyHasSwiftUnit == false)
                                        {
                                            if (swiftCardPositions != null && swiftCardPositions.Count > 0)
                                            {
                                                cardID = swiftCardPositions[0];
                                            }
                                        }
                                    }
                                    SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, cardID);
                                    if (card != null)
                                    {
                                        unitPower = card.powerCost[3]; // Assume unit fully upgraded for now!!!!
                                    }
                                    spawn = SpawnUnit(myPower, myArmyPos, (byte)cardID, unitPower, currentTick.V, ref myPower);
                                }
                                else // Build more wells or orbs // NGE05222024!!!!!
                                {
                                    if (myWalls.Count() > 0 && myDefendSquads != null && myDefendSquads.Count() > 0) // Make sure archers are on walls if near them!
                                    {
                                        //archerSquad
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

                                //Squad? attackSquad = null;
                                //foreach (var s in enemyOrbs) // Attack enemy Orbs
                                //{
                                //    foreach (Squad squad in mySquads) 
                                //    {
                                //        float squadDistanceToOrb = DistanceSquared(squad.Entity.Position.To2D(), s.Entity.Position);
                                //        if (squadDistanceToOrb < nearestOrbDistance)
                                //        {
                                //            nearestOrbDistance = squadDistanceToOrb;
                                //            targetOrb = s.Entity.Id;
                                //            targetEntityOrb = s.Entity;
                                //            attackSquad = squad;
                                //        }
                                //    }
                                //}

                                if (attackClosestWell == true || enemyOrbs.Count() == 0) // Since orbs are gone, attack the wells!
                                {
                                    foreach (var s in enemyWells) // Attack enemy Wells
                                    {
                                        foreach (Squad squad in myAttackSquads)
                                        {
                                            float squadDistanceToWell = DistanceSquared(squad.Entity.Position.To2D(), s.Entity.Position);
                                            if (squadDistanceToWell < nearestWellDistance)
                                            {
                                                nearestWellDistance = squadDistanceToWell;
                                                targetWell = s.Entity.Id;
                                                targetEntityWell = s.Entity;
                                                attackSquad = squad;
                                            }
                                        }
                                    }
                                }

                                //// NGE05232024 Below !!!!!!
                                //// if (isEnemyNearBase == false) // NGE06282024 
                                //{
                                //    isEnemyNearBase = IsEnemyNearBase(100f, out List<Squad> enemySquadAttackingNearBase); // 75 seems to include nearby well or orb
                                //    enemyAttackingSquads = enemySquadAttackingNearBase;
                                //}
                                //if (isEnemyNearBase == false) // NGE06282024  // else
                                //{
                                //    enemyAttackingSquads = null;
                                //    isEnemyNearBase = false;
                                //}
                                //// NGE05232024 Above !!!!!!

                                // NGE06252024 if (enemyAttackingSquads != null && enemyAttackingSquads.Count() > 0)
                                // NGE06252024 {
                                // NGE06252024     // if (defaultDefendSquads < enemyAttackingSquads.Count())
                                // NGE06252024     {
                                // NGE06252024         defaultDefendSquads = Math.Max(defaultDefendSquads, enemyAttackingSquads.Count());
                                // NGE06252024     }
                                // NGE06252024 }

                                /* */ // NGE08272024!!!!!  // NGE05232024 !!!!!!

                                /* // NGE05232024 !!!!!! 
                                #region // If an enemy squad is a certain distance from my main orb, attack it!
                                //if (attackingEnemyNearBase == false)
                                {
                                    if (IsEnemyNearBase(175f, out List<Squad> enemySquadAttackingNearBase) == true) // I think 175 deals with long range enemy fire???
                                    {
                                        List<Squad>? nearbySquads = NearestSquadsToEnemy(enemySquadAttackingNearBase);
                                        // How many enemies are attacking?
                                        int enemiesAttacking = enemySquadAttackingNearBase.Count();
                                        if (nearbySquads != null && nearbySquads.Count() > 0)
                                        {
                                            //if (nearbySquads.Count() < enemiesAttacking)
                                            enemyAttackingSquads = enemySquadAttackingNearBase;
                                            if (enemyAttackingSquads.Count() > 0)
                                            {
                                                targetSquad = enemyAttackingSquads[0].Entity.Id;
                                                targetEntitySquad = enemyAttackingSquads[0].Entity;
                                                Command? attackNearestEnemy = AttackNearbyEnemy(targetSquad, "squad", nearbySquads, targetEntitySquad.Position.To2D(), 1);
                                                if (attackNearestEnemy != null)
                                                {
                                                    attackingEnemyNearBase = true;
                                                    commands.Add(attackNearestEnemy);
                                                    // NGE05122024!!!!!! return commands.ToArray();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            enemyAttackingSquads = null;
                                            attackingEnemyNearBase = false;
                                        }

                                        #region // NGE08272024 Just 1 squad
                                        // NGE08272024 Squad? nearestSquad = NearestSquadToEnemy(enemySquadAttackingNearBase);
                                        // NGE08272024 if (nearestSquad != null)
                                        // NGE08272024 {
                                        // NGE08272024     enemyAttackingSquads = enemySquadAttackingNearBase;
                                        // NGE08272024     if (enemyAttackingSquads.Count() > 0)
                                        // NGE08272024     {
                                        // NGE08272024         targetSquad = enemyAttackingSquads[0].Entity.Id;
                                        // NGE08272024         targetEntitySquad = enemyAttackingSquads[0].Entity;
                                        // NGE08272024         attackSquad = nearestSquad;
                                        // NGE08272024         string messageAttackEnemyNearBase = string.Format("Attack enemy near base:{0} with squad:{1}", targetEntitySquad.Id, nearestSquad.Entity.Id);
                                        // NGE08272024         Command? attackNearestEnemy = AttackNearbyEnemy(targetSquad, "squad", new List<Squad> { nearestSquad }, targetEntitySquad.Position.To2D(), 1);
                                        // NGE08272024         if (attackNearestEnemy != null)
                                        // NGE08272024         {
                                        // NGE08272024             if (enemyAttackingSquads.Contains(nearestSquad) == false)
                                        // NGE08272024             {
                                        // NGE08272024                 enemyAttackingSquads.Add(nearestSquad);
                                        // NGE08272024             }
                                        // NGE08272024             attackingEnemyNearBase = true;
                                        // NGE08272024             commands.Add(attackNearestEnemy);
                                        // NGE08272024             // NGE05122024!!!!!! return commands.ToArray();
                                        // NGE08272024         }
                                        // NGE08272024     }
                                        // NGE08272024 }
                                        // NGE08272024 else
                                        // NGE08272024 {
                                        // NGE08272024     enemyAttackingSquads = null;
                                        // NGE08272024     attackingEnemyNearBase = false;
                                        // NGE08272024 }
                                #endregion // NGE08272024 Just 1 squad
                            }
                            else
                                    {
                                        enemyAttackingSquads = null;
                                        attackingEnemyNearBase = false;
                                    }
                                }
                                //else
                                //{
                                //    if (attackingEnemyNearBaseSquads.Count > 0) // Check health of squad 
                                //    {
                                //        double health = 0;
                                //        foreach (var squad in attackingEnemyNearBaseSquads)
                                //        {
                                //            GetSquadHealth(state, squad, out double squadHealth, out List<EntityId> unitsHealthy);
                                //            if (squadHealth != 0)
                                //            {
                                //                health = squadHealth;
                                //            }
                                //            else // remove Squad from attackingEnemyNearBaseSquads
                                //            {
                                //                // attackingEnemyNearBaseSquads.Remove(squad);
                                //            }
                                //        }
                                //        if (health == 0)
                                //        {
                                //            attackingEnemyNearBaseSquads.Clear();
                                //            attackingEnemyNearBase = false;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        attackingEnemyNearBase = false;
                                //    }
                                //}
                                #endregion // If an enemy squad is a certain distance from my main orb, attack it!
                                */ // NGE05232024 !!!!!!

                                /* // NGE08272024!!!!! */ // NGE05232024 !!!!!!

                                float nearestOpponentSquadDistance = float.MaxValue;
                                if (attackClosestSquad == true || (enemyOrbs.Count() == 0 && enemyWells.Count() == 0))
                                {
                                    try
                                    {
                                        lock (enemySquadsLock)
                                        {
                                            foreach (var s in enemySquads) // if squad is near me attack it
                                            {
                                                foreach (Squad squad in myAttackSquads)
                                                {
                                                    float squadDistanceToOpponent = DistanceSquared(squad.Entity.Position.To2D(), s.Entity.Position);
                                                    if (squadDistanceToOpponent < nearestOpponentSquadDistance)
                                                    {
                                                        nearestOpponentSquadDistance = squadDistanceToOpponent;
                                                        targetSquad = s.Entity.Id;
                                                        targetEntitySquad = s.Entity;
                                                        attackSquad = squad;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string exMessage = ex.Message;
                                    }
                                }

                                float nearestOpponentBarrierDistance = float.MaxValue;
                                Position nearestOpponentBarrierPosition;

                                bool isBarrierOpen = true;
                                // List<Entity>? enemyBarrierModuleEntities = null;
                                List<BarrierModule>? enemyBarrierModules = null;
                                List<BarrierSet>? wallsToAttack = GetWallsNearOrbs(enemyWalls, enemyOrbs); // First attack enemyWalls that are near an enemy orb!


                                if (wallsToAttack != null && wallsToAttack.Count() > 0)
                                {
                                    AttackBarrierModules(wallsToAttack, state, enemyBarrierModules, ref nearestOpponentBarrierDistance, ref isBarrierOpen, ref targetEntityWall, ref targetWall, ref attackSquad);
                                    /* // NGE05292024 
                                    foreach (var wall in wallsToAttack)
                                    {
                                        isBarrierOpen = BreachBarrierSetModules(state, wall, enemyWallModules);
                                        if (isBarrierOpen == false)
                                        {
                                            if (enemyBarrierModules == null)
                                            {
                                                enemyBarrierModules = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                                            }
                                            else
                                            {
                                                List<BarrierModule>? barrierModuleTemp = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                                                if (barrierModuleTemp != null && barrierModuleTemp.Count() > 0)
                                                {
                                                    enemyBarrierModules.AddRange(barrierModuleTemp);
                                                }
                                            }
                                        }
                                    }
                                    */ // NGE05292024 
                                }
                                else
                                {
                                    wallsToAttack = GetWallsNearWells(enemyWalls, enemyWells); // Second attack enemyWalls that are near an enemy well!
                                    if (wallsToAttack != null && wallsToAttack.Count() > 0)
                                    {
                                        AttackBarrierModules(wallsToAttack, state, enemyBarrierModules, ref nearestOpponentBarrierDistance, ref isBarrierOpen, ref targetEntityWall, ref targetWall, ref attackSquad);
                                        /* // NGE05292024 
                                        foreach (var wall in wallsToAttack)
                                        {
                                            isBarrierOpen = BreachBarrierSetModules(state, wall, enemyWallModules);
                                            if (isBarrierOpen == false)
                                            {
                                                if (enemyBarrierModules == null)
                                                {
                                                    enemyBarrierModules = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                                                }
                                                else
                                                {
                                                    List<BarrierModule>? barrierModuleTemp = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                                                    if (barrierModuleTemp != null && barrierModuleTemp.Count() > 0)
                                                    {
                                                        enemyBarrierModules.AddRange(barrierModuleTemp);
                                                    }
                                                }
                                            }
                                        }
                                        */ // NGE05292024 
                                    }
                                    else
                                    {
                                        if (wallsToAttack != null && wallsToAttack.Count() > 0)
                                        {
                                            AttackBarrierModules(wallsToAttack, state, enemyBarrierModules, ref nearestOpponentBarrierDistance, ref isBarrierOpen, ref targetEntityWall, ref targetWall, ref attackSquad);
                                            /* // NGE05292024 
                                            foreach (var wall in wallsToAttack)
                                            {
                                                isBarrierOpen = BreachBarrierSetModules(state, wall, enemyWallModules);
                                                if (isBarrierOpen == false)
                                                {
                                                    if (enemyBarrierModules == null)
                                                    {
                                                        enemyBarrierModules = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                                                    }
                                                    else
                                                    {
                                                        List<BarrierModule>? barrierModuleTemp = enemyWallModules.Where(m => m.Set == wall.Entity.Id).Select(s => s).ToList();
                                                        if (barrierModuleTemp != null && barrierModuleTemp.Count() > 0)
                                                        {
                                                            enemyBarrierModules.AddRange(barrierModuleTemp);
                                                        }
                                                    }
                                                }
                                            }
                                            */ // NGE05292024 
                                        }
                                    }
                                }

                                /* // NGE05292024 
                                if (isBarrierOpen == false && enemyBarrierModules != null)
                                {
                                    foreach (var s in enemyBarrierModules) // NGE05222024 enemyWallModules) // if barrier module is near me attack it if it is not broken or the gate is open
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
                                        foreach (Squad squad in myAttackSquads)
                                        {
                                            float squadDistanceToWall = DistanceSquared(squad.Entity.Position.To2D(), nearestOpponentBarrierPosition);
                                            //float squadDistanceToWall = DistanceSquared(attackSquad.Entity.Position.To2D(), s.Entity.Position);
                                            if (squadDistanceToWall < nearestOpponentBarrierDistance)
                                            {
                                                nearestOpponentBarrierDistance = squadDistanceToWall;
                                                targetWall = s.Entity.Id;
                                                targetEntityWall = s.Entity;
                                                attackSquad = squad;
                                            }
                                        }
                                    }
                                }
                                */ // NGE05292024 

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

                                List<Squad>? squads = null;
                                if (targetEntity != null && attackSquad != null)
                                {
                                    if (engagingEnemyAttackingSquads != null && myAttackSquads != null && engagingEnemyAttackingSquads.Count() > 0)
                                    {
                                        myAttackSquads = myAttackSquads.Except(engagingEnemyAttackingSquads).ToList();
                                    }
                                    squads = myAttackSquads; // NGE05212024 

                                    /* // NGE05212024 
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
                                    */ // NGE05212024 
                                }

                                // NGE05222024 If a squad is closer to an attacking enemy, attack the enemy; otherwise, attack the original target
                                if (targetEntity != null && targetType != null && squads != null && squads.Count() > 0 && enemyAttackingSquads != null && enemyAttackingSquads.Count() > 0)
                                {
                                    GetAttackingSquadsCloserToEnemyThanTarget(enemyAttackingSquads, squads, targetEntity, out List<Squad>? engageEnemyAttackingSquads, out List<Squad>? myAttackingSquadsUpdated);
                                    if (engageEnemyAttackingSquads != null && engageEnemyAttackingSquads.Count() > 0)
                                    {
                                        cmdOpenGate = OpenGateWhenNeeded(currentTick);

                                        attackEnemy = Attack(enemyAttackingSquads[0].Entity.Id, "squad", engageEnemyAttackingSquads, targetEntity.Position.To2D(), unitsNeededBeforeAttack);
                                    }
                                    if (myAttackingSquadsUpdated != null && myAttackingSquadsUpdated.Count() > 0)
                                    {
                                        cmdOpenGate = OpenGateWhenNeeded(currentTick);

                                        attack = Attack(target, targetType, myAttackingSquadsUpdated, targetEntity.Position.To2D(), unitsNeededBeforeAttack);
                                    }
                                }
                                else // Attack original target!
                                {
                                    if (squads != null && squads.Count() > 0 && currentTick.V % defaultTickUpdateRate == 0) // NGE05062024 added  && currentTick.V % defaultTickUpdateRate == 0
                                    {
                                        cmdOpenGate = OpenGateWhenNeeded(currentTick);

                                        attack = Attack(target, targetType, squads, targetEntity.Position.To2D(), unitsNeededBeforeAttack);
                                    }
                                }


                                if (squads != null && currentTick.V % defaultTickUpdateRate == 0)
                                {
                                    if (previousTarget != target && previousSquadCount != squads.Count())
                                    {
                                        string msg = string.Format("Tick: {0} Target:{1} My Power:{2} Squads:{3} Enemy Squads:{4}", currentTick, target, (int)myPower, squads.Count(), enemySquads.Count());
                                        ConsoleWriteLine(consoleWriteline, msg);
                                    }
                                }

                                if (spawn == null && attack == null && attackEnemy == null)
                                {
                                    // return Array.Empty<Command>();
                                }
                                else if (spawn != null && attack != null && attackEnemy != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Spawn unit, attack target and enemy");
                                    }
                                    commands.Add(spawn);
                                    if (cmdOpenGate != null)
                                    {
                                        commands.Add(cmdOpenGate);
                                    }
                                    commands.Add(attackEnemy);
                                    commands.Add(attack);
                                }
                                else if (spawn != null && attack != null && attackEnemy == null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Spawn unit, attack target");
                                    }
                                    commands.Add(spawn);
                                    if (cmdOpenGate != null)
                                    {
                                        commands.Add(cmdOpenGate);
                                    }
                                    commands.Add(attack);
                                }
                                else if (spawn != null && attack == null && attackEnemy != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Spawn unit, attack target and enemy");
                                    }
                                    commands.Add(spawn);
                                    if (cmdOpenGate != null)
                                    {
                                        commands.Add(cmdOpenGate);
                                    }
                                    commands.Add(attackEnemy);
                                }
                                else if (spawn != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Spawn unit");
                                    }
                                    commands.Add(spawn);
                                }
                                else if (attack != null && attackEnemy != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Attack target and enemy");
                                    }
                                    if (cmdOpenGate != null)
                                    {
                                        commands.Add(cmdOpenGate);
                                    }
                                    commands.Add(attack);
                                    commands.Add(attackEnemy);
                                }
                                else if (attack == null && attackEnemy != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Attack enemy");
                                    }
                                    if (cmdOpenGate != null)
                                    {
                                        commands.Add(cmdOpenGate);
                                    }
                                    commands.Add(attackEnemy);
                                }
                                else if (attack != null)
                                {
                                    if (botState.isGameStart == true)
                                    {
                                        Console.WriteLine("Attack target");
                                    }
                                    if (cmdOpenGate != null)
                                    {
                                        commands.Add(cmdOpenGate);
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

        public bool IsPosNearWellOrOrb(Position2D pos, List<TokenSlot> orbs, List<PowerSlot> wells, float distance = 15)
        {
            foreach (TokenSlot orb in orbs)
            {
                double distanceToWell = Math.Sqrt(DistanceSquared(pos, orb.Entity.Position));
                if (distanceToWell <= distance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            foreach (PowerSlot well in wells)
            {
                double distanceToWell = Math.Sqrt(DistanceSquared(pos, well.Entity.Position));
                if (distanceToWell <= distance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        // Note: need to wait for the cast steps or resolve steps before creating a squad - in this case 1000 milliseconds or 1 second
        public void CallRallyingCrySpellWhenAppropriate(bool test, Position2D pos, List<Command> commands, ref float powerRemaining)
        {
            // Only call when the squad being spawned is not near a well or orb // more than 15 meters away
            if (test == true && attackSquadCount > 0 && wreckerCardPosition != null)
            {
                // See if a unit in the squad is a wrecker; if so, activate the rallying cry
                List<Squad> wreckerSquads = myAttackSquads.Where(a => a.CardId == currentDeck.Cards[(int)wreckerCardPosition]).ToList(); // See if I have any wreckers
                if (wreckerSquads != null && wreckerSquads.Count > 0)
                {
                    // See if the wrecker squad is near the pos
                    foreach (Squad s in wreckerSquads)
                    {
                        if (Math.Sqrt(DistanceSquared(pos, s.Entity.Position)) <= 15)
                        {
                            int unitOfficialCardId = myCurrentDeckOfficialCardIds.Ids[(int)wreckerCardPosition];
                            SMJCard? card = GetCardFromOfficialCardId(cardsSMJ, unitOfficialCardId);
                            if (card != null && card.abilities != null && card.abilities.Count > 0)
                            {
                                Console.WriteLine("cast card {0} spell {1} id {2}", card.cardName, card.abilities[0].abilityName, 3001274);
                                Command command = new CommandCastSpellEntity
                                {
                                    Entity = wreckerSquads[0].Entity.Id,
                                    Spell = new SpellId((uint)3001274),
                                    Target = new SingleTargetHolder(new SingleTargetSingleEntity { Id = wreckerSquads[0].Entity.Id }),
                                };
                                powerRemaining -= card.abilities[0].abilityCost[0];
                                commands.Add(command);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private Command? SpawnArcherOnWall(Entity wall, List<BarrierModule> barrierModules, uint currentTick, float myPower, int unitPower, ref float powerRemaining)
        {
            if (myWalls.Count() > 0)
            {
                Position2D wallPos = wall.Position.To2D(); // Get nearest wall pos
                EntityId myWallId = myWalls[0].Entity.Id;

                if (archerCardPositions != null)
                {
                    Command? spawnOnBarrier = null;
                    foreach (var b in barrierModules)
                    {
                        var gateAspect = b.Entity.Aspects.FirstOrDefault(g => g.BarrierGate != null) ?? null;
                        if (b.Entity.PlayerEntityId == botState.myId && gateAspect == null) // When gateAspect == null then it is not a gate
                        {
                            // How to find barrier module closest to attacking enemy in order to put the archer as close to enemy as possible?
                            Position2D pos = new Position2D { X = wallPos.X - (wallPos.X - botState.myStart.X) / 2, Y = wallPos.Y - (wallPos.Y - botState.myStart.Y) / 2 }; // Center of Wall and starting orb

                            double distance = Math.Sqrt(DistanceSquared(wallPos, b.Entity.Position));
                            //string message = string.Format("distance:{0:0.0}", distance);
                            //ConsoleWriteLine(consoleWriteline, message);

                            if (b.FreeSlots >= 6 && myPower >= unitPower && botState.canPlayCardAt < currentTick && distance < 15) // NGE06282024 !!!!!!!!! // NGE08272024 Must be 15 or less!!!!!!
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

        public List<BarrierModule>? GetWallModulesNearOrbs(List<BarrierModule> allWallModules, List<TokenSlot> allOrbs)
        {
            float maxDistanceBetweenWallAndOrb = 15;

            List<BarrierModule>? wallsNearObs = null;
            foreach (var o in allOrbs)
            {
                foreach (var w in allWallModules)
                {
                    float orbDistanceToWall = MathF.Sqrt(DistanceSquared(w.Entity.Position.To2D(), o.Entity.Position.To2D()));
                    {
                        if (orbDistanceToWall < maxDistanceBetweenWallAndOrb)
                        {
                            if (wallsNearObs == null)
                            {
                                wallsNearObs = new List<BarrierModule>();
                            }
                            if (wallsNearObs.Contains(w) == false)
                            {
                                wallsNearObs.Add(w);
                                break; // foreach (var w in allWalls)
                            }
                        }
                    }

                }
            }
            return wallsNearObs;
        }

        public List<BarrierModule>? GetWallModulesNearWells(List<BarrierModule> allWalls, List<PowerSlot> allWells)
        {
            float maxDistanceBetweenWallAndWell = 15;

            List<BarrierModule>? wallsNearWells = null;
            foreach (var o in allWells)
            {
                foreach (var w in allWalls)
                {
                    float wellDistanceToWall = MathF.Sqrt(DistanceSquared(w.Entity.Position.To2D(), o.Entity.Position.To2D()));
                    {
                        if (wellDistanceToWall < maxDistanceBetweenWallAndWell)
                        {
                            if (wallsNearWells == null)
                            {
                                wallsNearWells = new List<BarrierModule>();
                            }
                            if (wallsNearWells.Contains(w) == false)
                            {
                                wallsNearWells.Add(w);
                                break; // foreach (var w in allWalls)
                            }
                        }
                    }

                }
            }
            return wallsNearWells;
        }

        public List<BarrierSet>? GetWallsNearOrbs(List<BarrierSet> allWalls, List<TokenSlot> allOrbs)
        {
            float maxDistanceBetweenWallAndOrb = 15;

            List<BarrierSet>? wallsNearObs = null;
            foreach (var o in allOrbs)
            {
                foreach (var w in allWalls)
                {
                    float orbDistanceToWall = MathF.Sqrt(DistanceSquared(w.Entity.Position.To2D(), o.Entity.Position.To2D()));

                    if (orbDistanceToWall < maxDistanceBetweenWallAndOrb)
                    {
                        if (wallsNearObs == null)
                        {
                            wallsNearObs = new List<BarrierSet>();
                        }
                        if (wallsNearObs.Contains(w) == false)
                        {
                            wallsNearObs.Add(w);
                            break; // foreach (var w in allWalls)
                        }
                    }
                }
            }
            return wallsNearObs;
        }

        public BarrierSet? GetWallNearOrb(List<BarrierSet> allWalls, TokenSlot orb)
        {
            float maxDistanceBetweenWallAndOrb = 15;

            BarrierSet? wallsNearOb = null;
            foreach (var w in allWalls)
            {
                float orbDistanceToWall = MathF.Sqrt(DistanceSquared(w.Entity.Position.To2D(), orb.Entity.Position.To2D()));

                if (orbDistanceToWall < maxDistanceBetweenWallAndOrb)
                {
                    wallsNearOb = w;
                }
            }
            return wallsNearOb;
        }

        public List<BarrierSet>? GetWallsNearWells(List<BarrierSet> allWalls, List<PowerSlot> allWells)
        {
            float maxDistanceBetweenWallAndWell = 15;

            List<BarrierSet>? wallsNearWells = null;
            foreach (var o in allWells)
            {
                foreach (var w in allWalls)
                {
                    float wellDistanceToWall = MathF.Sqrt(DistanceSquared(w.Entity.Position.To2D(), o.Entity.Position.To2D()));
                    {
                        if (wellDistanceToWall < maxDistanceBetweenWallAndWell)
                        {
                            if (wallsNearWells == null)
                            {
                                wallsNearWells = new List<BarrierSet>();
                            }
                            if (wallsNearWells.Contains(w) == false)
                            {
                                wallsNearWells.Add(w);
                                break; // foreach (var w in allWalls)
                            }
                        }
                    }

                }
            }
            return wallsNearWells;
        }

        public BarrierSet? GetWallNearWell(List<BarrierSet> allWalls, PowerSlot well)
        {
            float maxDistanceBetweenWallAndOrb = 15;

            BarrierSet? wallsNearWell = null;
            foreach (var w in allWalls)
            {
                float orbDistanceToWall = MathF.Sqrt(DistanceSquared(w.Entity.Position.To2D(), well.Entity.Position.To2D()));

                if (orbDistanceToWall < maxDistanceBetweenWallAndOrb)
                {
                    wallsNearWell = w;
                }
            }
            return wallsNearWell;
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
                                ConsoleWriteLine(consoleWriteline, "Build Well");
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
                                ConsoleWriteLine(consoleWriteline, "Build Orb");
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
                ConsoleWriteLine(consoleWriteline, "produce swiftclaw");
                tasks.swiftclawTasks.produceSwiftclaw = true;
                return new CommandProduceSquad
                {
                    CardPosition = 0,
                    Xy = botState.myStart
                };
            }
            else if (tasks.swiftclawTasks.produceSwiftclaw && tasks.swiftclawTasks.swiftclaw.V == 0)
            {
                var SEARCH_ID = CardIdCreator.New(Api.CardTemplate.Swiftclaw, Api.Upgrade.U3);
                var swiftclaw = Array.Find(entities.Squads, s => s.CardId == SEARCH_ID);
                if (swiftclaw != null)
                {
                    tasks.swiftclawTasks.swiftclaw = swiftclaw.Entity.Id;
                    ConsoleWriteLine(consoleWriteline, $"Swiftclaw = {swiftclaw.Entity.Id}");
                    // We found him, but give him a task on next tick
                }
                return null;
            }
            else if (tasks.swiftclawTasks.produceSwiftclaw && tasks.swiftclawTasks.swiftclaw.V > 0)
            {
                var swiftclaw = entities.Squads.FirstOrDefault(s => s.Entity.Id == tasks.swiftclawTasks.swiftclaw);
                if (swiftclaw == null)
                {
                    ConsoleWriteLine(consoleWriteline, "Swiftclaw was killed");
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
                        ConsoleWriteLine(consoleWriteline, "Build Primal Defender");
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
                            ConsoleWriteLine(consoleWriteline, "Token slot build");
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
                            ConsoleWriteLine(consoleWriteline, "Swiftclaw hold position");
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
                            ConsoleWriteLine(consoleWriteline, "Power slot build");
                            tasks.swiftclawTasks.buildPowerSlot = true;
                            return new CommandPowerSlotBuild
                            {
                                SlotId = tasks.swiftclawTasks.closestPowerSlot
                            };
                        }
                        else if (!tasks.swiftclawTasks.holdPosition)
                        {
                            tasks.swiftclawTasks.holdPosition = true;
                            ConsoleWriteLine(consoleWriteline, "Swiftclaw hold position");
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
                            ConsoleWriteLine(consoleWriteline, "Swiftclaw hold position");
                            return new CommandGroupHoldPosition
                            {
                                Squads = new[] { tasks.swiftclawTasks.swiftclaw }
                            };
                        }
                        else if (!tasks.swiftclawTasks.changeMode)
                        {
                            tasks.swiftclawTasks.changeMode = true;
                            ConsoleWriteLine(consoleWriteline, "Swiftclaw change mode");
                            return new CommandModeChange
                            {
                                EntityId = tasks.swiftclawTasks.swiftclaw,
                                NewModeId = new ModeId(3000497)
                            };
                        }
                        else
                        {
                            ConsoleWriteLine(consoleWriteline, "Swiftclaw done");
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
                            ConsoleWriteLine(consoleWriteline, "Swiftclaw Patrol");
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
                CardIdCreator.New(Api.CardTemplate.Windweavers, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Swiftclaw, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Shaman, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Spearmen, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.NastySurprise, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DarkelfAssassins, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Nightcrawler, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPaladins, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AmiiPhantom, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.ShadowPhoenix, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AuraofCorruption, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Tranquility, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CultistMaster, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.AshbonePyro, Api.Upgrade.U3),
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
                CardIdCreator.New(Api.CardTemplate.Wrecker, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Scavenger, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Mine, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Wallbreaker, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Enforcer, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Firedancer, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BreedingGrounds, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SwampDrake, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StrangleholdAShadow, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thunderstorm, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Api.Upgrade.U3),
    }
        };

        public static readonly DeckOfficialCardIds TopDeckCardIds = new()
        {
            Name = "TopDeck",
            Ids = new int[20]
    {
                (int)Api.CardTemplate.Wrecker,
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
                CardIdCreator.New(Api.CardTemplate.NomadANature, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Mine, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Eruption, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunderer, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Firedancer, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SkyfireDrake, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Ravage, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.CurseofOink, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.EnsnaringRoots, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.BreedingGrounds, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SurgeOfLight, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Sunstriders, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Hurricane, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Burrower, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.MagmaHurler, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.SwampDrake, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.StrangleholdAShadow, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DeathgliderAFrost, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.Thunderstorm, Api.Upgrade.U3),
                CardIdCreator.New(Api.CardTemplate.DryadAFrost, Api.Upgrade.U3),
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
            PvPCardDecks.TaintedFlame, PvPCardDecks.GiftedFlame, PvPCardDecks.GiftedFlameNew, PvPCardDecks.BlessedFlame, PvPCardDecks.InfusedFlame,
        };

        private static List<DeckOfficialCardIds> myDeckOfficialCardIds = new List<DeckOfficialCardIds>() { TopDeckCardIds, BattleGroundsCardIds, FireNatureCardIds,
              PvPCardDecks.TaintedDarknessCardIds, PvPCardDecks.GiftedDarknessCardIds, PvPCardDecks.BlessedDarknessCardIds, PvPCardDecks.InfusedDarknessCardIds,
              PvPCardDecks.TaintedFloraCardIds, PvPCardDecks.GiftedFloraCardIds, PvPCardDecks.BlessedFloraCardIds, PvPCardDecks.InfusedFloraCardIds,
              PvPCardDecks.TaintedIceCardIds, PvPCardDecks.GiftedIceCardIds, PvPCardDecks.BlessedIceCardIds, PvPCardDecks.InfusedIceCardIds,
              PvPCardDecks.TaintedFlameCardIds, PvPCardDecks.GiftedFlameCardIds, PvPCardDecks.GiftedFlameNewCardIds, PvPCardDecks.BlessedFlameCardIds, PvPCardDecks.InfusedFlameCardIds,
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
