using UnityEngine;

namespace Terraria
{
    [CreateAssetMenu]
    public class ColorSchema : ScriptableObject
    {
        [SerializeField] Color minDensityColor;
        [SerializeField] Color maxDensityColor;
        public Color grassColor;
        
        public Color ColorFromDensity(float density)
        {
            return Color.Lerp(minDensityColor, maxDensityColor, density);
        }
    }
}