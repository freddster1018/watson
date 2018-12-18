﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NPC;
using Things;

// (T)ype (o)f (M)essage
enum ToM : int {CHARACTER, THING};

public class CameraRaycasting : MonoBehaviour
{

    public float raycastDistance;
    RaycastHit objectHit;
    bool display = false;
    bool converse = false;
    int type;
    Collider entity;
    public GUISkin skin;
    public string stringToEdit = "Strike up a conversation!";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * raycastDistance, Color.yellow);

        if (Physics.Raycast(this.transform.position, this.transform.forward, out objectHit, raycastDistance))
        {
            entity = objectHit.collider;

            NPCController npc = objectHit.collider.GetComponent<NPCController>();
            if (npc != null)
            {
                type = (int)ToM.CHARACTER;
                display = true;

                // Interact with character
                if (Input.GetMouseButtonDown(0))
                {
                    converse = true;
                }
            }

            Thing obj = objectHit.collider.GetComponent<Thing>();
            if (obj != null)
            {
                //Debug.Log("true");
                type = (int)ToM.THING;
                display = true;
            }
        }
        else
        {
            display = false;
        }

    }

    void OnGUI()
    {
        GUI.skin = skin;
        // When conversing
        if (converse)
        {
            Pause(true);
            int width = 800;
            int height = 600;
            stringToEdit = GUI.TextArea(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width, height), stringToEdit);
        }
        if (display && !converse)
        {
            string message = "";

            switch (type)
            {
                // NPC
                case (int)ToM.CHARACTER:
                    NPCController npc = entity.GetComponent<NPCController>();
                    message = "Talk to " + npc.charName;
                    break;
                case (int)ToM.THING:
                    Thing thing = entity.GetComponent<Thing>();
                    if (thing.CanPickUp())
                    {
                        message = "Pick up ";
                    }
                    message += thing.objName;
                    break;
                default:
                    display = false;
                    break;
            }
            GUI.Box(new Rect(Screen.width / 2 + 20, Screen.height / 2 - 25, 240, 60), message);

        }
    }

    private void Pause(bool pause)
    {
        switch (pause)
        {
            case true:
                Time.timeScale = 0;
                break;
            default:
                Time.timeScale = 1;
                break;
        }
    }
}