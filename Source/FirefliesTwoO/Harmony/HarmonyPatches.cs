using HarmonyLib;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new (id: "com.firefliestwoo");
        }
    }
}