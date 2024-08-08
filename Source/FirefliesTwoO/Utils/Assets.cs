using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    [StaticConstructorOnStartup]
    public class Assets
    {
        public static readonly Texture2D Firefly = ContentFinder<Texture2D>.Get("Effects/Firefly");
    }
}