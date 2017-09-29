using UnityEngine;

namespace Assets.Scripts
{
    public class SelectableUnit : MonoBehaviour
    {
        private GameObject _selectionIndicator;
        [SerializeField] private GameObject _selectionIndicatorPrefab;

        public void Select()
        {
            _selectionIndicator = Instantiate(_selectionIndicatorPrefab) as GameObject;
            _selectionIndicator.transform.position = transform.position + Vector3.up * 0.01f;
            _selectionIndicator.transform.parent = this.transform;
        }

        public void RemoveSelection()
        {
            Destroy(_selectionIndicator.gameObject);
        }
    }
}
