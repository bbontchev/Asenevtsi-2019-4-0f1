using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace AssemblyCSharpEditor
{
    public class GameWordSoupDef
    {
        [XmlAttribute("Points")]
        public string Points;
        [XmlArray("Rows")]
        [XmlArrayItem("Row")]
        public List<string> rows = new List<string>();
        [XmlArray("Dictionary")]
        [XmlArrayItem("Word")]
        public List<string> words = new List<string>();
        [XmlElement("Hints")]
        public string Hints;
    }
}