using UnityEngine;

namespace SiberianWellness.Common
{
    [System.Serializable]
    public struct RangeMinMax
    {
        public float min;
        public float max;

        public RangeMinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }


        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }

        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

    [System.Serializable]
    public struct RangeMinMaxInt
    {
        public int min;
        public int max;

        public RangeMinMaxInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }


        public int Clamp(int value)
        {
            return Mathf.Clamp(value, min, max);
        }

        public int Random()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}