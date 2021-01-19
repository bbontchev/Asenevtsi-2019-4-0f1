using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharpEditor;

public class GameRollABallBuilder : miniGameBuilder
{
    public void Build2DMiniGame(Slide slide, GameObject slideText)
    {
    }

    public void Build3DMiniGame(AssemblyCSharpEditor.Room room)
    {
        GameObject roomObject = GameObject.Find("/" + room.Name);
        GameObject game = new GameObject("GameRollABall");
        game.transform.SetParent(roomObject.transform);
        game.transform.position = new Vector3(0, 0, 0);

        GameObject ballPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/BallBoard.prefab", typeof(GameObject));
        GameObject circlePrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/CircleBoard.prefab", typeof(GameObject));
        GameObject ringPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/TorusBoard.prefab", typeof(GameObject));

        int ballCount = 0, ballRowCount = 1;
        int circleCount = 0, circleRowCount = 1;

        if (room.GameRollABall != null && room.GameRollABall.gameElements != null && room.GameRollABall.gameElements.Count > 0)
        {
            foreach (AssemblyCSharpEditor.GameElement gameElement in room.GameRollABall.gameElements)
            {
                int x;
                GameObject newElement = null;
                if (gameElement.Type == "Ball")
                {
                    newElement = Utils.createObject(ballPrefab, (gameElement.Name != null && gameElement.Name != "" ? gameElement.Name : "Ball"));
                    if (ballRowCount < 2)
                    {
                        x = 9;
                    }
                    else if (ballRowCount == 2)
                    {
                        x = 12;
                    }
                    else
                    {
                        x = 9 - 3 * (ballRowCount - 2);
                    }
                    newElement.transform.position = new Vector3(roomObject.transform.position.x + x, 0, roomObject.transform.position.z - ballCount * 3);
                    newElement.transform.SetParent(game.transform);
                    ballCount++;
                    if (ballCount == 6)
                    {
                        ballRowCount++;
                        ballCount = 0;
                    }

                    if (room.GameRollABall.MinPoints > 0)
                    {
                        //newElement.transform.FindChild ("Ball").gameObject.SendMessage ("setMinPoints", room.GameRollABall.MinPoints);
                    }
                }

                if (newElement != null)
                {
                    if (gameElement.Text != null && gameElement.Text != "")
                    {
                        newElement.transform.Find("Board").Find("Text").GetComponent<TextMesh>().text = gameElement.Text;
                        newElement.transform.Find("Board").Find("Text_Back").GetComponent<TextMesh>().text = gameElement.Text;
                    }
                    else
                    {
                        newElement.transform.Find("Board").Find("Text").gameObject.SetActive(false);
                        newElement.transform.Find("Board").Find("Text_Back").gameObject.SetActive(false);
                    }

                    GameObject plane = newElement.transform.Find("Board").Find("Plane").gameObject;
                    GameObject plane_back = newElement.transform.Find("Board").Find("Plane_Back").gameObject;
                    if (gameElement.Image != null && gameElement.Image != "")
                    {
                        Material tempMaterial = new Material(plane.GetComponent<MeshRenderer>().sharedMaterial);
                        tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + gameElement.Image, typeof(Texture));
                        plane.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                        plane_back.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                    }
                    else
                    {
                        plane.SetActive(false);
                        plane_back.SetActive(false);
                        newElement.transform.Find("Board").Find("Text_Back").gameObject.SetActive(false); //BB, for not showing back text in case of no image, 05.09.2019
                    }

                    if (gameElement.Texture != null && gameElement.Texture != "")
                    {
                        GameObject actualElement = null;
                        if (gameElement.Type == "Ball")
                        {
                            actualElement = newElement.transform.Find("Ball").gameObject;
                        }

                        if (actualElement != null)
                        {
                            Material tempMaterial = new Material(actualElement.GetComponent<MeshRenderer>().sharedMaterial);
                            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + gameElement.Texture, typeof(Texture));
                            actualElement.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                        }
                    }
                }
            }

