using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace AssemblyCSharpEditor
{
    public class GameOtherTypeDef
    {
        //this is a sample structure of the settings of GameOtherType:
        [XmlAttribute("AttrName")]
        public string AttrName;
        [XmlArray("Settings")]
        [XmlArrayItem("Setting")]
        public List<string> rows = new List<string>();
        [XmlArray("Dictionary")]
        [XmlArrayItem("Word")]
        public List<string> words = new List<string>();
        [XmlArray("Hints")]
        [XmlArrayItem("Hint")]
        public List<string> hints = new List<string>();
    }
}