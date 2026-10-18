using HarmonyLib;
using Verse;

namespace CustomXenotypeExporterTool;

[StaticConstructorOnStartup]
public static class CustomXenotypeExporterTool
{
	static CustomXenotypeExporterTool()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		new Harmony("CustomXenotypeExporterTool").PatchAll();
	}
}
