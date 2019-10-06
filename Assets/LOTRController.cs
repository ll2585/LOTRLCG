using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LOTRController : MonoBehaviour
{

    public LOTRGame game;
    public Text player_1_resources;
    public Text prompt_text;
    public Text cur_phase_text;
    public Text cur_state_text;
    public Text threat_text;
    public Text quest_progress_text;
    public Text location_progress_text;
    public Text willpower_committed_text;
    public Button guard;
    public Button tracker;
    public Button end_plan_phase_button;
    public Button finished_committing_button;
    public Button finished_engaging_enemies_button;
    public Button let_attack_go_undefended_button;
    public Button done_attacking_button;
    public Button done_choosing_attackers_button;
    public Button do_not_travel_button; //TODO: maybe just make one button
    public Button no_response_button; //TODO: maybe just make one button
    public Button yes_response_button; //TODO: maybe just make one button

    public GameObject card_prefab;
    
    public Dictionary<GameObject, PlayerCard> mapping_of_obj_to_player_card;
    public Dictionary<PlayerCard, GameObject> mapping_of_player_card_to_obj;

    public Dictionary<GameObject, EnemyCard> mapping_of_obj_to_enemies_staging;
    public Dictionary<EnemyCard, GameObject> mapping_of_enemies_to_obj_staging;
    public Dictionary<GameObject, EnemyCard> mapping_of_obj_to_enemies_engaged;
    public Dictionary<EnemyCard, GameObject> mapping_of_enemies_to_obj_engaged;

    public Dictionary<GameObject, LocationCard> mapping_of_obj_to_location_card;
    public Dictionary<LocationCard, GameObject> mapping_of_location_card_to_obj;

    
    public Dictionary<GameObject, PlayerCard> mapping_of_obj_to_cards_in_hand;
    public Dictionary<PlayerCard, GameObject> mapping_of_cards_in_hand_to_obj;

    private List<PlayerCard> guards;

    private Dictionary<QuestCard, GameObject> mapping_of_cur_quest_to_obj;
    private GameObject cur_quest_obj;

    // Start is called before the first frame update
    void Start()
    {
        game = new LOTRGame();
        cur_quest_obj = null;
        mapping_of_obj_to_player_card = new Dictionary<GameObject, PlayerCard>();
        mapping_of_player_card_to_obj = new Dictionary<PlayerCard, GameObject>();
        mapping_of_obj_to_enemies_staging = new Dictionary<GameObject, EnemyCard>();
        mapping_of_enemies_to_obj_staging = new Dictionary<EnemyCard, GameObject>();
        
        mapping_of_obj_to_enemies_engaged = new Dictionary<GameObject, EnemyCard>();
        mapping_of_enemies_to_obj_engaged = new Dictionary<EnemyCard, GameObject>();

        
        mapping_of_obj_to_location_card = new Dictionary<GameObject, LocationCard>();
        mapping_of_location_card_to_obj = new Dictionary<LocationCard, GameObject>();
        
        mapping_of_cur_quest_to_obj = new Dictionary<QuestCard, GameObject>();
        
        mapping_of_obj_to_cards_in_hand = new Dictionary<GameObject, PlayerCard>();
        mapping_of_cards_in_hand_to_obj = new Dictionary<PlayerCard, GameObject>();

        game.initialize_game();
        debug();
        //guard.onClick.AddListener(() => { play_card(get_guard());});
        //tracker.onClick.AddListener(() => { play_card(PlayerCard.SON_OF_ARNOR()); });
        end_plan_phase_button.onClick.AddListener(end_plan_phase_button_clicked);
        finished_committing_button.onClick.AddListener(finished_committing_button_clicked);
        finished_engaging_enemies_button.onClick.AddListener(finished_engaging_enemies_button_clicked);
        let_attack_go_undefended_button.onClick.AddListener(let_attack_go_undefended_button_clicked);
        done_attacking_button.onClick.AddListener(done_attacking_button_clicked);
        done_choosing_attackers_button.onClick.AddListener(done_choosing_attackers_button_clicked);
        do_not_travel_button.onClick.AddListener(do_not_travel_button_clicked);
        no_response_button.onClick.AddListener(no_response_button_clicked);
        yes_response_button.onClick.AddListener(yes_response_button_clicked);
    }
    
    void debug()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        threat_text.text = "THREAT\n" + game.get_player_threat(game.get_cur_player());
        quest_progress_text.text = "QUEST: " + game.get_cur_quest_progress() + "/" + game.get_cur_quest_amount_needed();
        location_progress_text.text = "LOCATION: " + game.get_cur_location_progress() + "/" + game.get_cur_location_amount_needed();
        willpower_committed_text.text = "Commited Willpower: " + game.get_willpower_committed() + "\nEnemy Threat: " + game.get_enemy_threat();
        cur_phase_text.text = game.get_cur_phase().ToString();
        cur_state_text.text = game.get_cur_state().ToString();
        player_1_resources.text = get_resources_in_readable_format();
        guard.gameObject.SetActive(game.get_cur_phase() == LOTRGame.GAMEPHASE.PLANNING);
        tracker.gameObject.SetActive(game.get_cur_phase() == LOTRGame.GAMEPHASE.PLANNING);
        end_plan_phase_button.gameObject.SetActive(game.get_cur_phase() == LOTRGame.GAMEPHASE.PLANNING);
        finished_committing_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.COMMITTING_CHARACTERS);
        finished_engaging_enemies_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.PLAYER_ENGAGEMENT);
        let_attack_go_undefended_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.DECLARING_DEFENDER);
        done_attacking_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.DECLARING_ENEMY_TO_ATTACK);
        done_choosing_attackers_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.DECLARING_ATTACKERS);
        do_not_travel_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.CHOOSING_TRAVEL);
        no_response_button.gameObject.SetActive(game.get_cur_state() == LOTRGame.GAMESTATE.WAITING_FOR_RESPONSE);
        yes_response_button.gameObject.SetActive(game.is_response_yes_no());
        
        update_prompt(game.get_cur_state());
        update_player_hand();
        update_player_cards(); //includes allies and attachments
        update_enemy_display("staging");
        update_enemy_display("engaged");
        update_location_card();
        update_quest_card();
        get_mouse_click();
    }

    void update_prompt(LOTRGame.GAMESTATE state)
    {
        if (state == LOTRGame.GAMESTATE.PLAYING_CARDS)
        {
            prompt_text.text = "Play your cards.";
        }else if (state == LOTRGame.GAMESTATE.CHOOSING_CHARACTER_TO_ATTACH_ATTACHMENT)
        {
            prompt_text.text = "Select a character to attach.";
        }else if (state == LOTRGame.GAMESTATE.PLAYER_ENGAGEMENT)
        {
            prompt_text.text = "Please choose an enemy to engage.";
        }else if (state == LOTRGame.GAMESTATE.COMMITTING_CHARACTERS)
        {
            prompt_text.text = "Commit characters to the quest.";
        } else if (state == LOTRGame.GAMESTATE.CHOOSING_ENEMY_TO_RESOLVE_ENEMY_ATTACK)
        {
            prompt_text.text = "Please choose an enemy to resolve its attack.";
        } else if (state == LOTRGame.GAMESTATE.DECLARING_DEFENDER)
        {
            prompt_text.text = "Please choose a defender.";
        } else if (state == LOTRGame.GAMESTATE.ASSIGNING_UNDEFENDED_DAMAGE)
        {
            prompt_text.text = "Assign the undefended damage to a hero.";
        }else if (state == LOTRGame.GAMESTATE.DECLARING_ENEMY_TO_ATTACK)
        {
            prompt_text.text = "Choose an enemy to attack.";
        }else if (state == LOTRGame.GAMESTATE.DECLARING_ATTACKERS)
        {
            prompt_text.text = "Choose your attackers.";
        }else if (state == LOTRGame.GAMESTATE.CHOOSING_TRAVEL)
        {
            prompt_text.text = "Choose a location to travel to.";
        }else if (state == LOTRGame.GAMESTATE.WAITING_FOR_RESPONSE)
        {
            prompt_text.text = "Do you want to respond?";
        }
    }

    void update_player_cards()
    {
        List<LOTRHero> my_heroes = game.get_cur_player().get_heroes(); //do heroes first
        List<PlayerCard> the_allies = game.get_cur_player().get_allies();
        
        for (int i = 0; i < my_heroes.Count; i++)
        {
            LOTRHero hero = my_heroes[i];
            if (!mapping_of_player_card_to_obj.ContainsKey(hero))
            {
                GameObject hero_obj = create_new_player_card(hero.get_name(),index:i, type: hero.get_type(), destination:"hero");
                mapping_of_player_card_to_obj[hero] = hero_obj;
                mapping_of_obj_to_player_card[hero_obj] = hero;
            }

            update_attachment_mappings(hero.get_attachments(), mapping_of_player_card_to_obj[hero]);
        }
        
        for (int i = 0; i < the_allies.Count; i++)
        {
            PlayerCard ally = the_allies[i];
            if (!mapping_of_player_card_to_obj.ContainsKey(ally))
            {
                GameObject ally_obj = create_new_player_card(ally.get_name(), index: i, type: ally.get_type(), destination:"ally");
                mapping_of_player_card_to_obj[ally] = ally_obj;
                mapping_of_obj_to_player_card[ally_obj] = ally;
            }
            update_attachment_mappings(ally.get_attachments(), mapping_of_player_card_to_obj[ally]);
        }

        update_exhausted_cards();
        remove_dead_cards();

    }
    
    void update_attachment_mappings(List<AttachmentCard> attachment_cards, GameObject owner)
    {

        for (int i = 0; i < attachment_cards.Count; i++)
        {
            AttachmentCard attachment_card = attachment_cards[i];
            if (!mapping_of_player_card_to_obj.ContainsKey(attachment_card))
            {
                GameObject attachment_obj = create_attachment_card_for_character(attachment_card.get_name(),index:i,character:owner );
                mapping_of_player_card_to_obj[attachment_card] = attachment_obj;
                mapping_of_obj_to_player_card[attachment_obj] = attachment_card;
            }
        }
    }
    
    void update_player_hand()
    {
        List<PlayerCard> cards_in_hand = game.get_cur_player().get_cards_in_hand();
        
        for (int i = 0; i < cards_in_hand.Count; i++)
        {
            PlayerCard card_in_hand = cards_in_hand[i];
            if (!mapping_of_cards_in_hand_to_obj.ContainsKey(card_in_hand))
            {
                GameObject card_in_hand_obj = create_new_player_card(card_in_hand.get_name(), index:i, type: card_in_hand.get_type(), destination: "hand");
                mapping_of_cards_in_hand_to_obj[card_in_hand] = card_in_hand_obj;
                mapping_of_obj_to_cards_in_hand[card_in_hand_obj] = card_in_hand;
            }
        }

    }

    void update_exhausted_cards()
    {
        
        foreach(KeyValuePair<GameObject, PlayerCard> entry in mapping_of_obj_to_player_card)
        {
            GameObject the_object = entry.Key;
            PlayerCard the_card = entry.Value;

            if (the_card.is_exhausted())
            {
                if (the_object.transform.eulerAngles == Vector3.zero)
                {
                    the_object.transform.Rotate (Vector3.forward * -90);
                }
            }
            else
            {
                if (the_object.transform.eulerAngles != Vector3.zero)
                {
                    the_object.transform.eulerAngles = Vector3.zero;
                }
            }
        }
    }

    void remove_dead_cards()
    {
        foreach(KeyValuePair<GameObject, PlayerCard> entry in mapping_of_obj_to_player_card)
        {
            GameObject the_object = entry.Key;
            PlayerCard the_card = entry.Value;

            if (the_card.is_dead())
            {
                Destroy(the_object);
                mapping_of_obj_to_player_card.Remove(the_object);
                mapping_of_player_card_to_obj.Remove(the_card);
            }
        }
    }
    
    void update_enemy_display(string which_enemy)
    {
        Dictionary<EnemyCard, GameObject> mapping_of_enemies_to_obj = null;
        Dictionary<GameObject, EnemyCard> mapping_of_obj_to_enemies = null;
        List<EnemyCard> enemy_list = null;
        float x = 0F;
        float y = 0F;
        float z = .001F;
        switch (which_enemy)
        {
            case("staging"):
                mapping_of_enemies_to_obj = mapping_of_enemies_to_obj_staging;
                mapping_of_obj_to_enemies = mapping_of_obj_to_enemies_staging;
                enemy_list = game.get_staged_enemies();
                break;
            case("engaged"):
                mapping_of_enemies_to_obj = mapping_of_enemies_to_obj_engaged;
                mapping_of_obj_to_enemies = mapping_of_obj_to_enemies_engaged;
                enemy_list = game.get_engaged_enemies(game.get_cur_player());
                break;
            
        }

        if (mapping_of_enemies_to_obj == null)
        {
            throw new Exception("WHAT THE FUCK");
        }
        
        for (int i = 0; i < enemy_list.Count; i++)
        {
            EnemyCard the_enemy = enemy_list[i];
            if (!mapping_of_enemies_to_obj.ContainsKey(the_enemy))
            {
                GameObject enemy_obj = create_new_enemy_card(the_enemy.get_name(), destination: which_enemy, card_type: the_enemy.get_type(), index:i);
                mapping_of_enemies_to_obj[the_enemy] = enemy_obj;
                mapping_of_obj_to_enemies[enemy_obj] = the_enemy;
            }
        }
        
        List<GameObject> objs_to_destroy = new List<GameObject>();
        foreach(KeyValuePair<GameObject, EnemyCard> entry in mapping_of_obj_to_enemies)
        {
            GameObject the_object = entry.Key;
            EnemyCard the_enemy = entry.Value;

            if (!enemy_list.Contains(the_enemy))
            {
                objs_to_destroy.Add(the_object);
                mapping_of_obj_to_enemies.Remove(the_object);
                Destroy(the_object);
                
            }
        }

    }
    
    void update_location_card()
    {
        Dictionary<EnemyCard, GameObject> mapping_of_enemies_to_obj = null;
        Dictionary<GameObject, EnemyCard> mapping_of_obj_to_enemies = null;
        LocationCard cur_location = game.get_cur_location();
        float x = 6.52F;
        float y = 1.29F;
        float z = .001F;
        if (cur_location != null)
        {
            if (!mapping_of_location_card_to_obj.ContainsKey(cur_location))
            {
                GameObject location_obj = create_new_card_from_name(cur_location.get_name(), type: cur_location.get_type(), x: x, y: y, z: z);
                mapping_of_location_card_to_obj[cur_location] = location_obj;
                mapping_of_obj_to_location_card[location_obj] = cur_location;
            }
        }
        
        foreach(KeyValuePair<GameObject, LocationCard> entry in mapping_of_obj_to_location_card) //TODO: maybe dont need a dictionary
        {
            GameObject the_object = entry.Key;
            LocationCard the_location = entry.Value;

            if (the_location != cur_location)
            {
                Destroy(the_object);
                mapping_of_obj_to_location_card.Remove(the_object);
            }
        }
    }
    
    void update_quest_card()
    {
        QuestCard cur_quest = game.get_cur_quest();
        
        float x = 3.54F;
        float y = 2.46F;
        float z = .001F;
        if (cur_quest != null)
        {
            if (cur_quest_obj is null || !mapping_of_cur_quest_to_obj.ContainsKey(cur_quest))
            {
                GameObject quest_obj = create_new_card_from_name(cur_quest.get_name(), type: cur_quest.get_type(), x: x, y: y, z: z);
                cur_quest_obj = quest_obj;
                mapping_of_cur_quest_to_obj[cur_quest] = quest_obj;
            }
        }
        
        foreach(KeyValuePair<QuestCard, GameObject> entry in mapping_of_cur_quest_to_obj) //TODO: maybe dont need a dictionary
        {
            QuestCard the_card = entry.Key;
            GameObject the_object = entry.Value;

            if (the_card != cur_quest)
            {
                Destroy(the_object);
                mapping_of_cur_quest_to_obj.Remove(the_card);
            }
        }
    }
    
    void get_mouse_click()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (game.get_cur_state() == LOTRGame.GAMESTATE.WAITING_FOR_RESPONSE)
                {
                    Card this_card = null;
                    GameObject object_clicked = hit.collider.gameObject;

                    if (mapping_of_obj_to_cards_in_hand.ContainsKey(object_clicked))
                    {
                        this_card = mapping_of_obj_to_cards_in_hand[object_clicked];
                    } else if (mapping_of_obj_to_player_card.ContainsKey(object_clicked))
                    {
                        this_card = mapping_of_obj_to_player_card[object_clicked];
                    }else if (mapping_of_obj_to_enemies_engaged.ContainsKey(object_clicked))
                    {
                        this_card = mapping_of_obj_to_enemies_engaged[object_clicked];
                    }else if (mapping_of_obj_to_enemies_staging.ContainsKey(object_clicked))
                    {
                        this_card = mapping_of_obj_to_enemies_staging[object_clicked];
                    }else if (mapping_of_obj_to_location_card.ContainsKey(object_clicked))
                    {
                        this_card = mapping_of_obj_to_location_card[object_clicked];
                    }

                    if (this_card == null)
                    {
                        print("HOW IS THIS SHIT STILL NULL??!!?#@O($#&*(&*%(TUJGNIO$WHTUG");
                    }
                    else
                    {
                        print(this_card.get_name());
                        if (this_card.is_valid_response_target())
                        {
                            game.yes_responded(this_card); //ONLY TIME CALLING GAME because of hte shenanigans
                        }
                    }
                    
                }
                else if (mapping_of_obj_to_cards_in_hand.ContainsKey(hit.collider.gameObject))
                {
                    if (game.get_cur_state() == LOTRGame.GAMESTATE.PLAYING_CARDS)
                    {
                        play_card(hit.collider.gameObject);
                    }
                } else if (hit.collider.CompareTag("Hero") || hit.collider.CompareTag("Ally"))
                {
                    if (game.get_cur_state() == LOTRGame.GAMESTATE.PAYING_FOR_CARD && hit.collider.CompareTag("Hero")) //only heroes can pay resources
                    {
                        pay_resource(hit.collider.gameObject); //needs to be paying
                    } else if (game.get_cur_state() == LOTRGame.GAMESTATE.COMMITTING_CHARACTERS)
                    {
                        character_committed(hit.collider.gameObject);
                    } else if (game.get_cur_state() == LOTRGame.GAMESTATE.DECLARING_DEFENDER)
                    {
                        defender_chosen(hit.collider.gameObject);
                    } else if (game.get_cur_state() == LOTRGame.GAMESTATE.ASSIGNING_UNDEFENDED_DAMAGE && hit.collider.CompareTag("Hero")) //only to heroes
                    {
                        hero_assigned_undefended_damage(hit.collider.gameObject);
                    } else if (game.get_cur_state() == LOTRGame.GAMESTATE.DECLARING_ATTACKERS)
                    {
                        card_chosen_as_attacker(hit.collider.gameObject);
                    } else if (game.get_cur_state() ==  LOTRGame.GAMESTATE.CHOOSING_CHARACTER_TO_ATTACH_ATTACHMENT)
                    {
                        card_attached(hit.collider.gameObject);
                    }
                } else if (hit.collider.CompareTag("Enemy"))
                {
                    if (game.get_cur_state() == LOTRGame.GAMESTATE.PLAYER_ENGAGEMENT)
                    {
                        enemy_engaged(hit.collider.gameObject); //needs to be paying
                    } else if (game.get_cur_state() == LOTRGame.GAMESTATE.CHOOSING_ENEMY_TO_RESOLVE_ENEMY_ATTACK)
                    {
                        enemy_attacking(hit.collider.gameObject); //needs to be paying
                    } else if (game.get_cur_state() == LOTRGame.GAMESTATE.DECLARING_ENEMY_TO_ATTACK)
                    {
                        enemy_chosen_to_attack(hit.collider.gameObject);
                    }  
                }else if (hit.collider.CompareTag("Location"))
                {
                    if (game.get_cur_state() == LOTRGame.GAMESTATE.CHOOSING_TRAVEL)
                    {
                        location_chosen_to_travel(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        
        foreach (KeyValuePair<GameObject, PlayerCard> entry in mapping_of_obj_to_cards_in_hand)
        {
            Card the_card = entry.Value;
            GameObject the_obj = entry.Key;
            if (the_card.is_valid_response_target())
            {
                highlight_object(the_obj);
            }else
            {
                unhighlight_object(the_obj);
            }
        }
        foreach (KeyValuePair<GameObject, PlayerCard> entry in mapping_of_obj_to_player_card)
        {
            Card the_card = entry.Value;
            GameObject the_obj = entry.Key;
            if (the_card.is_valid_response_target())
            {
                highlight_object(the_obj);
            }else
            {
                unhighlight_object(the_obj);
            }
        }
        foreach (KeyValuePair<GameObject, EnemyCard> entry in mapping_of_obj_to_enemies_engaged)
        {
            Card the_card = entry.Value;
            GameObject the_obj = entry.Key;
            if (the_card.is_valid_response_target())
            {
                highlight_object(the_obj);
            }else
            {
                unhighlight_object(the_obj);
            }
        }
        foreach (KeyValuePair<GameObject, EnemyCard> entry in mapping_of_obj_to_enemies_staging)
        {
            Card the_card = entry.Value;
            GameObject the_obj = entry.Key;
            if (the_card.is_valid_response_target())
            {
                highlight_object(the_obj);
            }else
            {
                unhighlight_object(the_obj);
            }
        }
        foreach (KeyValuePair<GameObject, LocationCard> entry in mapping_of_obj_to_location_card)
        {
            Card the_card = entry.Value;
            GameObject the_obj = entry.Key;
            if (the_card.is_valid_response_target())
            {
                highlight_object(the_obj);
            }
            else
            {
                unhighlight_object(the_obj);
            }
        }
        
    }

    void highlight_object(GameObject obj)
    {
        obj.GetComponent<SpriteRenderer>().color = new Color32(255,255,153,255);
    }
    void unhighlight_object(GameObject obj)
    {
        obj.GetComponent<SpriteRenderer>().color = new Color32(255,255,225,255);
    }

    void card_attached(GameObject obj)
    {
        if (mapping_of_obj_to_player_card.ContainsKey(obj))
        {
            PlayerCard character_attaching_card = mapping_of_obj_to_player_card[obj];
            game.card_attached(game.get_cur_player(), character_attaching_card);
        }
    }

    void location_chosen_to_travel(GameObject obj)
    {
        print("CHOOSE TRAVEL ENGAGED");
        if (game.get_cur_state() != LOTRGame.GAMESTATE.CHOOSING_TRAVEL)
        {
            return;
        }

        if (mapping_of_obj_to_enemies_staging.ContainsKey(obj))
        {
            LocationCard chosen_location = (LocationCard) mapping_of_obj_to_enemies_staging[obj];
            game.location_chosen(chosen_location);
        }
    }
    
    void enemy_chosen_to_attack(GameObject obj)
    {
        print("ENEMY ENGAGED");
        if (game.get_cur_state() != LOTRGame.GAMESTATE.DECLARING_ENEMY_TO_ATTACK)
        {
            return;
        }

        if (mapping_of_obj_to_enemies_engaged.ContainsKey(obj))
        {
            EnemyCard attacked_enemy = mapping_of_obj_to_enemies_engaged[obj];
            game.player_chose_enemy_to_attack(game.get_cur_player(), attacked_enemy);
        }
    }
    
    void card_chosen_as_attacker(GameObject obj)
    {
        if (game.get_cur_state() != LOTRGame.GAMESTATE.DECLARING_ATTACKERS)
        {
            return;
        }
        if (mapping_of_obj_to_player_card.ContainsKey(obj))
        {
            PlayerCard the_attacker = mapping_of_obj_to_player_card[obj];
            game.attacker_chosen(game.get_cur_player(), the_attacker);
        }
    }

    void hero_assigned_undefended_damage(GameObject obj)
    {
        if (mapping_of_obj_to_player_card.ContainsKey(obj))
        {
            LOTRHero the_hero = (LOTRHero) mapping_of_obj_to_player_card[obj];
            game.hero_chosen_to_take_undefended_damage(game.get_cur_player(), the_hero);
        }
    }
    void defender_chosen(GameObject obj)
    {
        if (mapping_of_obj_to_player_card.ContainsKey(obj))
        {
            PlayerCard the_defender = mapping_of_obj_to_player_card[obj];
            game.player_chose_defender(game.get_cur_player(), the_defender);
        }
    }

    void let_attack_go_undefended_button_clicked()
    {
        game.player_chose_defender(game.get_cur_player(), null);
    }

    void enemy_engaged(GameObject obj)
    {
        print("ENEMY ENGAGED");
        if (game.get_cur_state() != LOTRGame.GAMESTATE.PLAYER_ENGAGEMENT)
        {
            return;
        }

        if (mapping_of_obj_to_enemies_staging.ContainsKey(obj))
        {
            EnemyCard engaged_enemy = mapping_of_obj_to_enemies_staging[obj];
            game.player_engage_enemy(game.get_cur_player(), engaged_enemy);
        }
    }
    
    void enemy_attacking(GameObject obj)
    {
        print("ENEMY ATTACKING");
        if (game.get_cur_state() != LOTRGame.GAMESTATE.CHOOSING_ENEMY_TO_RESOLVE_ENEMY_ATTACK)
        {
            return;
        }

        if (mapping_of_obj_to_enemies_engaged.ContainsKey(obj))
        {
            EnemyCard attacking_enemy = mapping_of_obj_to_enemies_engaged[obj];
            game.player_chose_enemy_to_resolve(game.get_cur_player(), attacking_enemy);
        }
    }

    void character_committed(GameObject obj)
    {
        if (game.get_cur_state() != LOTRGame.GAMESTATE.COMMITTING_CHARACTERS)
        {
            return;
        }
        foreach(KeyValuePair<GameObject, PlayerCard> entry in mapping_of_obj_to_player_card)
        {
            // do something with entry.Value or entry.Key
            GameObject the_object = entry.Key;
            PlayerCard the_character = entry.Value;
            if (the_object == obj)
            {
                if (!the_character.is_exhausted() && !the_character.is_committed())
                {
                    game.character_committed(game.get_cur_player(), the_character);
                }
                break;
            }
        }
    }
    void pay_resource(GameObject obj)
    {
        if (game.get_cur_state() != LOTRGame.GAMESTATE.PAYING_FOR_CARD)
        {
            return;
        }

        foreach(KeyValuePair<GameObject, PlayerCard> entry in mapping_of_obj_to_player_card)
        {
            // do something with entry.Value or entry.Key
            GameObject the_hero_object = entry.Key;
            LOTRHero the_hero = (LOTRHero) entry.Value;
            if (the_hero_object == obj)
            {
                game.hero_resource_paid(game.get_cur_player(), the_hero);
                break;
            }
        }
        
    }


    string get_resources_in_readable_format()
    {
        Dictionary<LOTRHero, int> result = game.get_resources(game.get_cur_player());
        string s = string.Join(";", result.Select(x => x.Key.get_name() + "=" + x.Value).ToArray());
        return s;
    }

    void play_guard()
    {
        
        
         print("GUARD");
     }
    void play_tracker()
    {
        print("TRACKER");
    }

    void play_card(GameObject obj)
    {
        var card = mapping_of_obj_to_cards_in_hand[obj];
        //TODO: place the card in the stage
        var amt = card.get_cost();
        var resource = card.get_resource_type();
        var name = card.get_name();
        prompt_text.text = $"Please pay for {name}. It costs {amt} {resource}";

        if (mapping_of_cards_in_hand_to_obj.ContainsKey(card))
        {
            GameObject object_in_hand = mapping_of_cards_in_hand_to_obj[card];
            Destroy(object_in_hand);
            mapping_of_cards_in_hand_to_obj.Remove(card);

        }
        game.player_played_card(game.get_cur_player(), card);
    }

    void end_plan_phase_button_clicked()
    {
        game.plan_phase_ended();
    }
    
    void finished_committing_button_clicked()
    {
        game.finish_committing();
    }
    
    void finished_engaging_enemies_button_clicked()
    {
        game.finished_engaging_enemies();
    }

    void done_attacking_button_clicked()
    {
        game.done_attacking_enemies();
    }
    
    void done_choosing_attackers_button_clicked()
    {
        game.player_finished_choosing_attackers(game.get_cur_player());
    }
    
    void do_not_travel_button_clicked()
    {
        game.do_not_travel();
    }

    void no_response_button_clicked()
    {
        game.not_going_to_respond();
    }
    
    void yes_response_button_clicked()
    {
        game.yes_responded();
    }
    
    private GameObject create_new_enemy_card(string card_name, int index, string destination, string card_type)
    {
        if (destination == "staging")
        {
            return create_new_card_from_name(card_name, type: card_type, x: -6.83F + (2.06F * index), y: 3.63F,
                z: .001F);
        } else if (destination == "engaged")
        {
            return create_new_card_from_name(card_name, type:card_type, x: -2.99F + (2.06F * index), y: -0.45F,
                z: .001F);
        }
        else
        {
            throw new Exception("WTF LOL");
            return null;
        }
    }
    
    private GameObject create_attachment_card_for_character(string card_name, int index, GameObject character)
    {
        float x = character.transform.position.x;
        float y = character.transform.position.y -0.5F- (0.5F*index);
        float z = 0.01F;
        return create_new_card_from_name(card_name, type: "ATTACHMENT", x: x, y: y,
            z: z);
       
    }

    private GameObject create_new_player_card(string card_name, int index, string type, string destination)
    {
        if (destination == "hero")
        {
            return create_new_card_from_name(card_name, type: type, x: -9.75F + (2 * index), y: -3.02F,
                z: .001F);
        } else if (destination == "ally")
        {
            return create_new_card_from_name(card_name, type: type, x: -2.47F + (2 * index), y: -2.21F,
                z: .001F);
        }
        else if (destination == "hand")
        {
            
            return create_new_card_from_name(card_name, type: type, x: -3.46F + (2 * index), y: -3.29F,
                z: .001F);
        }
        else
        {
            throw new Exception("WTF LOL");
            return null;
        }
    }
    
    


    private GameObject create_new_card_from_name(string card_name, string type, float x = 0, float y = 0,
        float z = 0, bool rotate_90 = false)
    {
        var path = "";
        switch (type)
        {
            case "HERO":
                path = $"Assets/Sprites/Cards/Heroes/{card_name}.jpg";
                break;
            case "ALLY":
                path = $"Assets/Sprites/Cards/Ally/{card_name}.jpg";
                break;
            case "ENEMY":
                path = $"Assets/Sprites/Cards/Enemy/{card_name}.jpg";
                break;
            case "LOCATION":
                path = $"Assets/Sprites/Cards/Location/{card_name}.jpg";
                break;
            case "TREACHERY":
                path = $"Assets/Sprites/Cards/Treachery/{card_name}.jpg";
                break;
            case "QUEST":
                path = $"Assets/Sprites/Cards/Quest/{card_name}.jpg";
                break;
            case "ATTACHMENT":
                path = $"Assets/Sprites/Cards/Attachment/{card_name}.jpg";
                break;
            case "EVENT":
                path = $"Assets/Sprites/Cards/Event/{card_name}.jpg";
                break;
            default:
                break;
        }
        Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));   
        GameObject new_card = Instantiate(card_prefab, new Vector3(
                x, y, z),
            Quaternion.identity);
        new_card.GetComponent<SpriteRenderer>().sprite = sprite;
        new_card.name = card_name;
        
        new_card.transform.localScale = new Vector3(
            0.5F, 0.5F, 0.5F);
        switch (type)
        {
            case "HERO":
                new_card.tag = "Hero";
                break;
            case "ALLY":
                new_card.tag = "Ally";
                break;
            case "ENEMY":
                new_card.tag = "Enemy";
                break;
            case "LOCATION":
                new_card.tag = "Location";
                break;
            case "TREACHERY":
                new_card.tag = "Treachery";
                break;
            case "QUEST":
                new_card.tag = "Quest";
                break;
            case "ATTACHMENT":
                new_card.tag = "Attachment";
                break;
            case "EVENT":
                new_card.tag = "Event";
                break;
            default:
                break;
        }
        
        var _collider = new_card.GetComponent<BoxCollider2D>();
 
        _collider.offset = new Vector2(0, 0);
        _collider.size = new Vector3(sprite.bounds.size.x / transform.lossyScale.x,
            sprite.bounds.size.y / transform.lossyScale.y,
            sprite.bounds.size.z / transform.lossyScale.z);
        return new_card;
    }
    
}
