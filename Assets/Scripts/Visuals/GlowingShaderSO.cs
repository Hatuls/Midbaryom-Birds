using System;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Visual
{
    [CreateAssetMenu (menuName ="ScriptableObjects/Shaders/New Glowing Shader Data/New Glowing Shader Config")]
    public class GlowingShaderSO : ScriptableObject
    {
        public const string SHADER_ACTIVATION_REFERENCE = "Vector1_efb32ed04052442bbc005a3c2485ecb2";
        public const string SHADER_TIMER_REFERENCE = "Vector1_caa8068c50e24ca09d6d31ae7037302c";
        public const string SHADER_COLOR_REFERENCE = "Color_e8e2b54f0fe540f28aa988fec94910b9";
        public const string SHADER_HIGHLIGHT_SCALE_REFERENCE = "Vector1_2285d45a899e40c3af4f6f3a744cba32";
        public const string SHADER_FRESNEL_SCALE_REFERENCE = "Vector1_f03b909ccbd8434684d466b60fffbfaa";
        private const int ACTIVATE = 0;
        private const int DEACTIVATE = 1;

        [SerializeField]
        private GlowingEffect _redEffect;
        [SerializeField]
        private GlowingEffect _whieEffect;

        public GlowingEffect RedEffect => _redEffect;
        public GlowingEffect WhiteEffect => _whieEffect;
        public void ApplyEffect(IReadOnlyList<Material> materials, GlowingEffect glowingEffect)
        {
            if (materials == null )
                return;
            
            int count = materials.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Material material = materials[i];
                    material.SetFloat(SHADER_TIMER_REFERENCE, glowingEffect.Timer);
                    material.SetFloat(SHADER_HIGHLIGHT_SCALE_REFERENCE, glowingEffect.HighLightPower);
                    material.SetFloat(SHADER_FRESNEL_SCALE_REFERENCE, glowingEffect.FresnelPower);
                    material.SetColor(SHADER_COLOR_REFERENCE, glowingEffect.Color);
                    material.SetFloat(SHADER_ACTIVATION_REFERENCE, ACTIVATE);
                }
            }
        }

        public void RemoveEffect(IReadOnlyList<Material> materials)
        {
            if (materials == null)
                return;

            int count = materials.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    materials[i].SetFloat(SHADER_ACTIVATION_REFERENCE, DEACTIVATE);
                }
            }
        }

        [Serializable]
        public class GlowingEffect
        {
            public Color Color;
            public float Timer = 1f;
            [Range(0,2f)]
            public float FresnelPower = 3.19f;
            [Range(0,100f)]
            public float HighLightPower = 6.51f;
        }
    }


}