using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlideSeparator; //BB 20190910
using TMPro; //BB 20210103

namespace AssemblyCSharpEditor
{
    public class Utils : MonoBehaviour
    {
        //place your utilities here...

        public static string checkLabyrinthParameter(string origParameterValue, string newValue)
        {
            if (newValue != null && newValue != "")
            {
                return newValue;
            }
            return origParameterValue;
        }

        //create one object from prefab
        public static GameObject createObject(GameObject original, string name)
        {
            Object newObject;

            //check if prefab exist
            if (PrefabUtility.GetPrefabType(original).ToString() != "None")
            {
                newObject = PrefabUtility.InstantiatePrefab(original);
            }
            else
            {
                newObject = Instantiate(original, Vector3.zero, Quaternion.identity);
            }

            ((GameObject)newObject).transform.position = new Vector3(0, 0, 0);
            newObject.name = name;

            return (GameObject)newObject;
        }

        public static void setLights(string roomName)
        {
            for (int i = 0; i <= 7; i++)
            {
                string suffix;
                if (i == 0)
                    suffix = "";
                else
                    suffix = " (" + i.ToString() + ")";
                Light l = GameObject.Find("/" + roomName + "/Lights/Point light" + suffix).GetComponent<Light>();
                l.intensity = 0.9f;
                l.range = 33;
                l.color = Color.Lerp(Color.red, Color.yellow, 0.8f);
            }
        }

