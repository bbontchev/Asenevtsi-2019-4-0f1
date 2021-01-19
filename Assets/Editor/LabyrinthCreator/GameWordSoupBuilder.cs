using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using AssemblyCSharpEditor;

public class GameWordSoupBuilder : miniGameBuilder
{
    private const int HEIGHT = 5; //BB 20210103
    private const int WIDTH = 8;

    public void Build2DMiniGame(Slide slide, GameObject slideText)
    {       //BB, 20210102
            Debug.Log("GameWordSoup.Points: " + slide.GameWordSoup.Points);
            int i = 0;
            GameObject dictionary = GameObject.Find("WordSoupDictionary");
            foreach (string row in slide.GameWordSoup.rows)
            {
                //Debug.Log("Row: " + row);
                int len = row.Length;
                string currRow = row;
                if (len < WIDTH)
                {
                    for (int k = 0; k < WIDTH - len; k++)
                    {
                        currRow += '#'; //we fill the row with # up to WIDTH characters
                    }
                }
                GameObject tilePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/WordSoup/Tile01.prefab", typeof(GameObject)); //BB, 20210103
                for (int j = 0; j < WIDTH; j++)
                {
                    GameObject tile = Utils.createObject(tilePrefab, "Tile" + System.Convert.ToChar(i + '0') + System.Convert.ToChar(j + '0'));
                    tile.transform.SetParent(slideText.transform.parent);
                    tile.transform.localPosition = new Vector3(0.429f - 0.125f * (float)j, 0.217f - 0.111f * (float)i, 0.024f);
                    tile.transform.localScale = new Vector3(0.1f, 0.1f, 0.03f);
                    string s = "";
                    s += currRow[j];
                    tile.transform.GetChild(0).GetComponent<TextMeshPro>().text = s;
                    tile.transform.GetChild(0).GetComponent<TextMeshPro>().enabled = false;
                }
                i += 1;
                if (i == HEIGHT) break;
            }
            int ii=0;
            foreach (string word in slide.GameWordSoup.words)
            {
                //Debug.Log("Word: " + word);
                if (dictionary == null)
                {
                    //we create a dictionary as an invisible object containing the words of the game dictionary
                    GameObject disctionaryPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/WordSoup/Dictionary.prefab", typeof(GameObject));
                    dictionary = Utils.createObject(disctionaryPrefab, "WordSoupDictionary");
                    dictionary.transform.SetParent(slideText.transform.parent);
                    dictionary.transform.localPosition = new Vector3(-0.95f, -0.3f, 10.0f);
                    dictionary.transform.GetChild(0).GetComponent<TextMeshPro>().text = word;
                    dictionary.transform.GetChild(1).GetComponent<TextMeshPro>().text = slide.GameWordSoup.Points;
                    dictionary.SetActive(false);
                }
                else
                {
                    //GameWordSoup.dictionaryWordSoup.Add(word);
                    dictionary.transform.GetChild(0).GetComponent<TextMeshPro>().text += " " + word;
                }
                int k = 12;
                DynamicValueKeeper.Instance.SaveValue("key" + ii.ToString(), word);
                object result = null;
                DynamicValueKeeper.Instance.GetValue("key" + ii.ToString(), out result);
                if (result == null) Debug.Log("NULL result returned from DynamicValueKeeper.Instance.GetValue");  
                else Debug.Log("Read from DynamicValueKeeper for key" + ii.ToString() + ": " + (string)result);
                ii += 1;
            }
            if (slide.GameWordSoup.Hints != null)
            {
                //Debug.Log("Hints: " + slide.GameWordSoup.Hints);
                GameObject hintsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/WordSoup/PlaneHints.prefab", typeof(GameObject));
                GameObject hints = Utils.createObject(hintsPrefab, "Hints");
                hints.transform.SetParent(slideText.transform.parent);
                hints.transform.localPosition = new Vector3(-0.95f, -0.3f, 0.0f);
                hints.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                hints.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                hints.transform.GetChild(0).GetComponent<TextMeshPro>().text = "\n\n" + slide.GameWordSoup.Hints;
                hints.SetActive(false);
            }
        }

    public void Build3DMiniGame(AssemblyCSharpEditor.Room room)
    {
    }
}
