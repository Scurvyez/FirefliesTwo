using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public static class Textures
    {
        public static readonly Texture2D Firefly = ContentFinder<Texture2D>.Get("Effects/Firefly");
    }
}