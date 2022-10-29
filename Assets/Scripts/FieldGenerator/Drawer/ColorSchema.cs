using UnityEngine;

namespace FieldGenerator
{
    [CreateAssetMenu]
    public class ColorSchema : ScriptableObject
    {
        [SerializeField] Color minDensityColor;
        [SerializeField] Color maxDensityColor;
        public Color grassColor;

        public Color ColorFromDensity(float density, float minDensity, float maxDensity)
        {
            float k = (density - minDensity) / (maxDensity - minDensity);
            return Color.Lerp(minDensityColor, maxDensityColor, k);
        }
    }
}