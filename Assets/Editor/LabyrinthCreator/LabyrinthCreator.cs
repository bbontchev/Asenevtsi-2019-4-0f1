using UnityEngine;
using UnityEditor;
using System.Collections;
using SlideSeparator; //BB 20190910
using System.Collections.Generic; //BB 20210103
using TMPro; //BB 20210103
using AssemblyCSharpEditor;

class MazeBuilder : EditorWindow
{
    public string language = "EN";
    public string defaultSlideBackground = "marble-black.jpg"; //to be set from the XML global settings later in lab construction 
    public string illumination = "Lamps";
    public string showDoorLock = "Yes";
    public string showSlideFrames = "Yes";
    public string showPlants = "Yes";
    public float ceilingTiling = 1.0f;
    public float floorTiling = 1.0f;

    [MenuItem("Maze Builder/Create from XML")]

    static void Init()
    {
        MazeBuilder window = (MazeBuilder)EditorWindow.GetWindow(typeof(MazeBuilder));
        window.Show();
    }

    void OnGUI()
    {
        //defaultSlideBackground = "marble-black.jpg";

        if (GUILayout.Button("Import Textures"))
        {

            string path = EditorUtility.OpenFolderPanel("Select path for textures to import", "", "");
            if (path != "")
            {
                try
                {
                    string[] files = System.IO.Directory.GetFiles(path);
                    for (int i = 0; i < files.Length; i++)
                    {
                        System.IO.File.Copy(path + "/" + System.IO.Path.GetFileName(files[i]), "Assets/Materials/Textures/" + System.IO.Path.GetFileName(files[i]), true);
                        AssetDatabase.ImportAsset("Assets/Materials/Textures/" + System.IO.Path.GetFileName(files[i]), ImportAssetOptions.ForceUpdate);
                    }
                }
                catch (System.Exception)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Error", "Error copying files.", "Ok", "");
                }


            }
        }
        if (GUILayout.Button("Import Audio"))
        {

            string path = EditorUtility.OpenFolderPanel("Select path for audio files to import", "", "");
            if (path != "")
            {
                try
                {
                    string[] files = System.IO.Directory.GetFiles(path);
                    for (int i = 0; i < files.Length; i++)
                    {
                        System.IO.File.Copy(path + "/" + System.IO.Path.GetFileName(files[i]), "Assets/Materials/Sounds/" + System.IO.Path.GetFileName(files[i]), true);
                        AssetDatabase.ImportAsset("Assets/Materials/Sounds/" + System.IO.Path.GetFileName(files[i]), ImportAssetOptions.ForceUpdate);
                    }
                }
                catch (System.Exception)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Error", "Error copying files.", "Ok", "");
                }


            }
        }
        if (GUILayout.Button("Import XML"))
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().rootCount > 0)
            {
                int answer = UnityEditor.EditorUtility.DisplayDialogComplex("Warning", "All existing objects will be removed from the current scene.", "OK", "Cancel", "Keep Existing Objects");
                if (answer == 0)
                {

                    GameObject[] allActiveOjects = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().GetRootGameObjects();
                    foreach (GameObject rootObject in allActiveOjects)
                    {
                        DestroyImmediate(rootObject);
                    }
                }
                else if (answer == 1)
                {
                    return;
                }
            }
            Labyrinth newLabyrinth = null;
            string path = EditorUtility.OpenFilePanel("Select path for XML to import", "", "");
            if (path != "")
            {
                try
                {
                    newLabyrinth = Labyrinth.LoadXml(path);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Error parsing XML, e.Message: " + e.Message);
                    UnityEditor.EditorUtility.DisplayDialog("Error parsing XML", e.Message, "OK");
                }
                createLabyrinth(newLabyrinth);
            }
        }
    }

    public bool createLabyrinth(AssemblyCSharpEditor.Labyrinth newLabyrinth)
    {
        Utils.addGlobalGameManager();
        //Debug.Log("GlobalSettings.DefaultSlideBackground = " + newLabyrinth.GlobalSettings.DefaultSlideBackground); //BB, 20171207
        //Debug.Log("Language:" + newLabyrinth.GlobalSettings.Language);
        language = Utils.checkLabyrinthParameter(language, newLabyrinth.GlobalSettings.Language);
        defaultSlideBackground = Utils.checkLabyrinthParameter(defaultSlideBackground, newLabyrinth.GlobalSettings.DefaultSlideBackground); //slides with back image different than defaultSlideBackground or NONE will ot be shown intil the camera will arroach them
        illumination = Utils.checkLabyrinthParameter(illumination, newLabyrinth.GlobalSettings.Illumination);
        showDoorLock = Utils.checkLabyrinthParameter(showDoorLock, newLabyrinth.GlobalSettings.ShowDoorLock);
        showSlideFrames = Utils.checkLabyrinthParameter(showSlideFrames, newLabyrinth.GlobalSettings.ShowSlideFrames);
        showPlants = Utils.checkLabyrinthParameter(showPlants, newLabyrinth.GlobalSettings.ShowPlants);
        ceilingTiling = newLabyrinth.GlobalSettings.CeilingTiling;
        floorTiling = newLabyrinth.GlobalSettings.FloorTiling;
        TextManager.Language = language; //BB, 20171208
        TextManager.GetInstance();
        //Debug.Log("Localization test:" + TextManager.Get("lastAnswer_____"));

        foreach (AssemblyCSharpEditor.Room room in newLabyrinth.Rooms)
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/Room.prefab", typeof(GameObject));
            if (prefab != null)
            {
                Utils.createObject(prefab, room.Name);
                setQuestion(room, "QuestionN");
                setQuestion(room, "QuestionE");
                setQuestion(room, "QuestionS");
                setQuestion(room, "QuestionW");
                setSlidePair(room, "N");
                setSlidePair(room, "E");
                setSlidePair(room, "S");
                setSlidePair(room, "W");

                if (room.WallTexture != null && room.WallTexture != "")
                {
                    Utils.setWallTexture("WallN", room.Name, room.WallTexture);
                    Utils.setWallTexture("WallE", room.Name, room.WallTexture);
                    Utils.setWallTexture("WallW", room.Name, room.WallTexture);
                    Utils.setWallTexture("WallS", room.Name, room.WallTexture);
                }
                if (room.FloorTexture != null && room.FloorTexture != "")
                {
                    GameObject floor = GameObject.Find("/" + room.Name + "/Floor");
                    Material tempMaterial = new Material(floor.GetComponent<MeshRenderer>().sharedMaterial);
                    tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + room.FloorTexture, typeof(Texture));
                    floor.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                    floor.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(floorTiling, floorTiling); //BB, 20171203, setting tiling to the XML value
                    floor.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_BumpMap", new Texture2D(32, 32)); //BB, 20171203, set a new Texture for ellimination of the wall16_Normap map
                    floor.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_OcclusionMap", new Texture2D(32, 32)); //BB, 20171203, set a new Texture for ellimination of the wall16_Ambient_Occlusion map
                }
                if (room.CeilingTexture != null && room.CeilingTexture != "")
                {
                    GameObject ceiling = GameObject.Find("/" + room.Name + "/Ceiling");
                    Material tempMaterial = new Material(ceiling.GetComponent<MeshRenderer>().sharedMaterial);
                    tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + room.CeilingTexture, typeof(Texture));
                    ceiling.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                    ceiling.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(ceilingTiling, ceilingTiling); //BB, 20171203, setting tiling to the XML value
                    ceiling.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_BumpMap", new Texture2D(32, 32)); //BB, 20171203, set a new Texture for ellimination of the wall16_Normap map
                    ceiling.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_OcclusionMap", new Texture2D(32, 32)); //BB, 20171203, set a new Texture for ellimination of the wall16_Ambient_Occlusion map
                }

                if (room.StartingRoom)
                {
                    setStartingRoom(room.Name);
                }

                Utils.addGameManager(room);

                Utils.setRoomGame(room);

                addHiddenObject(room);

                if (room.Map != null && room.Map != "")
                {
                    GameObject map = GameObject.Find("/" + room.Name + "/FloorMap");
                    map.SetActive(true);
                    Material tempMaterial = new Material(map.GetComponent<MeshRenderer>().sharedMaterial);
                    tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + room.Map, typeof(Texture));
                    map.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                }
                else
                {
                    GameObject map = GameObject.Find("/" + room.Name + "/FloorMap");
                    map.SetActive(false);
                }
                if (showPlants != "Yes")
                {
                    GameObject plants = GameObject.Find("/" + room.Name + "/Plants"); //BB, 20171203, done by XML settings
                    plants.SetActive(false);
                }
                if (illumination != "Lamps")
                {
                    GameObject lamps = GameObject.Find("/" + room.Name + "/Lamps"); //BB, 20171203
                    lamps.SetActive(false);
                }
                GameObject this_room = GameObject.Find("/" + room.Name); //for Thrace game modifications, as follows:
                if (illumination == "Torches")
                {
                    GameObject torches_prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/Torches.prefab", typeof(GameObject));
                    if (torches_prefab != null)
                    {
                        GameObject torches = Utils.createObject(torches_prefab, "Torches");
                        torches.transform.SetParent(this_room.transform);
                        torches.transform.position = new Vector3(0, 0, 0);
                    }
                }
                GameObject al_prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/AmbientLights.prefab", typeof(GameObject));
                if (al_prefab != null)
                {
                    GameObject al = Utils.createObject(al_prefab, "Ambient Lights");
                    al.transform.SetParent(this_room.transform);
                    al.transform.position = new Vector3(0, 0, 0);
                }
                if (room.CeilingTexture == "NONE")
                {
                    GameObject.Find("/" + room.Name + "/Ceiling").SetActive(false); //BB, 20171203, hiding the NONE ceiling
                }
                if (showDoorLock == "No") Utils.hideDoorManiglias(room.Name);
                Utils.setLights(room.Name);
            }
            else
            {
                return false;
            }
        }
        //Position rooms in 3D space
        Utils.setRoomPositions(newLabyrinth.Rooms);
        return true;
    }

    private void setQuestion(AssemblyCSharpEditor.Room room, string questionDirection)
    {
        Door door = null;
        switch (questionDirection)
        {
            case "QuestionN": door = room.DoorN; break;
            case "QuestionE": door = room.DoorE; break;
            case "QuestionS": door = room.DoorS; break;
            case "QuestionW": door = room.DoorW; break;
        }
        GameObject question = GameObject.Find("/" + room.Name + "/Questions/" + questionDirection);
        if (door != null && door.Question != null)
        {
            // Set Text, copy material and set image
            question.SetActive(true);
            createQuestion(door.Question, questionDirection, room.Name, question);

        }
        else
        {
            question.SetActive(false);
        }
    }

    private void setSlidePair(AssemblyCSharpEditor.Room room, string slidePairDirection)
    {
        GameObject slide = GameObject.Find("/" + room.Name + "/Slides/Slide" + slidePairDirection + "_1");
        Slide slide1 = null, slide2 = null;
        switch (slidePairDirection)
        {
            case "N": slide1 = room.SlideN_1; slide2 = room.SlideN_2; break;
            case "E": slide1 = room.SlideE_1; slide2 = room.SlideE_2; break;
            case "S": slide1 = room.SlideS_1; slide2 = room.SlideS_2; break;
            case "W": slide1 = room.SlideW_1; slide2 = room.SlideW_2; break;
        }
        if (slide1 != null)
        {
            slide.SetActive(true);
            createSlide(slide1, "Slide" + slidePairDirection + "_1", room.Name, slide);

        }
        else
        {
            slide.SetActive(false);
        }
        slide = GameObject.Find("/" + room.Name + "/Slides/Slide" + slidePairDirection + "_2");
        if (slide2 != null)
        {
            slide.SetActive(true);
            createSlide(slide2, "Slide" + slidePairDirection + "_2", room.Name, slide);
        }
        else
        {
            slide.SetActive(false);
        }
    }

    private void createQuestion(AssemblyCSharpEditor.Question question, string questionName, string roomName, GameObject questionObject)
    {
        if (question.Text != null)
        {
            GameObject questionText = GameObject.Find("/" + roomName + "/Questions/" + questionName + "/QuestionText");
            questionText.GetComponent<TextMesh>().text = question.Text;
            GameObject lastAnswerText = GameObject.Find("/" + roomName + "/Questions/" + questionName + "/AnswerText"); //BB, 20171208
            lastAnswerText.GetComponent<TextMesh>().text = TextManager.Get("lastAnswer_____") + " ";
        }
        if (question.Image != null)
        {
            GameObject questionPlane = GameObject.Find("/" + roomName + "/Questions/" + questionName + "/Plane");
            Material tempMaterial = new Material(questionPlane.GetComponent<MeshRenderer>().sharedMaterial);
            tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + question.Image, typeof(Texture));
            questionPlane.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
        }
        if (question.Answer != null)
        {
            questionObject.GetComponent<ShowQuestion>().Answer = question.Answer;
        }
    }

    private void createSlide(AssemblyCSharpEditor.Slide slide, string slideName, string roomName, GameObject slidePlane)
    {
        Utils.setSlideTextAndImage(slide, slideName, roomName, slidePlane, defaultSlideBackground, showSlideFrames);

        Utils.setSlideGame(slide, slideName, roomName);
    }

    private void setStartingRoom(string roomName)
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/CustomFPSController.prefab", typeof(GameObject));
        if (prefab != null)
        {
            GameObject fpsController = Utils.createObject(prefab, "CustomFPSController");
            GameObject room = GameObject.Find("/" + roomName);
            fpsController.transform.position = new Vector3(room.transform.position.x, room.transform.position.y, room.transform.position.z - 15);
        }
    }

    private void addHiddenObject(AssemblyCSharpEditor.Room xmlRoom)
    {
        string roomName = xmlRoom.Name;
        GameObject room = GameObject.Find("/" + roomName);
        GameObject hiddenObjectParent = new GameObject("HiddenObjects");
        hiddenObjectParent.transform.SetParent(room.transform);
        hiddenObjectParent.transform.position = new Vector3(0, 0, 0);
        if (xmlRoom.hiddenObjects != null && xmlRoom.hiddenObjects.Count > 0)
        {
            float X_init = -12.0f; //BB, 20171208, initial X coordinate for the first hidden object placed within the room
            float X_offset = 1.5f; //BB, 20171208, step of offset for placing next hidden object
            int k = 0; //counter of hidden objects
            foreach (AssemblyCSharpEditor.HiddenObject currentHiddenObject in xmlRoom.hiddenObjects)
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/_Prefabs/HiddenObject.prefab", typeof(GameObject));
                if (prefab != null)
                {
                    GameObject hiddenObject = Utils.createObject(prefab, "HiddenObject");
                    hiddenObject.transform.SetParent(hiddenObjectParent.transform);
                    hiddenObject.transform.position = new Vector3(X_init + k * X_offset, -2.5f, 3.5f); //BB, 20171210, instead of new Vector3(0, 0, 0)
                    if (currentHiddenObject.Texture != null && currentHiddenObject.Texture != "")
                    {
                        Material tempMaterial = new Material(hiddenObject.GetComponent<MeshRenderer>().sharedMaterial);
                        tempMaterial.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Materials/Textures/" + currentHiddenObject.Texture, typeof(Texture));
                        Color tempColor = tempMaterial.color;
                        tempColor.a = 0.25f;
                        tempMaterial.color = tempColor;
                        hiddenObject.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
                    }
                    if (currentHiddenObject.Points > 0)
                    {
                        //hiddenObject.SendMessage ("setPoints", currentHiddenObject.Points); //commented by BB on 09.09.2019  because of: 
                        //Assertion failed: Assertion failed on expression: 'ShouldRunBehaviour()' 	UnityEngine.GameObject:SendMessage(String, Object)
                    }
                    k++; //BB, 20171208
                }
            }
        }
    }
}