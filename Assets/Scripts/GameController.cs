using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        private const int GoblinsCount = 5;
        private const int SkeletonsCount = 4;
        private const float CursorScaleW = 0.8f;
        private const float CursorScaleH = 0.6f;
        private const float MaxAngle = 180;
        private const float StartingPositionHeightAboveGround = 0.05f;

        [SerializeField] private GameObject _goblinPrefab;
        [SerializeField] private GameObject _skeletonPrefab;
        [SerializeField] private GameObject _necromancerPrefab;
        [SerializeField] private Camera _camera;

        private Texture2D _cursorTexture;
        public static List<GameObject> WorldUnits;
        private static readonly Rect GoblinsStartingArea = new Rect(-60, -5, 10, 10);
        private static readonly Rect SkeletonsStartingArea = new Rect(60, 10, 20, 20);

        private void Start()
        {
            _cursorTexture = Resources.Load(Constants.Resources.Sword) as Texture2D;
            if (_cursorTexture != null)
            {
                Cursor.SetCursor(_cursorTexture, new Vector2(_cursorTexture.width * CursorScaleW, _cursorTexture.height * CursorScaleH), CursorMode.Auto);
            }

            WorldUnits = new List<GameObject>();

            for (var i = 0; i < GoblinsCount; i++)
            {
                CreateUnit(_goblinPrefab, Constants.Tags.Player, GoblinsStartingArea);
            }

            for (var i = 0; i < SkeletonsCount; i++)
            {
                CreateUnit(_skeletonPrefab, Constants.Tags.Enemy, SkeletonsStartingArea);
            }
            CreateUnit(_necromancerPrefab, Constants.Tags.Enemy, SkeletonsStartingArea);
        }

        private void CreateUnit(GameObject prefab, string newUnitTag, Rect bounds)
        {
            GameObject gameObj = Instantiate(prefab);
            gameObj.tag = newUnitTag;
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
                newY = Terrain.activeTerrain.SampleHeight(position);
            } while (newY < Double.Epsilon);
            position.y = newY + StartingPositionHeightAboveGround;
            rotation = new Vector3(0, Random.Range(-MaxAngle, MaxAngle), 0);
        }
    }
}
