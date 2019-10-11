
using System;
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
    private List<EnemyCard> shadow_cards;
    private int resource_tokens; //used for chieftan ufthak
    private bool shadow_effect_resolved;
    


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
        shadow_cards = new List<EnemyCard>();
        resource_tokens = 0;
        shadow_effect_resolved = false;
    }

    public bool has_shadow_cards()
    {
        return this.shadow_cards.Count > 0;
    }

    public bool all_shadow_cards_resolved()
    {
        foreach (EnemyCard shadow_card in shadow_cards)
        {
            if (!shadow_card.is_shadow_effect_resolved())
            {
                return false;
            }
        }
        return true;
    }

    public void set_shadow_effect_resolved()
    {
        this.shadow_effect_resolved = true;
    }
    
    public bool is_shadow_effect_resolved()
    {
        return(shadow_effect_resolved);
    }
    

    public string get_type()
    {
        return this.type;
    }

    public static List<EnemyCard> DEBUG_DECK()
    {
        return TREE_CARDS();
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

    public void discard_shadow_cards()
    {
        this.shadow_cards = new List<EnemyCard>();
        reset_flags();
    }

    public void add_shadow_card(EnemyCard shadow_card)
    {
        //?!?!
        if (shadow_card != null)
        {
            this.shadow_cards.Add(shadow_card);
        }
    }

    public List<EnemyCard> get_shadow_cards()
    {
        return shadow_cards;
    }

    public int get_resource_tokens()
    {
        return this.resource_tokens;
    }

    public void add_resource_tokens(int amt = 1)
    {
        this.resource_tokens += amt;
    }

    public int get_attack()
    {
        int result = attack_strength;
        if (has_flag(LOTRAbility.ABILITY_FLAGS.CHIEFTAN_UFTHAK))
        {
            result += get_resource_tokens();
        }
        if (has_flag(LOTRAbility.ABILITY_FLAGS.FOREST_SPIDER))
        {
            result += 1;
        }

        foreach (LOTRAbility.ABILITY_FLAGS flag in my_flags)
        {
            if (flag == LOTRAbility.ABILITY_FLAGS.PLUS_ONE_ATTACK_ENEMY)
            {
                result += 1;
            }

            if (flag == LOTRAbility.ABILITY_FLAGS.PLUS_THREE_ATTACK_ENEMY)
            {
                result += 3;
            }
        }

        return result;
    }

    public int get_hp()
    {
        return this.hit_points;
    }
    
    
    public int get_defense()
    {
        return this.defense_strength;
    }

    public void clear_flags(LOTRAbility.CLEAR_MARKERS marker)
    {
        if (marker == LOTRAbility.CLEAR_MARKERS.END_OF_ROUND)
        {
            remove_flag(LOTRAbility.ABILITY_FLAGS.FOREST_SPIDER);
        }
    }

    public int get_damage_taken()
    {
        return damage_tokens_taken;
    }
    public void take_damage(int damage)
    {
        if (damage > 0)
        {
            damage_tokens_taken += damage;
        }
       

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

    public string get_ability()
    {
        return ability;
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
        int result = threat_strength;
        if (has_flag(LOTRAbility.ABILITY_FLAGS.PLUS_ONE_THREAT))
        {
            result += 1;
        }
        return result;
    }
    
    public static EnemyCard KING_SPIDER()
    {
        EnemyCard result = new EnemyCard("King Spider",
            20, 2, 3, 1, -1, 3, "SPIDER", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.SPIDER}, "When Revealed: Each player must choose and exhaust 1 character he controls."
            ,"Shadow: Defending player must choose and exhaust 1 character he controls. (2 characters instead if this attack is undefended.)", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_at_least_one_ready_character};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, 
                EnemyCardResponses.king_spider,
                result, valid_targets:CardEnablers.valid_targets_player_characters_ready));
        
        List<Func<EventArgs, Card, bool>> shadow_card_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.king_spider_shadow, result));
        return result;
    }
    
    public static EnemyCard HUMMERHORNS()
    {
        EnemyCard result = new EnemyCard("Hummerhorns",
            40, 1, 2, 0, -1, 3, "SPIDER", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.INSECT},
            "Forced: After Hummerhorns engages you, deal 5 damage to a single hero you control."
            ,"Shadow: Deal 1 damage to each character the defending player controls. (2 damage instead if this attack is undefended.)", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> shadow_card_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.hummerhorns_shadow, result));
        return result;
    }
    
    public static EnemyCard UNGOLIANTS_SPAWN()
    {
        EnemyCard result = new EnemyCard("Ungoliant's Spawn",
            32, 3, 5, 2, -1, 9, "SPIDER", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.SPIDER},
            "When Revealed: Each character currently committed to a quest gets -1 Willpower until the end of the phase."
            ,"Shadow: Raise defending player's threat by 4. (Raise defending player's threat by 8 instead if this attack is undefended.)", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_at_least_one_character_committed};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, EnemyCardResponses.ungoliants_spawn, result));
        
        List<Func<EventArgs, Card, bool>> shadow_card_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.ungoliants_spawn_shadow, result));
        return result;
    }
    
    public static EnemyCard DOL_GULDUR_ORCS()
    {
        EnemyCard result = new EnemyCard("Dol Guldur Orcs",
            10, 2, 2, 0, -1, 3, "ORC", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DOL_GULDUR, LOTRGame.TRAITS.ORC}, "When Revealed: The first player chooses 1 character currently committed to a quest. Deal 2 damage to that character."
            ,"Shadow: attacking enemy gets 1 Attack. (3 Attack instead if this attack is undefended.)", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_at_least_one_character_committed};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, 
                EnemyCardResponses.dol_guldur_orcs,
                result, valid_targets:CardEnablers.valid_targets_player_characters_committed));
        
        List<Func<EventArgs, Card, bool>> shadow_card_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.dol_guldur_orcs_shadow, result));
        return result;
    }
    
    public static EnemyCard CHIEFTAN_UFTHAK()
    {
        EnemyCard result = new EnemyCard("Chieftan Ufthak",
            35, 2, 3, 3, -1, 6, "ORC", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DOL_GULDUR, LOTRGame.TRAITS.ORC}, "Chieftain Ufthak get 2 Attack for each resource token on him. Forced: After Chieftain Ufthak attacks, place 1 resource token on him."
            ,"", "ENEMY", "??", "??");
        result.add_flag(LOTRAbility.ABILITY_FLAGS.CHIEFTAN_UFTHAK);
        List<Func<EventArgs, Card, bool>> attack_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.ENEMY_ATTACK_RESOLVED,
            PlayerCardResponses.action_maker(attack_criteria, EnemyCardResponses.chieftan_ufthak_forced, result));
        return result;
    }
    
    public static EnemyCard DOL_GULDUR_BEASTMASTER()
    {
        EnemyCard result = new EnemyCard("Dol Guldur Beastmaster",
            35, 2, 3, 1, -1, 5, "ORC", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.DOL_GULDUR, LOTRGame.TRAITS.ORC}, "Forced: When Dol Guldur Beastmaster attacks, deal it 1 additional shadow card."
            ,"", "ENEMY", "??", "??");
        return result;
    }
    
    public static EnemyCard FOREST_SPIDER()
    {
        EnemyCard result = new EnemyCard("Forest Spider",
            25, 2, 2, 1, -1, 4, "TREE", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE, LOTRGame.TRAITS.SPIDER}, "Forced: After Forest Spider engages a player, it gets 1 Attack until the end of the round."
            ,"Shadow: Defending player must choose and discard 1 attachment he controls.", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> shadow_card_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.forest_spider_shadow, result));
        result.respond_to_event(GameEvent.ENEMY_ENGAGED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.forest_spider_forced, result));
        return result;
    }

    public static EnemyCard EAST_BIGHT_PATROL()
    {
        EnemyCard result = new EnemyCard("East Bight Patrol",5, 3, 3, 1, -1, 2, "TREE", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GOBLIN, LOTRGame.TRAITS.ORC}, "","Shadow: attacking enemy gets 1 Attack. (If this attack is undefended, also raise your threat by 3.)", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> shadow_card_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me};
        result.respond_to_event(GameEvent.SHADOW_CARD_REVEALED,
            PlayerCardResponses.action_maker(shadow_card_criteria, EnemyCardResponses.east_bight_patrol_shadow, result));
        return result;
    } 

    public static EnemyCard BLACK_FOREST_BATS()
    {
        EnemyCard result = new EnemyCard("Black Forest Bats",
            15, 1, 1, 0, -1, 2, "TREE", 
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.CREATURE}, "When Revealed: Each player must choose 1 character currently committed to a quest, and remove that character from the quest. (The chosen character does not ready.)"
            ,"", "ENEMY", "??", "??");
        List<Func<EventArgs, Card, bool>> card_played_criteria = new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.has_at_least_one_character_committed};
        result.respond_to_event(GameEvent.CARD_REVEALED,
            PlayerCardResponses.action_maker(card_played_criteria, 
                EnemyCardResponses.black_forest_bats,
                result, valid_targets:CardEnablers.valid_targets_player_characters_committed));
        return result;
    }

    public static List<EnemyCard> PASSAGE_THROUGH_MIRWOOD_ENEMIES()
    {
        List<EnemyCard> result = new List<EnemyCard>();
        result.Add(TreacheryCard.THE_NECROMANCERS_REACH());
        result.AddRange(SPIDER_CARDS());
        result.AddRange(ORC_CARDS());
        result.AddRange(TREE_CARDS());
        return result;
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
        result.Add(TreacheryCard.CAUGHT_IN_A_WEB());
        return result;
    }
    
    public static List<EnemyCard> ORC_CARDS()
    {
        List<EnemyCard> result = new List<EnemyCard>();
        result.Add(DOL_GULDUR_ORCS());
        result.Add(DOL_GULDUR_ORCS());
        result.Add(DOL_GULDUR_ORCS());
        result.Add(CHIEFTAN_UFTHAK());
        result.Add(DOL_GULDUR_BEASTMASTER());
        result.Add(DOL_GULDUR_BEASTMASTER());
        result.Add(TreacheryCard.DRIVEN_BY_SHADOW());
        result.Add(TreacheryCard.THE_NECROMANCERS_REACH());
        result.Add(TreacheryCard.THE_NECROMANCERS_REACH());
        result.Add(TreacheryCard.THE_NECROMANCERS_REACH());
        result.Add(LocationCard.NECROMANCERS_PASS());
        result.Add(LocationCard.NECROMANCERS_PASS());
        result.Add(LocationCard.ENCHANTED_STREAM());
        result.Add(LocationCard.ENCHANTED_STREAM());
        return result;
    }
    public static List<EnemyCard> TREE_CARDS()
    {
        List<EnemyCard> result = new List<EnemyCard>();
        result.Add(FOREST_SPIDER());
        result.Add(FOREST_SPIDER());
        result.Add(FOREST_SPIDER());
        result.Add(FOREST_SPIDER());
        result.Add(EAST_BIGHT_PATROL());
        result.Add(BLACK_FOREST_BATS());
        result.Add(LocationCard.OLD_FOREST_ROAD());
        result.Add(LocationCard.OLD_FOREST_ROAD());
        result.Add(LocationCard.FOREST_GATE());
        result.Add(LocationCard.FOREST_GATE());
        return result;
    }
    
    
}
