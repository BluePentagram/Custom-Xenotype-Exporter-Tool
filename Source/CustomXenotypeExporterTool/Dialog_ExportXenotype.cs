using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomXenotypeExporterTool;

public class Dialog_ExportXenotype : Window
{
	private string customXenotypeName;

	private XenotypeIconDef customIcon;

	private string customDescription;

	private string customDescriptionShort;

	private bool customInheritable;

	private List<GeneDef> customXenotypeGenes = new List<GeneDef>();

	public override Vector2 InitialSize => new Vector2(base.InitialSize.x, (float)UI.screenHeight);

	public Dialog_ExportXenotype(string xenotypeName, XenotypeIconDef icon, List<GeneDef> xenotypeGenes, bool inheritable)
	{
		customXenotypeName = xenotypeName;
		customIcon = icon;
		customXenotypeGenes = xenotypeGenes;
		customInheritable = inheritable;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.Begin(inRect);
		listing_Standard.Label("XenotypeExporter".Translate());
		listing_Standard.GapLine();
		listing_Standard.Label("XenotypeDescription".Translate());
		customDescription = listing_Standard.TextEntry(customDescription, 6);
		listing_Standard.Label("XenotypeDescriptionShort".Translate());
		customDescriptionShort = listing_Standard.TextEntry(customDescriptionShort, 3);
		listing_Standard.GapLine();
		if (listing_Standard.ButtonText("CopyToClipboard".Translate()))
		{
			XenotypeExporter.ExportXenotype(ExportVariant.ToClipboard, customXenotypeName, customIcon, customDescription, customDescriptionShort, customXenotypeGenes, customInheritable);
		}
		if (listing_Standard.ButtonText("ExportToLocalModsFolder".Translate()))
		{
			XenotypeExporter.ExportXenotype(ExportVariant.ToDef, customXenotypeName, customIcon, customDescription, customDescriptionShort, customXenotypeGenes, customInheritable);
		}
		listing_Standard.GapLine();
		if (listing_Standard.ButtonText((XenotypeExporter.existingXenotype == null) ? "ChooseExistingXenotype".Translate().ToString() : XenotypeExporter.existingXenotype.ToString()))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (XenotypeDef xenotype in DefDatabase<XenotypeDef>.AllDefsListForReading)
			{
				list.Add(new FloatMenuOption(xenotype.defName, delegate
				{
					XenotypeExporter.existingXenotype = xenotype;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		if (listing_Standard.ButtonText("ExportAsPatchToLocalModsFolder".Translate()))
		{
			XenotypeExporter.ExportXenotype(ExportVariant.AsPatch, customXenotypeName, customIcon, customDescription, customDescriptionShort, customXenotypeGenes, customInheritable);
		}
		listing_Standard.GapLine();
		if (listing_Standard.ButtonText("OpenExportedFolder".Translate()))
		{
			XenotypeExporter.ExportXenotype(ExportVariant.OpenFolder, customXenotypeName, customIcon, customDescription, customDescriptionShort, customXenotypeGenes, customInheritable);
		}
		listing_Standard.GapLine();
		if (listing_Standard.ButtonText("Close".Translate()))
		{
			Close();
		}
		listing_Standard.End();
	}
}
