using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class HealthPointsBar : MonoBehaviour
    {
        private static readonly Vector2 HealthBarSize = new Vector2(50, 8);
        private const float UnitHeight = 3;
        private const float BarBorderWidth = 2;

        public Camera Camera { get; set; }
        private SelectionController _selectionController;
        private CameraMovement _cameraMovement;

        [SerializeField] private int _maxHealthPoints;
        public int HealthPoints { get; set; }

        private void Start()
        {
            _selectionController = Camera.GetComponent<SelectionController>();
            _cameraMovement = Camera.GetComponent<CameraMovement>();
            HealthPoints = _maxHealthPoints;
        }

        private void OnGUI()
        {
            if (HealthPoints > 0)
            {
                if (_cameraMovement.State == CameraMovement.CameraState.Strategic)
                {
                    Vector2 hpPosition = Camera.WorldToScreenPoint(transform.position + Vector3.up * UnitHeight);
                    hpPosition.y = Screen.height - hpPosition.y;
                    hpPosition.x -= HealthBarSize.x / 2;
                    Vector2 remainingHealth = new Vector2(HealthBarSize.x * HealthPoints / _maxHealthPoints, HealthBarSize.y);
                    Utils.DrawScreenRect(new Rect(hpPosition, remainingHealth), Constants.Colors.HealthPointsBarMainColor);
                    Utils.DrawScreenRectBorder(new Rect(hpPosition, HealthBarSize), BarBorderWidth, Constants.Colors.HealthPointsBarBorderColor);
                }
            }
            else
            {
                StartCoroutine(Die());
            }
        }

        private IEnumerator Die()
        {
            _selectionController.Remove(gameObject);
            GetComponent<Animator>().SetBool(Constants.AnimatorParameters.Alive, false);
            GetComponent<NavMeshAgent>().isStopped = true;
            GetComponentInChildren<FieldOfView>().Die();
            yield return new WaitForSeconds(Constants.Intervals.FromDeathToDisappearing);
            Destroy(gameObject);
        }
    }
}
