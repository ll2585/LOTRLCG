//Attach this script to a GameObject to have it output messages when your mouse hovers over it.

using System;
using UnityEngine;

public class OnMouseOverExample : MonoBehaviour
{

    private bool printed_text = false;
    public void Start()
    {
       
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        if (!printed_text)
        {
            Debug.Log("Mouse is over " + gameObject.name);
            printed_text = true;
        }
        
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        //Debug.Log("Mouse is no longer on " + gameObject.name);
        printed_text = false;
    }
    
}