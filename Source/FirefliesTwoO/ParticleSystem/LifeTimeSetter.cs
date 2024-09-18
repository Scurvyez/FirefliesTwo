using UnityEngine;

namespace FirefliesTwoO
{
    public static class LifeTimeSetter
    {
        public static AnimationCurve GetLifetimeCurve()
        {
            AnimationCurve minCurve = new ();
            minCurve.AddKey(0.75f,Random.Range(0.1f, 1.5f));
            minCurve.AddKey(0.90f,Random.Range(1.6f, 2.35f));
            minCurve.AddKey(1f,Random.Range(2.36f, 4.2f));

            return minCurve;
        }
    }
}