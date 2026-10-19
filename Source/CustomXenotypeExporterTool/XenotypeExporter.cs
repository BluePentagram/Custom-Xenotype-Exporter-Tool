using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomXenotypeExporterTool;

public static class XenotypeExporter
{
	private static readonly string storageModFolderName = "ExportedXenotypeStorageModFolder";

	private static readonly string aboutFolderName = "About";

	private static readonly string currentVersionName = VersionControl.CurrentVersionStringWithoutBuild ;

	private static readonly string defsFolderName = "Defs";

	private static readonly string xenotypeDefsFolderName = "XenotypeDefs";

	private static readonly string patchesFolderName = "Patches";

	private static readonly string aboutFileName = "About.xml";

	private static readonly string authorName = "Gege";

	public static XenotypeDef existingXenotype;

	public static void ExportXenotype(ExportVariant exportVariant, string xenotypeName, XenotypeIconDef icon, string description, string descriptionShort, List<GeneDef> geneList, bool inheritable)
	{
		string path;
		string path2;
		string path3;
		string path4;
		switch (exportVariant)
		{
		case ExportVariant.ToClipboard:
			try
			{
				GUIUtility.systemCopyBuffer = GenerateXenotypeDefFile(xenotypeName, icon, description, descriptionShort, geneList, inheritable).ToString();
				Messages.Message("Copied".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				break;
			}
			catch (Exception)
			{
				Log.Error("Could not copy");
				break;
			}
		case ExportVariant.ToDef:
			try
			{
				if (xenotypeName.Length == 0)
				{
					throw new ArgumentOutOfRangeException("Xenotype name cannot be empty");
				}
				if (description.Length == 0)
				{
					throw new ArgumentOutOfRangeException("Description cannot be empty");
				}
				GetOrCreateStorageModFolders(out path, out path2, out path3, out path4);
				GenerateAboutFile(path);
				XDocument xDocument = new XDocument(new XElement("Defs"));
				xDocument.Element("Defs").Add(GenerateXenotypeDefFile(xenotypeName, icon, description, descriptionShort, geneList, inheritable));
				xDocument.Save(Path.Combine(path3, xenotypeName + ".xml"));
				Messages.Message("ExportComplete".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				break;
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				Messages.Message("ExportFailed".Translate(ex4.Message), MessageTypeDefOf.ThreatBig, historical: false);
				break;
			}
			catch (Exception)
			{
				Log.Error("Could not access mod folders");
				break;
			}
		case ExportVariant.AsPatch:
			try
			{
				if (existingXenotype == null)
				{
					throw new ArgumentOutOfRangeException("Choose an existing xenotype");
				}
				GetOrCreateStorageModFolders(out path, out path2, out path3, out path4);
				GenerateAboutFile(path);
				GeneratePatchFile(xenotypeName, icon, description, descriptionShort, geneList, inheritable, path4);
				Messages.Message("ExportComplete".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				break;
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				Messages.Message("ExportFailed".Translate(ex2.Message), MessageTypeDefOf.ThreatBig, historical: false);
				break;
			}
			catch (Exception)
			{
				Log.Error("Could not access mod folders");
				break;
			}
		case ExportVariant.OpenFolder:
			try
			{
				GetOrCreateStorageModFolders(out path, out path2, out path3, out path4);
				GenerateAboutFile(path);
				Application.OpenURL(path2);
				break;
			}
			catch (Exception)
			{
				Log.Error("Could not access mod folders");
				break;
			}
		}
	}

	public static void GetOrCreateStorageModFolders(out string path2, out string path3, out string path5, out string path6)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(new DirectoryInfo(GenFilePaths.ModsFolderPath).ToString(), storageModFolderName));
		if (!directoryInfo.Exists)
		{
			directoryInfo.Create();
		}
		path2 = Path.Combine(directoryInfo.ToString(), aboutFolderName);
		DirectoryInfo directoryInfo2 = new DirectoryInfo(path2);
		if (!directoryInfo2.Exists)
		{
			directoryInfo2.Create();
		}
		path3 = Path.Combine(directoryInfo.ToString(), currentVersionName);
		DirectoryInfo directoryInfo3 = new DirectoryInfo(path3);
		if (!directoryInfo3.Exists)
		{
			directoryInfo3.Create();
		}
		DirectoryInfo directoryInfo4 = new DirectoryInfo(Path.Combine(directoryInfo3.ToString(), defsFolderName));
		if (!directoryInfo4.Exists)
		{
			directoryInfo4.Create();
		}
		path5 = Path.Combine(directoryInfo4.ToString(), xenotypeDefsFolderName);
		DirectoryInfo directoryInfo5 = new DirectoryInfo(path5);
		if (!directoryInfo5.Exists)
		{
			directoryInfo5.Create();
		}
		path6 = Path.Combine(directoryInfo3.ToString(), patchesFolderName);
		DirectoryInfo directoryInfo6 = new DirectoryInfo(path6);
		if (!directoryInfo6.Exists)
		{
			directoryInfo6.Create();
		}
	}

	public static void GenerateAboutFile(string aboutFolderLocation)
	{
		string fileName = Path.Combine(aboutFolderLocation, aboutFileName);
		new XDocument(new XElement("ModMetaData", new XElement("name", storageModFolderName), new XElement("author", authorName), new XElement("packageId", authorName.ToLower() + "." + storageModFolderName.ToLower()), new XElement("supportedVersions", new XElement("li", currentVersionName)), new XElement("description", "Exported xenotypes storage."), new XElement("modDependencies", new XElement("li", new XElement("packageId", "Ludeon.RimWorld.Biotech"), new XElement("displayName", "Biotech"))), new XElement("loadAfter", new XElement("li", "Ludeon.RimWorld.Biotech")))).Save(fileName);
	}

	public static XElement GenerateXenotypeDefFile(string xenotypeName, XenotypeIconDef icon, string description, string descriptionShort, List<GeneDef> geneList, bool inheritable)
	{
		string iconPath = icon.texPath;

        if (DefDatabase<XenotypeDef>.GetNamedSilentFail(xenotypeName) != null)
		{ 
			iconPath = DefDatabase<XenotypeDef>.GetNamed(xenotypeName).iconPath; 
		}

		XElement xElement = new XElement("XenotypeDef", 
		new XElement("defName", "GegeXT_" + xenotypeName.Replace(" ", "")), 
		new XElement("label", xenotypeName), 
		new XElement("description", description), 
		new XElement("descriptionShort", descriptionShort), 
		new XElement("iconPath", iconPath), 
		new XElement("inheritable", inheritable), 
		new XElement("genes"));
		XElement xElement2 = xElement.Element("genes");
		AddGeneToListXML(xElement2,geneList);
		return xElement;
	}

	private static void AddGeneToListXML(XElement Genes, List<GeneDef> geneList)
	{
		var geneAddedByCode = new List<GeneDef>();
		foreach (GeneDef gene in geneList)
		{
			if (gene.modContentPack == null)
			{
				Genes.Add(new XElement("li", new XAttribute("MayRequire", "ludeon.rimworld"), gene.defName)); // C# Added Gene that does not have modContentPack
				geneAddedByCode.Add(gene);
				continue;
			}
			if (gene.modContentPack.IsCoreMod || gene.modContentPack.PackageId == "ludeon.rimworld.biotech")
			{
				Genes.Add(new XElement("li", gene.defName)); //No additional DLC or Mod's need to be enabled for these versions
				continue;
			}
			Genes.Add(new XElement("li", new XAttribute("MayRequire", gene.modContentPack.PackageId), gene.defName)); // additional DLC or Mod's need to be enabled for these versions
		}
		if (!geneAddedByCode.Empty())
		{
			Log.Error("Exported Xenotype has genes that don't have any packageID, reason being it was mostly added by code that depends on other mods, they have been marked as useing base game package. Printed Below.");
			foreach (GeneDef genestring in geneAddedByCode)
			{
				Log.Message(genestring);
			}
		}
	}

	public static void GeneratePatchFile(string xenotypeName, XenotypeIconDef icon, string description, string descriptionShort, List<GeneDef> geneList, bool inheritable, string patchesFolderLocation)
	{
		string fileName = Path.Combine(patchesFolderLocation, existingXenotype.defName + ".xml");
		//string iconPath = existingXenotype.iconPath; //Was just replaceing the icon path with the same icon path
		// My Newer way
        XDocument xDocument = new(new XElement("Patch"));

		XElement xElement = xDocument.Element("Patch");

		// Patch the name of the Xenotype
		if (xenotypeName.Length != 0)
		{
			xElement.Add(new XElement("Operation", 
			new XAttribute("Class", "PatchOperationConditional"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/label"), 
			new XElement("nomatch", new XAttribute("Class", "PatchOperationAdd"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]"), 
			new XElement("value", new XElement("label", xenotypeName))), 
			new XElement("match", new XAttribute("Class", "PatchOperationReplace"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/label"), 
			new XElement("value", new XElement("label", xenotypeName)))));
		}
		// Patch the description of the Xenotype
		if (description.Length != 0)
		{
			xElement.Add(new XElement("Operation", 
			new XAttribute("Class", "PatchOperationConditional"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/description"), 
			new XElement("nomatch", new XAttribute("Class", "PatchOperationAdd"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]"), 
			new XElement("value", new XElement("description", description))), 
			new XElement("match", new XAttribute("Class", "PatchOperationReplace"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/description"), 
			new XElement("value", new XElement("description", description)))));
		}
		// Patch the description of the Xenotype
		if (descriptionShort.Length != 0)
		{
			xElement.Add(new XElement("Operation", 
			new XAttribute("Class", "PatchOperationConditional"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/descriptionShort"), 
			new XElement("nomatch", new XAttribute("Class", "PatchOperationAdd"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]"), 
			new XElement("value", new XElement("descriptionShort", descriptionShort))), 
			new XElement("match", new XAttribute("Class", "PatchOperationReplace"), 
			new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/descriptionShort"), 
			new XElement("value", new XElement("descriptionShort", descriptionShort)))));
		}
		// Patch the Inheritable of the Xenotype
		xElement.Add(new XElement("Operation", 
		new XAttribute("Class", "PatchOperationConditional"), 
		new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/inheritable"), 
		new XElement("nomatch", new XAttribute("Class", "PatchOperationAdd"), 
		new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]"), 
		new XElement("value", new XElement("inheritable", inheritable))), 
		new XElement("match", new XAttribute("Class", "PatchOperationReplace"), 
		new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/inheritable"), 
		new XElement("value", new XElement("inheritable", inheritable)))));
		// Patch the Genes of the Xenotype
		if (!geneList.Empty())
		{
			XElement xElement2 = new XElement("genes");
			AddGeneToListXML(xElement2,geneList);

			xElement.Add(new XElement("Operation", 
				new XAttribute("Class", "PatchOperationConditional"), 
				new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/genes"), 
				new XElement("nomatch", new XAttribute("Class", "PatchOperationAdd"), 
					new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]"), 
					new XElement("value", xElement2)
				),
				new XElement("match", new XAttribute("Class", "PatchOperationReplace"), 
					new XElement("xpath", "Defs/XenotypeDef[defName=\"" + existingXenotype.defName + "\"]/genes"), 
					new XElement("value", xElement2)
				))
			);
		}
		xDocument.Save(fileName);
	}
}
