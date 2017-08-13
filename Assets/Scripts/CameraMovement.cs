using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float _speed = 10.0f;
    private Vector3 _transition;
    private Vector3 _rotation;
    private uint _step;
    private const uint StepsCount = 100;
    private GameObject _attachedGameObject;
    private Vector3 _enterRotation;

    public enum CameraState
    {
        Strategic,
        Transition,
        FirstPerson
    }

    public CameraState State;
    private bool _transitionToFP;

    void Start()
    {
        State = CameraState.Strategic;
    }

    void Update()
    {
        if (State == CameraState.Strategic)
        {
            float deltaX = Input.GetAxis("Horizontal");
            float deltaZ = Input.GetAxis("Vertical");
            Vector3 forward = transform.forward;
            forward.y = 0;
            Vector3 cameraMovement = (forward.normalized * deltaZ + transform.right * deltaX) * _speed * Time.deltaTime;
            transform.Translate(cameraMovement, Space.World);
        }
        if (State == CameraState.Transition)
        {
            transform.Translate(_transition, Space.World);
            transform.eulerAngles += _rotation;
            _step++;
            if (_step == StepsCount)
            {
                if (_transitionToFP)
                {
                    State = CameraState.FirstPerson;
                    _enterRotation = transform.eulerAngles;
                    GetComponent<MouseLook>().enabled = true;
                    transform.parent = _attachedGameObject.transform;
                }
                else
                {
                    State = CameraState.Strategic;
                }
            }
        }
    }

    public void SwitchToFP(GameObject gameObj, Transform to)
    {
        if (State != CameraState.Strategic)
        {
            return;
        }
        _step = 0;
        State = CameraState.Transition;
        _rotation = (to.eulerAngles - transform.eulerAngles) / StepsCount;
        _transition = (to.position - transform.position + Vector3.up) / StepsCount;
        _attachedGameObject = gameObj;
        _transitionToFP = true;
    }

    public void SwitchToStrategic()
    {
        if (State != CameraState.FirstPerson)
        {
            return;
        }
        GetComponent<MouseLook>().enabled = false;
        transform.parent = null;
        transform.eulerAngles = _enterRotation;
        _step = 0;
        State = CameraState.Transition;
        _transition = -_transition;
        _rotation = -_rotation;
        _transitionToFP = false;
    }
}
