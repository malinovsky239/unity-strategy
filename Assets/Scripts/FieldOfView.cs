using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldOfView : MonoBehaviour
    {
        private const int Steps = 10;
        private const float HeightAboveGround = 0.5f;

        [SerializeField] private float _barColorRed;
        [SerializeField] private float _barColorGreen;
        [SerializeField] private float _barColorBlue;

        [SerializeField] private int _maxDistance;
        [SerializeField] private float _fieldAngle;
        private Mesh _mesh;

        private void Awake()
        {
            Generate();
        }

        private void Generate()
        {
            GetComponent<MeshRenderer>().materials = new Material[1] { new Material(Shader.Find("Particles/Additive")) };
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            Vector3[] vertices = new Vector3[Steps + 2];
            vertices[0] = Vector3.zero;
            int cnt = 1;
            for (float angle = -_fieldAngle / 2; angle <= _fieldAngle / 2 + Mathf.Epsilon; angle += _fieldAngle / Steps)
            {
                float x = Mathf.Sin(angle * Mathf.Deg2Rad) * _maxDistance;
                float y = Mathf.Cos(angle * Mathf.Deg2Rad) * _maxDistance;
                vertices[cnt++] = new Vector3(x, HeightAboveGround, y);
            }
            _mesh.vertices = vertices;

            Color barColor = new Color(_barColorRed, _barColorGreen, _barColorBlue);
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                colors[i] = barColor;
            }
            _mesh.colors = colors;

            int[] triangles = new int[(Steps + 1) * 3];
            for (int i = 0; i < Steps; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
            _mesh.triangles = triangles;
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();
        }

        public bool IsInField(Vector3 position)
        {
            Vector3 to = position - transform.position;
            float distanceToTarget = to.magnitude;
            float angle = Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), new Vector2(to.x, to.z));
            return distanceToTarget < _maxDistance && Mathf.Abs(angle) < _fieldAngle / 2;
        }

        public void Die()
        {
            if (_mesh)
            {
                _mesh.Clear();
            }
        }
    }
}
