using System.Collections.Generic;

public class LocationCard : EnemyCard
{

    private int progress_tokens_added;
    public LocationCard(string card_name, int threat_strength, int quest_points, 
        string ability, string encounter_set, List<LOTRGame.TRAITS> traits,
        string set_information) : base(card_name, engagement_cost: -1, threat_strength: threat_strength, attack: -1,
        defense: -1, quest_points: quest_points, hp: -1,encounter_set: encounter_set, traits: traits, ability: ability, shadow_effect_icon: "", card_type: "LOCATION", set_information: set_information, scenario_title: null)
    {
        progress_tokens_added = 0;
    }

    public void add_progress_token(int? i = 1)
    {
        progress_tokens_added += 1;
    }
    public int? get_progress()
    {
        return this.progress_tokens_added;
    }
    
    public static LocationCard GREAT_FOREST_WEB()
    {
        LocationCard the_card = new LocationCard("Great Forest Web",
            2, 2, "Travel: Each player must exhaust 1 hero he controls to travel here."
            ,"SPIDER", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST}, "");
        return the_card;
    }
    
    public static LocationCard MOUNTAINS_OF_MIRKWOOD()
    {
        LocationCard the_card = new LocationCard("Mountains of Mirkwood",
            2, 3, "Travel: Reveal the top card of the encounter deck and add it to the staging area to travel here." +
                  "Response: After Mountains of Mirkwood leaves play as an explored location, each player may search the top 5 cards of his deck for 1 card and add it to his hand. " +
                  "Shuffle the rest of the searched cards back into their owners' decks."
            ,"SPIDER", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST, LOTRGame.TRAITS.MOUNTAIN},  "??");
        return the_card;
    }
    
    
}