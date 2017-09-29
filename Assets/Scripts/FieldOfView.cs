using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldOfView : MonoBehaviour
    { 
        private int steps = 10;

        [SerializeField] public float _barColorRed;
        [SerializeField] public float _barColorGreen;
        [SerializeField] public float _barColorBlue;

        [SerializeField] public int _maxDistance = 15;
        [SerializeField] public float _fieldAngle;
        private Mesh _mesh;    
    
        private void Awake()
        {
            // temporary fix for the issue with displaying multiple meshes
            if (GetComponentInParent<NecromancerBehaviour>())
            {
                Generate();
            }
        }
    
        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();            
        
            Vector3[] vertices = new Vector3[steps + 2];                
            vertices[0] = new Vector3(0, 0, 0);
            int cnt = 1;
            for (float angle = - _fieldAngle / 2; angle <= _fieldAngle / 2 + Mathf.Epsilon; angle += _fieldAngle / steps)
            {
                float x = Mathf.Sin(angle * Mathf.Deg2Rad) * _maxDistance;
                float y = Mathf.Cos(angle * Mathf.Deg2Rad) * _maxDistance;
                vertices[cnt++] = new Vector3(x, 0.5f, y);
            }        
            _mesh.vertices = vertices;
              
            Color barColor = new Color(_barColorRed, _barColorGreen, _barColorBlue);
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                colors[i] = barColor;
            }
            _mesh.colors = colors;
        
            int[] triangles = new int[(steps + 1) * 3];
            for (int i = 0; i < steps; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }        
            _mesh.triangles = triangles;
        
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
