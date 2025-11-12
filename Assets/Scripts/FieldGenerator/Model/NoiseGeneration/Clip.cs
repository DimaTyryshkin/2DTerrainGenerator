using GamePackages.Core.Validation;
using UnityEngine;

namespace FieldGenerator.Terraria.NoiseGeneration
{
    [CreateAssetMenu(fileName = "Clip")]
    public sealed class Clip : Function3D
    {
        [SerializeField, IsntNull] Function3D noise;
        [SerializeField, IsntNull] Function3D clipNoise;

        public override float GetValue(Vector3 p)
        {
            if (clipNoise.GetValue(p) < 0)
                return 0;
            else
                return noise.GetValue(p);
        }
    }
}