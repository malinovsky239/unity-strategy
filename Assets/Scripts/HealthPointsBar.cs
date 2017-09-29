using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class HealthPointsBar : MonoBehaviour
    {
        [SerializeField] public Camera Camera;
        private readonly Vector2 _healthBarSize = new Vector2(50, 8);
        private readonly float _unitHeight = 3;
        private readonly float _borderWidth = 2;
        private readonly Color _hpMainColor = new Color(1, 0, 0, 0.5f);
        private readonly Color _hpBorderColor = new Color(0, 0, 0, 0.5f);
        private SelectionController _selectionController;
        private CameraMovement _cameraMovement;
        public int MaxHealthPoints = 200;
        public int HealthPoints = 150;

        private void Start()
        {
            _selectionController = Camera.GetComponent<SelectionController>();
            _cameraMovement = Camera.GetComponent<CameraMovement>();
        }

        private void OnGUI()
        {
            if (HealthPoints > 0)
            {
                if (_cameraMovement.State == CameraMovement.CameraState.Strategic)
                {
                    Vector2 hpPosition = Camera.WorldToScreenPoint(transform.position + new Vector3(0, _unitHeight, 0));
                    hpPosition.y = Screen.height - hpPosition.y;
                    hpPosition.x -= _healthBarSize.x / 2;
                    Vector2 remainingHealth = new Vector2(_healthBarSize.x * HealthPoints / MaxHealthPoints,
                        _healthBarSize.y);
                    Utils.DrawScreenRect(new Rect(hpPosition, remainingHealth), _hpMainColor);
                    Utils.DrawScreenRectBorder(new Rect(hpPosition, _healthBarSize), _borderWidth, _hpBorderColor);
                }
            }
            else
            {
                StartCoroutine(Die());
            }
        }

        public IEnumerator Die()
        {
            _selectionController.Remove(this.gameObject);
            GetComponent<Animator>().SetBool(Constants.AnimatorParameters.Alive, false);
            GetComponent<NavMeshAgent>().isStopped = true;
            GetComponentInChildren<FieldOfView>().Die();
            yield return new WaitForSeconds(10);
            Destroy(this.gameObject);
        }
    }
}
