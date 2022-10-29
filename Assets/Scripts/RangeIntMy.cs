using System;

namespace Terraria
{
    [Serializable]
    public struct RangeIntMy
    { 
        public int start; 
        public int end;

        public float length => end - start; 
 
        public RangeIntMy(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

    }
}