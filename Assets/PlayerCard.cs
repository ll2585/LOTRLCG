
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard : Card
{
    private string name;
    private int resource_tokens;
    private bool is_unique;
    private int threat_cost;
    private LOTRGame.SPHERE_OF_INFLUENCE sphere_of_influence;
    private int willpower_strength;
    private int attack_strength;
    private int defense_strength;
    private int hit_points;
    private List<LOTRGame.TRAITS> traits;
    private string ability;
    private string type;
    private string set;
    private int cost;
    private bool exhausted;
    private bool dead;
    private bool committed;
    private LOTRDispatcher card_observer;
    private string uuid;
    private List<AttachmentCard> my_attachments;

    
    public PlayerCard(string card_name, LOTRGame.SPHERE_OF_INFLUENCE sphere, List<LOTRGame.TRAITS> traits,  int cost,
        int willpower, int attack, int defense, int hp, string ability,string set, string type,  bool unique) : base(name: card_name)
    {
        this.name = card_name;
        this.cost = cost;
        sphere_of_influence = sphere;
        this.traits = traits;
        this.willpower_strength = willpower;
        this.attack_strength = attack;
        this.defense_strength = defense;
        this.hit_points = hp;
        this.ability = ability;
        this.set = set;
        this.type = type;
        resource_tokens = 0;
        this.exhausted = false;
        this.dead = false;
        this.committed = false;
        this.card_observer = null;
        this.uuid = Guid.NewGuid().ToString();
        my_attachments = new List<AttachmentCard>();
    }
    
    
    public void commit()
    {
        this.committed = true;
        exhaust();
    }
    
    public void ready()
    {
        this.committed = false;
        this.exhausted = false;
    }
    
    public void unexhaust()
    {
        this.exhausted = false;
    }
    public bool is_committed()
    {
        return committed;
    }

    public void exhaust()
    {
        this.exhausted = true;
    }

    public string get_name()
    {
        return name;
    }

    public void add_resource_token()
    {
        resource_tokens += 1;
    }

    public bool is_attachment()
    {
        return this.type == "ATTACHMENT";
    }

    public void add_attachment(AttachmentCard attachment)
    {
        my_attachments.Add(attachment);
    }

    public List<AttachmentCard> get_attachments()
    {
        return my_attachments;
    }

    public int get_cost()
    {
        return this.cost;
    }
    
    public LOTRGame.SPHERE_OF_INFLUENCE  get_resource_type()
    {
        return this.sphere_of_influence;
    }

    public int get_resources()
    {
        return resource_tokens;
    }

    public void set_resources(int amt)
    {
        this.resource_tokens = amt;
    }

    public int get_defense()
    {
        return defense_strength;
    }

    public void take_damage(int damage)
    {
        
    }
    

    public bool is_ally()
    {
        return this.type == "ALLY";
    }

    public string get_type()
    {
        return this.type;
    }


    public bool is_dead()
    {
        return this.dead;
    }
    public void declared_as_defender()
    {
        this.exhausted = true;
        Debug.Log("I AM DEFENDING" + is_exhausted());
        
    }
    
    public bool is_exhausted()
    {
        return exhausted;
    }


    public void declared_as_attacker()
    {
        this.exhausted = true;
    }

    public int get_attack_strength()
    {
        return attack_strength;
    }

    public bool responds_to_event(GameEvent e)
    {
        if (this.card_observer is null)
        {
            return false;
        }

        return this.card_observer.listening_to_event(e);
    }

    public void respond_on_event(GameEvent e, Action<GameArgs> function)
    {
        if (this.card_observer is null)
        {
            this.card_observer = new LOTRDispatcher();
        }
        this.card_observer.on(e, function);
    }

    public bool has_card_observer()
    {
        return this.card_observer != null;
    }

    public LOTRDispatcher get_card_observer()
    {
        return this.card_observer;
    }

    public string get_uuid()
    {
        return this.uuid;
    }




    public static PlayerCard GUARD_OF_THE_CITADEL()
    {
        PlayerCard the_card = new PlayerCard("Guard of the Citadel", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GONDOR, LOTRGame.TRAITS.WARRIOR},
            cost: 2, willpower: 1, attack: 1, defense: 0, hp: 2,
            ability:
            "",
            set: "??", type: "ALLY", unique: false);
        return the_card;
    }
    
    public static PlayerCard FARAMIR()
    {
        PlayerCard the_card = new PlayerCard("Faramir", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GONDOR, LOTRGame.TRAITS.NOBLE, LOTRGame.TRAITS.RANGER},
            cost: 4, willpower: 2, attack: 1, defense: 2, hp: 3,
            ability:
            "Action: Exhaust Faramir to choose a player. Each character controlled by that player gets +1 Willpower until the end of the phase.",
            set: "??", type: "ALLY", unique: true);
        return the_card;
    }
    
    public static PlayerCard SON_OF_ARNOR()
    {
        PlayerCard the_card = new PlayerCard("Son of Arnor", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DUNEDAIN},
            cost: 3, willpower: 0, attack: 2, defense: 0, hp: 2,
            ability:
            "Response: After Son of Arnor enters play, choose an enemy card in the staging area or currently engaged with another player. Engage that enemy.",
            set: "??", type: "ALLY", unique: true);
        the_card.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.CARD_ENTERS_PLAY),
            PlayerCardResponses.son_of_arnor);
        return the_card;
    }
    
    public static PlayerCard SNOWBOURN_SCOUT()
    {
        PlayerCard the_card = new PlayerCard("Snowbourn Scout", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.ROHAN, LOTRGame.TRAITS.SCOUT},
            cost: 1, willpower: 0, attack: 0, defense: 1, hp: 1,
            ability:
            "Response: After Snowbourn Scout enters play, choose a location. Place 1 progress token on that location.",
            set: "??", type: "ALLY", unique: false);
        the_card.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.CARD_ENTERS_PLAY),
            PlayerCardResponses.snowbourn_scout);
        return the_card;
    }
    
    public static PlayerCard SILVERLODE_ARCHER()
    {
        PlayerCard the_card = new PlayerCard("Silverlode Archer", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.ARCHER, LOTRGame.TRAITS.SILVAN},
            cost: 3, willpower: 1, attack: 2, defense: 0, hp: 1,
            ability:
            "Ranged",
            set: "??", type: "ALLY", unique: false);
        return the_card;
    }
    
    public static PlayerCard LONGBEARD_ORC_SLAYER()
    {
        PlayerCard the_card = new PlayerCard("Longbeard Orc Slayer", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DWARF, LOTRGame.TRAITS.WARRIOR},
            cost: 4, willpower: 0, attack: 2, defense: 1, hp: 3,
            ability:
            "Response: After Longbeard Orc Slayer enters play, deal 1 damage to each Orc enemy in play.",
            set: "??", type: "ALLY", unique: false);
        the_card.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.CARD_ENTERS_PLAY),
            PlayerCardResponses.longbeard_orc_slayer);
        return the_card;
    }
    
    public static PlayerCard BROK_IRONFIST()
    {
        PlayerCard the_card = new PlayerCard("Brok Ironfist", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DWARF, LOTRGame.TRAITS.WARRIOR},
            cost: 4, willpower: 0, attack: 2, defense: 1, hp: 3,
            ability:
            "Response: After a Dwarf hero you control leaves play, put Brok Ironfist into play from your hand.",
            set: "??", type: "ALLY", unique: true);
        the_card.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.CARD_LEAVES_PLAY),
            PlayerCardResponses.brok_ironfist);
        return the_card;
    }

    public static PlayerCard NORTHERN_TRACKER()
    {
        return new PlayerCard("Northern Tracker", LOTRGame.SPHERE_OF_INFLUENCE.SPIRIT,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DUNEDAIN, LOTRGame.TRAITS.RANGER},
            cost: 4, willpower: 1, attack: 2, defense: 2, hp: 3,
            ability:
            "Response: after norther tracker commits to a quest, place 1 progress token on each location in the staging area",
            set: "??", type: "ALLY", unique: false);
    }


    public static List<PlayerCard> LEADERSHIP_CARDS()
    {
        var result = new List<PlayerCard>();
        result = add_cards_to_list(result, SON_OF_ARNOR, 2);
        result = add_cards_to_list(result, SNOWBOURN_SCOUT, 3);
        result = add_cards_to_list(result, AttachmentCard.CELEBRIANS_STONE, 1);
        result = add_cards_to_list(result, GUARD_OF_THE_CITADEL, 3);
        result = add_cards_to_list(result, FARAMIR, 2);
        result = add_cards_to_list(result, SON_OF_ARNOR, 2);
        result = add_cards_to_list(result, SNOWBOURN_SCOUT, 3);
        result = add_cards_to_list(result, SILVERLODE_ARCHER, 2);
        result = add_cards_to_list(result, LONGBEARD_ORC_SLAYER, 2);
        result = add_cards_to_list(result, BROK_IRONFIST, 1);
        result = add_cards_to_list(result, EventCard.EVER_VIGILANT, 2);
        result = add_cards_to_list(result, EventCard.COMMON_CAUSE, 2);
        result = add_cards_to_list(result, EventCard.FOR_GONDOR, 2);
        result = add_cards_to_list(result, EventCard.SNEAK_ATTACK, 2);
        result = add_cards_to_list(result, EventCard.VALIANT_SACRIFICE, 2);
        result = add_cards_to_list(result, EventCard.GRIM_RESOLVE, 1);
        result = add_cards_to_list(result, AttachmentCard.STEWARD_OF_GONDOR, 2);
        result = add_cards_to_list(result, AttachmentCard.CELEBRIANS_STONE, 1);
        return result;
    }

    static List<PlayerCard> add_cards_to_list(List<PlayerCard> cards, Func<PlayerCard> card, int number)
    {
        List<PlayerCard> card_copy = new List<PlayerCard>(cards);
        for (var i = 0; i < number; i++)
        {
            card_copy.Add(card());    
        }

        return card_copy;
    }
}
