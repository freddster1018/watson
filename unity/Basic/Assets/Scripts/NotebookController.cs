﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NPC;
using System;

namespace Notebook
{
    enum Page : int { CHARACTER, ITEM, INVENTORY, NOTES, MENU };
    //enum Character : int { ACTRESS, BUTLER, COLONEL, COUNTESS, EARL, GANGSTER, POLICE };
    enum Item : int { KEY, BOOK, POISON };

    public class NotebookController : MonoBehaviour
    {

        public GameObject container;
        public PlayerController player;

        //GameObject tabsLeft;
        public GameObject tabsRightChars;
        public GameObject tabsEmpty;
        Page currentPageEnum = Page.MENU;

        // Notebook pages
        public GameObject charPage;
        public GameObject itemPage;
        public GameObject invtPage;
        public GameObject notePage;
        public GameObject menuPage;
        GameObject currentPage;

        // Character pages
        public GameObject charActress;
        public GameObject charButler;
        public GameObject charColonel;
        public GameObject charCountess;
        public GameObject charEarl;
        public GameObject charGangster;
        public GameObject charPolice;
        GameObject currentChar;
        Character currentCharEnum = Character.ACTRESS;

        // Character clues
        List<Tuple<string, string> > cluesActress = new List<Tuple<string, string> >();
        List<Tuple<string, string>> cluesButler = new List<Tuple<string, string>>();
        List<Tuple<string, string>> cluesColonel = new List<Tuple<string, string>>();
        List<Tuple<string, string>> cluesCountess = new List<Tuple<string, string>>();
        List<Tuple<string, string>> cluesEarl = new List<Tuple<string, string>>();
        List<Tuple<string, string>> cluesGangster = new List<Tuple<string, string>>();
        List<Tuple<string, string>> cluesPolice = new List<Tuple<string, string>>();
        List<List<Tuple<string, string>>> cluesDirectory = new List<List<Tuple<string, string>>>();

        public Text actressClueBox;
        public Text butlerClueBox;
        public Text colonelClueBox;
        public Text countessClueBox;
        public Text earlClueBox;
        public Text gangsterClueBox;
        public Text policeClueBox;

        public Text inventoryText;

