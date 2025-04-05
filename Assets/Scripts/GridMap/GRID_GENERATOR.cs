using System.Collections.Generic;
using UnityEngine;

public class GRID_GENERATOR : MonoBehaviour
{
    public GameObject[] roadPrefabs;
    public GameObject[] buildingPrefabs;
    public int gridWidth = 7;
    public int gridHeight = 5;
    public float cellSize = 5f;
    public Transform player;

    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>(); // Para suelos
    private Dictionary<Vector2, GameObject> activeBuildings = new Dictionary<Vector2, GameObject>(); // Para edificios
    private Dictionary<Vector2, GameObject> activeEnemies = new Dictionary<Vector2, GameObject>(); // Para enemigos

    private float spawnZ = 0f;
    private float safeZone = 5f;  // Distancia adicional para eliminar objetos

    private int roadWidth;
    private int roadStartX;

    public GameObject[] enemyPrefabs;
    public float enemySpawnChance = 0.2f;

    void Start()
    {
        roadWidth = Mathf.Max(3, gridWidth * 70 / 100);
        if (roadWidth % 2 == 0) roadWidth--;

        roadStartX = (gridWidth - roadWidth) / 2;
        float playerStartX = (gridWidth / 2) * cellSize;
        player.position = new Vector3(playerStartX, player.position.y, 0);

        for (int z = 0; z < gridHeight; z++)
        {
            SpawnRow();
        }
    }

    void Update()
    {
        // Verifica si el jugador ha avanzado lo suficiente como para generar una nueva fila
        if (player.position.z - safeZone > (spawnZ - gridHeight * cellSize))
        {
            SpawnRow();
            DeleteOldRow();  // Elimina las filas antiguas de objetos
        }

        // Elimina los enemigos que ya han quedado atrás
        DeleteOldEnemies();
    }

    void SpawnRow()
    {
        int direction = Random.Range(0, 3);
        if (direction == 0 && roadStartX > 1) roadStartX--;
        else if (direction == 2 && roadStartX + roadWidth < gridWidth - 1) roadStartX++;

        bool placeBuildingNext = true; // Comenzamos con la colocación de un edificio

        for (int x = 0; x < gridWidth; x++)
        {
            float spawnX = (x - (gridWidth / 2)) * cellSize;
            Vector2 key = new Vector2(spawnX, spawnZ);

            // Asegurarse de que no haya objetos duplicados en la misma celda para el suelo
            if (!activeTiles.ContainsKey(key)) // Si la celda no tiene objeto, colocar un piso
            {
                GameObject floorPrefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)];
                if (floorPrefab != null)
                {
                    GameObject newFloor = Instantiate(floorPrefab, new Vector3(spawnX, 0f, spawnZ), Quaternion.identity);
                    activeTiles.Add(key, newFloor);  // Añadimos el piso a las tiles activas
                }
                else
                {
                    Debug.LogError("Floor prefab is missing.");
                }
            }

            // Instanciar edificios o carreteras encima del piso
            GameObject prefab = null;
            float yPosition = 0.5f;

