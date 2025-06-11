using UnityEngine;

namespace FirefliesTwoO
{
    public static class LifeTimeSetter
    {
        public static AnimationCurve GetMinLifetimeCurve()
        {
            AnimationCurve minCurve = new ();
            minCurve.AddKey(0.75f,1.75f);
            minCurve.AddKey(0.90f,2.25f);
            minCurve.AddKey(1f,2.25f);

            return minCurve;
        }
        
        public static AnimationCurve GetMaxLifetimeCurve()
        {
            AnimationCurve minCurve = new ();
            minCurve.AddKey(0.75f,2.25f);
            minCurve.AddKey(0.90f,2.65f);
            minCurve.AddKey(1f,3.2f);

            return minCurve;
        }
    }
}