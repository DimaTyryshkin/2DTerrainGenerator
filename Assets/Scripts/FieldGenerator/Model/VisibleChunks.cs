using FieldGenerator;
using Game.NoizeMixerTool;
using GamePackages.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    class VisibleChunks : MonoBehaviour
    {
        [SerializeField] int visibilityRadius;
        [SerializeField] Transform player;
        [SerializeField] NoizeMixerToolModel model;
        [SerializeField] BlockCollection blockCollection;
        [SerializeField] ChunkGenerator chunkGenerator;

        Rect playrRect;
        Dictionary<Vector2Int, Chunk> cellToChunk;

        private void Start()
        {
            cellToChunk = new Dictionary<Vector2Int, Chunk>();
        }

        private void Update()
        {
            Vector3 pos = player.position;

            Vector2Int chunkCell = new Vector2Int(
              Mathf.RoundToInt((pos.x / Chunk.chunkSize - 0.5f)),
              Mathf.RoundToInt(pos.z / Chunk.chunkSize - 0.5f));

            playrRect = new Rect(chunkCell * Chunk.chunkSize, Vector2.one * Chunk.chunkSize);


            int minX = chunkCell.x - visibilityRadius;
            int maxX = chunkCell.x + visibilityRadius;

            int minY = chunkCell.y - visibilityRadius;
            int maxY = chunkCell.y + visibilityRadius;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2Int c = new Vector2Int(x, y);
                    if (!cellToChunk.TryGetValue(c, out Chunk chunk))
                    {
                        Vector2Int position2D = c * Chunk.chunkSize;
                        chunk = new Chunk(position2D);
                        cellToChunk[c] = chunk;

                        Texture2D texture = model.DrawMainTexture(Chunk.chunkSize, Chunk.chunkSize, position2D);
                        WorldPoint2D[,] heightMap = TextureToGeightMap(texture);
                        chunk.Generate(chunkGenerator, blockCollection, heightMap);
                    }

                    chunk.Update();
                }
            }
        }

        WorldPoint2D[,] TextureToGeightMap(Texture2D texture)
        {
            int width = texture.width;
            int height = texture.height;

            WorldPoint2D[,] heightMap = new WorldPoint2D[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = texture.GetPixel(x, y);
                    heightMap[x, y] = new WorldPoint2D() { h = color.r };
                }
            }

            return heightMap;
        }

        private void OnDrawGizmosSelected()
        {
            GizmosExtension.DrawRectXZ(playrRect);
        }
    }
}