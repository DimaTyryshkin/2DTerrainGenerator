using FieldGenerator;
using GamePackages.Core;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    class VisibleChunks : MonoBehaviour
    {
        [SerializeField] int visibilityRadius;
        [SerializeField] Transform player;
        [SerializeField] BlockCollection blockCollection;
        [SerializeField] ChunkGenerator chunkGenerator;

        Rect playrRect;

        Dictionary<Vector2Int, Chunk> cellToChunk;
        Dictionary<Vector2Int, SpawnChunkTask> cellToTask;
        ConcurrentQueue<SpawnChunkTask> generationQueue;

        SemaphoreSlim semaphore;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        IEnumerator Start()
        {
            blockCollection.Init();
            generationQueue = new ConcurrentQueue<SpawnChunkTask>();
            cellToChunk = new Dictionary<Vector2Int, Chunk>();
            cellToTask = new Dictionary<Vector2Int, SpawnChunkTask>();

            yield return chunkGenerator.GenerateWorld();
            yield return null;

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            semaphore = new SemaphoreSlim(0);

            //StartThread();
        }

        void StartThread()
        {
            Thread thread = new Thread(ThreadLoop);
            thread.Start();
        }

        private void OnDestroy()
        {
            cancellationTokenSource.Cancel();
        }

        [Button]
        public void Generate()
        {
            StartCoroutine(chunkGenerator.GenerateWorld());
        }


        public bool log;
        private void Update()
        {
            if (semaphore == null)
                return;

            if (log) Debug.Log($"Update==========");

            ProcessTask();

            Vector3 pos = player.position;
            Vector3 n = player.forward;
            float d = MathExtension.GetPlaneD(player.forward, pos);
            float k = Mathf.Sqrt(Vector3.Dot(n, n));

            Vector2Int chunkCell = new Vector2Int(
              Mathf.RoundToInt((pos.x / Chunk.chunkSize - 0.5f)),
              Mathf.RoundToInt(pos.z / Chunk.chunkSize - 0.5f));

            playrRect = new Rect(chunkCell * Chunk.chunkSize, Vector2.one * Chunk.chunkSize);


            int minX = chunkCell.x - visibilityRadius;
            int maxX = chunkCell.x + visibilityRadius;

            int minY = chunkCell.y - visibilityRadius;
            int maxY = chunkCell.y + visibilityRadius;

            int visibilityRadius2Power = visibilityRadius * visibilityRadius;
            bool canSpawnInThisFrame = true;
            for (int dx = -visibilityRadius; dx <= visibilityRadius; dx++)
            {
                for (int dy = -visibilityRadius; dy <= visibilityRadius; dy++)
                {
                    if ((dx * dx + dy * dy) > visibilityRadius2Power)
                        continue;

                    int x = chunkCell.x + dx;
                    int y = chunkCell.y + dy;

                    if (x < 0 || y < 0 || x >= 32 || y >= 32)
                        continue;

                    //Vector3 chunkWorldPos = new Vector3(x, 0, y) * Chunk.chunkSize + new Vector3(Chunk.chunkSize * 0.5f, 129, Chunk.chunkSize * 0.5f);
                    //float value = Vector3.Dot(player.forward, chunkWorldPos) + d;
                    //bool isInvisibleChunk = value / k <= 0;
                    //if (isInvisibleChunk)
                    //    continue;

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

                                Assert.IsNotNull(newChunk);
                                Assert.IsNotNull(newChunk.grid);
                                Assert.IsNotNull(newChunk.grid.items);

                                Debug.Log($"newChunk.grid.items.count = {newChunk.grid.items.Length}");
                                Debug.Log($"newChunk.grid.items.count2 = {newChunk.grid.items.Where(x => x.blockName == BlockName.Ground)}");
                                Debug.Log(newChunk.grid.items.GroupBy(x => x.blockName).ToStringMultiline("Объекты в чанке", x => $"{x.Key} {x.Count()}"));

                                //s.Stop();
                                //Debug.Log($"InitGpuInstancer ElapsedTime='{s.Elapsed.TotalMilliseconds}ms'");
                                cellToChunk[c] = newChunk;
                                newChunk.Update();
                            }
                        }
                        else
                        {
                            SpawnChunkTask newTask = new SpawnChunkTask(c);
                            cellToTask[c] = newTask;
                            generationQueue.Enqueue(newTask);


                            //Debug.Log($"Add task");
                            semaphore.Release();
                            //Debug.Log($"semaphore.CurrentCount={semaphore.CurrentCount}");
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
                ProcessTask();
            }

            Debug.Log("ThreadLoop stop");
        }

        void ProcessTask()
        {
            if (generationQueue.TryDequeue(out SpawnChunkTask task))
            {
                Debug.Log($"GenerateChunk {task.chunkPosition2D.x} {task.chunkPosition2D.y}");
                Chunk newChunk = new Chunk(task.chunkPosition2D * Chunk.chunkSize);
                chunkGenerator.Generate(task.chunkPosition2D, newChunk.grid);
                Debug.Log(newChunk.grid.items.GroupBy(x => x.blockName).ToStringMultiline("Объекты в чанке", x => $"{x.Key} {x.Count()}"));
                newChunk.InitGpuInstancer(blockCollection, chunkGenerator);

                Assert.IsNotNull(newChunk);
                Assert.IsNotNull(newChunk.grid);
                Assert.IsNotNull(newChunk.grid.items);
                task.resutChunk = newChunk;
                task.isReady = true;
            }
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