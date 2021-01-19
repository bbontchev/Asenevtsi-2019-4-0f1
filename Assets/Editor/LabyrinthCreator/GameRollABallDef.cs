using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace AssemblyCSharpEditor
{
    public class GameRollABallDef
    {
        public int MinPoints;
        [XmlArray("GameElements")]
        [XmlArrayItem("GameElement")]
        public List<GameElement> gameElements = new List<GameElement>();
    }

    public class GameElement
    {
        public string Text;
        public string Image;
        public string Texture;
        public string Name;
        public string Type;
        public string Ball;
    }
}