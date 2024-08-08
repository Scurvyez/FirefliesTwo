using System.IO;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class FFMod : Mod
    {
        public static FFMod mod;
        
        public FFMod(ModContentPack content) : base(content)
        {
            mod = this;

            Harmony harmony = new ("com.firefliestwoo");
            harmony.PatchAll();
        }
    }
}