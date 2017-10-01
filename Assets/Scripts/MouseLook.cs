using UnityEngine;

namespace Assets.Scripts
{
    public class MouseLook : MonoBehaviour
    {
        public enum RotationType
        {
            Horizontal = 0,
            Vertical = 1,
            Both = 2
        }

        [SerializeField] private RotationType _rotationDir;
        [SerializeField] private float _minAngle;
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _sensitivityHor;
        [SerializeField] private float _sensitivityVer;

        private float _rotationX;
        private float _rotationY;

        private void Update()
        {
            switch (_rotationDir)
            {
                case RotationType.Horizontal:
                    _rotationY += Input.GetAxis(Constants.Axes.MouseX) * _sensitivityHor;
                    break;

                case RotationType.Vertical:
                    _rotationX -= Input.GetAxis(Constants.Axes.MouseY) * _sensitivityVer;
                    _rotationX = Mathf.Clamp(_rotationX, _minAngle, _maxAngle);
                    break;

                case RotationType.Both:
                    _rotationY += Input.GetAxis(Constants.Axes.MouseX) * _sensitivityHor;
                    _rotationX -= Input.GetAxis(Constants.Axes.MouseY) * _sensitivityVer;
                    _rotationX = Mathf.Clamp(_rotationX, _minAngle, _maxAngle);
                    break;
            }
            transform.eulerAngles = new Vector3(_rotationX, _rotationY, 0);
        }
    }
}
