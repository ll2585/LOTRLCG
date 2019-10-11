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
    private GAMESTATE state_before_actions;
    private bool response_is_yes_no;
    private Action action_after_responding_yes;
    private Action<Card> action_with_card_after_responding_yes;
    private List<Card> cards_set_true;

    private LOTRGameEventHandler game_event_handler;
    private Action continuing_after_response;

    private Action after_actions_played;
    private bool allowing_actions;

    private Action<Card> action_after_paying_for_card;
    private bool forced_response_after_action;
    private int times_to_respond;
    private int times_responded;
    private bool waiting_for_player_response;
    private Card card_to_respond_to;
    private bool allow_actions_after_response;

    private int num_options_to_display;
    
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
        WAITING_FOR_RESPONSE,
        WAITING_FOR_RESPONSE_FORCED,
        WAITING_FOR_PLAYER_TO_DECIDE_IF_HE_WANTS_TO_USE_ACTIONS
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
        NEUTRAL,
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
        TITLE,
        DOL_GULDUR,
        STRONGHOLD,
        ITEM,
        ARTIFACT,
        ISTARI
    }

    public LOTRGame()
    {
        
    }

    public GAMESTATE get_cur_state()
    {
        return cur_state;
    }

    public Card get_card_to_respond_to()
    {
        return card_to_respond_to;
    }


    public void initialize_game()
    {
        this.game_event_handler = new LOTRGameEventHandler(this);
        response_is_yes_no = false;
        waiting_for_player_response = false;
        EnemyCardResponses.set_game(this);
        PlayerCardResponses.set_game(this);
        CardEnablers.set_game(this);
        cur_player = null;
        cur_location = null;
        players = new List<LOTRPlayer>();
        staged_cards = new List<EnemyCard>();
        encounter_discard_pile = new List<EnemyCard>();
        enemy_deck = EnemyCard.PASSAGE_THROUGH_MIRWOOD_ENEMIES();
        //Utils.Shuffle(enemy_deck);
        for (var i = 0; i < num_players; i++)
        {
            players.Add(new LOTRPlayer());
        }
        players[0].add_hero(LOTRHero.ARAGORN());
        players[0].add_hero(LOTRHero.GLOIN());
        players[0].add_hero(LOTRHero.THEODRED());
        player_done_engaging = new Dictionary<LOTRPlayer, bool>();
        allowing_actions = false;
        forced_response_after_action = false;
        times_to_respond = 1;
        card_to_respond_to = null;
        times_responded = 0;
        num_options_to_display = -1;
        begin_game();
    }

    void begin_game()
    {
        cur_state = GAMESTATE.GAME_START;
        cur_quest = QuestCard.PASSAGE_THROUGH_MIRKWOOD_1B();
        cur_player = players[0];
        for (int i = 0; i < 6; i++)
        {
            cur_player.draw_card(); //mulligan blah
        }
        begin_resource_phase();
    }

    void begin_resource_phase()
    {
        new_phase_started(GAMEPHASE.RESOURCE);
        disallow_actions();
        foreach (var player in players)
        {
            player.add_resource_token_to_all_heroes();
            if (cur_location != null && (cur_location.get_name() == LocationCard.ENCHANTED_STREAM().get_name()))
            {
                
            }
            else
            {
                player.draw_card();
            }
        }

        allow_actions_to_be_played(begin_planning_phase);
    }

    public List<LOTRPlayer> get_players()
    {
        return this.players;
    }
    
    public void allow_actions_to_be_played(Action next_function)
    {
        allow_actions_to_be_played();
        Debug.Log(get_cur_phase() + " and next is " + next_function);
        after_actions_played = next_function;
        if (after_actions_played == null)
        {
            Debug.Log("WTF WHY IS THIS NULL");
        }

        if (player_can_play_actions())
        {
            play_actions_phase();
        }
        else
        {
            Debug.Log("ACTIONSF INISHED FIRING");
            actions_finished();
        }
    }

    void new_phase_started(LOTRGame.GAMEPHASE new_phase)
    {
        Debug.Log("new phase started");
        cur_phase = new_phase;
        foreach (LOTRPlayer player in players)
        {
            ///player.clear_abilities_on_phase_change(cur_phase);
            player.new_phase_started();
        }
    }
    
    public bool player_can_play_actions()
    {
        foreach (LOTRPlayer player in players)
        {
            if (player.can_play_actions(new GameArgs(game: this, p: player)))
            {
                return true;
            }
        }
        return false;
    }
    
    public void action_played(PlayerCard card)
    {
        state_before_actions = get_cur_state();
        Debug.Log("OK was played " + card.get_name());
        disallow_actions(); //todo: maybe not if you can chain actions
        game_event_handler.fire_game_event(GameEvent.ACTIVATED_ACTION, new GameArgs(game: this, p: get_cur_player(), c:card));
    }

    public void execute_action(Action a)
    {
        a();
        allow_actions_to_be_played();
    }

    public void played_action_from_hand(PlayerCard card, bool needs_response)
    {
        //game.wait_for_forced_response(after_responding_with_card: the_action);
        Debug.Log("PLAUED " + card.get_name() + " FROM HAND");
        forced_response_after_action = needs_response;
        player_played_card(get_cur_player(), card);
    }
    
    
    public void play_actions_phase()
    {
        Debug.Log("OK actiosn played");
        cur_state = GAMESTATE.WAITING_FOR_PLAYER_TO_DECIDE_IF_HE_WANTS_TO_USE_ACTIONS;
        allow_actions_to_be_played();
        //actions_finished();
    }

    private void allow_actions_to_be_played()
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!ALOWING ACTIONS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11");
        allowing_actions = true;
    }

    private void disallow_actions()
    {
        allowing_actions = false;
    }

    public bool is_allowing_actions()
    {
        return allowing_actions;
    }
    
    public void actions_finished()
    {
        disallow_actions();
        after_actions_played();
        Debug.Log("OK ACTIONS FINISHED");
    }



    void begin_planning_phase()
    {
        allow_actions_to_be_played();
        new_phase_started(GAMEPHASE.PLANNING);
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
        disallow_actions();
        cur_state = GAMESTATE.PAYING_FOR_CARD;
        resources_paid = 0;
        card_played = card;
        Debug.Log("@L$ERK#TJ$KGJM");
        if (card_played.get_cost() == 0 && cur_player.has_hero_of_sphere(card.get_sphere()))
        {
            if (card_played.is_attachment())
            {
                Debug.Log("ATTACGHMENT PLAYED");
                cur_state = GAMESTATE.CHOOSING_CHARACTER_TO_ATTACH_ATTACHMENT;
            }
            else
            {
                Debug.Log("CARD PLAYED");
                card_has_been_played(player);
            }
        }
    }

    public void hero_resource_paid(LOTRPlayer player, LOTRHero hero)
    {
        //todo: put in check that player owns hero
        //TODO: check for neutral, cost of zero
        if (card_played != null)
        {
            if ((card_played.get_resource_type() == hero.get_resource_type()) || (card_played.get_resource_type() == SPHERE_OF_INFLUENCE.NEUTRAL))
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
                    Debug.Log("CARD PLAYED");
                    card_has_been_played(player);
                }
            }
        }
    }

    public PlayerCard get_card_played()
    {
        return card_played;
    }

    private void card_has_been_played(LOTRPlayer player)
    {
        player.play_card(card_played);
        //game_event_handler.register_cards(new List<PlayerCard>() {card_played});
        Debug.Log("CARD HAS BEEN PLAYED#(O$I()*%$U*(GJ$IOG");
        if (cur_phase == GAMEPHASE.PLANNING)
        {
            game_event_handler.fire_game_event(GameEvent.CARD_PLAYED_KEY, new GameArgs(c:card_played, a:()=>
            {
                Debug.Log("OK WHY"); 
                cur_state = GAMESTATE.PLAYING_CARDS; 
                card_played = null;
                allow_actions_to_be_played();
            },  p: get_cur_player(), game:this));
        }
        else //playing actions not in planning phase
        {
            Debug.Log(cur_state);
           // cur_state = GAMESTATE.WAITING_FOR_PLAYER_TO_DECIDE_IF_HE_WANTS_TO_USE_ACTIONS;
            game_event_handler.fire_game_event(GameEvent.CARD_PLAYED_KEY, new GameArgs(c:card_played, a:()=>
            {
                Debug.Log("Firing this game event CARD PLAYED CALLBACK");
                card_played = null;
                if (forced_response_after_action)
                {
                    done_responding();
                }
                
                cur_state = state_before_actions;
                //allow_actions_to_be_played();
            },  p: get_cur_player(), game:this));
        }
        
    }

    public void card_attached(LOTRPlayer player, PlayerCard character)
    {
        Debug.Log("Attached " + card_played.get_name() + " to " + character.get_name());
        character.add_attachment((AttachmentCard) card_played);
        Debug.Log(string.Join(",", character.get_attachments()
            .Select(array => string.Join(" ", array))));
        card_has_been_played(player);
    }
    
    public void character_committed(LOTRPlayer player, PlayerCard character)
    {
        //todo: check that player owns hero
        character.commit();
        game_event_handler.fire_game_event(GameEvent.CARD_COMMITTED_KEY,new GameArgs(c:character));
        Debug.Log("Does this actullay wait");
        //fire_game_event(player, GameEvent.GAME_EVENT_TYPE.COMMITTED, new GameArgs(c:character));
    }

    public bool can_commit_card(PlayerCard card)
    {
        bool is_exhausted = card.is_exhausted();
        bool is_committed = card.is_committed();
        bool is_hero_played = get_cur_player().get_heroes().Contains(card);
        bool is_ally_played = get_cur_player().get_allies().Contains(card);
        return !is_committed && !is_exhausted && (is_ally_played || is_hero_played);
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
        disallow_actions();
        foreach (var p in players)
        {
            var new_card = draw_from_deck();
            reveal(new_card);
            if (!new_card.is_treachery_card())
            {
                staged_cards.Add(new_card); 
            }
        }
    }

    public void add_new_encounter_card_to_staging()
    {
        var new_card = draw_from_deck();
        reveal(new_card);
        if (!new_card.is_treachery_card())
        {
            staged_cards.Add(new_card); 
        }
        
    }

    public void cur_player_discard_attachment(AttachmentCard attachment=null, bool all=false)
    {
        if (all)
        {
            foreach (PlayerCard hero in get_cur_player().get_heroes())
            {
                foreach (AttachmentCard a in hero.get_attachments())
                {
                    cur_player.discard_attachment(a);
                }
            }

            foreach (PlayerCard ally in get_cur_player().get_allies())
            {
                foreach (AttachmentCard a in ally.get_attachments())
                {
                    cur_player.discard_attachment(a);
                }
            } //TODO: fire events blahblah
        }
        else
        {
            cur_player.discard_attachment(attachment);
        }
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
        card.enters_the_game();
        game_event_handler.fire_game_event(GameEvent.CARD_REVEALED_ALLOW_COUNTER,new GameArgs(c:card, a: () =>
        {
            game_event_handler.fire_game_event(GameEvent.CARD_REVEALED, new GameArgs(c: card, a: () =>
            {
                if (card.is_treachery_card())
                {
                    resolve_treachery_card((TreacheryCard) card);
                }

                Debug.Log("OK STAGING FINISHED");
                allow_actions_to_be_played(quest_resolution); //todo: change this so it can fit multiple players
            }, game: this, p: get_cur_player()));
        }, game: this, p: get_cur_player()));
        
    }

    public void continue_staging()
    {
        
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
        disallow_actions();
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
        Debug.Log("OK QUEST FINISHED");
        allow_actions_to_be_played(begin_travel_phase);
    }

    public int get_willpower_committed()
    {
        int willpower_committed = 0;
        foreach (var p in players)
        {
            willpower_committed += p.get_willpower_committed(cur_phase);
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
        new_phase_started(GAMEPHASE.QUEST);
        cur_state = GAMESTATE.COMMITTING_CHARACTERS;
    }
    
    void begin_travel_phase()
    {
        new_phase_started(GAMEPHASE.TRAVEL);
        disallow_actions();
        cur_state = GAMESTATE.CHOOSING_TRAVEL;
        if (no_travel_card_in_staging() || already_traveled() || !can_travel_to_any_locations())
        {
            begin_encounter_phase();
        }
    }

    bool can_travel_to_any_locations()
    {
        foreach (var card in staged_cards)
        {
            if (card.get_type() == "LOCATION")
            {
                if (((LocationCard) card).can_travel_here(new GameArgs(game: this, p: get_cur_player())))
                {
                    return true;
                }
            }
        }
        return false;
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
                game_event_handler.fire_game_event(GameEvent.LOCATION_TRAVELED, new GameArgs(game: this, p: get_cur_player(), c:location, a:
                    () =>
                    {
                        finish_travel_phase();
                    }));
                break;
            }
        } //TODO: travel effects

        
    }

    public void do_not_travel()
    {
        finish_travel_phase();
    }

    public void finish_travel_phase()
    {
        allow_actions_to_be_played(begin_encounter_phase);
    }
    
    void begin_encounter_phase()
    {
        new_phase_started(GAMEPHASE.ENCOUNTER);
        disallow_actions();
        //check for no encounters
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
        disallow_actions();
        //TODO: limit to 1
        for (var i = 0; i < staged_cards.Count; i++)
        {
            if (staged_cards[i] == enemy)
            {
                player.engage_enemy(enemy);
                game_event_handler.fire_game_event(GameEvent.ENEMY_ENGAGED, new GameArgs(
                    c: enemy)); //hopefulyl no callback
                staged_cards.RemoveAt(i);
                break;
            }
        } //TODO: engage effects
        allow_actions_to_be_played();
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
        disallow_actions();
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
        allow_actions_to_be_played(begin_combat_phase);
    }
    void begin_combat_phase()
    {
        new_phase_started(GAMEPHASE.COMBAT);
        foreach (var player in players)
        {
            player.reset_engaged_enemy_resolutions();
            deal_shadow_card_to_players_engaged_enemies(player);
        }
    }

    void deal_shadow_card_to_players_engaged_enemies(LOTRPlayer player)
    {
        disallow_actions();
        foreach (var engaged_enemy in player.get_engaged_enemies())
        {
            if (!engaged_enemy.has_shadow_cards())
            {
                if (enemy_deck.Count > 0)
                {
                    EnemyCard shadow_card = enemy_deck[0];
                    enemy_deck.RemoveAt(0);
                    shadow_card.set_face_down();
                    engaged_enemy.add_shadow_card(shadow_card); //todo: highest engagement first
                    //TODO: maybe throw an event for this guy but for now just do this here....
                    if (engaged_enemy.get_name() == EnemyCard.DOL_GULDUR_BEASTMASTER().get_name())
                    {
                        EnemyCard shadow_card2 = enemy_deck[0];
                        enemy_deck.RemoveAt(0);
                        shadow_card2.set_face_down();
                        engaged_enemy.add_shadow_card(shadow_card2); //todo: highest engagement first
                    }
                }
                
            }
            
        }
        allow_actions_to_be_played(resolve_enemy_attacks);
       
    }

    void resolve_enemy_attacks()
    {
        Action callback = () =>
        {
            allow_actions_to_be_played();
            if (cur_player.has_engaged_enemies())
            {
                cur_state = GAMESTATE.CHOOSING_ENEMY_TO_RESOLVE_ENEMY_ATTACK;
            }
            else
            {
                attack_enemies();
                //todo: loop to the next player
            }
        };
        if (attacking_enemy != null)
        {
            game_event_handler.fire_game_event(GameEvent.ENEMY_ATTACK_RESOLVED, new GameArgs(
                c: attacking_enemy,
                a: callback));
        } else //the first enemy chosen
        {
            callback();
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
        disallow_actions();
        //todo: check that enemy is engaging player
        attacking_enemy = enemy;
        cur_state = GAMESTATE.DECLARING_DEFENDER;
        allow_actions_to_be_played();
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
        foreach (var player in players)
        {
            foreach (var engaged_enemy in player.get_engaged_enemies())
            {
                if (engaged_enemy.has_shadow_cards())
                {
                    
                    encounter_discard_pile.AddRange(engaged_enemy.get_shadow_cards());
                    engaged_enemy.discard_shadow_cards();
                    Debug.Log("BYE SHADOW CARD FOR " + engaged_enemy.get_name());
                }
            }
        }
    }
    

    public void player_chose_defender(LOTRPlayer player, PlayerCard defender)
    {
        disallow_actions();
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
        allow_actions_to_be_played();
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
        disallow_actions();
        cur_state = GAMESTATE.RESOLVING_SHADOW_EFFECT;
        if (enemy.has_shadow_cards())
        {
            foreach (EnemyCard shadow_card in enemy.get_shadow_cards())
            {
                //DOES THIS WORK?!?!?!?!
                //EnemyCard shadow_card = enemy.get_shadow_card();
                shadow_card.set_face_up();
                shadow_card.enters_the_game();
                game_event_handler.fire_game_event(GameEvent.SHADOW_CARD_REVEALED,new GameArgs(
                    c:shadow_card,
                    a: () =>
                    {
                        shadow_card.set_shadow_effect_resolved();
                        if (enemy.all_shadow_cards_resolved())
                        {
                            Debug.Log("DONE RESOLVING SHADOW EFFECT");
                            allow_actions_to_be_played();
                            determine_combat_damage_enemy_attack_player_defend();
                        }
                        
                    }, 
                    attack_undefended:attack_is_undefended(),
                    secondary_card: enemy));
            }
            
        }
        else
        {
            allow_actions_to_be_played();
            determine_combat_damage_enemy_attack_player_defend();
        }
        
        
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
            Action callback = () => {
                cur_player.enemy_attack_resolved(attacking_enemy);
                resolve_enemy_attacks();
            };
            card_takes_damage(defending_player_card, damage_to_be_dealt, callback);

        }
        else
        {
            cur_state = GAMESTATE.ASSIGNING_UNDEFENDED_DAMAGE;
        }
    }

    public void card_takes_damage(PlayerCard card, int damage, Action callback=null)
    {
        card.take_damage(damage);
        Debug.Log(card.get_name() + " is taking " + damage + " damage.");
        game_event_handler.fire_game_event(GameEvent.CHARACTER_TOOK_DAMAGE,new GameArgs(c:card, 
            i: damage,
            a: () =>
            {
                if (card.is_dead())
                {
                    if (card.is_ally())
                    {
                        cur_player.ally_died(card);
                        game_event_handler.fire_game_event(GameEvent.CARD_LEAVES_PLAY,new GameArgs(c:card,
                            a: () =>
                            {
                                Debug.Log("SOMEONE DIED WHAT TO DO");
                            },
                            p: get_cur_player(),
                            game: this));
                    }
                    else
                    {
                        Debug.Log("HERO DIED WHAT TO DO");
                    }
                    
                }
                Debug.Log("SOMEONE TOOK DAMAGE");

                if (callback != null)
                {
                    callback();
                }
                
            }));
    }

    public int get_damage_to_be_dealt()
    {
        return damage_to_be_dealt;
    }

    public void hero_chosen_to_take_undefended_damage(LOTRPlayer player, LOTRHero hero)
    {
        Action callback = () =>
        {
            cur_player.enemy_attack_resolved(attacking_enemy);
            resolve_enemy_attacks();
        };
        card_takes_damage(hero, damage_to_be_dealt, callback);
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
        disallow_actions();
        new_phase_started(GAMEPHASE.REFRESH);
        foreach (var player in players)
        {
            player.ready_all_exhausted_cards();
            player.increase_threat(1);
        }
        //all exhausted cards ready
        //each player increases threat by 1
        //first player passes the first player token to the next player clockwise on his left
        //check for game over?
        allow_actions_to_be_played(new_round);
    }

    void new_round()
    {
        foreach (LOTRPlayer player in players)
        {
            foreach (EnemyCard engaged_enemy in get_engaged_enemies(player))
            {
                engaged_enemy.clear_flags(LOTRAbility.CLEAR_MARKERS.END_OF_ROUND);
            }
        }
        
        begin_resource_phase();
    }

    public int get_player_threat(LOTRPlayer player)
    {
        return player.get_threat_level();
    }

    public bool is_waiting_for_player_response()
    {
        return waiting_for_player_response;
    }


    public void wait_for_response(Card card_to_respond_to, Action if_yes = null, bool response_is_yes_no = false,
        Action<Card> if_yes_with_card = null, Action what_to_do_after_responding = null, int times_to_respond = 1
        )
    {
        allow_actions_after_response = is_allowing_actions();
        disallow_actions();
        this.response_is_yes_no = response_is_yes_no;
        waiting_for_player_response = true;
        action_after_responding_yes = if_yes;
        this.times_to_respond = times_to_respond;
        this.card_to_respond_to = card_to_respond_to;
        action_with_card_after_responding_yes = if_yes_with_card;
        continuing_after_response = what_to_do_after_responding;
        Debug.Log("DO U WANNA RESPOND");
        state_before_response = cur_state;
        cur_state = GAMESTATE.WAITING_FOR_RESPONSE;
    }

    public void wait_for_forced_response(Card card_to_respond_to, Action<Card> after_responding_with_card = null,
        Action what_to_do_after_responding = null, int times_to_respond = 1)
    {
        allow_actions_after_response = is_allowing_actions();
        disallow_actions();
        action_with_card_after_responding_yes = after_responding_with_card;
        continuing_after_response = what_to_do_after_responding;
        Debug.Log("RESPOND FUCKER");
        waiting_for_player_response = true;
        this.card_to_respond_to = card_to_respond_to;
        state_before_response = cur_state;
        cur_state = GAMESTATE.WAITING_FOR_RESPONSE_FORCED;
        this.times_to_respond = times_to_respond;
    }

    public bool is_response_yes_no()
    {
        return this.response_is_yes_no;
    }
    
    public void not_going_to_respond()
    {
        waiting_for_player_response = false;
        done_responding();
    }

    public void yes_responded(Card c=null)
    {
        Debug.Log("Yes, I responded");
        waiting_for_player_response = false;
        times_responded += 1;
        if (c == null && action_after_responding_yes != null)
        {
            Debug.Log("DO THE CLALBACK");
            action_after_responding_yes();
            this.response_is_yes_no = false;

        } else if (action_after_responding_yes == null && action_with_card_after_responding_yes != null)
        {
            Debug.Log("I RESPONDED FUCKLER WITH CARD");
            action_with_card_after_responding_yes(c);
            this.response_is_yes_no = false;
        }
        Debug.Log("I RESPONDED FUCKLER " + times_responded + " AND " + times_to_respond);
        if (times_responded >= times_to_respond)
        {
            done_responding();
        }
        
    }
    
    public void done_responding()
    {
        if (allow_actions_after_response)
        {
            allow_actions_to_be_played();
        }
        else
        {
            disallow_actions();
        }
        times_responded = 0;
        times_to_respond = 1;
        this.response_is_yes_no = false;
        action_with_card_after_responding_yes = null;
        action_after_responding_yes = null;
        card_to_respond_to = null;
        reset_valid_targets();
        cur_state = state_before_response;
        forced_response_after_action = false;
        Debug.Log("Here we do event_controller.resume_handling BUT WHEN?!");
        if (continuing_after_response != null)
        {
            Debug.Log("Continuing");
            continuing_after_response();
        }
        Debug.Log("Do we resume handling here?!");
        game_event_handler.resume_handling();
        continuing_after_response = null;
        //allow_actions_to_be_played();
    }

    public void reset_valid_targets()
    {
        if (cards_set_true != null)
        {
            foreach (var card in cards_set_true)
            {
                card.reset_response_target();
            }
        }
    }

    public void set_valid_response_target(List<Card> cards_to_set_valid)
    {
        foreach (var card in cards_to_set_valid)
        {
            card.set_response_target_true();
        }

        cards_set_true = cards_to_set_valid;
    }

    public void show_options(int num_options)
    {
        num_options_to_display = num_options;
    }

    public bool is_showing_options()
    {
        return num_options_to_display != -1;
    }
    public void option_selected(int which_option)
    {
        num_options_to_display = -1;
        object event_to_fire = null;
        if (which_option == 1)
        {
            event_to_fire = GameEvent.OPTION_1_PICKED;
        } else if (which_option == 2)
        {
            event_to_fire = GameEvent.OPTION_2_PICKED;
        } else if (which_option == 3)
        {
            event_to_fire = GameEvent.OPTION_3_PICKED;
        }
        waiting_for_player_response = false;
        game_event_handler.fire_game_event(event_to_fire, new GameArgs(game: this, p: get_cur_player(), c:card_to_respond_to, a:
            () =>
            {
                //yes_responded();
            }));
    }

}
