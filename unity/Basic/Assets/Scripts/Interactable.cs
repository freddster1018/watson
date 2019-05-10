﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using UnityEngine.UI;
using Notebook;

public class Interactable : MonoBehaviour
{
    public NPCController owner;
    public bool pickup;
    private bool show = false;
    public string objName;
    public int category;
    public string description;
    public Image image = null;
    public GUISkin skin;
    public GameObject hover = null;
    private bool glowing = false;
    public int propEnum;
    public NotebookController notebook;
    public Button close;

    public enum Category : int {BOOK, KEY, CONTAINER, OBJECT};

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public bool CanPickUp()
    {
        return pickup;
    }

    //once per GUI update
    private void OnGUI()
    {
        GUI.skin = skin;
        if (show)
        {
            //Draw the GUI layer
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), description);
        }
    }

    public void HideInspect()
    {
        if (image)
        {
            image.gameObject.SetActive(false);
        }
        show = false;
    }

    public void InspectObject()
    {
        show = true;
        close.gameObject.SetActive(true);
        if (image)
        {
            image.gameObject.SetActive(true);
        }
        notebook.currentInspect = transform.gameObject;
        notebook.inspect = true;
    }

    public void Glow(bool status)
    {
        if (hover)
        {
            if (!glowing)
            {
                hover.SetActive(status);
                glowing = status;
            }
            else if (glowing && !status)
            {
                hover.SetActive(status);
                glowing = status;
            }
        }
    }

}

