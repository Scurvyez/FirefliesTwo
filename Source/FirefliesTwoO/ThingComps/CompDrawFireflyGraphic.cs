using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class CompDrawFireflyGraphic : ThingComp
    {
        private Material _fireflyMat;
        private Color _fireflyColor;
        private readonly MaterialPropertyBlock _mpb = new();
        
        private int _flickerStartTick;
        private int _flickerDuration;
        private int _nextFlickerTick;
        private float _currentAlpha = 0f;
        private bool _isFlickering;
        
        private Vector2 _startOffset;
        private Vector2 _endOffset;
        private int _transitionStartTick;
        private int _transitionDuration;
        private Vector2 _controlPoint;
        private float _step;
        private float _maxOffsetX;
        private float _maxOffsetZ;
        
        private CompProperties_DrawFireflyGraphic Props 
            => (CompProperties_DrawFireflyGraphic)props;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            
            _fireflyColor = ColorManager.RandomWeightedColor();
            Vector2 drawSize = parent.def.graphicData.drawSize;
            _maxOffsetX = drawSize.x * Props.maxOffsetFactorX;
            _maxOffsetZ = drawSize.y * Props.maxOffsetFactorZ;
        }
        
        public override void CompTick()
        {
            base.CompTick();
            
            int ticksGame = Find.TickManager.TicksGame;
            int ticksSinceStart = ticksGame - _transitionStartTick;
            if (ticksSinceStart >= _transitionDuration)
            {
                BeginNewFlutter();
                ticksSinceStart = 0;
            }
            
            _step = ticksSinceStart / (float)_transitionDuration;
            _step = Mathf.SmoothStep(0f, 1f, _step);
            
            if (_isFlickering)
            {
                int flickerProgress = ticksGame - _flickerStartTick;
                float flickerStep = flickerProgress / (float)_flickerDuration;
                
                _currentAlpha = flickerStep < 0.5f 
                    ? Mathf.SmoothStep(0f, 1f, flickerStep * 2f) 
                    : Mathf.SmoothStep(1f, 0f, (flickerStep - 0.5f) * 2f);
                
                if (flickerProgress < _flickerDuration) 
                    return;
                
                _isFlickering = false;
                _currentAlpha = 0f;
                _nextFlickerTick = Props.nextFlickerTick.RandomInRange;
            }
            else if (ticksGame >= _nextFlickerTick && parent.IsHashIntervalTick(120))
            {
                BeginNewFlicker();
            }
        }
        
        public override void PostDraw()
        {
            if (Props.fireFlyGraphic == null)
                return;
            
            if (_fireflyMat == null)
            {
                _fireflyMat = Props.fireFlyGraphic.Graphic.MatSingle;
            }

            Vector3 drawPos = parent.DrawPos;
            if (parent is {  } twc)
            {
                CompAdjustableDrawPos drawOffsetComp = twc
                    .GetComp<CompAdjustableDrawPos>();
                if (drawOffsetComp != null)
                {
                    drawPos.x += drawOffsetComp.OffsetX;
                    drawPos.z += drawOffsetComp.OffsetZ;
                }
            }
            
            Vector2 interpolatedOffset = 
                Mathf.Pow(1f - _step, 2) * _startOffset +
                2f * (1f - _step) * _step * _controlPoint +
                Mathf.Pow(_step, 2) * _endOffset;
            
            float interpX = interpolatedOffset.x;
            float interpZ = interpolatedOffset.y;
            float normX = interpX / _maxOffsetX;
            float normZ = interpZ / _maxOffsetZ;
            float distSq = normX * normX + normZ * normZ;
            
            if (distSq > 1f)
            {
                float scale = 1f / Mathf.Sqrt(distSq);
                interpX *= scale;
                interpZ *= scale;
            }
            
            drawPos.x += interpX;
            drawPos.z += interpZ - Props.initialOffsetZ;
            
            _mpb.Clear();
            
            Color drawColor = _fireflyColor;
            drawColor.a = _currentAlpha;
            _mpb.SetColor(ShaderPropertyIDs.Color, drawColor);
            
            Graphics.DrawMesh(MeshPool.plane10,
                Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one),
                _fireflyMat, 0, null, 0, _mpb);
        }
        
        private void BeginNewFlutter()
        {
            _startOffset = _endOffset;
            Vector2 unitCircle = Rand.InsideUnitCircle;
            _endOffset = new Vector2(unitCircle.x * _maxOffsetX, unitCircle.y * _maxOffsetZ);
            _controlPoint = (_startOffset + _endOffset) * 0.5f + Rand.UnitVector2 * 0.1f;
            _transitionStartTick = Find.TickManager.TicksGame;
            _transitionDuration = Props.transitionDuration.RandomInRange;
        }
        
        private void BeginNewFlicker()
        {
            _isFlickering = true;
            _flickerStartTick = Find.TickManager.TicksGame;
            _flickerDuration = Props.flickerDuration.RandomInRange;
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref _fireflyColor, "_fireflyColor");
            Scribe_Values.Look(ref _flickerStartTick, "_flickerStartTick");
            Scribe_Values.Look(ref _flickerDuration, "_flickerDuration", 90);
            Scribe_Values.Look(ref _nextFlickerTick, "_nextFlickerTick");
            Scribe_Values.Look(ref _currentAlpha, "_currentAlpha");
            Scribe_Values.Look(ref _isFlickering, "_isFlickering");
            Scribe_Values.Look(ref _startOffset, "_startOffset");
            Scribe_Values.Look(ref _endOffset, "_endOffset");
            Scribe_Values.Look(ref _transitionStartTick, "_transitionStartTick");
            Scribe_Values.Look(ref _transitionDuration, "_transitionDuration", 60);
            Scribe_Values.Look(ref _controlPoint, "_controlPoint");
            Scribe_Values.Look(ref _step, "_step");
            Scribe_Values.Look(ref _maxOffsetX, "_maxOffsetX");
            Scribe_Values.Look(ref _maxOffsetZ, "_maxOffsetZ");
        }
    }
}