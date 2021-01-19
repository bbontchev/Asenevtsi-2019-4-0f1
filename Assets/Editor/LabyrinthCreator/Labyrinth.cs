using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssemblyCSharpEditor
{
	[XmlRoot("Labyrinth")]	
	public class Labyrinth
	{
        [XmlElement("GlobalSettings")] //BB, 20171207
        public GlobalSettings GlobalSettings { get; set; }

        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<Room> Rooms = new List<Room>();

		public static Labyrinth LoadXml(string path)
		{
			XmlSerializer serializer = new XmlSerializer (typeof(Labyrinth));
			using (FileStream stream = new FileStream (path, FileMode.Open)) {
				return serializer.Deserialize (stream) as Labyrinth;
			}
		}
	}

    public class GlobalSettings //BB, 20171207
    {
        [XmlElement("Language")]
        public string Language;
        [XmlElement("DefaultSlideBackground")]
        public string DefaultSlideBackground;
        [XmlElement("Illumination")]
        public string Illumination;
        [XmlElement("ShowDoorLock")]
        public string ShowDoorLock;
        [XmlElement("ShowSlideFrames")]
        public string ShowSlideFrames;
        [XmlElement("ShowPlants")]
        public string ShowPlants;
        [XmlElement("CeilingTiling")]
        public float CeilingTiling;
        [XmlElement("FloorTiling")]
        public float FloorTiling;
    }

	public class Room
	{
		public string Name;

		public Audio AudioClip;

		public Door DoorN;
		public Door DoorE;
		public Door DoorW;
		public Door DoorS;

		public Slide SlideN_1;
		public Slide SlideN_2;
		public Slide SlideE_1;
		public Slide SlideE_2;
		public Slide SlideW_1;
		public Slide SlideW_2;
		public Slide SlideS_1;
		public Slide SlideS_2;

		public string WallTexture;
		public string FloorTexture;
		public string CeilingTexture;
		public string Map;

		[XmlArray("HiddenObjects")]
		[XmlArrayItem("HiddenObject")]
		public List<HiddenObject> hiddenObjects = new List<HiddenObject>();

		public bool StartingRoom;

        public GameRollABallDef GameRollABall; //BB - 20210102
        public GameOtherTypeDef GameOtherType; //BB - 20210102
    }
    
    public class Audio
	{
		public bool Loop;
		public string File;
	}

	public class HiddenObject
	{
		public string Texture;
		public int Points;
	}

	public class Slide
	{
		public string Text;
        public string Image;
        public GameWordSoupDef GameWordSoup; //BB - 20210102
    }

	public class Question
	{
		public string Text;
		public string Image;
		public string Answer;
	}

	public class Door {
		public string Room;
		public Question Question;
	}

    public interface miniGameBuilder
    {
        void Build2DMiniGame(Slide slide, GameObject slideText); //builds a 2D minigame to be shown on a slide in a maze room
        void Build3DMiniGame(Room room); //builds a 3D minigame to be shown in a maze room
    }
}