using System.Text;
using System.Xml;

namespace WixGenerator {

    public sealed class WixSourceGenerator {

        public static void Generate(IEnumerable<string> files, string outputFile, string groupId, string sourceName, string directoryId) {

            var settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    ",
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true
            };
            using (var xw = XmlWriter.Create(outputFile, settings)) {

                xw.WriteStartDocument();
                xw.WriteStartElement("Wix");

                xw.WriteStartElement("Fragment");

                xw.WriteStartElement("DirectoryRef");
                xw.WriteAttributeString("Id", directoryId);

                int counter = 0;
                foreach (var file in files) {

                    string componentId = $"{groupId}_C{counter}";
                    string fileId = $"{groupId}_F{counter}";
                    counter++;

                    string guid = Guid.NewGuid().ToString("B").ToUpper();

                    xw.WriteStartElement("Component");
                    xw.WriteAttributeString("Id", componentId);
                    xw.WriteAttributeString("Guid", guid);
                    xw.WriteStartElement("File");
                    xw.WriteAttributeString("Id", fileId);
                    xw.WriteAttributeString("Source", $"$({sourceName}){file}");
                    xw.WriteAttributeString("KeyPath", "yes");
                    xw.WriteEndElement(); // File
                    xw.WriteEndElement(); // Component
                }
                xw.WriteEndElement(); // DirectoryRef

                xw.WriteStartElement("ComponentGroup");
                xw.WriteAttributeString("Id", groupId);
                counter = 0;
                foreach (var file in files) {

                    string componentId = $"{groupId}_C{counter}";
                    counter++;

                    xw.WriteStartElement("ComponentRef");
                    xw.WriteAttributeString("Id", componentId);
                    xw.WriteEndElement(); // ComponentRef
                }
                xw.WriteEndElement(); // ComponentGroup

                xw.WriteEndElement(); // Fragment
                xw.WriteEndElement(); // Wix
                xw.WriteEndDocument();
            }
        }
    }
}