            foreach (AssemblyCSharpEditor.GameElement gameElement in room.GameRollABall.gameElements)
            {
                int x;
                GameObject newElement = null;
                if (gameElement.Type == "Circle")
                {
                    newElement = Utils.createObject(circlePrefab, (gameElement.Name != null && gameElement.Name != "" ? gameElement.Name : "Circle"));
                    if (circleRowCount < 2)
                    {
                        x = -9;
                    }
                    else if (circleRowCount == 2)
                    {
                        x = -12;
                    }
                    else
                    {
                        x = -9 + 3 * (circleRowCount - 2);
                    }
                    newElement.transform.position = new Vector3(roomObject.transform.position.x + x, 0, roomObject.transform.position.z - circleCount * 3);
                    newElement.transform.SetParent(game.transform);
                    circleCount++;
                    if (circleCount == 6)
                    {
                        circleRowCount++;
                        circleCount = 0;
                    }

                    if (gameElement.Ball != null && gameElement.Ball != "")
                    {
                        GameObject ball = GameObject.Find("/" + room.Name + "/GameRollABall/" + gameElement.Ball + "/Ball");
                        newElement.transform.Find("Circle").gameObject.GetComponent<MatchBall>().ball = ball;
                        //Debug.Log(newElement.transform.Find("Circle").gameObject.name + " matches: " + ball.name);
                    }
                }
                if (gameElement.Type == "Ring")
                {
                    newElement = Utils.createObject(ringPrefab, (gameElement.Name != null && gameElement.Name != "" ? gameElement.Name : "Ring"));
                    newElement.transform.Find("Torus").GetComponent<BoxCollider>().center = new Vector3(-1, 0, 0);
                    newElement.transform.Find("Torus").GetComponent<BoxCollider>().size = new Vector3(2, 0.2f, 0.52f);

                    if (circleRowCount < 2)
                    {
                        x = -9;
                    }
                    else if (circleRowCount == 2)
                    {
                        x = -12;
                    }
                    else
                    {
                        x = -9 + 3 * (circleRowCount - 2);
                    }
                    newElement.transform.position = new Vector3(roomObject.transform.position.x + x, 0, roomObject.transform.position.z - circleCount * 3);
                    newElement.transform.SetParent(game.transform);
                    circleCount++;
                    if (circleCount == 6)
                    {
                        circleRowCount++;
                        circleCount = 0;
                    }

                    if (gameElement.Ball != null && gameElement.Ball != "")
                    {
                        GameObject ball = GameObject.Find("/" + room.Name + "/GameRollABall/" + gameElement.Ball + "/Ball");
                        newElement.transform.Find("Torus").gameObject.GetComponent<TorusPassThrough>().ball = ball;
                    }
                }

                if (newElement != null)
                {
                    if (gameElement.Text != null && gameElement.Text != "")
                    {
                        newElement.transform.Find("Board").Find("Text").GetComponent<TextMesh>().text = gameElement.Text;
                        newElement.transform.Find("Board").Find("Text_Back").GetComponent<TextMesh>().text = gameElement.Text;
                    }
                    else
                    {
                        newElement.transform.Find("Board").Find("Text").gameObject.SetActive(false);
                        newElement.transform.Find("Board").Find("Text_Back").gameObject.SetActive(false);
                    }

                    GameObject plane = newElement.transform.Find("Board").Find("Plane").gameObject;
                    GameObject plane_back = newElement.transform.Find("Board").Find("Plane_Back").gameObject;
                    if (gameElement.Image != null && gameElement.Image != "")
                    {
                        Material tempMaterial = new Material(plane.GetComponent<MeshRenderer>().sharedMaterial);
                        tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + gameElement.Image, typeof(Texture));
                        plane.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                        plane_back.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                    }
                    else
                    {
                        plane.SetActive(false);
                        plane_back.SetActive(false);
                        newElement.transform.Find("Board").Find("Text_Back").gameObject.SetActive(false); //BB, for not showing back text in case of no image, 05.09.2019
                    }

                    if (gameElement.Texture != null && gameElement.Texture != "")
                    {
                        GameObject actualElement = null;
                        if (gameElement.Type == "Ball")
                        {
                            actualElement = newElement.transform.Find("Ball").gameObject;
                        }

                        if (gameElement.Type == "Circle")
                        {
                            actualElement = newElement.transform.Find("Circle").gameObject;
                        }

                        if (gameElement.Type == "Ring")
                        {
                            actualElement = newElement.transform.Find("Torus").gameObject;
                        }

                        if (actualElement != null)
                        {
                            Material tempMaterial = new Material(actualElement.GetComponent<MeshRenderer>().sharedMaterial);
                            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + gameElement.Texture, typeof(Texture));
                            actualElement.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                        }
                    }
                }
            }


        }
    }

}
