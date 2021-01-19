using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class GameWordSoup : MonoBehaviour
{
    //TODO: Need to make a Stack with the last clicked tiles and be able to unclick the last pressed
    public static string resultWord = "";
    public static int wordsFound = 0;
    //private List<string> dictionaryWordSoup = new List<string>();
    public static string[] dictionaryWordSoup = null;
    public static int points = 0; 
    public static Stack<GameObject> lastPressed = new Stack<GameObject>();
    public static int direction = 0; //could be either vertical downwards or horizontal to the right; 1 for vertical, 2 for horizontal
    private static GameObject globalGameManager;
    private static bool gameStarted = false;
    public const int HEIGHT = 5;
    public const int WIDTH = 8;
    private const int INIT = 3, FOUND_WORD = 2, END_GAME = 4;

    private bool contains(string[] allWords, string currentWord)
    {
        int len = allWords.Length;
        for (int i = 0; i < len; i++)
        {
            if (allWords[i] == currentWord)
            {
                return true;
            }
        }
        return false;
    }

    private bool isValidTile(string currentName)
    {
        //TODO: Check validity for tiles that are parts of already foud words

        string lastPressedName = lastPressed.Peek().name;
        //pretty hardcoded names for now, but this will be the idea

        //Debug.Log(lastPressedName);
        //Debug.Log(currentName);

        int LastTileNumber = Int32.Parse(lastPressedName.Substring(lastPressedName.Length - 2));
        int currentTileNumber = Int32.Parse(currentName.Substring(currentName.Length - 2));

        int diff = currentTileNumber - LastTileNumber; //distance ad direction

        if (diff == 1)
        {
            if (lastPressed.Count == 1)
            {
                direction = 2; // horizontal
                return true;
            }
            else
            {
                if (direction == 2)
                {
                    return true;
                }
                return false;
            }
        }

        if (diff == 10) //vertical
        {
            if (lastPressed.Count == 1)
            {
                direction = 1; // vertical
                return true;
            }
            else
            {
                if (direction == 1)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        return false;
    }

    private void playTaDaSound(int whatToPlay)
    {
        AudioSource[] audioSources = globalGameManager.GetComponents<AudioSource>();
        if (audioSources[whatToPlay] != null && !audioSources[whatToPlay].isPlaying && audioSources[whatToPlay].clip != null) {
            audioSources[whatToPlay].Play();
        }
    }

    private void initializeGlobalManager()
    {
        globalGameManager = GameObject.Find("/GlobalGameManager");
    }

    private void loadDictionary(GameObject slide) {
        GameObject dictionary = slide.transform.Find("WordSoupDictionary").gameObject;
        if (dictionary == null)
        {
            Debug.Log("No dictionary found for the Word Soup game at slide " + slide.name);
            return;
        }
        string words = dictionary.transform.Find("Words").GetComponent<TextMeshPro>().text;
        //Debug.Log("Words found: " + words);
        dictionaryWordSoup = words.Split(" "[0]);
    }

    void OnMouseDown()
    {
        GameObject currentObject = GameObject.Find(gameObject.name);
        GameObject slide = currentObject.transform.parent.gameObject;
        if (!gameStarted)
        {
            initializeGlobalManager();
            //parseDictionaryFromXML();
            //initializeLetters();
            //display Chars And Hints
            slide.transform.Find("Text").gameObject.SetActive(false);
            //slide.transform.GetChild(0).GetComponent().enabled = false;
            //slide.transform.Find("Tile00").gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().enabled = true;
            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    slide.transform.Find("Tile" + System.Convert.ToChar(i + '0') + System.Convert.ToChar(j + '0')).gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().enabled = true;
                }
            }
            slide.transform.Find("Hints").gameObject.SetActive(true);
            loadDictionary(slide);
            points = Convert.ToInt32(slide.transform.Find("WordSoupDictionary").gameObject.transform.Find("Points").GetComponent<TextMeshPro>().text);
            //Debug.Log("WordSoup Game for slide: " + slide.name + ", points for each found word: " + points.ToString());
            playTaDaSound(INIT); //using the same sound to start the game
            gameStarted = true;
            return;
        }

        string text = gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text;

        GameObject current = GameObject.Find(gameObject.name);
        Material tileMaterial = GetComponent<Renderer>().material;

        // means the word was already found. Probably should work with tags instead of colors ¯\_(ツ)_/¯
        if (tileMaterial.color == Color.blue)
        {
            return;
        }

        if (lastPressed.Count > 0)
        {
            bool valid = isValidTile(gameObject.name);
            if (!valid && tileMaterial.color != Color.red)
            {
                return;
            }
        }

        //List<GameObject> pressedTiles = new List<GameObject>();

        if (tileMaterial.color != Color.red)
        {
            tileMaterial.color = Color.red;
            lastPressed.Push(current);
            resultWord += text[0];
        }
        else
        {
            //tileMaterial = unclickedMaterial;
            GameObject top = lastPressed.Peek();
            if (top == current)
            {
                //Debug.Log("Is Last Pressed");
                lastPressed.Pop();
                resultWord = resultWord.Remove(resultWord.Length - 1);
                tileMaterial.color = Color.green;
            }
            else
            {
                //Debug.Log("Not Last");
                return;
            }
        }

        //Debug.Log(resultWord);

        if (contains(dictionaryWordSoup, resultWord))
        {
            wordsFound += 1;
            int whatToPlay = wordsFound == dictionaryWordSoup.Length ? END_GAME : FOUND_WORD;
            playTaDaSound(whatToPlay);
            GlobalGameManager.increasePointsOnly(points);
            if (whatToPlay == END_GAME)
            {
                slide.transform.Find("Text").GetComponent<TextMesh>().text = "Всички думи са открити! Край на играта.";
                slide.transform.Find("Text").gameObject.SetActive(true);
            }
            while (lastPressed.Count > 0)
            {
                lastPressed.Peek().GetComponent<Renderer>().material.color = Color.blue;
                lastPressed.Pop();
            }
            resultWord = "";
        }
        //Debug.Log(res);

    }
}
    