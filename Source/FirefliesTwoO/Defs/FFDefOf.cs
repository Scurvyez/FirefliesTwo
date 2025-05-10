using RimWorld;
using Verse;

namespace FirefliesTwoO
{
    [DefOf]
    public class FFDefOf
    {
        public static FFConfigDef FF_Config;
        
        public static ThoughtDef FF_ChasedFireflies;
        public static ThoughtDef FF_SawFireflies;
        public static ThoughtDef FF_CaughtAFirefly;
        public static ThoughtDef FF_SquishedAFirefly;
        
        public static TraitDef Nimble;
        public static TraitDef NightOwl;

        public static ThingDef FF_Mote_BugNet;
        public static ThingDef FF_JarGlassA;
        
        static FFDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FFDefOf));
        }
    }
}