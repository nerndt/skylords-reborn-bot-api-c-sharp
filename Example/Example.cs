using Api;

namespace Bots
{
    // /AI: add CsExampleAI Tutorial 4
    // Once the Elyon - spectator map is chosen in Sparring Grounds, write a command in the message such as:
    // Which means call the AI script and add the bot called XanderAI, the deck Tutorial and the slot 4 (starting position on map for player)
    public class Example : IAspWrapperImpl
    {
        string IAspWrapperImpl.Name => "CsExampleAI";

        public Deck[] DecksForMap(Maps map, string? name, ulong crc)
        {
            if (map == Maps.LajeshSpectator)
            {
                return new[] { TUTORIAL_DECK };
            } else if (map == Maps.YrmiaSpectator || map == Maps.FyreSpectator)
            {
                return new[] { TAINTED_FLORA, TUTORIAL_DECK, BG1_DECK };
            } else
            {
                return Array.Empty<Deck>();
            }
        }

        public void PrepareForBattle(Maps map, string? name, ulong crc, Deck deck)
        {
            botState = new BotState
            {
                selectedDeck = deck
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
                }
                break;
            }
            foreach (var s in entities.PowerSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    Console.WriteLine($"Power slot: {s.Entity.Id} at {s.Entity.Position.X}/{s.Entity.Position.Z}");
                    botState.myStart = s.Entity.Position.To2D();
                }
            }
            foreach (var s in entities.TokenSlots)
            {
                if (s.Entity.PlayerEntityId == yourPlayerId)
                {
                    Console.WriteLine($"Power slot: {s.Entity.Id} at {s.Entity.Position.X}/{s.Entity.Position.Z}");
                }
            }
        }

        public Command[] Tick(GameState state)
        {
            var currentTick = state.CurrentTick;
            var entities = state.Entities;
            var myArmy = new List<EntityId>();
            var myOrbs = new List<EntityId>();
            var myWells = new List<EntityId>();
            var target = new EntityId(0u);
#pragma warning disable CS8602 // Player must exist
            var myPower = Array.Find(state.Players, p => p.Id == botState.myId).Power;
#pragma warning restore CS8602
            foreach (var s in entities.Squads)
            {
                if (s.Entity.PlayerEntityId == botState.myId)
                {
                    myArmy.Add(s.Entity.Id);
                }
            }
            foreach (var s in entities.TokenSlots)
            {
                if (s.Entity.PlayerEntityId != null && botState.oponents.Contains(s.Entity.PlayerEntityId))
                {
                    target = s.Entity.Id;
                }
            }
            Console.WriteLine($"Tick: {currentTick} target: {target} my power: {myPower} my army size: {myArmy.Count}");

            var spawn = SpawnUnit(myPower);
            var attack = Attack(target, myArmy);
            if (spawn == null && attack == null)
            {
                return Array.Empty<Command>();
            } else if (spawn != null && attack != null)
            {
                return new[] { spawn, attack };
            } else if (spawn != null)
            {
                return new[] { spawn };
            } else if (attack != null)
            {
                return new[] { attack };
            } else
            {
                throw new InvalidOperationException("Unreachable, because all cases are handled above");
            }
        }

        private Command? Attack(EntityId target, List<EntityId> squads)
        {
            if (target.V == 0)
            {
                return null;
            } else
            {
                return new CommandGroupAttack { Squads = squads.ToArray(), TargetEntityId = target, ForceAttack = false };
            }
        }

        private Command? SpawnUnit(float myPower)
        {
            if (botState.selectedDeck == TUTORIAL_DECK)
            {
                if (myPower >= 50.0f)
                {
                    return new CommandProduceSquad { CardPosition = 1, Xy = botState.myStart };
                } else
                {
                    return null;
                }
            } else if (botState.selectedDeck == TAINTED_FLORA)
            {
                if (myPower >= 70.0f)
                {
                    return new CommandProduceSquad { CardPosition = 2, Xy = botState.myStart };
                }
                else
                {
                    // Am I in trouble?
                    return null;
                }
            } else
            {
                throw new ApplicationException("I do not play with any other deck");
            }
        }

        private BotState botState;

        private static readonly Deck TUTORIAL_DECK = new()
        {
            Name = "Tutorial", 
            CoverCardIndex = 3,
            Cards = new CardId[20]
            {
                CardIdCreator.New(CardTemplate.MasterArchers, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Northguards, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Eruption, Upgrade.U3),
                CardIdCreator.New(CardTemplate.CannonTower, Upgrade.U3),
                CardIdCreator.New(CardTemplate.FireStalker, Upgrade.U3),
                CardIdCreator.New(CardTemplate.MagmaHurler, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Tremor, Upgrade.U3),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
                CardIdCreator.New(CardTemplate.NotACard, Upgrade.U0),
            }
        };

        private static readonly Deck TAINTED_FLORA = new()
        {
            Name = "TaintedFlora",
            CoverCardIndex = 0,
            Cards = new CardId[20]
            {
                CardIdCreator.New(CardTemplate.Swiftclaw, Upgrade.U3),
                CardIdCreator.New(CardTemplate.DryadAFrost, Upgrade.U3),
                CardIdCreator.New(CardTemplate.Windweavers, Upgrade.U3),
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
            public List<EntityId> oponents;
            public Position2D myStart;
            public EntityId myId;
            public Deck selectedDeck;
        }
    }
}