        public static void setWallTexture(string wallName, string roomName, string roomWallTexture)
        {
            GameObject wall = GameObject.Find("/" + roomName + "/Walls/" + wallName + "/DoorCover");
            Material tempMaterial = new Material(wall.GetComponent<MeshRenderer>().sharedMaterial);
            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + roomWallTexture, typeof(Texture));
            wall.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
            wall = GameObject.Find("/" + roomName + "/Walls/" + wallName + "/DoorEntrance");
            tempMaterial = new Material(wall.GetComponent<MeshRenderer>().sharedMaterial);
            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + roomWallTexture, typeof(Texture));
            wall.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
            wall = GameObject.Find("/" + roomName + "/Walls/" + wallName + "/WallSideLeft");
            tempMaterial = new Material(wall.GetComponent<MeshRenderer>().sharedMaterial);
            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + roomWallTexture, typeof(Texture));
            wall.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
            wall = GameObject.Find("/" + roomName + "/Walls/" + wallName + "/WallSideRight");
            tempMaterial = new Material(wall.GetComponent<MeshRenderer>().sharedMaterial);
            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + roomWallTexture, typeof(Texture));
            wall.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
        }

        public static void hideDoorManiglias(string roomName)
        {
            GameObject.Find("/" + roomName + "/Walls/WallN/door/porta/maniglia").SetActive(false); //BB, 20171203, hiding the maniglias
            GameObject.Find("/" + roomName + "/Walls/WallS/door/porta/maniglia").SetActive(false); //BB, 20171203, hiding the maniglias
            GameObject.Find("/" + roomName + "/Walls/WallW/door/porta/maniglia").SetActive(false); //BB, 20171203, hiding the maniglias
            GameObject.Find("/" + roomName + "/Walls/WallE/door/porta/maniglia").SetActive(false); //BB, 20171203, hiding the maniglias
        }

        public static void setSlideTextAndImage(AssemblyCSharpEditor.Slide slide, string slideName, string roomName, GameObject slidePlane, string defaultSlideBackground, string showSlideFrames)
        {
            GameObject slideText = GameObject.Find("/" + roomName + "/Slides/" + slideName + "/Text"); //BB 20210103
            if (slide.Text != null)
            {
                GameObject slideControlPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/SlideControl.prefab", typeof(GameObject)); //BB, 10.09.2019
                SlideTextPortions slideBuffer = slideText.AddComponent<SlideTextPortions>(); //BB, 20190909
                slideBuffer.SetInputText(slide.Text);
                slideText.GetComponent<TextMesh>().text = (string)slideBuffer.textSlidePortions[0];
                slideText.GetComponent<TextMesh>().characterSize = 0.14f; //BB, 20171202, it was = 0.15f
                slideText.GetComponent<TextMesh>().alignment = TextAlignment.Left;
                slideText.GetComponent<Renderer>().enabled = true;
                if (slideBuffer.slidePortionsNo > 1)
                { //we need to create a slide control set
                    string textWithPageFooter = (string)slideBuffer.textSlidePortions[0] + "                                --- стр. 1 от " + (slideBuffer.slidePortionsNo).ToString() + " ---";
                    slideText.GetComponent<TextMesh>().text = textWithPageFooter;
                    GameObject slideControl = Utils.createObject(slideControlPrefab, slideName + "Control");		//BB, 10.09.2019
                    slideControl.transform.SetParent(slideText.transform.parent);							//BB, 10.09.2019
                    slideControl.transform.localPosition = new Vector3(0f, -0.45f, -0.025f);				//BB, 10.09.2019
                    slideControl.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);					//BB, 10.09.2019
                    slideControl.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);					//BB, 13.09.2019
                }
            }
            else
            { //BB
                slideText.GetComponent<TextMesh>().text = "";
            }
            if (slide.Image != null)
            {
                if (slide.Image != "NONE")
                { //BB When slide.Image == "NONE" then no background will be benerated
                    Material tempMaterial = new Material(slidePlane.GetComponent<MeshRenderer>().sharedMaterials[1]);
                    tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + slide.Image, typeof(Texture));
                    Material[] tempMaterials = slidePlane.GetComponent<MeshRenderer>().sharedMaterials;
                    tempMaterials[1] = tempMaterial;
                    if (showSlideFrames == "No")
                    {
                        tempMaterials[0] = ((Material)AssetDatabase.LoadAssetAtPath("Assets/ProCore/ProBuilder/Resources/Materials/InvisibleFace.mat", typeof(Material))); //BB, 20171203, frames invisible!
                    }
                    slidePlane.GetComponent<MeshRenderer>().sharedMaterials = tempMaterials;
                }
                else
                {
                    //slide.Image = "NONE" makes the MeshRenderer of that slide to be invisible
                    MeshRenderer mr = slidePlane.GetComponent<MeshRenderer>();
                    mr.enabled = false;
                }
            }
            if (!(slide.Text == null || slide.Text == "") && !(slide.Image == defaultSlideBackground) && !(slide.Image == "NONE"))
            { //BB 20171202
                var boxCollider = slideText.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(12.0f, 8.0f, 2.0f);
                slideText.AddComponent<SlideTextOnOff>();
                slideText.GetComponent<Renderer>().enabled = false; //slides with back image different than defaultSlideBackground or NONE will ot be shown intil the camera will arroach them
            }
        }

        //sets a 2D game over the slide
        public static void setSlideGame(AssemblyCSharpEditor.Slide slide, string slideName, string roomName)
        {
            GameObject slideText = GameObject.Find("/" + roomName + "/Slides/" + slideName + "/Text");
            if (slide.GameWordSoup != null)
            {
                miniGameBuilder builder = new GameWordSoupBuilder();
                builder.Build2DMiniGame(slide, slideText);
            }
            //if (slide.GameOther2DGame != null)
            //{ do it in a similar way
            //}
        }

        //sets 3D game(s) in the maze room
        public static void setRoomGame(Room room)
        {
            miniGameBuilder builder = new GameRollABallBuilder();
            builder.Build3DMiniGame(room);
            //create GameOtherType in the same way!
        }

        public static void setRoomPositions(List<Room> rooms)
        {
            //Position rooms in 3D space
            ArrayList processedRoomsList = new ArrayList();
            foreach (AssemblyCSharpEditor.Room room in rooms)
            {

                // add the first room so that is stays in this initial position
                if (processedRoomsList.Count == 0)
                {
                    processedRoomsList.Add(room.Name);
                }

                GameObject currentRoom = GameObject.Find("/" + room.Name);

                if (room.DoorN != null)
                {
                    GameObject doorCover = GameObject.Find("/" + room.Name + "/Walls/WallN/DoorCover");
                    doorCover.SetActive(false);
                    if (room.DoorN.Question != null)
                    {
                        GameObject question = GameObject.Find("/" + room.Name + "/Questions/QuestionN");
                        GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallN/door");
                        door.GetComponent<DoorOpenScript>().Question = question;
                        door.GetComponent<DoorOpenScript>().RotationSide = DoorOpenScript.SideOfRotation.Right; //BB, to open the door from the player to the next room, 04.09.2019
                        door.GetComponent<DoorOpenScript>().Speed = 1F;
                    }

                    GameObject nextRoom = GameObject.Find("/" + room.DoorN.Room);
                    GameObject doorCoverNext = GameObject.Find("/" + room.DoorN.Room + "/Walls/WallS/DoorCover");
                    doorCoverNext.SetActive(false);
                    GameObject doorNext = GameObject.Find("/" + room.DoorN.Room + "/Walls/WallS/door");
                    doorNext.SetActive(false);

                    if (!processedRoomsList.Contains(room.DoorN.Room))
                    {
                        nextRoom.transform.position = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, currentRoom.transform.position.z + 30);
                        processedRoomsList.Add(room.DoorN.Room);
                    }
                }
                else
                {
                    /* disable door to avoid raycasting issue */
                    GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallN/door");
                    door.SetActive(false);
                }

                if (room.DoorE != null)
                {
                    GameObject doorCover = GameObject.Find("/" + room.Name + "/Walls/WallE/DoorCover");
                    doorCover.SetActive(false);
                    if (room.DoorE.Question != null)
                    {
                        GameObject question = GameObject.Find("/" + room.Name + "/Questions/QuestionE");
                        GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallE/door");
                        door.GetComponent<DoorOpenScript>().Question = question;
                        door.GetComponent<DoorOpenScript>().RotationSide = DoorOpenScript.SideOfRotation.Right; //BB, to open the door from the player to the next room, 04.09.2019
                        door.GetComponent<DoorOpenScript>().Speed = 1F;
                    }
                    GameObject nextRoom = GameObject.Find("/" + room.DoorE.Room);
                    GameObject doorCoverNext = GameObject.Find("/" + room.DoorE.Room + "/Walls/WallW/DoorCover");
                    doorCoverNext.SetActive(false);
                    GameObject doorNext = GameObject.Find("/" + room.DoorE.Room + "/Walls/WallW/door");
                    doorNext.SetActive(false);

                    if (!processedRoomsList.Contains(room.DoorE.Room))
                    {
                        nextRoom.transform.position = new Vector3(currentRoom.transform.position.x + 30, currentRoom.transform.position.y, currentRoom.transform.position.z);
                        processedRoomsList.Add(room.DoorE.Room);
                    }
                }
                else
                {
                    /* disable door to avoid raycasting issue */
                    GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallE/door");
                    door.SetActive(false);
                }

                if (room.DoorW != null)
                {
                    GameObject doorCover = GameObject.Find("/" + room.Name + "/Walls/WallW/DoorCover");
                    doorCover.SetActive(false);
                    if (room.DoorW.Question != null)
                    {
                        GameObject question = GameObject.Find("/" + room.Name + "/Questions/QuestionW");
                        GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallW/door");
                        door.GetComponent<DoorOpenScript>().Question = question;
                        door.GetComponent<DoorOpenScript>().RotationSide = DoorOpenScript.SideOfRotation.Right; //BB, to open the door from the player to the next room, 04.09.2019
                        door.GetComponent<DoorOpenScript>().Speed = 1F;
                    }
                    GameObject nextRoom = GameObject.Find("/" + room.DoorW.Room);
                    GameObject doorCoverNext = GameObject.Find("/" + room.DoorW.Room + "/Walls/WallE/DoorCover");
                    doorCoverNext.SetActive(false);
                    GameObject doorNext = GameObject.Find("/" + room.DoorW.Room + "/Walls/WallE/door");
                    doorNext.SetActive(false);
                    if (!processedRoomsList.Contains(room.DoorW.Room))
                    {
                        nextRoom.transform.position = new Vector3(currentRoom.transform.position.x - 30, currentRoom.transform.position.y, currentRoom.transform.position.z);
                        processedRoomsList.Add(room.DoorW.Room);
                    }
                }
                else
                {
                    /* disable door to avoid raycasting issue */
                    GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallW/door");
                    door.SetActive(false);
                }

                if (room.DoorS != null)
                {
                    GameObject doorCover = GameObject.Find("/" + room.Name + "/Walls/WallS/DoorCover");
                    doorCover.SetActive(false);
                    if (room.DoorS.Question != null)
                    {
                        GameObject question = GameObject.Find("/" + room.Name + "/Questions/QuestionS");
                        GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallS/door");
                        door.GetComponent<DoorOpenScript>().Question = question;
                        door.GetComponent<DoorOpenScript>().RotationSide = DoorOpenScript.SideOfRotation.Right; //BB, to open the door from the player to the next room, 04.09.2019
                        door.GetComponent<DoorOpenScript>().Speed = 1F;
                    }
                    GameObject nextRoom = GameObject.Find("/" + room.DoorS.Room);
                    GameObject doorCoverNext = GameObject.Find("/" + room.DoorS.Room + "/Walls/WallN/DoorCover");
                    doorCoverNext.SetActive(false);
                    GameObject doorNext = GameObject.Find("/" + room.DoorS.Room + "/Walls/WallN/door");
                    doorNext.SetActive(false);

                    if (!processedRoomsList.Contains(room.DoorS.Room))
                    {
                        nextRoom.transform.position = new Vector3(currentRoom.transform.position.x, currentRoom.transform.position.y, currentRoom.transform.position.z - 30);
                        processedRoomsList.Add(room.DoorS.Room);
                    }
                }
                else
                {
                    /* disable door to avoid raycasting issue */
                    GameObject door = GameObject.Find("/" + room.Name + "/Walls/WallS/door");
                    door.SetActive(false);
                }
            }
        }

        public static void addGameManager(AssemblyCSharpEditor.Room xmlRoom)
        {
            string roomName = xmlRoom.Name;
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/GameManager.prefab", typeof(GameObject));
            if (prefab != null)
            {
                GameObject gameManager = Utils.createObject(prefab, "GameManager");
                GameObject room = GameObject.Find("/" + roomName);
                gameManager.transform.SetParent(room.transform);
                gameManager.transform.position = new Vector3(0, 0, 0);

                GameObject fpsController = GameObject.Find("/CustomFPSController");
                gameManager.GetComponent<GameManager>().fpsCharacter = fpsController;

                if (xmlRoom.AudioClip != null && xmlRoom.AudioClip.File != null && xmlRoom.AudioClip.File != "")
                {
                    gameManager.GetComponent<AudioSource>().clip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Materials/Sounds/" + xmlRoom.AudioClip.File, typeof(AudioClip));
                    gameManager.GetComponent<AudioSource>().loop = xmlRoom.AudioClip.Loop;
                }
            }
        }

        public static void addGlobalGameManager()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/GlobalGameManager.prefab", typeof(GameObject));
            if (prefab != null)
            {
                GameObject gameManager = Utils.createObject(prefab, "GlobalGameManager");
                gameManager.transform.position = new Vector3(0, 0, 0);
            }
        }
    }
}
