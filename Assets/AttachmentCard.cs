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

        return result;
    }
    
    public static AttachmentCard CELEBRIANS_STONE()
    {
        var result = new AttachmentCard(card_name: "Celebrian's Stone", cost: 2, sphere: LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP, 
            traits: new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GONDOR, LOTRGame.TRAITS.TITLE}, 
            ability: "Attach to a hero. Attached hero gains the Gondor trait. Action: Exhaust Steward of Gondor to add 2 resources to attached hero's resource pool.",
            set: "???");

        return result;
    }
}