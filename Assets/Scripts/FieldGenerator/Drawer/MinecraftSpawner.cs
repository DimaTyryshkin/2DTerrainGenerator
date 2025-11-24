using System.Collections.Generic;
using GamePackages.Core;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FieldGenerator
{
    public class MinecraftSpawner : MonoBehaviour
    {
        [SerializeField]
        bool gpuInstancing;

        [Header("Настройки спавна")]
        [SerializeField] GameObject cubePrefab;
        [SerializeField] int gridSize = 50;

        [Header("GPU Instancing")]
        public Material gpuInstancedMaterial;

        List<Matrix4x4> matrices = new List<Matrix4x4>();
        Mesh cubeMesh;
        MaterialPropertyBlock propertyBlock;

        void Start()
        {
            cubeMesh = cubePrefab.GetComponent<MeshFilter>().sharedMesh;
            propertyBlock = new MaterialPropertyBlock();

            SpawnCubes();
        }

        [Button]
        void SpawnCubes()
        {
            transform.DestroyChildren();

            matrices.Clear();

            int n = 0;
            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    // Создаем случайную высоту для рельефа
                    int height = Random.Range(15, 25);

                    for (int y = 0; y < height; y++)
                    {
                        Vector3 position = new Vector3(
                            x,
                            y,
                            z
                        );

                        n++;
                        if (gpuInstancing)
                        {
                            Quaternion rotation = Quaternion.identity;
                            Vector3 scale = Vector3.one;

                            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
                            matrices.Add(matrix);
                        }
                        else
                        {
                            var go = transform.InstantiateAsChild(cubePrefab);
                            go.transform.position = position;
                        }
                    }
                }
            }

            Debug.Log($"Создано {n} кубов для отрисовки");
        }

        void Update()
        {
            if (!gpuInstancing || matrices.Count == 0) return;

            // Отрисовываем все кубы за один вызов с помощью GPU Instancing
            Graphics.DrawMeshInstanced(cubeMesh, 0, gpuInstancedMaterial, matrices, propertyBlock);
        }
    }
}