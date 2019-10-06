using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LOTRGame 
{

    private int num_players = 1;
    private LOTRPlayer cur_player;
    private GAMEPHASE cur_phase;
    private List<LOTRPlayer> players;
    private GAMESTATE cur_state;
    private int resources_paid;
    private PlayerCard card_played;
    private List<EnemyCard> staged_cards;
    private List<EnemyCard> encounter_discard_pile;

    private List<EnemyCard> enemy_deck;
    private QuestCard cur_quest;
    private LocationCard cur_location;
    private EnemyCard attacking_enemy;
    private EnemyCard defending_enemy;
    private PlayerCard defending_player_card;
    private List<PlayerCard> attacking_player_cards;
    private bool attack_undefended;
    Dictionary<LOTRPlayer, bool> player_done_engaging; //TODO; maybe maybe this local variable?
    private int damage_to_be_dealt;
    private GAMESTATE state_before_response;
    private bool response_is_yes_no;
    private Action action_after_responding_yes;
    private Action<Card> action_with_card_after_responding_yes;
    private List<Card> cards_set_true;


    private LOTRGameEventHandler game_event_handler;
    private Action continuing_after_response;
    
    public enum GAMESTATE
    {
        GAME_START,
        PLAYING_CARDS,
        PAYING_FOR_CARD,
        CHOOSING_CHARACTER_TO_ATTACH_ATTACHMENT,
        COMMITTING_CHARACTERS,
        STAGING,
        PLAYER_ENGAGEMENT,
        ENGAGEMENT_CHECKS,
        CHOOSING_ENEMY_TO_RESOLVE_ENEMY_ATTACK,
        DECLARING_DEFENDER,
        RESOLVING_SHADOW_EFFECT,
        DETERMINING_COMBAT_DAMAGE,
        ASSIGNING_UNDEFENDED_DAMAGE,
        DECLARING_ENEMY_TO_ATTACK,
        DECLARING_ATTACKERS,
        DONE_ATTACKING,
        CHOOSING_TRAVEL,
        WAITING_FOR_RESPONSE
    }
    public enum GAMEPHASE
    {
        RESOURCE,
        PLANNING,
        QUEST,
        TRAVEL,
        ENCOUNTER,
        COMBAT,
        REFRESH
    }

    public enum SPHERE_OF_INFLUENCE
    {
        LEADERSHIP,
        LORE,
        SPIRIT,
        TACTICS
    }

    public enum TRAITS
    {
        DWARF,
        NOBLE,
        ROHAN,
        GONDOR,
        WARRIOR,
        DUNEDAIN,
        RANGER,
        GOBLIN,
        ORC,
        SCOUT,
        ARCHER,
        SILVAN,
        CREATURE,
        SPIDER,
        INSECT,
        FOREST,
        MOUNTAIN,
        TITLE
    }

    public LOTRGame()
    {
        
    }

    public GAMESTATE get_cur_state()
    {
        return cur_state;
    }


    public void initialize_game()
    {
        this.game_event_handler = new LOTRGameEventHandler(this);
        response_is_yes_no = false;
        PlayerCardResponses.set_game(this);
        cur_player = null;
        cur_location = null;
        players = new List<LOTRPlayer>();
        staged_cards = new List<EnemyCard>();
        encounter_discard_pile = new List<EnemyCard>();
        enemy_deck = EnemyCard.DEBUG_DECK();
        for (var i = 0; i < num_players; i++)
        {
            players.Add(new LOTRPlayer());
        }
        players[0].add_hero(LOTRHero.ARAGORN());
        players[0].add_hero(LOTRHero.GLOIN());
        players[0].add_hero(LOTRHero.THEODRED());
        player_done_engaging = new Dictionary<LOTRPlayer, bool>();
        game_event_handler.register_cards(players[0].get_heroes().Cast<PlayerCard>().ToList());
        begin_game();
    }

    void begin_game()
    {
        cur_state = GAMESTATE.GAME_START;
        cur_quest = QuestCard.PASSAGE_THROUGH_MIRKWOOD_1B();
        begin_resource_phase();
    }

    void begin_resource_phase()
    {
        cur_phase = GAMEPHASE.RESOURCE;
        foreach (var player in players)
        {
            player.add_resource_token_to_all_heroes();
            player.draw_card();
        }

        begin_planning_phase();
    }

    void begin_planning_phase()
    {
        cur_phase = GAMEPHASE.PLANNING;
        cur_state = GAMESTATE.PLAYING_CARDS;
        cur_player = players[0];
        debug();
    }

    void debug()
    {
        cur_player.debug();
    }

    public GAMEPHASE get_cur_phase()
    {
        return cur_phase;
    }

    public LOTRPlayer get_cur_player()
    {
        return cur_player;
    }

    public Dictionary<LOTRHero, int> get_resources(LOTRPlayer player)
    {
        return player.get_resources();
    }

    public void player_played_card(LOTRPlayer player, PlayerCard card)
    {
        cur_state = GAMESTATE.PAYING_FOR_CARD;
        resources_paid = 0;
        card_played = card;
    }

    public void hero_resource_paid(LOTRPlayer player, LOTRHero hero)
    {
        //todo: put in check that player owns hero
        //TODO: check for neutral, cost of zero
        if (card_played != null)
        {
            if (card_played.get_resource_type() == hero.get_resource_type())
            {
                hero.pay_resource();
                resources_paid += 1;
            }

            if (resources_paid == card_played.get_cost())
            {

                if (card_played.is_attachment())
                {
                    Debug.Log("ATTACGHMENT PLAYED");
                    cur_state = GAMESTATE.CHOOSING_CHARACTER_TO_ATTACH_ATTACHMENT;
                }
                else
                {
                    card_has_been_played(player);
                }
            }
        }
    }

    private void card_has_been_played(LOTRPlayer player)
    {
        player.play_card(card_played);
        game_event_handler.register_cards(new List<PlayerCard>() {card_played});
        fire_game_event(cur_player, GameEvent.GAME_EVENT_TYPE.CARD_ENTERS_PLAY, new GameArgs(c:card_played, a:()=>{cur_state = GAMESTATE.PLAYING_CARDS; card_played = null;}));
        
    }

    public void card_attached(LOTRPlayer player, PlayerCard character)
    {
        character.add_attachment((AttachmentCard) card_played);
        card_has_been_played(player);
    }
    
    public void character_committed(LOTRPlayer player, PlayerCard character)
    {
        //todo: check that player owns hero
        character.commit();
        fire_game_event(player, GameEvent.GAME_EVENT_TYPE.COMMITTED, new GameArgs(c:character));
    }

    private void fire_game_event(LOTRPlayer player, GameEvent.GAME_EVENT_TYPE event_type, GameArgs data)
    {
        GameEvent e = GameEvent.get_instance(event_type, data);
        if (player.has_responses(e))
        {
            game_event_handler.fire_event(e, data);
        }
        else
        {
            data.what_to_do_after_event_or_if_no_response();
        }
    }
    
    public void finish_committing()
    {
        cur_state = GAMESTATE.STAGING;
        do_staging();
    }

    public int? get_cur_quest_progress()
    {
        return cur_quest.get_progress();
    }
    
    public int? get_cur_location_progress()
    {
        return cur_location?.get_progress();
    }


    public int get_cur_quest_amount_needed()
    {
        return cur_quest.get_quest_points();
    }

    public int get_cur_location_amount_needed()
    {
        if (cur_location == null)
        {
            return 0;
        }
        return cur_location.get_quest_points();
    }

    void do_staging()
    {
        foreach (var p in players)
        {
            var new_card = draw_from_deck();
            staged_cards.Add(new_card); 
            reveal(new_card);
        }

        quest_resolution();
    }

    public List<EnemyCard> get_staged_enemies()
    {
        return staged_cards;
    }
    
    public List<EnemyCard> get_engaged_enemies(LOTRPlayer player)
    {
        return player.get_engaged_enemies();
    }

    void reveal(EnemyCard card)
    {
        //TODO: when revealed
        if (card.is_treachery_card())
        {
            resolve_treachery_card((TreacheryCard) card);
            
        }
    }

    void resolve_treachery_card(TreacheryCard card)
    {
        Debug.Log("RESOLVING " + card);
        for(var i=0; i < staged_cards.Count; i++)
        {
            var this_card = staged_cards[i];
            if (this_card == card)
            {
                staged_cards.RemoveAt(i);
            }
        }
    }

    void quest_resolution()
    {
        int willpower_committed = get_willpower_committed();

        int threat_strength = get_enemy_threat();

        if (willpower_committed > threat_strength)
        {
            make_progress(willpower_committed, threat_strength);
        }
        else
        {
            unsuccessful_quest(willpower_committed, threat_strength);
        }
        
        begin_travel_phase();
    }

    public int get_willpower_committed()
    {
        int willpower_committed = 0;
        foreach (var p in players)
        {
            willpower_committed += p.get_willpower_committed();
        }

        return willpower_committed;
    }

    public int get_enemy_threat()
    {
        int threat_strength = 0;
        foreach (var c in staged_cards)
        {
            threat_strength += c.get_threat_strength();
        }

        return threat_strength;
    }

    void make_progress(int willpower_committed, int threat_strength)
    {
        int amt_to_increase = willpower_committed - threat_strength;
        if (cur_location == null)
        {
            increase_quest_progress(amt_to_increase);
        }
        else
        {
            increase_location_progress(amt_to_increase);
        }
       
    }

    void increase_quest_progress(int amt_to_increase)
    {
        cur_quest.add_progress_token(amt_to_increase);
        if (get_cur_quest_progress() >= get_cur_quest_amount_needed())
        {
            Debug.Log("YAYYYY WE PASSED THE QUEST");
            //YAY!
            finish_this_quest();
            start_next_quest();
        }
    }

    void finish_this_quest()
    {
        
    }

    void start_next_quest()
    {
        QuestCard next_quest = cur_quest.get_next_stage();
        if (next_quest.is_won_quest())
        {
            Debug.Log("---------------------OMG U WON---------------------");
        }
        else
        {
            cur_quest = next_quest;
        }
    }
    
    void increase_location_progress(int amt_to_increase)
    {
        cur_location.add_progress_token(amt_to_increase);
        if (get_cur_location_progress() >= get_cur_location_amount_needed())
        {
            int? left_over = get_cur_location_progress() - get_cur_location_amount_needed();
            Debug.Log("YAYYYY WE PASSED THE LOCATION");
            increase_quest_progress(left_over.Value);
            reset_location();
            //YAY!
        }
    }

    void reset_location()
    {
        cur_location = null;
    }

    void unsuccessful_quest(int willpower_committed, int threat_strength)
    {
        int threat_to_increase = threat_strength - willpower_committed;
        foreach (var p in players)
        {
            p.increase_threat(threat_to_increase);
        }
    }

    public void plan_phase_ended()
    {
        resources_paid = 0;
        card_played = null;
        begin_quest_phase();
    }

    void begin_quest_phase()
    {
        cur_phase = GAMEPHASE.QUEST;
        cur_state = GAMESTATE.COMMITTING_CHARACTERS;
    }
    
    void begin_travel_phase()
    {
        cur_phase = GAMEPHASE.TRAVEL;
        cur_state = GAMESTATE.CHOOSING_TRAVEL;
        if (no_travel_card_in_staging() || already_traveled())
        {
            begin_encounter_phase();
        }
    }

    public void location_chosen(LocationCard location_to_travel_to)
    {
        if (location_to_travel_to.is_location_card())
        {
            if (can_travel_to_location())
            {
                travel_to_location(location_to_travel_to);
            }
            else
            {
                Debug.Log("WHAT THE SHIT HOW ARE YOU TRAVELING");
            }
        }
    }

    bool no_travel_card_in_staging()
    {
        foreach (var card in staged_cards)
        {
            if (card.get_type() == "LOCATION")
            {
                return false;
            }
        }

        return true;
    }

    bool already_traveled()
    {
        return cur_location != null;
    }
    
    public bool can_travel_to_location()
    {
        return cur_location == null;
    }

    public void travel_to_location(LocationCard location)
    {
        for (var i = 0; i < staged_cards.Count; i++)
        {
            if (staged_cards[i] == location)
            {
                cur_location = location; //TOOD: remove from staging
                staged_cards.RemoveAt(i);
                break;
            }
        } //TODO: travel effects

        finish_travel_phase();
    }

    public void do_not_travel()
    {
        finish_travel_phase();
    }

    public void finish_travel_phase()
    {
        begin_encounter_phase();
    }
    
    void begin_encounter_phase()
    {
        //check for no encounters
        cur_phase = GAMEPHASE.ENCOUNTER;
        cur_state = GAMESTATE.PLAYER_ENGAGEMENT;
        if (no_cards_to_encounter())
        {
            Debug.Log("NO ENCOUNTER");
            finished_engaging_enemies();
        }
    }

    bool no_cards_to_encounter()
    {
        foreach (var card in staged_cards)
        {
            if (card.is_enemy_card())
            {
                return false;
            }
        }

        return true;
    }
    
    

    public LocationCard get_cur_location()
    {
        return cur_location;
    }

    public QuestCard get_cur_quest()
    {
        return cur_quest;
    }

    public void player_engage_enemy(LOTRPlayer player, EnemyCard enemy)
    {
        //TODO: limit to 1
        for (var i = 0; i < staged_cards.Count; i++)
        {
            if (staged_cards[i] == enemy)
            {
                player.engage_enemy(enemy);
                staged_cards.RemoveAt(i);
                break;
            }
        } //TODO: engage effects
    }
    
    

    public void finished_engaging_enemies()
    {
        cur_state = GAMESTATE.ENGAGEMENT_CHECKS;
        check_for_enemy_engagements();
    }

    bool enemies_remaining_that_can_engage()
    {
        foreach (var player in players)
        {
            if (!player_done_engaging[player])
            {
                return true;
            }
        }

        return false;
    }
    void check_for_enemy_engagements()
    {
        foreach (var player in players)
        {
            player_done_engaging[player] = false;
        }

        if (enemies_remaining_that_can_engage()) //todo: change to a while but for now its an if because dont want infinite loop
        {
            foreach (var player in players)
            {
                EnemyCard enemy_with_the_highest_engagement_cost = null;
                int highest_engagement_value = -1;
                int index_of_engaged_enemy = -1;
                for (var i = 0; i < staged_cards.Count; i++)
                {
                    EnemyCard this_enemy = staged_cards[i];
                    if (this_enemy.get_engagement_cost() > highest_engagement_value &&
                        this_enemy.get_engagement_cost() <= player.get_threat_level())
                    {
                        enemy_with_the_highest_engagement_cost = this_enemy;
                        index_of_engaged_enemy = i;
                    }
                   
                    
                } //TODO: engage effects
                
                if (index_of_engaged_enemy != -1)
                {
                    player.engage_enemy(enemy_with_the_highest_engagement_cost);
                    staged_cards.RemoveAt(index_of_engaged_enemy);
                }
                else
                {
                    player_done_engaging[player] = true;
                    //TODO: WT?F!
                }
            }
            //warning: infinite loop possibility...
        }
        //TODO: repeat across players, and then reiterate while there are engaged enemies
        begin_combat_phase();
    }
    void begin_combat_phase()
    {
        cur_phase = GAMEPHASE.COMBAT;
        foreach (var player in players)
        {
            player.reset_engaged_enemy_resolutions();
            deal_shadow_card_to_players_engaged_enemies(player);
        }
    }

    void deal_shadow_card_to_players_engaged_enemies(LOTRPlayer player)
    {
        foreach (var engaged_enemy in player.get_engaged_enemies())
        {
            engaged_enemy.add_shadow_card(get_shadow_card());
        }

        resolve_enemy_attacks();
    }

    void resolve_enemy_attacks()
    {
        if (cur_player.has_engaged_enemies())
        {
            cur_state = GAMESTATE.CHOOSING_ENEMY_TO_RESOLVE_ENEMY_ATTACK;
        }
        else
        {
            attack_enemies();
            //todo: loop to the next player
        }
        //TODO: enable playing actions
        //TODO; check to see if there are still enemies
    }

    void attack_enemies()
    {
        cur_state = GAMESTATE.DECLARING_ENEMY_TO_ATTACK; //TODO: check for having at least one ready character
        if (!cur_player.has_enemies_to_attack())
        {
            done_attacking_enemies();
        }
    }

    public void player_chose_enemy_to_resolve(LOTRPlayer player, EnemyCard enemy)
    {
        //todo: check that enemy is engaging player
        attacking_enemy = enemy;
        cur_state = GAMESTATE.DECLARING_DEFENDER;
    }

    public void player_chose_enemy_to_attack(LOTRPlayer player, EnemyCard enemy)
    {
        defending_enemy = enemy;
        cur_state = GAMESTATE.DECLARING_ATTACKERS;
        attacking_player_cards = new List<PlayerCard>();
    }

    public void player_finished_choosing_attackers(LOTRPlayer player)
    {
        foreach (var attacker in attacking_player_cards)
        {
            attacker.declared_as_attacker();
        }
        determine_combat_damage_player_attack_enemy_defend();
    }

    void determine_combat_damage_player_attack_enemy_defend()
    {
        int attack_strength = 0;
        foreach (var attacker in attacking_player_cards)
        {
            attack_strength += attacker.get_attack_strength();
        }
        
        int combat_damage = 0;
        int enemy_defense = defending_enemy.get_defense();

        combat_damage = attack_strength - enemy_defense;

        if (combat_damage > 0)
        {
            deal_damage_to_enemy(defending_enemy, combat_damage);
        }

        attack_enemies();
    }

    public void deal_damage_to_enemy(EnemyCard enemy, int combat_damage)
    {
        enemy.take_damage(combat_damage);
        if (enemy.is_dead())
        {
            encounter_discard_pile.Add(enemy);
            cur_player.enemy_defeated(enemy);
        }
    }

    public void done_attacking_enemies()
    {
        cur_state = GAMESTATE.DONE_ATTACKING;
        shadow_cards_leave_play();
        begin_refresh_phase();
    }

    void shadow_cards_leave_play()
    {
        //?!!
    }
    

    public void player_chose_defender(LOTRPlayer player, PlayerCard defender)
    {
        if (defender != null && defender.is_exhausted())
        {
            Debug.Log("CANT DEFEND, I AM EXHAUSTED");
            return;
        } 
        
        if (defender == null)
        {
            //TODO: undefended
            attack_undefended = true;
        }
        else
        {
            defending_player_card = defender;
            defending_player_card.declared_as_defender();
            Debug.Log("ATTACK DEFENDED BY " + defender.get_name());
            attack_undefended = false;
        }

        resolve_shadow_effect(attacking_enemy);
    }

    public void attacker_chosen(LOTRPlayer player, PlayerCard attacker)
    {
        if (!attacker.is_exhausted())
        {
            attacker.exhaust();
            attacking_player_cards.Add(attacker);
        }
        
    }

    void resolve_shadow_effect(EnemyCard enemy)
    {
        //TODO: ?!?!
        cur_state = GAMESTATE.RESOLVING_SHADOW_EFFECT;
        determine_combat_damage_enemy_attack_player_defend();
    }

    bool attack_is_undefended()
    {
        return attack_undefended || defending_player_card == null;
    }

    void determine_combat_damage_enemy_attack_player_defend()
    {
        cur_state = GAMESTATE.DETERMINING_COMBAT_DAMAGE;
        int attackers_attack = attacking_enemy.get_attack();
        int defenders_defense = 0;
        if (!attack_is_undefended())
        {
            defenders_defense = defending_player_card.get_defense();
        }

        damage_to_be_dealt = attackers_attack - defenders_defense;

        if (!attack_is_undefended())
        {
            defending_player_card.take_damage(damage_to_be_dealt);
            fire_game_event(cur_player, GameEvent.GAME_EVENT_TYPE.TAKE_DAMAGE, 
                new GameArgs(c:defending_player_card, 
                    i: damage_to_be_dealt,
                a: () => {cur_player.enemy_attack_resolved(attacking_enemy); resolve_enemy_attacks();}));
            
        }
        else
        {
            cur_state = GAMESTATE.ASSIGNING_UNDEFENDED_DAMAGE;
        }
    }

    public int get_damage_to_be_dealt()
    {
        return damage_to_be_dealt;
    }

    public void hero_chosen_to_take_undefended_damage(LOTRPlayer player, LOTRHero hero)
    {
        hero.take_damage(damage_to_be_dealt);
        fire_game_event(cur_player, GameEvent.GAME_EVENT_TYPE.TAKE_DAMAGE, new GameArgs(c: hero,
            i: damage_to_be_dealt,
            a: () => {cur_player.enemy_attack_resolved(attacking_enemy);
                resolve_enemy_attacks();
                //todo: check
                }));
        
    }

    EnemyCard get_shadow_card()
    {
        return draw_from_deck(shuffle_in_discard_pile: false); //if encounter deck runs out of cards, any enemies that have not been dealt shadow cards are not dealt
    }

    EnemyCard draw_from_deck(bool shuffle_in_discard_pile = true)
    {
        if (shuffle_in_discard_pile)
        {
            
        }

        if (enemy_deck.Count == 0)
        {
            return null;
        }
        var new_card = enemy_deck[0]; //TODO: shuffling
        enemy_deck.RemoveAt(0);
        return new_card;
    }
    void begin_refresh_phase()
    {
        cur_phase = GAMEPHASE.REFRESH;
        foreach (var player in players)
        {
            player.ready_all_exhausted_cards();
            player.increase_threat(1);
        }
        //all exhausted cards ready
        //each player increases threat by 1
        //first player passes the first player token to the next player clockwise on his left
        //check for game over?
        begin_resource_phase();
    }

    public int get_player_threat(LOTRPlayer player)
    {
        return player.get_threat_level();
    }



    public void wait_for_response(Action if_yes=null,  bool response_is_yes_no = false, Action<Card> if_yes_with_card=null, Action what_to_do_after_responding=null)
    {
        this.response_is_yes_no = response_is_yes_no;
        action_after_responding_yes = if_yes;
        action_with_card_after_responding_yes = if_yes_with_card;
        continuing_after_response = what_to_do_after_responding;
        Debug.Log("DO U WANNA RESPOND");
        state_before_response = cur_state;
        cur_state = GAMESTATE.WAITING_FOR_RESPONSE;
    }

    public bool is_response_yes_no()
    {
        return this.response_is_yes_no;
    }

    public void not_going_to_respond()
    {
        done_responding();
    }

    public void yes_responded(Card c=null)
    {
        if (c == null && action_after_responding_yes != null)
        {
            action_after_responding_yes();

        } else if (action_after_responding_yes == null && action_with_card_after_responding_yes != null)
        {
            Debug.Log("I RESPONDED FUCKLER WITH CARD");
            action_with_card_after_responding_yes(c);
        }
        Debug.Log("I RESPONDED FUCKLER");

        done_responding();
    }
    
    public void done_responding()
    {
        this.response_is_yes_no = false;
        action_with_card_after_responding_yes = null;
        action_after_responding_yes = null;
        if (cards_set_true != null)
        {
            foreach (var card in cards_set_true)
            {
                card.reset_response_target();
            }
        }
        cur_state = state_before_response;
        if (continuing_after_response != null)
        {
            continuing_after_response();
        }
        
        continuing_after_response = null;
    }

    public void set_valid_response_target(List<Card> cards_to_set_valid)
    {
        foreach (var card in cards_to_set_valid)
        {
            card.set_response_target_true();
        }

        cards_set_true = cards_to_set_valid;
    }
    

}
