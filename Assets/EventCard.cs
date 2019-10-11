using System;
using System.Collections.Generic;

public class EventCard : PlayerCard
{
    public EventCard(string card_name, int cost, LOTRGame.SPHERE_OF_INFLUENCE sphere, 
        string ability, string set, string type = "EVENT",  bool unique = false) : base(card_name: card_name, sphere: sphere, traits: null,
        cost: cost, willpower: -1, attack: -1, defense: -1, hp: -1, ability: ability, set: set, type: type, unique: unique)
    {
        
        
    }
    
    public static EventCard EVER_VIGILANT() {
        var result = new EventCard(card_name: "Ever Vigilant", cost: 1, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Choose and ready 1 ally card.", set: "??");
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.i_am_in_hand, CardEnablers.has_exhausted_allies};
        result.set_action_criteria(all_action_criteria);
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.pay_for_card,result));
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_exhausted_allies};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(card_played_criteria, PlayerCardResponses.ever_vigilant,result, valid_targets:CardEnablers.valid_targets_player_allies_exhausted));

        return result;
    }
    
    public static EventCard COMMON_CAUSE() {
        var result = new EventCard(card_name: "Common Cause", cost: 0, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Exhaust 1 hero you control to choose and ready a different hero.", set: "??");
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.i_am_in_hand, CardEnablers.has_exhausted_heroes, CardEnablers.has_at_least_one_ready_hero};
        result.set_action_criteria(all_action_criteria);
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.pay_for_card,result));
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_exhausted_heroes, CardEnablers.has_at_least_one_ready_hero};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(card_played_criteria, PlayerCardResponses.common_cause,result, valid_targets:CardEnablers.valid_targets_player_heroes_ready));

        return result;
    }
    
    public static EventCard FOR_GONDOR() {
        var result = new EventCard(card_name: "For Gondor!", cost: 2, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Until the end of the phase, all characters get +1 Attack. All Gondor characters also get +1 Defense until the end of the phase.", set: "??");
        
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.i_am_in_hand};
        result.set_action_criteria(all_action_criteria);
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.pay_for_card,result));
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(card_played_criteria, PlayerCardResponses.for_gondor,result));
        
        return result;
    }
    
    public static EventCard SNEAK_ATTACK() {
        var result = new EventCard(card_name: "Sneak Attack", cost: 1, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Put 1 ally card into play from your hand. At the end of the phase, if that ally is still in play, return it to your hand.", set: "??");
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.i_am_in_hand, CardEnablers.has_ally_in_hand};
        result.set_action_criteria(all_action_criteria);
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.pay_for_card,result));
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_ally_in_hand};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(card_played_criteria, PlayerCardResponses.sneak_attack,result, valid_targets:CardEnablers.valid_targets_player_allies_in_hand));

        return result;
    }
    
    public static EventCard VALIANT_SACRIFICE() {
        var result = new EventCard(card_name: "Valiant Sacrifice", cost: 1, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Response: After an ally card leaves play, that card's controller draws 2 cards.", set: "??");
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> when_can_i_be_played = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.i_am_in_hand};
        result.respond_to_event(GameEvent.CARD_LEAVES_PLAY,
            PlayerCardResponses.action_maker(when_can_i_be_played, PlayerCardResponses.prompt_reply_from_hand,result, fires_on_other_cards:true));
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(when_can_i_be_played, PlayerCardResponses.pay_for_card,result));
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me };
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(card_played_criteria, PlayerCardResponses.valiant_sacrifice,result));
        return result;
    }
    
    public static EventCard GRIM_RESOLVE() {
        var result = new EventCard(card_name: "Grim Resolve", cost: 5, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Ready all character cards in play.", set: "??");
        result.set_action_card();
        List<Func<EventArgs, Card, bool>> when_can_i_be_played = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.i_am_in_hand, CardEnablers.has_exhausted_characters};
        result.set_action_criteria(when_can_i_be_played);
        result.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(when_can_i_be_played, PlayerCardResponses.pay_for_card,result));
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_exhausted_characters};
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(card_played_criteria, PlayerCardResponses.grim_resolve,result));

        return result;
    }
}


