using System;
using System.Collections.Generic;

public class LocationCard : EnemyCard
{

    private int progress_tokens_added;
    private Func<GameArgs, bool> can_travel;
    public LocationCard(string card_name, int threat_strength, int quest_points, 
        string ability, string encounter_set, List<LOTRGame.TRAITS> traits,
        string set_information) : base(card_name, engagement_cost: -1, threat_strength: threat_strength, attack: -1,
        defense: -1, quest_points: quest_points, hp: -1,encounter_set: encounter_set, traits: traits, ability: ability, shadow_effect_icon: "", card_type: "LOCATION", set_information: set_information, scenario_title: null)
    {
        progress_tokens_added = 0;
        can_travel = null;
    }

    public void add_progress_token(int? i = 1)
    {
        progress_tokens_added += 1;
    }
    public int? get_progress()
    {
        return this.progress_tokens_added;
    }

    public void set_travel_criteria( Func<GameArgs, bool> travel_criteria)
    {
        can_travel = travel_criteria;
    }

    public bool can_travel_here(GameArgs args)
    {
        if (can_travel == null)
        {
            return true;
        }
        return can_travel(args);
    }
    
    public static LocationCard GREAT_FOREST_WEB()
    {
        LocationCard the_card = new LocationCard("Great Forest Web",
            2, 2, "Travel: Each player must exhaust 1 hero he controls to travel here."
            ,"SPIDER", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST}, "");
        Func<GameArgs, bool> criteria = (GameArgs args) =>
        {
            LOTRGame game = args.g;
            foreach (var player in game.get_players())
            {
                bool has_ready_character = false;
                foreach (var card in player.get_heroes())
                {
                    if (!card.is_exhausted())
                    {
                        has_ready_character = true;
                        break;
                    }
                }

                if (!has_ready_character)
                {
                    foreach (var card in player.get_allies())
                    {
                        if (!card.is_exhausted())
                        {
                            has_ready_character = true;
                            break;
                        }
                    }
                }

                if (!has_ready_character)
                {
                    return false;
                }
            }
            return true;
        };
        the_card.set_travel_criteria(criteria);
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me };
        the_card.respond_to_event(GameEvent.LOCATION_TRAVELED,
            PlayerCardResponses.action_maker(card_played_criteria, EnemyCardResponses.great_forest_web,the_card, valid_targets:CardEnablers.valid_targets_player_characters_ready));
        return the_card;
    }
    
    public static LocationCard MOUNTAINS_OF_MIRKWOOD()
    {
        LocationCard the_card = new LocationCard("Mountains of Mirkwood",
            2, 3, "Travel: Reveal the top card of the encounter deck and add it to the staging area to travel here." +
                  "Response: After Mountains of Mirkwood leaves play as an explored location, each player may search the top 5 cards of his deck for 1 card and add it to his hand. " +
                  "Shuffle the rest of the searched cards back into their owners' decks."
            ,"SPIDER", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST, LOTRGame.TRAITS.MOUNTAIN},  "??");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me };
        the_card.respond_to_event(GameEvent.LOCATION_TRAVELED,
            PlayerCardResponses.action_maker(card_played_criteria, EnemyCardResponses.mountains_of_mirkwood,the_card));
        return the_card;
    }
    
    public static LocationCard NECROMANCERS_PASS()
    {
        LocationCard the_card = new LocationCard("Necromancer's Pass",
            3, 2, "Travel: The first player must discard 2 cards from his hand at random to travel here."
            ,"ORC", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.STRONGHOLD, LOTRGame.TRAITS.DOL_GULDUR},  "??");
        Func<GameArgs, bool> criteria = (GameArgs args) =>
        {
            LOTRPlayer first_player = args.relevant_player;
            return first_player.get_cards_in_hand().Count >= 2;
        };
        the_card.set_travel_criteria(criteria);
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me };
        the_card.respond_to_event(GameEvent.LOCATION_TRAVELED,
            PlayerCardResponses.action_maker(card_played_criteria, EnemyCardResponses.necromancers_pass,the_card));

        return the_card;
    }
    
    public static LocationCard ENCHANTED_STREAM()
    {
        LocationCard the_card = new LocationCard("Enchanted Stream",
            2, 2, "While Enchanted Stream is the active location, players cannot draw cards."
            ,"ORC", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST},  "??");
        return the_card;
    }
    
    public static LocationCard OLD_FOREST_ROAD()
    {
        LocationCard result = new LocationCard("Old Forest Road",
            1, 3, "Response: After you travel to Old Forest Road, the first player may choose and ready 1 character he controls."
            ,"TREE", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST},  "??");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_exhausted_characters};
        result.respond_to_event(GameEvent.LOCATION_TRAVELED,
            PlayerCardResponses.action_maker(card_played_criteria, EnemyCardResponses.old_forest_road,result, valid_targets:CardEnablers.valid_targets_player_characters_exhausted));
        return result;
    }
    public static LocationCard FOREST_GATE()
    {
        LocationCard the_card = new LocationCard("Forest Gate",
            2, 4, "Response: After you travel to Forest Gate, the first player may draw 2 cards."
            ,"TREE", new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.FOREST},  "??");
        the_card.respond_to_event(GameEvent.LOCATION_TRAVELED,
            PlayerCardResponses.action_maker(new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me }, EnemyCardResponses.forest_gate,  the_card));
        return the_card;
    }
}