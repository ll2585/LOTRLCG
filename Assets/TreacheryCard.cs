using System;
using System.Collections.Generic;

public class TreacheryCard : EnemyCard
{
    public TreacheryCard(string card_name, string ability, string shadow_effect, string encounter_set_icon, string set_information) : base(card_name, engagement_cost: -1, threat_strength: 0, attack: -1,
        defense: -1, quest_points: -1, hp: -1,encounter_set: encounter_set_icon, traits: null, ability: ability, shadow_effect_icon: "", card_type: "TREACHERY", set_information: set_information, scenario_title: null)
    {

    }
    
    public static TreacheryCard EYES_OF_THE_FOREST()
    {
        TreacheryCard result = new TreacheryCard("Eyes of the Forest",
            "When Revealed: Each player discards all event cards in his hand.","", "SPIDER"
            ,"");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, 
                EnemyCardResponses.eyes_of_the_forest,
                result));
        return result;
    }
    
    public static TreacheryCard CAUGHT_IN_A_WEB()
    {
        TreacheryCard the_card = new TreacheryCard("Caught in a Web",
            "When Revealed: The player with the highest threat level attaches this card to one of his heroes. " +
            "(Counts as a Condition attachment with the text: 'Attached hero does not ready during the refresh phase unless you pay 2 resources from that hero's pool.')", "",
            "SPIDER"
            ,"");
        return the_card;
    }
    
    public static TreacheryCard DRIVEN_BY_SHADOW()
    {
        TreacheryCard result = new TreacheryCard("Driven by Shadow",
            "When Revealed: Each enemy and each location currently in the staging area gets 1 Threat until the end of the phase. " +
            "If there are no cards in the staging area, Driven by Shadow gains surge.", "Shadow: Choose and discard 1 attachment from the defending character. (If this attack is undefended, discard all attachments you control.)",
            "ORC"
            ,"");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, 
                EnemyCardResponses.driven_by_shadow,
                result));

        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, EnemyCardResponses.driven_by_shadow_shadow, result));
        return result;
        
    }
    
    public static TreacheryCard THE_NECROMANCERS_REACH()
    {
        TreacheryCard result = new TreacheryCard("The Necromancer's Reach",
            "When Revealed: Deal 1 damage to each exhausted character.", "",
            "ORC"
            ,"");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, 
                EnemyCardResponses.necromancers_reach,
                result));
        return result;
    }
}
