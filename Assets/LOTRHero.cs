
using System.Collections.Generic;

public class LOTRHero : PlayerCard
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
    

    public LOTRHero(string hero_name, LOTRGame.SPHERE_OF_INFLUENCE sphere, List<LOTRGame.TRAITS> traits, int threat, 
        int willpower, int attack, int defense, int hp, string ability,string set, string type = "HERO",  bool unique = true) : base(card_name: hero_name, sphere: sphere, traits: traits,
        cost: -1, willpower: willpower, attack: attack, defense: defense, hp: hp, ability: ability, set: set, type: type, unique: true)
    {
        this.name = hero_name;
        sphere_of_influence = sphere;
        this.traits = traits;
        this.threat_cost = threat;
        this.willpower_strength = willpower;
        this.attack_strength = attack;
        this.defense_strength = defense;
        this.hit_points = hp;
        this.ability = ability;
        this.set = set;
        resource_tokens = 0;
        
    }
    
    public void take_damage(int damage)
    {
        
    }

    public int get_threat_level()
    {
        return threat_cost;
    }
    
    
    public int get_willpower()
    {
        return this.willpower_strength;
    }

    public string get_name()
    {
        return name;
    }

    public void add_resource_token(int amount = 1)
    {
        resource_tokens += amount;
    }
    

    public int get_resources()
    {
        return resource_tokens;
    }

    public void set_resources(int amt)
    {
        this.resource_tokens = amt;
    }
    
    public void pay_resource()
    {
        this.resource_tokens -= 1;
    }
    
    public LOTRGame.SPHERE_OF_INFLUENCE  get_resource_type()
    {
        return this.sphere_of_influence;
    }


    public static LOTRHero ARAGORN()
    {
        LOTRHero the_hero = new LOTRHero("Aragorn", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DUNEDAIN, LOTRGame.TRAITS.NOBLE, LOTRGame.TRAITS.RANGER},
            threat: 12, willpower: 200, attack: 3, defense: 2, hp: 5,
            ability:
            "sentinel. response after he commits to a quest, spend 1 resource from his resource poool to ready him",
            set: "??");
        the_hero.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.COMMITTED),
            PlayerCardResponses.aragorn);
        return the_hero;
    }
    
    public static LOTRHero GLOIN()
    {
        LOTRHero the_hero = new LOTRHero("Gloin", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DWARF, LOTRGame.TRAITS.NOBLE},
            threat: 9, willpower: 2, attack: 2, defense: 1, hp: 4,
            ability:
            "Response: After Glóin suffers damage, add 1 resource to his resource pool for each point of damage he just suffered.",
            set: "??");
        the_hero.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.TAKE_DAMAGE),
            PlayerCardResponses.gloin);
        return the_hero;
    }
    
    public static LOTRHero THEODRED()
    {
        LOTRHero the_hero = new LOTRHero("Theodred", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.NOBLE, LOTRGame.TRAITS.ROHAN, LOTRGame.TRAITS.WARRIOR},
            threat: 8, willpower: 1, attack: 2, defense: 1, hp: 4,
            ability:
            "Response: After Théodred commits to a quest, choose a hero committed to that quest. Add 1 resource to that hero's resource pool.",
            set: "??");
        the_hero.respond_on_event(GameEvent.get_instance(GameEvent.GAME_EVENT_TYPE.COMMITTED),
            PlayerCardResponses.theodred);
        return the_hero;
    }
    
    

    public static LOTRHero EOWYN = new LOTRHero("Eowyn", LOTRGame.SPHERE_OF_INFLUENCE.SPIRIT,
        new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.NOBLE, LOTRGame.TRAITS.ROHAN},
        threat: 9, willpower: 4, attack: 1, defense: 1, hp: 3,
        ability:
        "Action: discard 1 card from your hand to give eowyn +1 until the end of the phase. this effect may be triggered by each player once each round",
        set: "??");
    
    public static LOTRHero ELEANOR = new LOTRHero("Eleanor", LOTRGame.SPHERE_OF_INFLUENCE.SPIRIT,
        new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GONDOR, LOTRGame.TRAITS.NOBLE},
        threat: 7, willpower: 1, attack: 1, defense: 2, hp: 3,
        ability:
        "Response: exhaust eleanor to cancel the when reveled effects of a treachery card just revelaed by the enocunter deck. then discard that card, and replace it with t he enxt card from the encounter deck.",
        set: "??");
}
