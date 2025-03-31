using System.Collections.Generic;
using UnityEngine;

public class GRID_GENERATOR : MonoBehaviour
{
    public GameObject[] terrainPrefabs; // Prefabs de terreno
    public int gridWidth = 3; // Número de columnas (carriles)
    public int gridHeight = 5; // Número de filas visibles al inicio
    public float cellSize = 5f; // Tamaño de cada celda
    public Transform player; // Referencia al jugador

    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>(); // Tiles activos
    private float spawnZ = 0f; // Posición Z de la siguiente fila a generar
    private float safeZone = 5f; // Distancia antes de eliminar filas

    void Start()
    {
        //  Generamos toda la grilla antes de que el jugador se mueva
        for (int z = 0; z < gridHeight; z++)
        {
            SpawnRow();
        }
    }

    void Update()
    {
        if (player.position.z - safeZone > (spawnZ - gridHeight * cellSize))
        {
            SpawnRow();
            DeleteOldRow();
        }
    }

    void SpawnRow()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Vector2 key = new Vector2(x, spawnZ);
            if (!activeTiles.ContainsKey(key))
            {
                GameObject prefab = terrainPrefabs[Random.Range(0, terrainPrefabs.Length)];
                GameObject newTile = Instantiate(prefab, new Vector3(x * cellSize, 0, spawnZ), Quaternion.identity);
                activeTiles.Add(key, newTile);
            }
        }
        spawnZ += cellSize;
    }

    void DeleteOldRow()
    {
        float deleteZ = spawnZ - (gridHeight + 1) * cellSize;
        List<Vector2> toRemove = new List<Vector2>();

        foreach (var tile in activeTiles)
        {
            if (tile.Key.y <= deleteZ)
            {
                Destroy(tile.Value);
                toRemove.Add(tile.Key);
            }
        }

        foreach (var key in toRemove)
        {
            activeTiles.Remove(key);
        }
    }

}
