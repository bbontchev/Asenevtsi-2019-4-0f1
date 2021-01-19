using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

/// 

/// TextManager V2
/// Inspired by http://forum.unity3d.com/threads/35617-TextManager-Localization-Script
/// 
/// Reads text files in the 'AssetsResources' directory into a dictionary. The text file 
/// consists of one line that has the key name, a space and the actual text to display.
/// 
/// Example:
/// 
/// Assume you have a text file called English.txt in AssetsResourcesLanguages
/// The file looks like this:
/// 
/// hello Hello and welcome!
/// goodbye Goodbye and thanks for playing!
/// 
/// In Unity code you have to set the Language property with the English.txt asset once:
/// TextManager.LoadResource = "LanguagesEnglish";
/// 
/// Then you can retrieve text by calling:
/// TextManager.GetText("hello");
/// This will return a string containing "Hello and welcome!"
/// 

public class TextManager
{
    // PRIVATE DECLARATIONS
    private static TextManager instance;

    private static readonly IDictionary<string, string> TextTable = new Dictionary<string, string>();
    
    private static string _asset;
    public static string Language = "BG";


    private TextManager() { }

    //-----------------------------------------------------------------------------------------------
    // Returns an instance of the TextManager
    //-----------------------------------------------------------------------------------------------
    private static TextManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TextManager();
                LoadLanguage();
            }
            return instance;
        }
    }

    //-----------------------------------------------------------------------------------------------
    // Returns an instance of the TextManager
    //-----------------------------------------------------------------------------------------------
    public static TextManager GetInstance()
    {
        return Instance;
    }  

    private static void LoadLanguage()
    {
        GetInstance();
		//Debug.Log("Application.streamingAssetsPath is: " + Application.streamingAssetsPath);

        string fullpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "LanguageResources" + Path.DirectorySeparatorChar + Language + ".txt";
		//string fullpath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + Language + ".txt";
		//Debug.Log("fullpath is: " + fullpath); //BB, 09.09.2019
		//Debug.Log("Application.isWebPlayer: " + Application.isWebPlayer);
		
        // clear the hashtable
        TextTable.Clear();

        var lineNumber = 1;
		string[] lines = {""};
		
		// BB - set default texts in case of Web GL build because the game cannot foid dictionary - 20180405
		//if (Application.isWebPlayer) { //hardcoded EN dictionary!!!
		if (Application.platform == RuntimePlatform.WebGLPlayer) { //hardcoded EN dictionary!!!
			lines = new string[] {
				"enterAnswer____::Enter the answer to the question. ", 
				"lastAnswer_____::Last answer: ",
				"rightAnswer____:: - OK", 
				"wrongAnswer____:: - Wrong", 
				"correctAnswer__::Correct answer!", 
				"tryAgain_______::Wrong answer, try again!", 
				"solvePuzzle____::Solve the game puzzle to unlock the question.", 
				"clickToOpen1___::Click to ", 
				"clickToOpen2___::open/close", 
				"clickToOpen3___::the door", 
				"points_________::Points: ", 
				"found__________::Found  ", 
				"from___________:: from ", 
				"hiddenObjects__:: hidden objects.", 
				"clickBall1_____::Click to select the ball.", 
				"clickBall2_____::Use the arrow keys to move it around and find the matching target.", 
				"clickBall3_____::Press Esc to regain control.", 
				"youNeed2Collect::You need to collect at least ", 
				"points2Unlock__:: points to unlock this puzzle game."
			};
		}
		else {
			try {
				lines = File.ReadAllLines(fullpath);
			}
			catch (FileNotFoundException e) {
				Debug.Log("FileNotFoundException when reading the dictionary: " + e);
			}
		}
		//Debug.Log("lines.Length: " + lines.Length);
		
	    foreach (var line in lines)
	    {
            var lineElements = line.Trim().Split(new string[] { "::" }, StringSplitOptions.None);
	        if (lineElements.Length > 1)
            {
                var key = lineElements[0];
                var val = lineElements[1];

                if (key != null && val != null)
                {
                    var loweredKey = key.ToLower();
                    if (TextTable.ContainsKey(loweredKey))
                    {
                        Debug.LogWarning(String.Format(
                                    "Duplicate key '{1}' in language file '{0}.txt' detected.", Language, key));
                    }
                    TextTable.Add(loweredKey, val);
                }
            }
            else
            {
                Debug.LogWarning(String.Format(
                        "Format error in language file '{0}.txt' on line {1}. Each line must be of format: key value", 
                        Language, lineNumber));
            }

	        lineNumber++;
	    }
	    
    }

    public static string Get(string key)
    {
        var loweredKey = key.ToLower();

        var result = String.Format("Couldn't find key '{0}' in dictionary.", key);

        if (TextTable.ContainsKey(loweredKey))
        {
            result = TextTable[loweredKey];
        }

        return result;
    }
}