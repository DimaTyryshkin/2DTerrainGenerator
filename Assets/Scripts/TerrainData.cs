using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terraria
{
    public class TerrainCell
    {
        public float density;
        public Square view;

        public TerrainCell(float density, Square view)
        {
            this.density = density;
            this.view = view;
        }
    }

    public class TerrainData:Grid<TerrainCell>
    { 
        public TerrainData(int width, int height):base(width,height)
        {
        } 
    }
}