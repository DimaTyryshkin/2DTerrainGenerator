using FieldGenerator;
using Game.NoizeMixerTool;
using GamePackages.Core;
using NaughtyAttributes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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
        Dictionary<Vector2Int, SpawnChunkTask> cellToTask;
        ConcurrentQueue<SpawnChunkTask> generationQueue;

        SemaphoreSlim semaphore;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        private void Start()
        {
            blockCollection.Init();
            generationQueue = new ConcurrentQueue<SpawnChunkTask>();
            cellToChunk = new Dictionary<Vector2Int, Chunk>();
            cellToTask = new Dictionary<Vector2Int, SpawnChunkTask>();

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            semaphore = new SemaphoreSlim(0);
        }

        [Button]
        void StartThread()
        {
            Thread thread = new Thread(ThreadLoop);
            thread.Start();
        }

        private void OnDestroy()
        {
            cancellationTokenSource.Cancel();
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

            bool canSpawnInThisFrame = true;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2Int c = new Vector2Int(x, y);
                    if (cellToChunk.TryGetValue(c, out Chunk chunk))
                    {
                        chunk.Update();
                    }
                    else if (canSpawnInThisFrame)
                    {
                        if (cellToTask.TryGetValue(c, out SpawnChunkTask task))
                        {
                            if (task.isReady)
                            {
                                //canSpawnInThisFrame = false;
                                cellToTask.Remove(c);
                                Chunk newChunk = task.resutChunk;
                                //System.Diagnostics.Stopwatch s = System.Diagnostics.Stopwatch.StartNew();

                                //s.Stop();
                                //Debug.Log($"InitGpuInstancer ElapsedTime='{s.Elapsed.TotalMilliseconds}ms'");
                                cellToChunk[c] = newChunk;
                                newChunk.Update();
                            }
                        }
                        else
                        {
                            Vector2Int position2D = c * Chunk.chunkSize;
                            SpawnChunkTask newTask = new SpawnChunkTask(position2D);
                            cellToTask[c] = newTask;
                            generationQueue.Enqueue(newTask);


                            Debug.Log($"Add task");
                            semaphore.Release();
                            Debug.Log($"semaphore.CurrentCount={semaphore.CurrentCount}");
                        }
                    }
                }
            }
        }

        void ThreadLoop()
        {
            Debug.Log("ThreadLoop start");

            while (!cancellationToken.IsCancellationRequested)
            {
                semaphore.Wait(cancellationToken);
                if (generationQueue.TryDequeue(out SpawnChunkTask task))
                {
                    MyTexture2D texture = model.DrawMainTexture(Chunk.chunkSize, Chunk.chunkSize, task.chunkPosition2D);
                    WorldPoint2D[,] heightMap = TextureToHeightMap(texture);
                    Chunk newChunk = new Chunk(task.chunkPosition2D);
                    //task.heightMap = heightMap;
                    newChunk.Generate(chunkGenerator, heightMap);
                    newChunk.InitGpuInstancer(blockCollection, chunkGenerator);
                    task.resutChunk = newChunk;
                    task.isReady = true;
                }
            }

            Debug.Log("ThreadLoop stop");
        }

        WorldPoint2D[,] TextureToHeightMap(MyTexture2D texture)
        {
            int width = texture.width;
            int height = texture.height;

            WorldPoint2D[,] heightMap = new WorldPoint2D[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = texture[x, y];
                    heightMap[x, y] = new WorldPoint2D() { h = color.r };
                }
            }

            return heightMap;
        }

        private void OnDrawGizmosSelected()
        {
            GizmosExtension.DrawRectXZ(playrRect);
        }


        class SpawnChunkTask //: IEqualityComparer<SpawnChunkTask>, IEquatable<SpawnChunkTask>
        {
            public readonly Vector2Int chunkPosition2D;
            public WorldPoint2D[,] heightMap;
            public Chunk resutChunk;
            public bool isReady;

            public SpawnChunkTask(Vector2Int chunkPosition2D) //: this()
            {
                this.chunkPosition2D = chunkPosition2D;
            }


            /*
            public bool Equals(SpawnChunkTask x, SpawnChunkTask y)
            {
                return x.chunkPosition2D == y.chunkPosition2D;
            }

            public bool Equals(SpawnChunkTask other)
            {
                return other.chunkPosition2D == chunkPosition2D;
            }

            public int GetHashCode(SpawnChunkTask obj)
            {
                return obj.chunkPosition2D.GetHashCode();
            }
            */
        }
    }
}