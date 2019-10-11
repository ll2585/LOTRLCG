
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyCardResponses
{
    private static LOTRGame game;
    private static PlayerCardResponses all_responses;

    public static void set_game(LOTRGame g)
    {
        game = g;
    }
    public static void eyes_of_the_forest(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Action the_action = () =>
        {
            foreach (var player in game.get_players())
            {
                player.discard_all_cards(type: "EVENT");
            }
        };
        game.execute_action(the_action);
    }
    
    public static void necromancers_reach(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Action the_action = () =>
        {
            foreach (var character in game.get_cur_player().get_allies())
            {
                if (character.is_exhausted())
                {
                    character.take_damage(1);
                }
            }
            foreach (var character in game.get_cur_player().get_heroes())
            {
                if (character.is_exhausted())
                {
                    character.take_damage(1);
                }
            }
        };
        game.execute_action(the_action);
    }
    
    public static void necromancers_pass(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Action the_action = () =>
        {
            LOTRPlayer the_player = game_args.relevant_player;
            Random rnd = new Random();
            the_player.discard_card_at_index(rnd.Next(the_player.get_cards_in_hand().Count));
            the_player.discard_card_at_index(rnd.Next(the_player.get_cards_in_hand().Count));
        };
        game.execute_action(the_action);
        game_args.what_to_do_after_event_or_if_no_response();
    }
    
    public static void great_forest_web(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (Card c) =>
        {
            PlayerCard card_exhusted = (PlayerCard) c;
            card_exhusted.exhaust();
        };
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to, 
            after_responding_with_card: the_action, 
            what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void mountains_of_mirkwood(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Action the_action = () => { game.add_new_encounter_card_to_staging(); };
        game.execute_action(the_action);
        game_args.what_to_do_after_event_or_if_no_response();
    }
    
    public static void ungoliants_spawn(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Action the_action = () =>
        {
            foreach (var hero in game.get_cur_player().get_heroes())
            {
                if (hero.is_committed())
                {
                    hero.add_flag(LOTRAbility.ABILITY_FLAGS.UNGOLIANTS_SPAWN);
                }
                
            }
            foreach (var ally in game.get_cur_player().get_allies())
            {
                if (ally.is_committed())
                {
                    ally.add_flag(LOTRAbility.ABILITY_FLAGS.UNGOLIANTS_SPAWN);
                }
                
            }
        };
        game.execute_action(the_action);
        //game_args.what_to_do_after_event_or_if_no_response();
    }
    
    public static void ungoliants_spawn_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        bool? attack_undefended = game_args.attack_undefended;
        Action the_action = () =>
        {
            int threat_to_raise = 4;
            if (attack_undefended!= null && attack_undefended.Value)
            {
                threat_to_raise = 8;
            }
            game.get_cur_player().increase_threat(threat_to_raise);
        };
        game.execute_action(the_action);
        //game_args.what_to_do_after_event_or_if_no_response();
    }

    public static void hummerhorns_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        bool? attack_undefended = game_args.attack_undefended;
        Action the_action = () =>
        {
            int damage_to_deal = 1;
            if (attack_undefended!= null && attack_undefended.Value)
            {
                damage_to_deal = 2;
            }

            foreach (PlayerCard ally in game.get_cur_player().get_allies())
            {
                game.card_takes_damage(ally, damage_to_deal);
            }
            foreach (LOTRHero hero in game.get_cur_player().get_heroes())
            {
                game.card_takes_damage(hero, damage_to_deal);
            }
        };
        game.execute_action(the_action);
        //game_args.what_to_do_after_event_or_if_no_response();
    }
    public static void king_spider(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (card_chosen) =>
        {
            ((PlayerCard) card_chosen).exhaust();
        };
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
            after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void king_spider_shadow(EventArgs args)
    {
        //Shadow: Defending player must choose and exhaust 1 character he controls. (2 characters instead if this attack is undefended.)
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        bool? attack_undefended = game_args.attack_undefended;
        Action the_action = () =>
        {
            int characters_to_exhaust = 1;
            if (attack_undefended!= null && attack_undefended.Value)
            {
                characters_to_exhaust = 2;
            }
            List<Card> valid_targets_first_response = new List<Card>();
            game.reset_valid_targets();
            foreach (Card hero in game.get_cur_player().get_heroes())
            {
                if (!((PlayerCard) hero).is_exhausted())
                {
                    valid_targets_first_response.Add(hero);
                }
            }
            foreach (Card ally in game.get_cur_player().get_allies())
            {
                if (!((PlayerCard) ally).is_exhausted())
                {
                    valid_targets_first_response.Add(ally);
                }
            }
            
            Action<Card> exhaust_character = (character_chosen) =>
            {
                ((PlayerCard) character_chosen).exhaust();
            };
            Action<Card> the_first_response = (character_chosen) =>
            {
                ((PlayerCard) character_chosen).exhaust();
                List<Card> valid_targets = new List<Card>();
                game.reset_valid_targets();
                foreach (Card hero in game.get_cur_player().get_heroes())
                {
                    if (!((PlayerCard) hero).is_exhausted())
                    {
                        valid_targets.Add(hero);
                    }
                }
                foreach (Card ally in game.get_cur_player().get_allies())
                {
                    if (!((PlayerCard) ally).is_exhausted())
                    {
                        valid_targets.Add(ally);
                    }
                }
                
                game.set_valid_response_target(valid_targets);
                game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
                    after_responding_with_card: exhaust_character, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response, times_to_respond: characters_to_exhaust);
            };
            Debug.Log(valid_targets_first_response.Count);
            Debug.Log(characters_to_exhaust);
            if (valid_targets_first_response.Count > 0)
            {
                game.reset_valid_targets();
                game.set_valid_response_target(valid_targets_first_response);
                if (valid_targets_first_response.Count == 1 || characters_to_exhaust == 1)
                {
                    Debug.Log("Exhaust 1");
                    
                    game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
                        after_responding_with_card: exhaust_character, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response, times_to_respond: 1);
                }
                else if(valid_targets_first_response.Count > 1 && characters_to_exhaust == 2)
                {
                    Debug.Log("Exhaust 2");
                    game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
                        after_responding_with_card: the_first_response, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response, times_to_respond: characters_to_exhaust);
                }
            }
        };
        game.execute_action(the_action);
        //game_args.what_to_do_after_event_or_if_no_response();
    }
    public static void dol_guldur_orcs(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (card_chosen) =>
        {
            ((PlayerCard) card_chosen).take_damage(2);
        };
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
            after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    public static void driven_by_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action the_action = () =>
        {
            if (game.get_staged_enemies().Count == 0)
            {
                game.add_new_encounter_card_to_staging();
            }
            else
            {
                foreach (EnemyCard staged_enemy in game.get_staged_enemies())
                {
                    staged_enemy.add_flag(LOTRAbility.ABILITY_FLAGS.PLUS_ONE_THREAT);
                }
            }
            
        };
        game.execute_action(the_action);
    }
    public static void driven_by_shadow_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        bool? attack_undefended = game_args.attack_undefended;
        Action the_action = () =>
        {
            if (attack_undefended!= null && attack_undefended.Value)
            {
                game.cur_player_discard_attachment(all:true);
            }
            else
            {
                List<Card> valid_targets = new List<Card>();
                game.reset_valid_targets();
                foreach (Card card in game.get_cur_player().all_characters())
                {
                    PlayerCard character = (PlayerCard) card;
                    if (character.get_attachments().Count > 0)
                    {
                        valid_targets.AddRange(character.get_attachments());
                    }
                }

                Action<Card> discard_attachment = (attachment_chosen) =>
                {
                    game.cur_player_discard_attachment(attachment:(AttachmentCard) attachment_chosen);
                };
                if (valid_targets.Count > 0)
                {
                    game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
                        after_responding_with_card: discard_attachment,
                        what_to_do_after_responding: game_args.what_to_do_after_event_or_if_no_response);
                }
            }
        };
        game.execute_action(the_action);
    }
    public static void dol_guldur_orcs_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        bool? attack_undefended = game_args.attack_undefended;
        EnemyCard the_enemy = (EnemyCard) game_args.secondary_card;
        Action the_action = () =>
        {
            LOTRAbility.ABILITY_FLAGS flag = LOTRAbility.ABILITY_FLAGS.PLUS_ONE_ATTACK_ENEMY;
            if (attack_undefended!= null && attack_undefended.Value)
            {
                flag = LOTRAbility.ABILITY_FLAGS.PLUS_THREE_ATTACK_ENEMY;
            }

            the_enemy.add_flag(flag);
        };
        game.execute_action(the_action);
        //game_args.what_to_do_after_event_or_if_no_response();
    }
    public static void east_bight_patrol_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        bool? attack_undefended = game_args.attack_undefended;
        EnemyCard the_enemy = (EnemyCard) game_args.secondary_card;
        Action the_action = () =>
        {
            LOTRAbility.ABILITY_FLAGS flag = LOTRAbility.ABILITY_FLAGS.PLUS_ONE_ATTACK_ENEMY;
            if (attack_undefended!= null && attack_undefended.Value)
            {
                game.get_cur_player().increase_threat(3);
            }

            the_enemy.add_flag(flag);
        };
        game.execute_action(the_action);
        //game_args.what_to_do_after_event_or_if_no_response();
    }
    public static void black_forest_bats(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (card_chosen) =>
        {
            ((PlayerCard) card_chosen).uncommit();
        };
        game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
            after_responding_with_card: the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    public static void forest_spider_shadow(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action the_action = () =>
        {
            List<Card> valid_targets = new List<Card>();
            game.reset_valid_targets();
            foreach (Card card in game.get_cur_player().all_characters())
            {
                PlayerCard character = (PlayerCard) card;
                if (character.get_attachments().Count > 0)
                {
                    valid_targets.AddRange(character.get_attachments());
                }
            }

            Action<Card> discard_attachment = (attachment_chosen) =>
            {
                game.cur_player_discard_attachment(attachment:(AttachmentCard) attachment_chosen);
            };
            if (valid_targets.Count > 0)
            {
                game.wait_for_forced_response(card_to_respond_to: card_to_respond_to,
                    after_responding_with_card: discard_attachment,
                    what_to_do_after_responding: game_args.what_to_do_after_event_or_if_no_response);
            }
        };
        game.execute_action(the_action);
    }
    public static void chieftan_ufthak_forced(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        EnemyCard me = (EnemyCard) game_args.relevant_card;
        Action the_action = () =>
        {
            me.add_resource_tokens();
        };
        game.execute_action(the_action);
    }
    public static void forest_spider_forced(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        EnemyCard me = (EnemyCard) game_args.relevant_card;
        Action the_action = () =>
        {
            me.add_flag(LOTRAbility.ABILITY_FLAGS.FOREST_SPIDER);
        };
        game.execute_action(the_action);
    }
    
    public static void old_forest_road(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action<Card> the_action = (Card character) => { ((PlayerCard) character).ready();};
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes_with_card:the_action, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    
    public static void forest_gate(EventArgs args)
    {
        GameArgs game_args = (GameArgs) args;
        Card card_to_respond_to = game_args.relevant_card;
        Action the_action = () => { game.get_cur_player().draw_card(); game.get_cur_player().draw_card(); }; //put me in play
        game.wait_for_response(card_to_respond_to: card_to_respond_to,
            if_yes:the_action, response_is_yes_no:true, what_to_do_after_responding:game_args.what_to_do_after_event_or_if_no_response);
    }
    

}
