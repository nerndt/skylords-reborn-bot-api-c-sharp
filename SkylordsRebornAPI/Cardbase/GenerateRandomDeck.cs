using SkylordsRebornAPI.Cardbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylordsRebornAPI.Cardbase
{
    // Notes from Maze on Discord: the first card that is added is always a t1 ground unit of the t1 orb color, so the deck is at least playable. It's not the prettiest code, but it works
    public static class GenerateRandomCardDeck
    {
        //public interface GenerateRandomDeckProps
        //{
        //    int[] OrbsOrder { get; set; }
        //    bool Neutrals { get; set; }
        //    bool Duplicates { get; set; }
        //}

        public class GenerateRandomDeckProps
        {
            public List<int> OrbsOrder { get; set; }
            public bool Neutrals { get; set; }
            public bool Duplicates { get; set; }
        }

        public static List<SMJCard> GenerateRandomDeck(SMJCard[] cards, GenerateRandomDeckProps props)
        {
            while (props.OrbsOrder.Count < 4)
                props.OrbsOrder.Add(-1);

            for (int i = 0; i < props.OrbsOrder.Count; i++)
            {
                if (props.OrbsOrder[i] == -1)
                    props.OrbsOrder[i] = (int)Math.Floor(new Random().NextDouble() * 4);
            }

            List<SMJCard> randomDeck = new List<SMJCard>();
            List<int> intProps = new List<int>();
            intProps.Add(0);
            for (int i = 0; i < props.OrbsOrder.Count; i++)
            {
                intProps.Add(props.OrbsOrder[i]);
            }

            List<object> allProps = new List<object>();
            allProps.Add("OrbsOrder");
            allProps.Add(intProps);
            allProps.Add(3);
            Params parms = new Params();
            parms.name = "OrbsOrder";
            parms.parameters = intProps;
            parms.upgrade = 3;

            bool moreCards = true;
            while (randomDeck.Count < 20 && moreCards)
            {
                List<SMJCard> preFilteredCards = (List<SMJCard>) cards.Where(sC =>
                    !randomDeck.Contains(sC) &&
                    (props.Neutrals || sC.color != -1) &&
                    (props.Duplicates || sC.promo != 1 || sC._id == "9M" || sC._id == "9N") &&
                    (props.Duplicates || !randomDeck.Any(rC => rC.cardNameSimple == sC.cardNameSimple)) &&
                    HandleFilterSpecial(sC, parms, 3)).ToList();

                if (preFilteredCards.Count > 0)
                {
                    SMJCard newRandomCard;

                    if (randomDeck.Count == 0)
                    {
                        List<SMJCard> filteredCards = preFilteredCards.Where(sC =>
                            sC.color == props.OrbsOrder[0] &&
                            sC.orbsTotal == 1 &&
                            sC.type == 0 &&
                            sC.movementType == 0
                        ).ToList();

                        newRandomCard = filteredCards[new Random().Next(filteredCards.Count)];
                    }
                    else
                    {
                        newRandomCard = preFilteredCards[new Random().Next(preFilteredCards.Count)];
                    }

                    randomDeck.Add(newRandomCard);
                }
                else
                {
                    moreCards = false;
                }
            }

            return randomDeck.OrderBy(a => a.orbsTotal)
                              .ThenBy(a => a.type)
                              .ThenBy(a => a.color)
                              .ThenBy(a => a._id)
                              .ToList();
        }

        public class HandleOrbsOrderValidationProps
        {
            public SMJCard card { get; set; }
            public int numNeutral { get; set; }
            public int numFire { get; set; }
            public int numShadow { get; set; }
            public int numNature { get; set; }
            public int numFrost { get; set; }
        }

        public static bool HandleOrbsOrderValidation(HandleOrbsOrderValidationProps props)
        {
            return HandleOrbsOrderValidation(props.card, props.numNeutral, props.numFire, props.numShadow, props.numNature, props.numFrost);
        }

        public static bool HandleOrbsOrderValidation(SMJCard card, int numNeutral, int numFire, int numShadow, int numNature, int numFrost)
        {
            bool fittingCard = false;

            if (numNeutral == 4)
            {
                return true;
            }
            else if (numNeutral == 0)
            {
                if (
                    numFire >= card.orbsFire &&
                    numShadow >= card.orbsShadow &&
                    numNature >= card.orbsNature &&
                    numFrost >= card.orbsFrost &&
                    numFire + numShadow >=
                        card.orbsFire + card.orbsShadow + card.orbsFireShadow &&
                    numNature + numFrost >=
                        card.orbsNature + card.orbsFrost + card.orbsNatureFrost &&
                    numFire + numNature >=
                        card.orbsFire + card.orbsNature + card.orbsFireNature &&
                    numShadow + numFrost >=
                        card.orbsShadow + card.orbsFrost + card.orbsShadowFrost &&
                    numShadow + numNature >=
                        card.orbsShadow + card.orbsNature + card.orbsShadowNature &&
                    numFire + numFrost >= card.orbsFire + card.orbsFrost + card.orbsFireFrost
                )
                {
                    return true;
                }
                return false;
            }

            int[] arr0 = new int[] { numFire, numShadow, numNature, numFrost };

            for (int i = 0; i < 4; i++)
            {
                int[] arr1 = (int[])arr0.Clone();
                arr1[i] += 1;

                if (numNeutral == 1)
                {
                    if (
                        arr1[0] >= card.orbsFire &&
                        arr1[1] >= card.orbsShadow &&
                        arr1[2] >= card.orbsNature &&
                        arr1[3] >= card.orbsFrost &&
                        arr1[0] + arr1[1] >=
                            card.orbsFire + card.orbsShadow + card.orbsFireShadow &&
                        arr1[2] + arr1[3] >=
                            card.orbsNature + card.orbsFrost + card.orbsNatureFrost &&
                        arr1[0] + arr1[2] >=
                            card.orbsFire + card.orbsNature + card.orbsFireNature &&
                        arr1[1] + arr1[3] >=
                            card.orbsShadow + card.orbsFrost + card.orbsShadowFrost &&
                        arr1[1] + arr1[2] >=
                            card.orbsShadow + card.orbsNature + card.orbsShadowNature &&
                        arr1[0] + arr1[3] >= card.orbsFire + card.orbsFrost + card.orbsFireFrost
                    )
                    {
                        fittingCard = true;
                        break;
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int[] arr2 = (int[])arr1.Clone();
                        arr2[j] += 1;

                        if (numNeutral == 2)
                        {
                            if (
                                arr2[0] >= card.orbsFire &&
                                arr2[1] >= card.orbsShadow &&
                                arr2[2] >= card.orbsNature &&
                                arr2[3] >= card.orbsFrost
                            )
                            {
                                fittingCard = true;
                                break;
                            }
                        }
                        else
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                int[] arr3 = (int[])arr2.Clone();
                                arr3[k] += 1;

                                if (numNeutral == 3)
                                {
                                    if (
                                        arr3[0] >= card.orbsFire &&
                                        arr3[1] >= card.orbsShadow &&
                                        arr3[2] >= card.orbsNature &&
                                        arr3[3] >= card.orbsFrost
                                    )
                                    {
                                        fittingCard = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return fittingCard;
        }

        public class Params
        {
            public string name;
            public List<int> parameters;
            public int upgrade;
        }

        //private static bool HandleFilterSpecial(StaticCardProps card, List<object> filterParams, int filterType)
        //{
        //    // Implement the HandleFilterSpecial function here
        //    throw new NotImplementedException();
        //}

        // public static bool HandleFilterSpecial(StaticCardProps card, (string, List<int> | List<string>) special, int upgrade)
        public static bool HandleFilterSpecial(SMJCard card, Params parms, int upgrade)
        {
            bool fitting = true;
            if ((string)parms.name == "orbsOrder")
            {
                int numNeutral = 0;
                int numFire = 0;
                int numShadow = 0;
                int numNature = 0;
                int numFrost = 0;
                List<int> paramIntList = parms.parameters;
                for (int i = 1; i < paramIntList.Count; i++)
                {
                    switch (paramIntList[i])
                    {
                        case -1:
                            numNeutral++;
                            break;
                        case 0:
                            numFire++;
                            break;
                        case 1:
                            numShadow++;
                            break;
                        case 2:
                            numNature++;
                            break;
                        case 3:
                            numFrost++;
                            break;
                        default:
                            break;
                    }

                    if (paramIntList[0] == 0 && card.orbsTotal == i)
                    {
                        if (!HandleOrbsOrderValidation(card, numNeutral, numFire, numShadow, numNature, numFrost))
                        {
                            fitting = false;
                            break;
                        }
                    }
                }

                if (paramIntList[0] == 1)
                {
                    if (!HandleOrbsOrderValidation(card, numNeutral, numFire, numShadow, numNature, numFrost))
                    {
                        fitting = false;
                    }
                }
            }
            return fitting;
        }


        public class StaticCardProps
        {
            public string Id { get; set; }
            public string CardNameSimple { get; set; }
            public int Color { get; set; }
            public int OrbsTotal { get; set; }
            public int Type { get; set; }
            public int MovementType { get; set; }
            public int Promo { get; set; }
        }

    }
}
