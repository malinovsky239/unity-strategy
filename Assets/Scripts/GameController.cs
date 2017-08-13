using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private const int GoblinsCount = 5;
    private const int SkeletonsCount = 4;
    [SerializeField] private GameObject _goblinPrefab;
    [SerializeField] private GameObject _skeletonPrefab;
    [SerializeField] private Camera _camera;
    [SerializeField] public Texture2D CursorTexture;
    public static List<GameObject> WorldUnits;

    void Start()
    {
        CursorTexture = Resources.Load("Sword") as Texture2D;
        Cursor.SetCursor(CursorTexture, new Vector2(CursorTexture.width * 0.8f, CursorTexture.height * 0.6f),
            CursorMode.Auto);

        WorldUnits = new List<GameObject>();
        for (var i = 0; i < GoblinsCount; i++)
        {
            CreateUnit(_goblinPrefab, "player", new Rect(-60, -5, 10, 10));
        }

        for (var i = 0; i < SkeletonsCount; i++)
        {
            CreateUnit(_skeletonPrefab, "enemy", new Rect(60, 10, 20, 20));
        }
    }

    private void CreateUnit(GameObject prefab, string tag, Rect bounds)
    {
        GameObject gameObj = Instantiate(prefab) as GameObject;
        gameObj.tag = tag;
        gameObj.GetComponent<HealthPointsBar>().Camera = _camera;
        Vector3 position, rotation;
        SetRandomPosition(out position, out rotation, bounds);
        gameObj.transform.position = position;
        gameObj.transform.eulerAngles = rotation;
        WorldUnits.Add(gameObj);
    }

    private void SetRandomPosition(out Vector3 position, out Vector3 rotation, Rect bounds)
    {
        float newY;
        do
        {
            position = new Vector3(Random.Range(bounds.xMin, bounds.xMax), 0, Random.Range(bounds.yMin, bounds.yMax));
            rotation = new Vector3(0, Random.Range(-180, 180), 0);
            newY = Terrain.activeTerrain.SampleHeight(position);
        } while (newY < Double.Epsilon);
        position.y = newY + 0.05f;
    }
}
