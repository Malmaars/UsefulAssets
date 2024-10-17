using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DictionaryScript
{
    public static WordDictionary wordDictionary;

    public static void CreateDictionary()
    {
        string json = System.IO.File.ReadAllText("Assets/WordLists/dictionary.json");
        wordDictionary = JsonUtility.FromJson<WordDictionary>(json);
    }

    public static bool CheckIfWordIsInDictionary(string _word)
    {        
        _word = _word.ToLower();
        //we can't just do a .contains, the dictionary is too big for that
        //so i need to write a simple sorting algorithm

        //check alphabetically

        int currentLocation = (int)wordDictionary.words.Length / 2;
        bool finished = false;
        int previousLocation = 0;
        int previousLowerLocation = 0;
        int previousHigherLocation = wordDictionary.words.Length;

        while (!finished)
        {
            string tempCheck = wordDictionary.words[currentLocation];
            //if _word is lower alphabetically, it gives -1, if it's higher (closer to z), it gives 1
            int alphabeticalOrder = String.Compare(_word, tempCheck);
            int newLocation = 0;
            if (alphabeticalOrder == 0)
            {
                return true;
            }
            if (alphabeticalOrder == -1)
            {
                if (currentLocation > previousLocation)
                {
                    newLocation = currentLocation - Mathf.CeilToInt((currentLocation - previousLocation) / 2f);
                }
                else if (currentLocation < previousLocation)
                {
                    newLocation = currentLocation - Mathf.CeilToInt((currentLocation - previousLowerLocation) / 2f);
                    previousHigherLocation = currentLocation;
                }
            }
            if (alphabeticalOrder == 1)
            {
                if (currentLocation > previousLocation)
                {
                    newLocation = currentLocation + Mathf.CeilToInt((previousHigherLocation - currentLocation) / 2f);
                    previousLowerLocation = currentLocation;
                }
                else if (currentLocation < previousLocation)
                {
                    newLocation = currentLocation + Mathf.CeilToInt(((previousLocation - currentLocation) / 2f));
                }
            }
            if (newLocation == previousLocation || newLocation == currentLocation)
            {
                finished = true;
                return false;
            }
            previousLocation = currentLocation;
            currentLocation = newLocation;
        }
        return false;

    }
}

public struct WordDictionary
{
    public string[] words;
}
