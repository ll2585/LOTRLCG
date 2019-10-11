
using System;
using System.Collections.Generic;


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
    private List<LOTRGame.TRAITS> temporary_traits;
    private string ability;
    private string type;
    private string set;
    private int cost;
    private bool exhausted;
    private bool dead;
    private bool committed;
    private int damage_taken;
    
    private List<AttachmentCard> my_attachments;
    private bool has_action;
    private List<Func<EventArgs, Card, bool>> action_criteria;

    
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
        this.damage_taken = 0;
        my_attachments = new List<AttachmentCard>();
        temporary_traits = new List<LOTRGame.TRAITS>();
        has_action = false;
        action_criteria = null;
    }

    public int get_hp()
    {
        return this.hit_points;
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

    public void uncommit()
    {
        this.committed = false;
    }

    public LOTRGame.SPHERE_OF_INFLUENCE get_sphere()
    {
        return this.sphere_of_influence;
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
    public string get_ability()
    {
        return ability;
    }

    public string get_name()
    {
        return name;
    }
    
    public int get_willpower()
    {
        int result = this.willpower_strength;
        if (this.has_flag(LOTRAbility.ABILITY_FLAGS.FARAMIR))
        {
            result += 1;
        }
        if (this.has_flag(LOTRAbility.ABILITY_FLAGS.CELEBRIANS_STONE))
        {
            result += 2;
        }
        if (this.has_flag(LOTRAbility.ABILITY_FLAGS.UNGOLIANTS_SPAWN))
        {
            result -= 1;
        }
        return result;
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

    public bool has_attachment(AttachmentCard attachment)
    {
        return my_attachments.Contains(attachment);
    }
    
    public void discard_attachment(AttachmentCard attachment)
    {
        if (has_attachment(attachment))
        {
            for (var i = 0; i < my_attachments.Count; i++)
            {
                if (my_attachments[i] == attachment)
                {
                    //maybe do an event whatever
                    if (attachment.get_name() == AttachmentCard.STEWARD_OF_GONDOR().get_name())
                    {
                        remove_trait(LOTRGame.TRAITS.GONDOR);
                    }
                    if (attachment.get_name() == AttachmentCard.CELEBRIANS_STONE().get_name())
                    {
                        remove_flag(LOTRAbility.ABILITY_FLAGS.CELEBRIANS_STONE);
                    }
                    my_attachments.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void add_trait(LOTRGame.TRAITS trait)
    {
        temporary_traits.Add(trait);
    }
    public void remove_trait(LOTRGame.TRAITS trait)
    {
        temporary_traits.Remove(trait);
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
        int result = defense_strength;
        if (has_flag(LOTRAbility.ABILITY_FLAGS.FOR_GONDOR) && has_trait(LOTRGame.TRAITS.GONDOR))
        {
            result += 1;
        }
        return result;
    }

    public bool has_trait(LOTRGame.TRAITS trait)
    {
        return traits.Contains(trait) || temporary_traits.Contains(trait);
    } 

    public void take_damage(int damage)
    {
        if (damage > 0)
        {
            this.damage_taken += damage;
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
    bool has_no_hp()
    {
        int hp = hit_points;
        int all_damage_taken = damage_taken;
        return all_damage_taken >= hp;
    }

    public int get_damage_taken()
    {
        return this.damage_taken;
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

    public void set_action_criteria(List<Func<EventArgs, Card, bool>> all_action_criteria)
    {
        this.action_criteria = all_action_criteria;
    }
    public void declared_as_defender()
    {
        this.exhausted = true;
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
        int result = attack_strength;
        if (has_flag(LOTRAbility.ABILITY_FLAGS.FOR_GONDOR))
        {
            result += 1;
        }
        return result;
    }
    
    

    
    public void set_action_card()
    {
        has_action = true;
    }
    

    public bool action_playable(EventArgs args)
    {
        //Debug.Log(action_criteria + " "+ get_name());
        if (has_action && action_criteria != null)
        {
            GameArgs game_args = (GameArgs) args;
            Card relevant_card = this;
            LOTRGame game = game_args.g;
            bool playable = true;
            foreach (var criteria in action_criteria)
            {
                playable = playable && criteria(game_args, relevant_card);
            }
            if (playable && game.is_allowing_actions())
            {
                //Debug.Log(get_name() + " IS ACTION PLAYABLE");
                return true;
            }
            else
            {
               // Debug.Log("NO(T PLAYABLE");
            }
        }

        return false;
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
        the_card.set_action_card();
        /*the_card.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(CardEnablers.i_am_played, PlayerCardResponses.faramir, 
                the_card));*/
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() { CardEnablers.i_am_played, CardEnablers.i_am_not_exhausted};
        the_card.set_action_criteria(all_action_criteria);
        the_card.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.beravor,
                the_card));
        return the_card;
    }
    
    public static PlayerCard FAKE_FARAMIR()
    {
        PlayerCard the_card = new PlayerCard("Faramir", LOTRGame.SPHERE_OF_INFLUENCE.LEADERSHIP,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.GONDOR, LOTRGame.TRAITS.NOBLE, LOTRGame.TRAITS.RANGER},
            cost: 4, willpower: 2, attack: 1, defense: 0, hp: 1,
            ability:
            "Action: Exhaust Faramir to choose a player. Each character controlled by that player gets +1 Willpower until the end of the phase.",
            set: "??", type: "ALLY", unique: true);
        the_card.set_action_card();
        /*the_card.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(CardEnablers.i_am_played, PlayerCardResponses.faramir, 
                the_card));*/
        List<Func<EventArgs, Card, bool>> all_action_criteria = new List<Func<EventArgs, Card, bool>>() { CardEnablers.i_am_played, CardEnablers.i_am_not_exhausted};
        the_card.set_action_criteria(all_action_criteria);
        the_card.respond_to_event(GameEvent.ACTIVATED_ACTION,
            PlayerCardResponses.action_maker(all_action_criteria, PlayerCardResponses.faramir,
                the_card));
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
        the_card.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me, CardEnablers.enemy_in_staging}, PlayerCardResponses.son_of_arnor,  the_card, valid_targets: CardEnablers.valid_targets_staged_enemies));
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
        the_card.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me}, PlayerCardResponses.snowbourn_scout, 
                the_card, valid_targets: CardEnablers.valid_targets_staged_locations_and_cur_location));
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
        the_card.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me}, PlayerCardResponses.longbeard_orc_slayer, 
                the_card, valid_targets: CardEnablers.LONGBEARD_ORC_SLAYER_staged_engaged_orcs));
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
        the_card.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(new List<Func<EventArgs, Card, bool>>() {  CardEnablers.card_is_me}, PlayerCardResponses.brok_ironfist, 
                the_card));
        return the_card;
    }
    
    public static PlayerCard GANDALF()
    {
        PlayerCard result = new PlayerCard("Gandalf", LOTRGame.SPHERE_OF_INFLUENCE.NEUTRAL,
            new List<LOTRGame.TRAITS>() {LOTRGame.TRAITS.ISTARI},
            cost: 5, willpower: 4, attack: 4, defense: 4, hp: 4,
            ability:
            "At the end of the round, discard Gandalf from play. " +
            "Response: After Gandalf enters play, (choose 1): draw 3 cards, deal 4 damage to 1 enemy in play, or reduce your threat by 5.",
            set: "??", type: "ALLY", unique: false);
        List<Func<EventArgs, Card, bool>> response_criteria = new List<Func<EventArgs, Card, bool>>() { CardEnablers.card_is_me };
        result.respond_to_event(GameEvent.CARD_PLAYED_KEY,
            PlayerCardResponses.action_maker(response_criteria, PlayerCardResponses.gandalf_played, result));
        result.respond_to_event(GameEvent.OPTION_1_PICKED,
            PlayerCardResponses.action_maker(response_criteria, PlayerCardResponses.gandalf_draw_3, result));
        result.respond_to_event(GameEvent.OPTION_2_PICKED,
            PlayerCardResponses.action_maker(response_criteria, PlayerCardResponses.gandalf_deal_4_damage, result));
        result.respond_to_event(GameEvent.OPTION_3_PICKED,
            PlayerCardResponses.action_maker(response_criteria, PlayerCardResponses.gandalf_reduce_threat, result));
            
        return result;
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
        result = add_cards_to_list(result, GANDALF, 1);
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
