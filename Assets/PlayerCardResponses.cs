
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

    public static void theodred(GameArgs args)
    {
        Card character_committed = args.relevant_card;
        LOTRHero theodred = (LOTRHero) character_committed;
        Action<Card> theodreds_action = (Card hero_chosen) => { ((LOTRHero) hero_chosen).add_resource_token(); };
        if (theodred.get_name() == LOTRHero.THEODRED().get_name())
        {
            Debug.Log("THEODRED WAS COMMITTED");
            game.set_valid_response_target(game.get_cur_player().get_heroes().Cast<Card>().ToList());
            game.wait_for_response(if_yes_with_card: theodreds_action, what_to_do_after_responding:args.what_to_do_after_event_or_if_no_response);
        }else
        {
            if (args.what_to_do_after_event_or_if_no_response != null)
            {
                args.what_to_do_after_event_or_if_no_response();
            }
        }
    }
    
    public static void gloin(GameArgs args)
    {
        Card character_damaged = args.relevant_card;
        int? amt_damaged = args.relevant_int;
        LOTRHero gloin = (LOTRHero) character_damaged;
        Action gloins_action = () => { gloin.add_resource_token(amt_damaged.Value); };
        if (gloin.get_name() == LOTRHero.GLOIN().get_name())
        {
            Debug.Log("GLOIN WAS DAMAGED");
            game.wait_for_response(if_yes: gloins_action, response_is_yes_no:true, what_to_do_after_responding:args.what_to_do_after_event_or_if_no_response);
        }else
        {
            if (args.what_to_do_after_event_or_if_no_response != null)
            {
                args.what_to_do_after_event_or_if_no_response();
            }
        }
    }
    
    public static void aragorn(GameArgs args)
    {
        Card character_committed = args.relevant_card;
        LOTRHero aragorn = (LOTRHero) character_committed;
        Action aragorns_action = () => { aragorn.pay_resource(); aragorn.unexhaust();};
        if (aragorn.get_name() == LOTRHero.ARAGORN().get_name() && aragorn.get_resources() > 0)
        {
            Debug.Log("ARAGORN WAS COMMITTED");
           game.wait_for_response(if_yes:aragorns_action, response_is_yes_no:true, what_to_do_after_responding:args.what_to_do_after_event_or_if_no_response);
        }else
        {
            if (args.what_to_do_after_event_or_if_no_response != null)
            {
                args.what_to_do_after_event_or_if_no_response();
            }
            
        }
    }

    public static void son_of_arnor(GameArgs args)
    {
        Card character_entered = args.relevant_card;
        PlayerCard the_card = (PlayerCard) character_entered;
        Action<Card> the_action = (Card enemy) => { game.player_engage_enemy(game.get_cur_player(), (EnemyCard) enemy);};
        List<Card> valid_targets = new List<Card>();
        foreach (var card in game.get_staged_enemies())
        {
            if (card.is_enemy_card()) //TODO: or engaged with another player
            {
                valid_targets.Add(card);
            }
        }
        
        if (the_card.get_name() == PlayerCard.SON_OF_ARNOR().get_name() && valid_targets.Count > 0) //TODO: check for location
        {
            game.set_valid_response_target(valid_targets);
            game.wait_for_response(if_yes_with_card:the_action, what_to_do_after_responding:args.what_to_do_after_event_or_if_no_response);
        }
        else
        {
            if (args.what_to_do_after_event_or_if_no_response != null)
            {
                args.what_to_do_after_event_or_if_no_response();
            }
        }
    }
    
    public static void snowbourn_scout(GameArgs args)
    {
        Card character_entered = args.relevant_card;
        PlayerCard snowborn_scout = (PlayerCard) character_entered;
        Action<Card> snowborn_scouts_action = (Card location) => { ((LocationCard) location).add_progress_token();};
        List<Card> valid_targets = new List<Card>();
        if (game.get_cur_location() != null)
        {
            valid_targets.Add(game.get_cur_location());
        }
        
        foreach (var card in game.get_staged_enemies())
        {
            if (card.is_location_card())
            {
                valid_targets.Add(card);
            }
        }
        
        if (snowborn_scout.get_name() == PlayerCard.SNOWBOURN_SCOUT().get_name() && valid_targets.Count > 0) //TODO: check for location
        {
            game.set_valid_response_target(valid_targets);
            game.wait_for_response(if_yes_with_card:snowborn_scouts_action, what_to_do_after_responding:args.what_to_do_after_event_or_if_no_response);
        }
        else
        {
            if (args.what_to_do_after_event_or_if_no_response != null)
            {
                args.what_to_do_after_event_or_if_no_response();
            }
        }

    }
    
    public static void longbeard_orc_slayer(GameArgs args)
    {
        Card character_entered = args.relevant_card;
        PlayerCard the_card = (PlayerCard) character_entered;
        Action<Card> the_action = (Card enemy) => { game.deal_damage_to_enemy((EnemyCard) enemy, 1);};
        List<Card> valid_targets = new List<Card>();
        foreach (var card in game.get_staged_enemies())
        {
            if (card.has_trait(LOTRGame.TRAITS.ORC)) //TODO: or engaged with another player
            {
                valid_targets.Add(card);
            }
        }
        foreach (var card in game.get_cur_player().get_engaged_enemies())
        {
            if (card.has_trait(LOTRGame.TRAITS.ORC)) //TODO: or engaged with another player
            {
                valid_targets.Add(card);
            }
        }
        
        if (the_card.get_name() == PlayerCard.LONGBEARD_ORC_SLAYER().get_name() && valid_targets.Count > 0) //TODO: check for location
        {
            game.set_valid_response_target(valid_targets);
            game.wait_for_response(if_yes_with_card:the_action, what_to_do_after_responding:args.what_to_do_after_event_or_if_no_response);
        }
        else
        {
            if (args.what_to_do_after_event_or_if_no_response != null)
            {
                args.what_to_do_after_event_or_if_no_response();
            }
        }
    }
    
    public static void brok_ironfist(GameArgs args)
    {
        Card card_leaving_play = args.relevant_card;
        Debug.Log("BROKK");
    }
        
    
    
}
