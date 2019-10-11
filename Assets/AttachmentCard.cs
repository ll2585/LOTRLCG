using System;
using System.Collections.Generic;

public class AttachmentCard : PlayerCard
{
    public AttachmentCard(string card_name, int cost, LOTRGame.SPHERE_OF_INFLUENCE sphere, List<LOTRGame.TRAITS> traits,
        string ability, string set, string type = "ATTACHMENT", bool unique = false) : base(card_name: card_name,
        sphere: sphere, traits: traits,
        cost: cost, willpower: -1, attack: -1, defense: -1, hp: -1, ability: ability, set: set, type: type,
        unique: unique)
    {
    }

    public static AttachmentCard STEWARD_OF_GONDOR()
    {
        var result = new AttachmentCard(card_name: "Steward of Gondor", cost: 2, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            traits: new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GONDOR, LOTRGame.TRAITS.TITLE}, 
            ability: "Attach to a hero. Attached hero gains the Gondor trait. Action: Exhaust Steward of Gondor to add 2 resources to attached hero's resource pool.",
            set: "???");
        List<Func<EventArgs, Card, bool>> its_me_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(its_me_criteria, PlayerCardResponses.steward_of_gondor_enters,result));
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() { CardEnablers.i_am_played, CardEnablers.i_am_not_exhausted};
        result.set_action_criteria(all_action_criteria);
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.steward_of_gondor_action,
                result));
        return result;
    }
    
    public static AttachmentCard CELEBRIANS_STONE()
    {
        var result = new AttachmentCard(card_name: "Celebrian's Stone", cost: 2, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            traits: new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.ARTIFACT, LOTRGame.TRAITS.ITEM}, 
            ability: "Restricted.  Attach to a hero. Attached hero gains +2 Willpower. If attached hero is Aragorn, he also gains a Spirit resource icon.",
            set: "???", unique:true);
        List<Func<EventArgs, Card, bool>> its_me_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(its_me_criteria, PlayerCardResponses.celebrians_stone_enters,result));
        return result;
    }
}