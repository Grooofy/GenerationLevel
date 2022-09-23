using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoxelTilePlacer : MonoBehaviour
{
    public VoxelTile[] TilePrefabs;
    public Vector2Int MapSize = new Vector2Int(10, 10);

    private VoxelTile[,] spawnedTiles;
    private Coroutine tileInstantiated;


    private void Start()
    {
        spawnedTiles = new VoxelTile[MapSize.x, MapSize.y];

        foreach (VoxelTile tilePrefab in TilePrefabs)
        {
            tilePrefab.CalculateSideColors();
        }
        tileInstantiated = StartCoroutine(Generate());
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StopCoroutine(tileInstantiated);

            foreach (VoxelTile spawnedTile in spawnedTiles)
            {
                if (spawnedTile != null) Destroy(spawnedTile.gameObject);
            }

            StartCoroutine(Generate());
        }
    }

    public IEnumerator Generate()
    {
        for (int x = 1; x < MapSize.x - 1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                yield return new WaitForSeconds(0.1f);
                PlaceTile(x, y);
            }
        }
    }

    public void PlaceTile(int x, int y)
    {
        List<VoxelTile> avelibleTiles = new List<VoxelTile>();

        foreach (VoxelTile tilePrefab in TilePrefabs)
        {
            if (CanAppendTileLeftSide(spawnedTiles[x - 1, y], tilePrefab) &&
                CanAppendTileRighSide(spawnedTiles[x + 1, y], tilePrefab) &&
                CanAppendTileForwardSide(spawnedTiles[x, y + 1], tilePrefab) &&
                CanAppendTileBackSide(spawnedTiles[x, y - 1], tilePrefab))
            {
                avelibleTiles.Add(tilePrefab);
            }
        }

        if (avelibleTiles.Count == 0) return;

        VoxelTile selectTile = avelibleTiles[Random.Range(0, avelibleTiles.Count)];
        Vector3 position = new Vector3(x, 0, y) * (selectTile.VoxelSize * selectTile.TileSideVoxels);
        spawnedTiles[x, y] = Instantiate(selectTile, position, Quaternion.identity);
    }

    private bool CanAppendTileLeftSide(VoxelTile existTile, VoxelTile tileToAppend)
    {
        if (existTile == null) return true;

        return Enumerable.SequenceEqual(existTile.ColorLeft, tileToAppend.ColorRight);
    }

    private bool CanAppendTileRighSide(VoxelTile existTile, VoxelTile tileToAppend)
    {
        if (existTile == null) return true;

        return Enumerable.SequenceEqual(existTile.ColorRight, tileToAppend.ColorLeft);
    }

    private bool CanAppendTileForwardSide(VoxelTile existTile, VoxelTile tileToAppend)
    {
        if (existTile == null) return true;

        return Enumerable.SequenceEqual(existTile.ColorForward, tileToAppend.ColorBack);
    }

    private bool CanAppendTileBackSide(VoxelTile existTile, VoxelTile tileToAppend)
    {
        if (existTile == null) return true;

        return Enumerable.SequenceEqual(existTile.ColorBack, tileToAppend.ColorForward);
    }

}