        // Use this for initialization
        void Start()
        {
            // Character pages
            currentChar = charActress;
            charActress.SetActive(true);
            charButler.SetActive(false);
            charColonel.SetActive(false);
            charCountess.SetActive(false);
            charEarl.SetActive(false);
            charGangster.SetActive(false);
            charPolice.SetActive(false);

            tabsRightChars.SetActive(false);
            tabsEmpty.SetActive(true);
            // Notebook pages
            charPage.SetActive(false);
            itemPage.SetActive(false);
            invtPage.SetActive(false);
            notePage.SetActive(false);
            menuPage.SetActive(true);

            currentPage = menuPage;

            // Add clue lists
            cluesDirectory.Add(cluesActress);
            cluesDirectory.Add(cluesButler);
            cluesDirectory.Add(cluesColonel);
            cluesDirectory.Add(cluesCountess);
            cluesDirectory.Add(cluesEarl);
            cluesDirectory.Add(cluesGangster);
            cluesDirectory.Add(cluesPolice);

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Activate(bool active)
        {
            container.SetActive(active);
            UpdateInventory();
            if (currentPageEnum == Page.CHARACTER)
            {
                ChangeCharacter((int)currentCharEnum);
            }
        }

        public void ChangePage(int target)
        {
            if (target != (int)currentPageEnum)
            {
                if (currentPageEnum == Page.CHARACTER)
                {
                    tabsRightChars.SetActive(false);
                    tabsEmpty.SetActive(true);
                }
                currentPage.SetActive(false);
                switch ((Page)target)
                {
                    case Page.CHARACTER:
                        tabsEmpty.SetActive(false);
                        charPage.SetActive(true);
                        tabsRightChars.SetActive(true);
                        currentPage = charPage;
                        ChangeCharacter((int)currentCharEnum);
                        break;
                    case Page.INVENTORY:
                        invtPage.SetActive(true);
                        currentPage = invtPage;
                        UpdateInventory();
                        break;
                    case Page.ITEM:
                        itemPage.SetActive(true);
                        currentPage = itemPage;
                        break;
                    case Page.NOTES:
                        notePage.SetActive(true);
                        currentPage = notePage;
                        break;
                    case Page.MENU:
                        menuPage.SetActive(true);
                        currentPage = menuPage;
                        break;
                }
                currentPageEnum = (Page)target;
            }
        }

        public void ChangeCharacter(int target)
        {
            //if ((int)currentCharEnum != target)
            //{
                currentChar.SetActive(false);
                switch ((Character)target)
                {
                    case Character.ACTRESS:
                        charActress.SetActive(true);
                        actressClueBox.text = UpdateClues(target);
                        currentChar = charActress;
                        currentCharEnum = Character.ACTRESS;
                        break;
                    case Character.BUTLER:
                        charButler.SetActive(true);
                        butlerClueBox.text = UpdateClues(target);
                        currentChar = charButler;
                        currentCharEnum = Character.BUTLER;
                        break;
                    case Character.COLONEL:
                        charColonel.SetActive(true);
                        colonelClueBox.text = UpdateClues(target);
                        currentChar = charColonel;
                        currentCharEnum = Character.COLONEL;
                        break;
                    case Character.COUNTESS:
                        charCountess.SetActive(true);
                        countessClueBox.text = UpdateClues(target);
                        currentChar = charCountess;
                        currentCharEnum = Character.COUNTESS;
                        break;
                    case Character.EARL:
                        charEarl.SetActive(true);
                        earlClueBox.text = UpdateClues(target);
                        currentChar = charEarl;
                        currentCharEnum = Character.EARL;
                        break;
                    case Character.GANGSTER:
                        charGangster.SetActive(true);
                        gangsterClueBox.text = UpdateClues(target);
                        currentChar = charGangster;
                        currentCharEnum = Character.GANGSTER;
                        break;
                    case Character.POLICE:
                        charPolice.SetActive(true);
                        policeClueBox.text = UpdateClues(target);
                        currentChar = charPolice;
                        currentCharEnum = Character.POLICE;
                        break;
                }
            //}
        }

        //public void AddClue(List<int> characters, List<int> items, string question, string clue)
        //{
        //    foreach (int character in characters)
        //    {
        //        cluesDirectory[character].Add(new Tuple<string, string>(question, clue));
        //    }
        //}

        public int CharToEnum(string name)
        {
            switch (name.ToLower())
            {
                case "actress":
                    return (int)Character.ACTRESS;
                case "butler":
                    return (int)Character.BUTLER;
                case "colonel":
                    return (int)Character.COLONEL;
                case "countess":
                    return (int)Character.COUNTESS;
                case "earl":
                    return (int)Character.EARL;
                case "gangster":
                    return (int)Character.GANGSTER;
                case "police":
                    return (int)Character.POLICE;
            }
            return (int)Character.POLICE; // Default
        }

        public void LogResponse(NPCController character, string question, string clue)
        {
            cluesDirectory[(int)character.GetEnum()].Add(new Tuple<string, string>(question, clue));
        }

        public string UpdateClues(int character)
        {
            string result = "";
            foreach (Tuple<string, string> exchange in cluesDirectory[character])
            {
                result += "<b>>  </b>";
                if (exchange.Item1 != "")
                {
                    result += "<i>\"" + exchange.Item1 + "</i>\"<b>  ~  ";
                }
                result += "\"" + exchange.Item2 + "\"</b>\n";
            }
            return result;
        }

        public void UpdateInventory()
        {
            inventoryText.text = player.list;
        }

    }
}
