using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomXenotypeExporterTool;

[HarmonyPatch(typeof(Dialog_CreateXenotype), "DrawSearchRect")]
internal class Patch
{
	private static void Postfix(Rect rect, ref string ___xenotypeName, ref XenotypeIconDef ___iconDef, ref List<GeneDef> ___selectedGenes, ref bool ___inheritable)
	{
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        if (Widgets.ButtonText(new Rect(200f, rect.y, 150f, 38f), "ExportXenotype".Translate()))
		{
			Find.WindowStack.Add(new Dialog_ExportXenotype(___xenotypeName, ___iconDef, ___selectedGenes, ___inheritable));
		}
	}
}
