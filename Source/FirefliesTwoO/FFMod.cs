using Verse;

namespace FirefliesTwoO
{
    public class FFMod : Mod
    {
        public static FFMod mod;
        
        public FFMod(ModContentPack content) : base(content)
        {
            mod = this;
        }
    }
}