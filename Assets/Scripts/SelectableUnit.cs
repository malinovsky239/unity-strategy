using UnityEngine;

namespace Assets.Scripts
{
    public class SelectableUnit : MonoBehaviour
    {
        private const float SelectionIndicatorYShift = 0.01f;
        private GameObject _selectionIndicator;
        [SerializeField] private GameObject _selectionIndicatorPrefab;

        public void Select()
        {
            _selectionIndicator = Instantiate(_selectionIndicatorPrefab);
            _selectionIndicator.transform.position = transform.position + Vector3.up * SelectionIndicatorYShift;
            _selectionIndicator.transform.parent = transform;
        }

        public void RemoveSelection()
        {
            Destroy(_selectionIndicator.gameObject);
        }
    }
}
