﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WatsonAI;
using System;
using System.Linq;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using NPC;


public class AIController : MonoBehaviour
{
    private bool newSession = false;
    private Watson watson;
    private bool loaded = false;
    private NPCController currentCharacter;

    public AIController()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor || Stats.Menu)
        {
            StartUp();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartUp()
    {
        string path = "Assets/StreamingAssets";
        if (!Application.isEditor) path = "Watson_Data/StreamingAssets/";
        this.watson = new Watson(path);
        loaded = true;
    }

    public Tuple<string, string> Run(string input, int character) 
    {
        Tuple<string, string> aiResponse = watson.Run(input, (int)currentCharacter.GetEnum());
        //string aiResponse = watson.Run(input);
        SaveFile(input, aiResponse.Item2);
        this.newSession = false;
        string question = input;
        if (!(aiResponse.Item1 == null))
        {
            question = aiResponse.Item1;
        }
        Tuple<string, string> returned = new Tuple<string, string>(question, aiResponse.Item2);
        return returned; 
    }

    public void StartSession(NPCController character) {
        this.newSession = true;
        currentCharacter = character;
        if (!loaded)
        {
            StartUp();
        }
    }


    public void SaveFile(string userInput, string aiResponse)
    {
        string path = Path.Combine(Application.persistentDataPath, "inputs.txt");
        Debug.Log(path);

       
        // This text is added only once to the file.
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                if (this.newSession)
                {
                    sw.WriteLine("");
                    sw.WriteLine("New Session");
                }
                // Create a file to write to
                sw.WriteLine("User: " + userInput);
                sw.WriteLine("AI: " + aiResponse);
            }
            
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
        using (StreamWriter sw = File.AppendText(path))
        {
            if (this.newSession)
            {
                sw.WriteLine("");
                sw.WriteLine("New Session");
            }
            // Create a file to write to
            sw.WriteLine("User: " + userInput);
            sw.WriteLine("AI: " + aiResponse);
        }
    }


}


