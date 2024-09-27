using System;
using System.IO;
using System.Linq;
using IesEdit.Ies;

namespace IesEdit
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Usage XML to IES: iesedit -c <input.xml> <output.ies> | -d <folder> [-r] [output folder]");
				Console.WriteLine("Usage IES to XML: iesedit -x <input.ies> <output.xml> | -dx <folder> [-r] [output folder]");
				return;
			}

			switch (args[0])
			{
				case "-c":
					{
						if (args.Length < 3)
						{
							Console.WriteLine("Usage: iesedit -c <input.xml> <output.ies>");
							return;
						}
						ConvertXmlToIes(args[1], args[2]);
						break;
					}

				case "-d":
					{
						if (args.Length < 2)
						{
							Console.WriteLine("Usage: iesedit -d <input folder> [-r] [output folder]");
							return;
						}

						var recursive = args.Length > 2 && args[2] == "-r";
						var outputFolder = (args.Length > 3) ? args[3] : null;
						ConvertAllXmlInDirectory(args[1], recursive, outputFolder);
						break;
					}
				case "-x":
					{
						if (args.Length < 3)
						{
							Console.WriteLine("Usage: iesedit -x <input.ies> <output.xml>");
							return;
						}
						ConvertIesToXml(args[1], args[2]);
						break;
					}
				case "-dx":
					{
						if (args.Length < 2)
						{
							Console.WriteLine("Usage: iesedit -dx <input folder> [-r] [output folder]");
							return;
						}

						var recursive = args.Length > 2 && args[2] == "-r";
						var outputFolder = (args.Length > 3) ? args[3] : null;
						ConvertAllIesInDirectory(args[1], recursive, outputFolder);
						break;
					}

				default:
					Console.WriteLine("Invalid option. Usage: iesedit -c <input.xml> <output.ies> | -d <input folder> [-r] [output folder]");
					break;
			}
		}

		static void ConvertXmlToIes(string xmlFilePath, string iesFilePath)
		{
			try
			{
				var iesFile = IesFile.LoadXmlFile(xmlFilePath);
				iesFile.SaveIes(iesFilePath);

				Console.WriteLine($"Converted {xmlFilePath} to {iesFilePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error converting {xmlFilePath}: {ex.Message}");
			}
		}

		static void ConvertAllXmlInDirectory(string directoryPath, bool recursive, string outputFolder)
		{
			if (!Directory.Exists(directoryPath))
			{
				Console.WriteLine($"Directory not found: {directoryPath}");
				return;
			}

			var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var xmlFiles = Directory.GetFiles(directoryPath, "*.xml", searchOption);

			foreach (var xmlFile in xmlFiles)
			{
				var outputDirectory = outputFolder ?? Path.GetDirectoryName(xmlFile);
				if (!Directory.Exists(outputDirectory))
				{
					Directory.CreateDirectory(outputDirectory);
				}

				var iesFilePath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(xmlFile) + ".ies");
				ConvertXmlToIes(xmlFile, iesFilePath);
			}
		}

		static void ConvertIesToXml(string iesFilePath, string xmlFilePath)
		{
			try
			{
				var iesFile = IesFile.LoadIesFile(iesFilePath);
				iesFile.SaveXml(xmlFilePath);

				Console.WriteLine($"Converted {iesFilePath} to {xmlFilePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error converting {iesFilePath}: {ex.Message}");
			}
		}

		static void ConvertAllIesInDirectory(string directoryPath, bool recursive, string outputFolder)
		{
			if (!Directory.Exists(directoryPath))
			{
				Console.WriteLine($"Directory not found: {directoryPath}");
				return;
			}

			var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var iesFiles = Directory.GetFiles(directoryPath, "*.ies", searchOption);

			foreach (var iesFile in iesFiles)
			{
				var outputDirectory = outputFolder ?? Path.GetDirectoryName(iesFile);
				if (!Directory.Exists(outputDirectory))
				{
					Directory.CreateDirectory(outputDirectory);
				}
				var xmlFilePath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(iesFile) + ".xml");
				ConvertIesToXml(iesFile, xmlFilePath);
			}
		}
	}
}
