using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;

namespace IesEdit.Ies
{
	public class XmlTsvConverter
	{
		// XML to TSV Conversion
		public static void XmlToTsv(string xml, string tsvFilePath)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);

			using var writer = new StreamWriter(tsvFilePath);
			// Get attribute names from the first Class node (assuming all have same attributes)
			var firstClassNode = doc.SelectSingleNode("//Class");
			if (firstClassNode != null)
			{
				var headerRow = string.Join("\t", firstClassNode.Attributes.Cast<XmlAttribute>().Select(a => a.Name));
				writer.WriteLine(headerRow);
			}

			foreach (XmlNode node in doc.SelectNodes("//Class"))
			{
				var values = node.Attributes.Cast<XmlAttribute>().Select(a => a.Value);
				writer.WriteLine(string.Join("\t", values));
			}
		}

		// TSV to XML Conversion
		public static string TsvToXml(string tsvFilePath, string rootElementName = "Category")
		{
			var doc = new XmlDocument();
			var root = doc.CreateElement(rootElementName);
			doc.AppendChild(root);

			string[] attributeNames = null;

			using (var reader = new StreamReader(tsvFilePath))
			{
				// Get attribute names from the header row
				attributeNames = reader.ReadLine().Split('\t');

				while (!reader.EndOfStream)
				{
					var values = reader.ReadLine().Split('\t');
					var element = doc.CreateElement("Class");

					// Add attributes to the element
					for (var i = 0; i < values.Length; i++)
					{
						if (string.IsNullOrEmpty(attributeNames[i]))
							continue;
						element.SetAttribute(attributeNames[i], values[i]);
					}

					root.AppendChild(element);
				}
			}

			//doc.Save(xmlFilePath);

			// Convert XML document to string
			var sb = new StringBuilder();
			using (var writer = XmlWriter.Create(sb, new XmlWriterSettings
			{
				Indent = true,
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = false // Important: Include the XML declaration
			}))
			{
				doc.Save(writer);
			}

			return sb.ToString();
		}
	}
}
