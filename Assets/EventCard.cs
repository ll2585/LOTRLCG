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
        return result;
    }
    
    public static EventCard COMMON_CAUSE() {
        var result = new EventCard(card_name: "Common Cause", cost: 0, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Exhaust 1 hero you control to choose and ready a different hero.", set: "??");
        return result;
    }
    
    public static EventCard FOR_GONDOR() {
        var result = new EventCard(card_name: "For Gondor!", cost: 2, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Until the end of the phase, all characters get +1 Attack. All Gondor characters also get +1 Defense until the end of the phase.", set: "??");
        return result;
    }
    
    public static EventCard SNEAK_ATTACK() {
        var result = new EventCard(card_name: "Sneak Attack", cost: 1, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Put 1 ally card into play from your hand. At the end of the phase, if that ally is still in play, return it to your hand.", set: "??");
        return result;
    }
    
    public static EventCard VALIANT_SACRIFICE() {
        var result = new EventCard(card_name: "Valiant Sacrifice", cost: 1, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Response: After an ally card leaves play, that card's controller draws 2 cards.", set: "??");
        return result;
    }
    
    public static EventCard GRIM_RESOLVE() {
        var result = new EventCard(card_name: "Grim Resolve", cost: 5, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            ability: "Action: Ready all character cards in play.", set: "??");
        return result;
    }
}


