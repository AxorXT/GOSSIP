using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class GRID_GENERATOR : MonoBehaviour
{
    public GameObject[] floorPrefabs;
    public GameObject[] roadPrefabs;
    public GameObject[] buildingPrefabs;
    public ObjectPool[] buildingPools;
    public ObjectPool[] floorPools;  // Cambié a un array de ObjectPool
    public ObjectPool[] roadPools;  // Cambié a un array de ObjectPool
    public ObjectPool[] enemyPools;
    public int gridWidth = 7;
    public int gridHeight = 5;
    public float cellSize = 5f;
    public Transform player;

    private Dictionary<Vector2, GameObject> activeTiles = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, GameObject> activeBuildings = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, GameObject> activeEnemies = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, GameObject> activeRoads = new Dictionary<Vector2, GameObject>(); // Diccionario para los caminos

    private float spawnZ = 0f;
    private float safeZone = 5f;
    private int currentRow = 0;

    private int roadWidth;
    private int roadStartX;

    public float enemySpawnChance = 0.2f;
    public int enemySpawnStartRow = 5;

    public GameObject initialBuildingPrefab;
    public ObjectPool initialBuildingPool;
    public int buildingControlRows = 5;

    private GameObject[] floorColumnPrefabs;

    public SkyboxManager skyboxManager;
    public List<int> triggerBlocks = new List<int> { 30, 60, 90 }; // Z en unidades, no filas
    private int nextTriggerIndex = 0;
    private bool isNight = false; // Para alternar entre día y noche
    private bool gameStarted = false;
    private float startZ = 0f; // Posición Z del jugador al comenzar

    void Start()
    {

        roadWidth = Mathf.Max(3, gridWidth * 70 / 100);
        if (roadWidth % 2 == 0) roadWidth--;

        roadStartX = (gridWidth - roadWidth) / 2;
        float playerStartX = (gridWidth / 2) * cellSize;
        player.position = new Vector3(playerStartX, player.position.y, 0);

        floorColumnPrefabs = new GameObject[gridWidth];
        for (int x = 0; x < gridWidth; x++)
        {
            floorColumnPrefabs[x] = floorPrefabs[x % floorPrefabs.Length];
        }

        for (int z = 0; z < gridHeight; z++)
        {
            SpawnRow();
        }
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) // Toca o clic para empezar
            {
                gameStarted = true;
                startZ = player.position.z;
                Debug.Log("Juego iniciado. Comenzamos a contar bloques.");
            }
            return; // Detener la ejecución hasta que empiece
        }

        CheckSkyboxTrigger();

        if (player.position.z - safeZone > (spawnZ - gridHeight * cellSize))
        {
            SpawnRow();
            DeleteOldRow();
        }

        DeleteOldEnemies();
    }

    void SpawnRow()
    {
        // Si aún no han pasado las primeras 15 filas, mantenemos el camino recto
        if (currentRow >= 15)
        {
            int direction = Random.Range(0, 3);
            if (direction == 0 && roadStartX > 1) roadStartX--;
            else if (direction == 2 && roadStartX + roadWidth < gridWidth - 1) roadStartX++;
        }

        bool placeBuildingNext = true;

        for (int x = 0; x < gridWidth; x++)
        {
            float spawnX = (x - (gridWidth / 2)) * cellSize;
            Vector2 key = new Vector2(spawnX, spawnZ);

            if (!activeTiles.ContainsKey(key))
            {
                // Seleccionamos el pool adecuado para el piso
                int floorIndex = x % floorPools.Length; // O puedes usar Random.Range(0, floorPools.Length) si prefieres aleatorio
                GameObject newFloor = floorPools[floorIndex].Get(new Vector3(spawnX, 0f, spawnZ), Quaternion.identity);
                StartCoroutine(Utils.AnimateScaleIn(newFloor));
                activeTiles.Add(key, newFloor);
            }

            GameObject prefab = null;
            float yPosition = 0.5f;

            if (x >= roadStartX && x < roadStartX + roadWidth)
            {
                // Usar un índice aleatorio para seleccionar el pool de road
                int roadIndex = Random.Range(0, roadPools.Length);
                GameObject road = roadPools[roadIndex].Get(new Vector3(spawnX, 0f, spawnZ), Quaternion.identity);
                StartCoroutine(Utils.AnimateScaleIn(road));
                activeRoads[key] = road; // Añadimos al diccionario de caminos

                if (spawnZ >= enemySpawnStartRow * cellSize && Random.value < enemySpawnChance && enemyPools.Length > 0 && !activeEnemies.ContainsKey(key))
                {
                    bool canPlaceEnemy = true;

                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        Vector2 checkKey = new Vector2(spawnX + offsetX * cellSize, spawnZ);
                        if (activeBuildings.ContainsKey(checkKey))
                        {
                            canPlaceEnemy = false;
                            break;
                        }
                    }

                    if (canPlaceEnemy)
                    {
                        Vector3 enemyPosition = new Vector3(spawnX, 0.5f, spawnZ);
                        int index = Random.Range(0, enemyPools.Length);
                        Quaternion lookRotation = Quaternion.LookRotation((player.position - enemyPosition).normalized);
                        GameObject newEnemy = enemyPools[index].Get(enemyPosition, lookRotation);
                        StartCoroutine(Utils.AnimateScaleIn(newEnemy));
                        activeEnemies.Add(key, newEnemy);
                    }
                }
            }
            else
            {
                if (placeBuildingNext && !activeBuildings.ContainsKey(key))
                {
                    ObjectPool pool = null;
                    if (spawnZ < buildingControlRows * cellSize)
                    {
                        pool = initialBuildingPool;
                    }
                    else
                    {
                        pool = buildingPools[Random.Range(0, buildingPools.Length)];
                    }

                    bool canPlaceBuilding = true;
                    for (int offsetX = 0; offsetX < 3; offsetX++)
                    {
                        for (int offsetZ = 0; offsetZ < 2; offsetZ++)
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

                    if (canPlaceBuilding && pool != null)
                    {
                        Quaternion rotation = Quaternion.identity;
                        if (spawnX < 0) rotation = Quaternion.Euler(0, 90f, 0);
                        else if (spawnX > 0) rotation = Quaternion.Euler(0, -90f, 0);

                        for (int offsetX = 0; offsetX < 3; offsetX++)
                        {
                            for (int offsetZ = 0; offsetZ < 2; offsetZ++)
                            {
                                Vector3 position = new Vector3(spawnX + offsetX * cellSize, yPosition, spawnZ + offsetZ * cellSize);
                                GameObject building = pool.Get(position, rotation);
                                activeBuildings[new Vector2(position.x, position.z)] = building;
                                StartCoroutine(Utils.AnimateScaleIn(building));
                            }
                        }
                    }
                }
                placeBuildingNext = !placeBuildingNext;
            }
        }

        spawnZ += cellSize;
        currentRow++;
    }

    void DeleteOldRow()
    {
        float deleteZ = spawnZ - (gridHeight + 1) * cellSize;
        List<Vector2> toRemove = new List<Vector2>();

        foreach (var tile in activeTiles)
        {
            if (tile.Key.y <= deleteZ)
            {
                floorPools[0].ReturnToPool(tile.Value); // Asegúrate de devolver al pool correcto
                toRemove.Add(tile.Key);
            }
        }
        foreach (var key in toRemove) activeTiles.Remove(key);

        toRemove.Clear();
        foreach (var road in activeRoads)
        {
            if (road.Key.y <= deleteZ)
            {
                foreach (var pool in roadPools)
                {
                    pool.ReturnToPool(road.Value);
                    break;
                }
                toRemove.Add(road.Key);
            }
        }
        foreach (var key in toRemove) activeRoads.Remove(key); // Eliminamos del diccionario de caminos

        toRemove.Clear();
        foreach (var enemy in activeEnemies)
        {
            if (enemy.Key.y <= deleteZ)
            {
                foreach (var pool in enemyPools)
                {
                    pool.ReturnToPool(enemy.Value);
                    break;
                }
                toRemove.Add(enemy.Key);
            }
        }
        foreach (var key in toRemove) activeEnemies.Remove(key);

        toRemove.Clear();
        foreach (var building in activeBuildings)
        {
            if (building.Key.y <= deleteZ)
            {
                foreach (var pool in buildingPools)
                {
                    pool.ReturnToPool(building.Value);
                    break;
                }
                toRemove.Add(building.Key);
            }
        }
        foreach (var key in toRemove) activeBuildings.Remove(key);
    }

    void DeleteOldEnemies()
    {
        float limitZ = spawnZ - (gridHeight + 1) * cellSize;
        List<Vector2> toRemove = new List<Vector2>();

        foreach (var enemy in activeEnemies)
        {
            if (enemy.Key.y <= limitZ)
            {
                foreach (var pool in enemyPools)
                {
                    pool.ReturnToPool(enemy.Value);
                    break;
                }
                toRemove.Add(enemy.Key);
            }
        }
        foreach (var key in toRemove) activeEnemies.Remove(key);
    }

    void CheckSkyboxTrigger()
    {
        if (!gameStarted || skyboxManager == null) return;

        float distanceTravelled = player.position.z - startZ;
        int blocksTravelled = Mathf.FloorToInt(distanceTravelled / cellSize);

        if (blocksTravelled >= (nextTriggerIndex + 1) * 60)
        {
            isNight = !isNight;
            skyboxManager.StartBlend(isNight);
            nextTriggerIndex++;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("GOSSIP_LEVEL");
    }
}
