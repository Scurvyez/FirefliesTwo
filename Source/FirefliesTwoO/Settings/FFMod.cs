using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace FirefliesTwoO
{
    public class FFMod : Mod
    {
        public static FFMod mod;

        public FFSettings settings;
        private float _halfWidth;
        private Vector2 _leftScrollPos = Vector2.zero;
        private Vector2 _rightScrollPos = Vector2.zero;
        private readonly Color _headerTextColor = new(0.4f, 1.0f, 0.54902f);

        private const float _newSectionGap = 6f;
        private const float _headerTextGap = 3f;
        private const float _spacing = 10f;
        private const float _sliderSpacing = 120f;
        private const float _labelWidth = 200f;
        private const float _textFieldWidth = 100f;
        private const float _elementHeight = 30f;

        public FFMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<FFSettings>();
        }
        
        public override string SettingsCategory() => "FF_ModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            _halfWidth = (inRect.width - 30) / 2;
            LeftSideScrollViewHandler(new Rect(inRect.x, inRect.y, _halfWidth, inRect.height));
        }

        private void LeftSideScrollViewHandler(Rect inRect)
        {
            Listing_Standard list1 = new();

            int biomeCount = DefDatabase<BiomeDef>.AllDefsListForReading
                .Count(b => b.HasModExtension<NightlySpawningExtension>());
            float biomeSectionHeight = _elementHeight + _spacing;
            float totalHeight = biomeCount * (biomeSectionHeight * 2f);

            Rect viewRect1 = new(inRect.x, inRect.y, inRect.width, inRect.height - 40);
            Rect vROffset1 = new(0, 0, inRect.width - 16f, totalHeight);
            Widgets.BeginScrollView(viewRect1, ref _leftScrollPos, vROffset1);

            list1.Begin(vROffset1);
            list1.Gap(_newSectionGap);
            
            list1.Label("FF_SpawnRateHeader".Translate().Colorize(_headerTextColor));
            list1.Gap(_headerTextGap);

            foreach (BiomeDef biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
            {
                if (!biomeDef.HasModExtension<NightlySpawningExtension>()) continue;
                if (!settings.biomeSpawnRates.TryGetValue(biomeDef.defName, out float currentSpawnRate))
                {
                    currentSpawnRate = 1f;
                    settings.biomeSpawnRates[biomeDef.defName] = currentSpawnRate;
                }

                Rect inRectForSlider = new(0, list1.CurHeight, list1.ColumnWidth, _elementHeight);
                DrawSettingWithSliderAndTextField(inRectForSlider, list1, biomeDef.LabelCap,
                    ref currentSpawnRate, 0f, 10f,
                    biomeDef.GetModExtension<NightlySpawningExtension>().biomeEmissionRate);

                settings.biomeSpawnRates[biomeDef.defName] = currentSpawnRate;
                list1.Gap(_newSectionGap);
            }

            list1.Gap(_newSectionGap);
            list1.End();
            Widgets.EndScrollView();
        }
        
        private static void DrawSettingWithSliderAndTextField<T>(Rect inRect, Listing_Standard list, string labelText, 
            ref T settingValue, T minValue, T maxValue, T defaultValue) where T : struct, IConvertible
        {
            float sliderWidth = inRect.width - _sliderSpacing;
            float settingFloat = Convert.ToSingle(settingValue);
            float minFloat = Convert.ToSingle(minValue);
            float maxFloat = Convert.ToSingle(maxValue);

            Rect labelRect = new(0, list.CurHeight, _labelWidth, _elementHeight);
            Widgets.Label(labelRect, labelText);

            list.Gap(_elementHeight);

            Rect sliderRect = new(0, list.CurHeight, sliderWidth, _elementHeight);
            float sliderValue = settingFloat;
            sliderValue = Widgets.HorizontalSlider(sliderRect, sliderValue, minFloat, maxFloat, true);
            settingFloat = sliderValue;
            settingValue = (T)Convert.ChangeType(settingFloat, typeof(T));

            Rect valueLabelRect = new(sliderWidth + _spacing, list.CurHeight, _textFieldWidth, _elementHeight);
            Widgets.Label(valueLabelRect, settingFloat.ToString("F1"));
            
            Rect resetButtonRect = new(sliderWidth + _spacing, labelRect.y, 50f, _elementHeight);
            if (Widgets.ButtonText(resetButtonRect, "FF_Reset".Translate()))
            {
                settingFloat = Convert.ToSingle(defaultValue);
                settingValue = (T)Convert.ChangeType(settingFloat, typeof(T));
            }
            list.Gap(_spacing + _elementHeight);
        }
    }
}