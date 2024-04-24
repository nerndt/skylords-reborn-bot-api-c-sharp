using System.Collections.Generic;
using SkylordsRebornAPI.Cardbase.Cards;
using SkylordsRebornAPI.Cardbase.Shared;

namespace SkylordsRebornAPI.Cardbase
{
    // List<SkylordsRebornCard> myDeserializedClass = JsonConvert.DeserializeObject<List<SkylordsRebornCard>>(myJsonResponse);
    public class SkylordsRebornCard0
    {
        /*
        Notes: From Maze on Discord application. If you need a tiny bit of information, I also created a random deck generator a while back: https://smj.cards/deck?random=true

        The correct official api docs can be found here: https://hub.backend.skylords.eu/api/docs/

        All others might be out of date, with the cardbase definitely being out of date.

        The endpoint that might be of interest for you is /api/auctions/cards, some of the data might not be correct though, as it disregards ' in names, doesn't have all hybrid orbs (FireNature).

        You can get all the card data through: https://hub.backend.skylords.eu/api/auctions/cards?id=all

        If you need a bit more in depth card data, you could use the api I provide. The data should be mostly up to date except for the abilities and can be found here: https://smj.cards/api/cards

        How to Upload Card images:
        I downloaded them from the wiki, or extracted them from the game's pack files. they are dynamically generated from the different parts, and I also expose them through an api:
        https://smj.cards/api/images/fullCard/00

        with the option to specify some stuff

        https://smj.cards/api/images/fullCard/00?upgrades=2&charges=1

        How to upload a deck into the game:

        Make a computer application that to generate a random deck and provide codes to import the deck ingame.
        There are a few different code versions that work ingame:

        Official Version M Code, Official Version A Code, Official Card IDs code

        with the array of official card ids being the simplest one

        How to import/export decks in the game:
        simply type /importdeck CODE  while CODE being in on of the above formats
        similarly /exportdeck will get the M version Code of you currently selected deck
        Example of deck import command in the Game Skylords Reborn:

        /importdeck [253,254,255,256,287,288,289,290,301,302,303,304,305,316,344,345,346,354,355,356]


  */

        public int cardId { get; set; }
        public string cardName { get; set; }
        public string rarity { get; set; }
        public string expansion { get; set; }
        public string promo { get; set; }
        public string obtainable { get; set; }
        public int fireOrbs { get; set; }
        public int frostOrbs { get; set; }
        public int natureOrbs { get; set; }
        public int shadowOrbs { get; set; }
        public int fireShadowOrbs { get; set; }
        public int fireFrostOrbs { get; set; }
        public int natureFrostOrbs { get; set; }
        public int shadownNatureOrbs { get; set; }
        public int shadowFrostOrbs { get; set; }
        public int neutralOrbs { get; set; }
        public string affinity { get; set; }
        public string cardType { get; set; }
    }
    // SkylordsRebornCard myDeserializedClass = JsonConvert.DeserializeObject<List<SkylordsRebornCard>>(myJsonResponse);
}