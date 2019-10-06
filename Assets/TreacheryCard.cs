public class TreacheryCard : EnemyCard
{
    public TreacheryCard(string card_name, string ability, string shadow_effect, string encounter_set_icon, string set_information) : base(card_name, engagement_cost: -1, threat_strength: 0, attack: -1,
        defense: -1, quest_points: -1, hp: -1,encounter_set: encounter_set_icon, traits: null, ability: ability, shadow_effect_icon: "", card_type: "TREACHERY", set_information: set_information, scenario_title: null)
    {

    }
    
    public static TreacheryCard EYES_OF_THE_FOREST()
    {
        TreacheryCard the_card = new TreacheryCard("Eyes of the Forest",
            "When Revealed: Each player discards all event cards in his hand.","", "SPIDER,"
            ,"");
        return the_card;
    }
    
    public static TreacheryCard CAUGHT_IN_A_WEB()
    {
        TreacheryCard the_card = new TreacheryCard("Caught in a Web",
            "When Revealed: The player with the highest threat level attaches this card to one of his heroes. " +
            "(Counts as a Condition attachment with the text: 'Attached hero does not ready during the refresh phase unless you pay 2 resources from that hero's pool.')", "",
            "SPIDER,"
            ,"");
        return the_card;
    }
}
