using System;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;

namespace ManagedFusion.Rewriter.Azure
{
    public class UriRewriterBodyWriter : BodyWriter
    {
        string sourcePattern;
        string targetPattern;
        XmlDictionaryReader reader;

        public UriRewriterBodyWriter(string sourcePattern, string targetPattern, XmlDictionaryReader reader)
            : base(false)
        {
            this.sourcePattern = sourcePattern;
            this.targetPattern = targetPattern;
            this.reader = reader;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            try
            {
                do
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                writer.WriteAttributeString(reader.LocalName, reader.NamespaceURI, reader.Value.Replace(sourcePattern, targetPattern));
                            }
                            reader.MoveToElement();
                            if (reader.IsEmptyElement)
                            {
                                writer.WriteEndElement();
                            }
                            break;

                        case XmlNodeType.Text:
                            writer.WriteString(reader.Value.Replace(sourcePattern, targetPattern));
                            break;

                        case XmlNodeType.Whitespace:
                        case XmlNodeType.SignificantWhitespace:
                            writer.WriteWhitespace(reader.Value);
                            break;

                        case XmlNodeType.CDATA:
                            writer.WriteCData(reader.Value);
                            break;

                        case XmlNodeType.EntityReference:
                            writer.WriteEntityRef(reader.Name);
                            break;

                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            writer.WriteProcessingInstruction(reader.Name, reader.Value);
                            break;
                        case XmlNodeType.DocumentType:
                            writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                            break;
                        case XmlNodeType.Comment:
                            writer.WriteComment(reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            writer.WriteFullEndElement();
                            break;
                    }
                }
                while (reader.Read());
                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void WriteToStream(Stream stream)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = System.Text.Encoding.UTF8,
                    Indent = false,
                    OmitXmlDeclaration = false
                };
                using (XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateDictionaryWriter(XmlDictionaryWriter.Create(stream, settings)))
                {
                    OnWriteBodyContents(xdw);
                    xdw.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        
    }
}
