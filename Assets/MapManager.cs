using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private Tilemap map;

    public AudioClip audioFromTile;
    public AudioClip noTileFoundSound;


    [SerializeField] private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake() {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    //reference this method on player when he walks or enters a new WorldCell (or grid sized cube)
    public AudioClip audioClipOnTile(Vector2 playerPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(playerPosition + Vector2.down);
        TileBase tile = map.GetTile(gridPosition);
        Debug.Log(tile);

        if(tile == null) return noTileFoundSound;
        return dataFromTiles[tile].soundOnTile;
    }

    

}
