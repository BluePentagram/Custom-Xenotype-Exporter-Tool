using HarmonyLib;
using Verse;

namespace CustomXenotypeExporterTool;

[StaticConstructorOnStartup]
public static class CustomXenotypeExporterTool
{
	static CustomXenotypeExporterTool()
	{
		new Harmony("CustomXenotypeExporterTool").PatchAll();
	}
}
