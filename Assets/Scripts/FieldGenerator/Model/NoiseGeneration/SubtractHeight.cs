using GamePackages.Core.Validation;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    public sealed class SubtractHeight : Function3D
    {
        [SerializeField, IsntNull] Function3D noiseGenerator;
        [SerializeField, IsntNull] FieldSettings noiseField;

        public override float GetValue(Vector3 p)
        {
            return noiseGenerator.GetValue(p) - p.y * noiseField.HeightScale;
        }
    }
}