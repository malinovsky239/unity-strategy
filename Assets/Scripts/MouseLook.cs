using UnityEngine;

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

    void Update()
    {
        switch (RotationDir)
        {
            case RotationType.Horizontal:
                _rotationY += Input.GetAxis("Mouse X") * SensitivityHor;
                break;

            case RotationType.Vertical:
                _rotationX -= Input.GetAxis("Mouse Y") * SensitivityVer;
                _rotationX = Mathf.Clamp(_rotationX, MinAngle, MaxAngle);
                break;

            case RotationType.Both:
                _rotationY += Input.GetAxis("Mouse X") * SensitivityHor;
                _rotationX -= Input.GetAxis("Mouse Y") * SensitivityVer;
                _rotationX = Mathf.Clamp(_rotationX, MinAngle, MaxAngle);
                break;
        }
        transform.eulerAngles = new Vector3(_rotationX, _rotationY, 0);
    }
}