            if (x >= roadStartX && x < roadStartX + roadWidth)
            {
                prefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)];
                yPosition = 0f;

                // Generar enemigos con probabilidad
                if (Random.value < enemySpawnChance && enemyPrefabs.Length > 0 && !activeEnemies.ContainsKey(key))
                {
                    bool canPlaceEnemy = true;

                    // Verificar que las celdas adyacentes al edificio están vacías
                    for (int offsetX = -1; offsetX <= 1; offsetX++)  // Comprobamos una celda antes y después del edificio
                    {
                        Vector2 checkKey = new Vector2(spawnX + offsetX * cellSize, spawnZ);
                        if (activeBuildings.ContainsKey(checkKey))
                        {
                            canPlaceEnemy = false;
                            break;
                        }
                    }

                    // Si el enemigo puede ser colocado
                    if (canPlaceEnemy)
                    {
                        Vector3 enemyPosition = new Vector3(spawnX, 0.5f, spawnZ);
                        int index = Random.Range(0, enemyPrefabs.Length);
                        GameObject newEnemy = Instantiate(enemyPrefabs[index], enemyPosition, Quaternion.identity);
                        activeEnemies.Add(key, newEnemy);
                    }
                }
            }
            else
            {
                // Alternar entre colocar edificios y árboles
                if (placeBuildingNext && !activeBuildings.ContainsKey(key))  // Verificamos si la celda no tiene edificio
                {
                    prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

                    // Verificar que las celdas necesarias para el edificio estén libres (3x2)
                    bool canPlaceBuilding = true;
                    for (int offsetX = 0; offsetX < 3; offsetX++)  // Comprobamos 3 celdas en X
                    {
                        for (int offsetZ = 0; offsetZ < 2; offsetZ++)  // Comprobamos 2 celdas en Z
                        {
                            Vector2 checkKey = new Vector2(spawnX + offsetX * cellSize, spawnZ + offsetZ * cellSize);
                            if (activeBuildings.ContainsKey(checkKey))
                            {
                                canPlaceBuilding = false;
                                break;
                            }
                        }
                        if (!canPlaceBuilding) break;
                    }

                    // Si se puede colocar el edificio
                    if (canPlaceBuilding && prefab != null)
                    {
                        Quaternion rotation = Quaternion.identity;

                        // Determinar la rotación según la posición
                        if (spawnX < 0)  // Si está en el lado izquierdo
                        {
                            rotation = Quaternion.Euler(0, 90f, 0);  // Rotar 90 grados en Y
                        }
                        else if (spawnX > 0)  // Si está en el lado derecho
                        {
                            rotation = Quaternion.Euler(0, -90f, 0); // Rotar -90 grados en Y
                        }

                        // Instanciar el edificio en las 6 celdas ocupadas
                        for (int offsetX = 0; offsetX < 3; offsetX++)  // Recorrer las celdas de X
                        {
                            for (int offsetZ = 0; offsetZ < 2; offsetZ++)  // Recorrer las celdas de Z
                            {
                                Vector3 position = new Vector3(spawnX + offsetX * cellSize, yPosition, spawnZ + offsetZ * cellSize);
                                Instantiate(prefab, position, rotation);
                                activeBuildings[new Vector2(spawnX + offsetX * cellSize, spawnZ + offsetZ * cellSize)] = prefab;
                            }
                        }
                    }
                }
                placeBuildingNext = !placeBuildingNext;  // Alternamos el valor para el siguiente objeto
            }
        }

        spawnZ += cellSize;  // Avanzamos en la dirección Z
    }

    void DeleteOldRow()
    {
        // Eliminamos todas las filas de objetos que ya no son visibles
        float deleteZ = spawnZ - (gridHeight + 1) * cellSize;
        List<Vector2> toRemove = new List<Vector2>();

        // Recorremos todos los objetos en activeTiles y los eliminamos si están fuera del alcance
        foreach (var tile in activeTiles)
        {
            if (tile.Key.y <= deleteZ)
            {
                Destroy(tile.Value);  // Destruir el objeto
                toRemove.Add(tile.Key);  // Añadir la clave para eliminarla de activeTiles
            }
        }

        // Eliminar todos los objetos de activeTiles
        foreach (var key in toRemove)
        {
            activeTiles.Remove(key);  // Eliminar la referencia del diccionario
        }

        // Eliminar también los enemigos que han quedado atrás
        List<Vector2> toRemoveEnemies = new List<Vector2>();
        foreach (var enemy in activeEnemies)
        {
            if (enemy.Key.y <= deleteZ)
            {
                Destroy(enemy.Value);  // Destruir al enemigo
                toRemoveEnemies.Add(enemy.Key);  // Añadir la clave para eliminarla de activeEnemies
            }
        }

        // Eliminar enemigos del diccionario
        foreach (var key in toRemoveEnemies)
        {
            activeEnemies.Remove(key);  // Eliminar la referencia del diccionario de enemigos
        }
    }

    void DeleteOldEnemies()
    {
        // Eliminamos enemigos que han quedado atrás en el camino
        List<Vector2> toRemove = new List<Vector2>();
        foreach (var enemy in activeEnemies)
        {
            if (enemy.Key.y <= spawnZ - (gridHeight + 1) * cellSize)
            {
                Destroy(enemy.Value);  // Destruir al enemigo
                toRemove.Add(enemy.Key);  // Añadir la clave para eliminarla de activeEnemies
            }
        }

        foreach (var key in toRemove)
        {
            activeEnemies.Remove(key);  // Eliminar la referencia del diccionario
        }
    }
}
