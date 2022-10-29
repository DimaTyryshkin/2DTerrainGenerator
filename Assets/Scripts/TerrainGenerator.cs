using FieldGenerator;
using UnityEngine; 

namespace Terraria
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer squarePrefab;
        [SerializeField] private Transform terrainRoot;
        [SerializeField] private Camera mainCamera;

        // 0.8 - random
        // 0.5 - min
        // 0.01 - max. Gradient
        [SerializeField] private float perlinScale1 =1;
        [SerializeField] private float perlinScale2 =1;
        [SerializeField] private float heightScale = 0.1f;
        [SerializeField] private int height = 10;
        [SerializeField] private int width = 20;
        
        private Transform terrain;
        private Vector2 offset1;
        private Vector2 offset2;

        
        

        [ContextMenu("Generate")]
        private void Start()
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            offset1 = GenerateOffset();
            offset2 = GenerateOffset();
            if (terrain)
                Destroy(terrain.gameObject);

            terrain = new GameObject("terrain").transform;
            terrain.parent = terrainRoot;
            terrain.position = Vector3.zero;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseValue1 = Mathf.PerlinNoise((x + offset1.x) * perlinScale1, (y + offset1.y) * perlinScale1);
                    float noiseValue2 = Mathf.PerlinNoise((x + offset2.x) * perlinScale2, (y + offset2.y) * perlinScale2);
                    //Debug.Log($"[d] noise = '{noiseValue}'");

                    int squareHeightUnderWater = y - height / 2;
                    float dencity = (noiseValue1+noiseValue2)*0.5f - squareHeightUnderWater * heightScale;


                    if (dencity > 0)
                    {
                        GameObject newSquare = Instantiate(squarePrefab.gameObject, new Vector3(x, y, 0), Quaternion.identity, terrain);
                        newSquare.GetComponent<Square>().SetColor(ColorFromNoise(dencity));
                    }

                }
            }

            mainCamera.transform.position = new Vector3(width * 0.5f, height * 0.5f, -10);
            mainCamera.orthographicSize = height * 0.6f;
        }

        Color ColorFromNoise(float noiseValue)
        {
            return new Color(noiseValue, noiseValue, noiseValue);
        }
        
        Vector2 GenerateOffset()
        {
           return new Vector2(Random.Range(0f, 10000f), Random.Range(0f, 10000f));
        }
    }
}