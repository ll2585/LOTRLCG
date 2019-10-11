//Attach this script to a GameObject to have it output messages when your mouse hovers over it.

using System;
using UnityEngine;
using UnityEngine.UI;

public class OnMouseOverExample : MonoBehaviour
{

    public GameObject card_info;
    public Text card_name;
    public Text card_attack;
    public Text card_defense;
    public Text card_health;
    public Text card_willpower;
    public Text card_damage;
    public LOTRController controller;
    public Text card_ability;
    
    private bool printed_text = false;
    
    public void Start()
    {
        card_info.SetActive(false);
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        if (!printed_text)
        {
            Debug.Log("Mouse is over " + gameObject.name);
            card_info.SetActive(true);
            
            Card card = controller.get_card_from_object(gameObject);
            card_name.text = card.get_name();
            if (card is PlayerCard player_card)
            {
                card_attack.text = "Attack: " + player_card.get_attack_strength().ToString();
                card_defense.text = "Defense: " + player_card.get_defense().ToString();
                card_health.text = "Health: " + player_card.get_hp().ToString();
                card_willpower.text = "Willpower: " + player_card.get_willpower().ToString();
                card_damage.text = "Damage taken: " + player_card.get_damage_taken();
                card_ability.text = player_card.get_ability();
            } else if (card is EnemyCard enemy_card)
            {
                card_attack.text = "Attack: " + enemy_card.get_attack().ToString();
                card_defense.text = "Defense: " + enemy_card.get_defense().ToString();
                card_health.text = "Health: " + enemy_card.get_hp().ToString();
                card_willpower.text = "Threat: " + enemy_card.get_threat_strength().ToString();
                card_damage.text = "Damage taken: " + enemy_card.get_damage_taken();
                card_ability.text = enemy_card.get_ability();
            }
            printed_text = true;
        }
        
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        //Debug.Log("Mouse is no longer on " + gameObject.name);
        printed_text = false;
        card_info.SetActive(false);
    }
    
}