
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCardResponses
{
    private static LOTRGame game;
    private static PlayerCardResponses all_responses;

    static PlayerCardResponses()
    {
    }
    
    public static void set_game(LOTRGame g)
    {
        game = g;
    }
    
    public static EventHandler action_maker(List<Func<EventArgs, Card, bool>> list_of_criteria, Action<EventArgs> do_effect, Card relevant_card, Func<List<Card>> valid_targets = null,
        bool fires_on_other_cards = false)
    {
        Action<object, EventArgs> a = (object o, EventArgs args) =>
        {
            GameArgs game_args = (GameArgs) args;
            Card the_card = game_args.relevant_card;
            if (relevant_card == the_card || fires_on_other_cards)
            {
                Debug.Log("ITSAME " + relevant_card.get_name());
                bool all_criteria_valid = true;
                foreach (var criteria in list_of_criteria)
                {
                    all_criteria_valid = all_criteria_valid && criteria(game_args, relevant_card);
                }
                Debug.Log(" is all criteria valid? " + all_criteria_valid + 
                          " whgat about null targets? " + (valid_targets == null || valid_targets().Count > 0) + 
                          " how about the relevant card being me or firing " + (relevant_card == the_card || fires_on_other_cards));

                if (all_criteria_valid && (valid_targets == null || valid_targets().Count > 0) &&
                    (relevant_card == the_card || fires_on_other_cards)) //only card played can be played
                {
                    Debug.Log("I am responding " + relevant_card.get_name());
                    if (fires_on_other_cards)
                    {
                        game_args.secondary_card = relevant_card;
                    }
                    do_effect(game_args);
                    if (valid_targets != null && valid_targets().Count > 0)
                    {
                        game.set_valid_response_target(valid_targets());
                    }
                }
                else
                {
                    Debug.Log("I did not respond " + relevant_card.get_name());
                    if (game_args.what_to_do_after_event_or_if_no_response != null)
                    {
                        Debug.Log("there is a callback" + relevant_card.get_name());
                        //game_args.what_to_do_after_event_or_if_no_response();
                    }
                    else
                    {
                        Debug.Log("WJHY ARE ITHRE NOTHIGN TO DO AFTER");
                    }
                }
            }
            else
            {
                Debug.Log("The card " + the_card.get_name() + " is not me " + relevant_card.get_name() + " so i do not care");
            }
            
        };
        EventHandler e2 = (object o, EventArgs args) => a(o, args);
        return e2;
    }

    public static void theodred(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> theodreds_action = (hero_chosen) => { ((LOTRHero) hero_chosen).add_resource_token(); };
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes_with_card: theodreds_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void gloin(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Card character_damaged = game_args.relevant_card;
        int? amt_damaged = game_args.relevant_int;
        LOTRHero gloin = (LOTRHero) character_damaged;
        Action gloins_action = () => { gloin.add_resource_token(amt_damaged.Value); };
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes: gloins_action, response_is_yes_no:true, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);

    }
    
    public static void aragorn(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Card character_committed = game_args.relevant_card;
        LOTRHero aragorn = (LOTRHero) character_committed;
        Action aragorns_action = () => { aragorn.pay_resource(); aragorn.unexhaust();};
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes:aragorns_action, response_is_yes_no:true, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void prompt_reply_from_hand(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        PlayerCard actual_card = (PlayerCard) game_args.secondary_card;
        Action the_action = () => { game.action_played(card: actual_card);};
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes:the_action, response_is_yes_no:true, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response, times_to_respond: 2);
    }

    public static void son_of_arnor(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (Card enemy) => { game.player_engage_enemy(game.get_cur_player(), (EnemyCard) enemy);};
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes_with_card:the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }

    public static void snowbourn_scout(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (location) => { ((LocationCard) location).add_progress_token();};
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes_with_card:the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void longbeard_orc_slayer(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (Card enemy) => { game.deal_damage_to_enemy((EnemyCard) enemy, 1);};
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes_with_card:the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void brok_ironfist(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        PlayerCard me = (PlayerCard) game_args.relevant_card;
        Action the_action = () => { game.get_cur_player().play_card(me);}; //put me in play
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes:the_action, response_is_yes_no:true, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }


    public static void faramir(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard me = (PlayerCard) game_args.relevant_card; //TODO: make faramir pick a player, but for now theres only one player
        
        Action the_action = () =>
        {
            me.exhaust();
            foreach (var hero in game.get_cur_player().get_heroes())
            {
                hero.add_flag(LOTRAbility.ABILITY_FLAGS.FARAMIR);
            }
            foreach (var ally in game.get_cur_player().get_allies())
            {
                ally.add_flag(LOTRAbility.ABILITY_FLAGS.FARAMIR);
            }
        }; //put me in play
        game.execute_action(the_action);
    }
    
    public static void beravor(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard me = (PlayerCard) game_args.relevant_card; //TODO: make faramir pick a player, but for now theres only one player
        Action the_action = () => { me.exhaust(); game.get_cur_player().draw_card(); game.get_cur_player().draw_card(); }; //put me in play
        game.execute_action(the_action);
    }
    
    public static void valiant_sacrifice(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard me = (PlayerCard) game_args.relevant_card; 
        Action the_action = () =>
        {
            game.get_cur_player().draw_card(); 
            game.get_cur_player().draw_card(); //todo: change so its the card's controller
        }; //put me in play
        game.execute_action(the_action);
        //if (game_args.what_to_do_after_event_or_if_no_response != null)
        //{
        //    game_args.what_to_do_after_event_or_if_no_response();
       // }
    }
    
    public static void grim_resolve(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard me = (PlayerCard) game_args.relevant_card; //TODO: make faramir pick a player, but for now theres only one player
        LOTRPlayer the_player = game_args.relevant_player;
        Action the_action = () => { the_player.ready_all_exhausted_cards(); }; //TODO: technically just charcter but whatever
        game.execute_action(the_action);
    }
    public static void shadow_card_test(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Debug.Log("SOMG SHADOW CARD FOR " + game_args.relevant_card.get_name());
        if (game_args.what_to_do_after_event_or_if_no_response != null)
        {
            game_args.what_to_do_after_event_or_if_no_response();
        }
    }
    
    public static void for_gondor(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard me = (PlayerCard) game_args.relevant_card; //TODO: make faramir pick a player, but for now theres only one player
        LOTRPlayer the_player = game_args.relevant_player;
        Action the_action = () =>
        {
            foreach (var hero in game.get_cur_player().get_heroes())
            {
                hero.add_flag(LOTRAbility.ABILITY_FLAGS.FOR_GONDOR);
            }
            foreach (var ally in game.get_cur_player().get_allies())
            {
                ally.add_flag(LOTRAbility.ABILITY_FLAGS.FOR_GONDOR);
            }
        }; //TODO: technically just charcter but whatever
        game.execute_action(the_action);
    }
    
    public static void common_cause(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        PlayerCard the_card = (PlayerCard) game_args.relevant_card;
        Action<Card> the_second_action = (ally_chosen) =>
        {
            ((PlayerCard) ally_chosen).ready();
        };
        Action<Card> the_action = (hero_chosen) =>
        {
            Debug.Log("WAIT A SECOND DO THIS SHIT");
            List<Card> valid_targets = new List<Card>();
            game.reset_valid_targets();
            foreach (Card hero in game.get_cur_player().get_heroes())
            {
                if (((PlayerCard) hero).is_exhausted() && hero != hero_chosen)
                {
                    valid_targets.Add(hero);
                }
            }
            ((PlayerCard) hero_chosen).exhaust();
            game.set_valid_response_target(valid_targets);
            Debug.Log("WHY ARE WE NOT DOING IT AGAIN");
            game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
                after_responding_with_card: the_second_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response, times_to_respond: 2);
        };
        
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
            after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response, times_to_respond: 2);
    }
    public static void ever_vigilant(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        PlayerCard the_card = (PlayerCard) game_args.relevant_card;
        Action<Card> the_action = (ally_chosen) => { ((PlayerCard) ally_chosen).ready(); };
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
            after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    public static void sneak_attack(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        PlayerCard the_card = (PlayerCard) game_args.relevant_card;
        LOTRPlayer the_player = game_args.relevant_player;
        Action<Card> the_action = (ally_chosen) =>
        {
            //put ally card in play. flag it with the phase.
            PlayerCard chosen_card = (PlayerCard) ally_chosen;
            chosen_card.add_flag(LOTRAbility.ABILITY_FLAGS.SNEAK_ATTACK);
            the_player.play_card(chosen_card);
        };
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
            after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }

    public static void pay_for_card(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard the_card = (PlayerCard) game_args.relevant_card;
        game.played_action_from_hand(card: the_card, needs_response: true);
    }
    
    public static void steward_of_gondor_enters(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        AttachmentCard the_card = (AttachmentCard) game_args.relevant_card;
        Action the_action = () =>
        {
            foreach (LOTRPlayer player in game.get_players())
            {
                foreach (PlayerCard character in player.all_characters())
                {
                    if (character.has_attachment(the_card))
                    {
                        character.add_trait(LOTRGame.TRAITS.GONDOR);
                    }
                }
            }
        };
        game.execute_action(the_action);
    }
    public static void celebrians_stone_enters(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        AttachmentCard the_card = (AttachmentCard) game_args.relevant_card;
        Action the_action = () =>
        {
            foreach (LOTRPlayer player in game.get_players())
            {
                foreach (PlayerCard character in player.all_characters())
                {
                    if (character.has_attachment(the_card))
                    {
                        character.add_flag(LOTRAbility.ABILITY_FLAGS.CELEBRIANS_STONE);
                    }
                }
            }
        };
        game.execute_action(the_action);
    }
    public static void steward_of_gondor_action(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        AttachmentCard the_card = (AttachmentCard) game_args.relevant_card;
        
        Action the_action = () =>
        {
            foreach (LOTRPlayer player in game.get_players())
            {
                foreach (LOTRHero character in player.get_heroes())
                {
                    if (character.has_attachment(the_card))
                    {
                        the_card.exhaust();
                        character.add_resource_token(2);
                        break;
                    }
                }
            }
        };
        game.execute_action(the_action);
    }
    
    public static void gandalf_played(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard the_card = (PlayerCard) game_args.relevant_card;
        Action the_action = () =>
        {
            game.show_options(3);
            game.wait_for_forced_response(card_to_respond_to: the_card, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
        };
        game.execute_action(the_action);
    }
    
    public static void gandalf_draw_3(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        LOTRPlayer player = game_args.relevant_player;
        Action the_action = () =>
        {
            player.draw_card();
            player.draw_card();
            player.draw_card();
        };
        game.execute_action(the_action);
    }
    public static void gandalf_deal_4_damage(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        PlayerCard the_card = (PlayerCard) game_args.relevant_card;
        Debug.Log("DAMAGE 4");
        List<Card> valid_targets = new List<Card>();
        game.reset_valid_targets();
        foreach (EnemyCard enemy in game.get_staged_enemies())
        {
            if (enemy.is_enemy_card())
            {
                valid_targets.Add(enemy);
            }
        }

        foreach (LOTRPlayer player in game.get_players())
        {
            foreach (EnemyCard enemy in player.get_engaged_enemies())
            {
                if (enemy.is_enemy_card())
                {
                    valid_targets.Add(enemy);
                }
            }
        }
        game.set_valid_response_target(valid_targets);
        Action<Card> the_action = (Card enemy_chosen) =>
        {
            EnemyCard enemy = (EnemyCard) enemy_chosen;
            enemy.take_damage(4);
        };
        if (valid_targets.Count > 0)
        {
            game.wait_for_forced_response(card_to_respond_to: the_card,
                after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
        }

    }
    public static void gandalf_reduce_threat(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        LOTRPlayer player = game_args.relevant_player;
        Action the_action = () =>
        {
            player.increase_threat(-5);
        };
        game.execute_action(the_action);
    }
    
}
