using UnityEngine;

namespace Assets.Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        public enum CameraState
        {
            Strategic,
            Transition,
            FirstPerson
        }

        public CameraState State;
        private bool _transitionToFP;

        private const float Speed = 10.0f;
        private const uint StepsCount = 100;
        private uint _step;
        private Vector3 _translationStep;
        private Vector3 _rotationStep;

        private GameObject _attachedGameObject;
        private Vector3 _rotationOnFirstPersonStateEntrance;

        private void Start()
        {
            State = CameraState.Strategic;
        }

        private void Update()
        {
            if (State == CameraState.Strategic)
            {
                float deltaX = Input.GetAxis(Constants.Axes.Horizontal);
                float deltaZ = Input.GetAxis(Constants.Axes.Vertical);
                Vector3 forward = transform.forward;
                forward.y = 0;
                Vector3 cameraMovement = (forward.normalized * deltaZ + transform.right * deltaX) * Speed * Time.deltaTime;
                transform.Translate(cameraMovement, Space.World);
            }
            if (State == CameraState.Transition)
            {
                transform.Translate(_translationStep, Space.World);
                transform.eulerAngles += _rotationStep;
                _step++;
                if (_step == StepsCount)
                {
                    if (_transitionToFP)
                    {
                        State = CameraState.FirstPerson;
                        _rotationOnFirstPersonStateEntrance = transform.eulerAngles;
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

        public void SwitchToFP(GameObject gameObj)
        {
            if (State != CameraState.Strategic)
            {
                return;
            }
            _attachedGameObject = gameObj;
            Transform to = gameObj.transform;
            _step = 0;
            State = CameraState.Transition;
            _rotationStep = (to.eulerAngles - transform.eulerAngles) / StepsCount;
            _translationStep = (to.position - transform.position + Vector3.up) / StepsCount;
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
            transform.eulerAngles = _rotationOnFirstPersonStateEntrance;
            _step = 0;
            State = CameraState.Transition;
            _rotationStep = -_rotationStep;
            _translationStep = -_translationStep;
            _transitionToFP = false;
        }
    }
}
