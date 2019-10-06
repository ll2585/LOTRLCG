
using System.Collections.Generic;

public class EnemyCard : Card
{
    private string name;
    private int engagement_cost;
    private int threat_strength;
    private int quest_points;
    private string encounter_set;
    private string shadow_effect_icon;
    private int attack_strength;
    private int defense_strength;
    private int hit_points;
    private List<LOTRGame.TRAITS> traits;
    private string ability;
    private string type;
    private string set;
    private string scenario_title;
    private bool attack_resolved;
    private int damage_tokens_taken;
    private bool dead;
    


    public EnemyCard(string card_name, int engagement_cost, int threat_strength, int attack, int defense, int quest_points, int hp, string encounter_set,  
        List<LOTRGame.TRAITS> traits,
        string ability, string shadow_effect_icon, string card_type,  string set_information, string scenario_title): base(name:card_name)
    {
        this.name = card_name;
        dead = false;
        this.engagement_cost = engagement_cost;
        this.threat_strength = threat_strength;
        this.traits = traits;
        this.quest_points = quest_points;
        this.encounter_set = encounter_set;
        this.shadow_effect_icon = shadow_effect_icon;
        this.scenario_title = scenario_title;
        this.type = card_type;
        this.attack_strength = attack;
        this.defense_strength = defense;
        this.hit_points = hp;
        this.ability = ability;
        this.set = set_information;
        attack_resolved = false;
        
    }
    
   

    public string get_type()
    {
        return this.type;
    }

    public static List<EnemyCard> DEBUG_DECK()
    {
        return SPIDER_CARDS();
    }

    public void resolve_attack()
    {
        attack_resolved = true;
    }

    public bool is_treachery_card()
    {
        return this.type == "TREACHERY";
    }

    public bool is_location_card()
    {
        return this.type == "LOCATION";
    }

    public bool is_attack_resolved()
    {
        return attack_resolved;
    }

    public void reset_for_combat_phase()
    {
        attack_resolved = false;
    }

    public void add_shadow_card(EnemyCard shadow_card)
    {
        //?!?!
        if (shadow_card != null)
        {
            
        }
    }

    public int get_attack()
    {
        return this.attack_strength;
    }

    
    
    public int get_defense()
    {
        return this.defense_strength;
    }

    public void take_damage(int damage)
    {
        damage_tokens_taken += damage;

        if (has_no_hp())
        {
            die();
        }
    }

    void die()
    {
        this.dead = true;
    }

    public bool is_dead()
    {
        return dead;
    }

    bool has_no_hp()
    {
        int hp = hit_points;
        int damage_taken = damage_tokens_taken;
        return damage_taken >= hp;
    }

    public bool has_trait(LOTRGame.TRAITS t)
    {
        foreach (LOTRGame.TRAITS trait in traits)
        {
            if (trait == t)
            {
                return true;
            }
        }

        return false;
    }
    
    

    public int get_quest_points()
    {
        return quest_points;
    }

    public bool is_enemy_card()
    {
        return this.type == "ENEMY";
    }
    

    public int get_engagement_cost()
    {
        return this.engagement_cost;
    }

    public int get_threat_strength()
    {
        return this.threat_strength;
    }
    
    public static EnemyCard EAST_BIGHT_PATROL = new EnemyCard("East Bight Patrol",5, 3, 3, 1, -1, 2, "tree", 
                                                    new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GOBLIN, LOTRGame.TRAITS.ORC}, "","Attacking enemy gets +1", "ENEMY", "??", "??");
    public static EnemyCard KING_SPIDER()
    {
        EnemyCard the_card = new EnemyCard("King Spider",
            20, 2, 3, 1, -1, 3, "spider", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.SPIDER}, "When Revealed: Each player must choose and exhaust 1 character he controls."
            ,"Shadow: Defending player must choose and exhaust 1 character he controls. (2 characters instead if this attack is undefended.)", "ENEMY", "??", "??");
        return the_card;
    }
    
    public static EnemyCard HUMMERHORNS()
    {
        EnemyCard the_card = new EnemyCard("Hummerhorns",
            40, 1, 2, 0, -1, 3, "spider", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.INSECT},
            "Forced: After Hummerhorns engages you, deal 5 damage to a single hero you control."
            ,"Shadow: Deal 1 damage to each character the defending player controls. (2 damage instead if this attack is undefended.)", "ENEMY", "??", "??");
        return the_card;
    }
    
    public static EnemyCard UNGOLIANTS_SPAWN()
    {
        EnemyCard the_card = new EnemyCard("Ungoliant's Spawn",
            32, 3, 5, 2, -1, 9, "spider", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.SPIDER},
            "When Revealed: Each character currently committed to a quest gets -1 Willpower until the end of the phase."
            ,"Shadow: Raise defending player's threat by 4. (Raise defending player's threat by 8 instead if this attack is undefended.)", "ENEMY", "??", "??");
        return the_card;
    }

    public static List<EnemyCard> SPIDER_CARDS()
    {
        List<EnemyCard> result = new List<EnemyCard>();
        result.Add(KING_SPIDER());
        result.Add(KING_SPIDER());
        result.Add(HUMMERHORNS());
        result.Add(UNGOLIANTS_SPAWN());
        result.Add(LocationCard.GREAT_FOREST_WEB());
        result.Add(LocationCard.GREAT_FOREST_WEB());
        result.Add(LocationCard.MOUNTAINS_OF_MIRKWOOD());
        result.Add(LocationCard.MOUNTAINS_OF_MIRKWOOD());
        result.Add(LocationCard.MOUNTAINS_OF_MIRKWOOD());
        result.Add(TreacheryCard.EYES_OF_THE_FOREST());
        result.Add(TreacheryCard.CAUGHT_IN_A_WEB());
        return result;
    }
    
    
}
