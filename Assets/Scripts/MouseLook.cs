using UnityEngine;

namespace Assets.Scripts
{
    public class MouseLook : MonoBehaviour
    {
        private float _rotationX;
        private float _rotationY;

        public float MinAngle = -5;
        public float MaxAngle = 5;
        public float SensitivityHor = 3.0f;
        public float SensitivityVer = 3.0f;

        public enum RotationType
        {
            Horizontal = 0,
            Vertical = 1,
            Both = 2
        }

        public RotationType RotationDir;

        private void Update()
        {
            switch (RotationDir)
            {
                case RotationType.Horizontal:
                    _rotationY += Input.GetAxis(Constants.Axes.MouseX) * SensitivityHor;
                    break;

                case RotationType.Vertical:
                    _rotationX -= Input.GetAxis(Constants.Axes.MouseY) * SensitivityVer;
                    _rotationX = Mathf.Clamp(_rotationX, MinAngle, MaxAngle);
                    break;

                case RotationType.Both:
                    _rotationY += Input.GetAxis(Constants.Axes.MouseX) * SensitivityHor;
                    _rotationX -= Input.GetAxis(Constants.Axes.MouseY) * SensitivityVer;
                    _rotationX = Mathf.Clamp(_rotationX, MinAngle, MaxAngle);
                    break;
            }
            transform.eulerAngles = new Vector3(_rotationX, _rotationY, 0);
        }
    }
}
