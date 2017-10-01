using UnityEngine;

namespace Assets.Scripts
{
    public class SelectGameObject : MonoBehaviour
    {
        private enum Mode
        {
            Normal,
            CameraRotation
        }

        private Mode _mode;

        private const float ScaleSensitivity = 10f, RotateSensitivity = 50f;
        private const float MinFieldOfView = 15, MaxFieldOfView = 60;

        private Camera _camera;
        private CameraMovement _cameraMovement;

        private SelectionController _selectionController;
        private bool _isSelecting;
        private Vector3 _selectionRectCorner;
        private const float SelectionRectBorderWidth = 2;

        private GameObject _destinationFlag;
        [SerializeField] private GameObject _flag;

        private void Start()
        {
            _mode = Mode.Normal;
            _camera = GetComponent<Camera>();
            _cameraMovement = GetComponent<CameraMovement>();
            _selectionController = GetComponent<SelectionController>();
        }

        private void Update()
        {
            ZoomCamera();

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.RightAlt))
            {
                _cameraMovement.SwitchToStrategic();
            }

            if (Input.GetMouseButtonDown(Constants.Mouse.LeftButton))
            {
                Vector3 clickPoint = Input.mousePosition;
                RaycastHit hit;
                if (Physics.Raycast(_camera.ScreenPointToRay(clickPoint), out hit))
                {
                    GameObject hitGameObject = hit.transform.gameObject;
                    SelectableUnit target = hitGameObject.GetComponent<SelectableUnit>();
                    if (target)
                    {
                        bool isAltPressed = Input.GetKey(KeyCode.LeftAlt);

                        if (isAltPressed)
                        {
                            _cameraMovement.SwitchToFP(hitGameObject);
                        }
                        else
                        {
                            SelectUnit(hitGameObject);
                        }
                    }
                    else
                    {
                        _isSelecting = true;
                        _selectionRectCorner = Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButtonUp(Constants.Mouse.LeftButton))
            {
                if (_isSelecting)
                {
                    SelectMultipleUnits();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Messenger.Broadcast(Constants.Messages.SwitchRotationWheelState);
                _mode = Mode.CameraRotation;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                Cursor.lockState = CursorLockMode.None;
                Messenger.Broadcast(Constants.Messages.SwitchRotationWheelState);
                _mode = Mode.Normal;
            }

            if (Input.GetMouseButton(Constants.Mouse.RightButton))
            {
                if (_mode == Mode.CameraRotation)
                {
                    RotateCamera();
                }
            }

            if (Input.GetMouseButtonDown(Constants.Mouse.RightButton))
            {
                if (_mode == Mode.Normal && _selectionController.CurrentSelection.Count > 0)
                {
                    PutDestinationFlag();
                }
            }
        }

        private void ZoomCamera()
        {
            float fieldOfView = Camera.main.fieldOfView;
            fieldOfView -= Input.GetAxis(Constants.Axes.MouseScrollWheel) * ScaleSensitivity;
            fieldOfView = Mathf.Clamp(fieldOfView, MinFieldOfView, MaxFieldOfView);
            Camera.main.fieldOfView = fieldOfView;
        }

        private void RotateCamera()
        {
            Vector3 rotationAxisBase = _camera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            transform.RotateAround(Utils.IgnoreHeight(rotationAxisBase), Vector3.up, Input.GetAxis(Constants.Axes.MouseX) * Time.deltaTime * RotateSensitivity);
        }

        private void SelectUnit(GameObject hitGameObject)
        {
            bool isControlPressed = Input.GetKey(KeyCode.LeftControl);
            if (!isControlPressed)
            {
                _selectionController.Clear();
                _selectionController.Add(hitGameObject);
            }
            else
            {
                _selectionController.Inverse(hitGameObject);
            }
        }

        private void SelectMultipleUnits()
        {
            _selectionController.Clear();
            _isSelecting = false;

            Vector3 selectionRectOppositeCorner = Input.mousePosition;

            RaycastHit hit;
            Vector3 cornerWorldInit = new Vector3(), cornerWorldOpposite = new Vector3();
            if (Physics.Raycast(_camera.ScreenPointToRay(_selectionRectCorner), out hit))
            {
                cornerWorldInit = Utils.IgnoreHeight(hit.point);
            }
            if (Physics.Raycast(_camera.ScreenPointToRay(selectionRectOppositeCorner), out hit))
            {
                cornerWorldOpposite = Utils.IgnoreHeight(hit.point);
            }
            Bounds selectionRectBounds = new Bounds();
            selectionRectBounds.SetMinMax(Vector3.Min(cornerWorldInit, cornerWorldOpposite),
                                          Vector3.Max(cornerWorldInit, cornerWorldOpposite));
            foreach (GameObject gameObj in GameController.WorldUnits)
            {
                Vector3 position = Utils.IgnoreHeight(gameObj.transform.position);
                if (selectionRectBounds.Contains(position))
                {
                    _selectionController.Add(gameObj);
                }
            }
        }

        private void PutDestinationFlag()
        {
            if (_destinationFlag)
            {
                Destroy(_destinationFlag.gameObject);
            }

            Vector3 clickPoint = Input.mousePosition;
            RaycastHit hit;
            if (Physics.Raycast(_camera.ScreenPointToRay(clickPoint), out hit))
            {
                Vector3 destination = hit.point;
                _destinationFlag = Instantiate(_flag, destination, Quaternion.identity);
                foreach (GameObject selectedGameObject in _selectionController.CurrentSelection)
                {
                    GoblinMovement goblinMovementControl = selectedGameObject.GetComponent<GoblinMovement>();
                    if (goblinMovementControl)
                    {
                        goblinMovementControl.MoveTo(destination);
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (_isSelecting)
            {
                var rect = Utils.GetScreenRect(_selectionRectCorner, Input.mousePosition);
                Utils.DrawScreenRect(rect, Constants.Colors.SelectionRect);
                Utils.DrawScreenRectBorder(rect, SelectionRectBorderWidth, Constants.Colors.SelectionRectBorder);
            }
        }
    }
}
