using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public class Textures
    {
        public static readonly Texture2D Firefly = ContentFinder<Texture2D>.Get("Effects/Firefly");
        public static readonly Texture2D Butterfly = ContentFinder<Texture2D>.Get("Effects/Butterfly");
        public static readonly Texture2D ButterflyMask = ContentFinder<Texture2D>.Get("Effects/Butterfly_m");
    }
}