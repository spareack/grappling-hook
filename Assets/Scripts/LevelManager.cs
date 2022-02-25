using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level texture")]
    [SerializeField] private Texture2D levelTexture;

    [Header("Tiles Prefabs")]
    [SerializeField] private GameObject prefabWallTitle;
    [SerializeField] private GameObject prefabRoadTitle;
    [SerializeField] private GameObject map;

    private Color colorWall = Color.white;
    private Color colorRoad = Color.black;

    private float unitPerPixel;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        GameObject obj = Instantiate(map, transform.position, Quaternion.identity, transform);
        //Generate();
    }

    private void Generate()
    {
        unitPerPixel = prefabWallTitle.transform.localScale.x;
        float halfUnitPerPixe = unitPerPixel / 2f;

        float width = levelTexture.width;
        float height = levelTexture.height;

        Vector3 offset = (new Vector3(width / 2f, height / 2f, 0f) * unitPerPixel)
            - new Vector3(halfUnitPerPixe, halfUnitPerPixe, 0f);
        //Debug.Log(offset);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Get pixel color
                Color pixelColor = levelTexture.GetPixel(x, y);
                Vector3 spawnPos = ((new Vector3(x, y, 0f) * unitPerPixel) - offset);

                if (pixelColor == colorWall)
                {
                    Spawn(prefabWallTitle, spawnPos);
                    Debug.Log(spawnPos + "  " + x);
                } 
                else if (pixelColor == colorRoad)
                {
                    Spawn(prefabRoadTitle, spawnPos);
                }
                   
            }
        }
    }

    private void Spawn(GameObject prefabTile, Vector3 position)
    {
        position.z = prefabTile.transform.position.z;
        GameObject obj = Instantiate(prefabTile, position, Quaternion.identity, transform);
    }
}
