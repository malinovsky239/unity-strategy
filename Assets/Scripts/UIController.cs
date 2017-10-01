using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Text goblinCount;
        [SerializeField] private GameObject goblinAvatar;
        [SerializeField] private GameObject goblinCountDisplay;
        [SerializeField] private GameObject formationsPanel;
        [SerializeField] private GameObject columnFormation;
        [SerializeField] private GameObject rowFormation;
        [SerializeField] private GameObject wedgeFormation;
        [SerializeField] private Sprite columnFormationActive;
        [SerializeField] private Sprite rowFormationActive;
        [SerializeField] private Sprite wedgeFormationActive;
        [SerializeField] private Sprite columnFormationInactive;
        [SerializeField] private Sprite rowFormationInactive;
        [SerializeField] private Sprite wedgeFormationInactive;
        [SerializeField] private GameObject rotationWheel;
        private bool _uiActive;

        private void Start()
        {
            _uiActive = false;
            rotationWheel.SetActive(false);
            ChangeUnitCount(0);
            DeactiveFormationPanel();
        }

        public void ChangeUnitCount(int newCount)
        {
            goblinCount.text = newCount.ToString();
            bool status = newCount != 0;
            goblinAvatar.SetActive(status);
            goblinCountDisplay.SetActive(status);
            formationsPanel.SetActive(status);
            _uiActive = status;
        }

        private void DeactiveFormationPanel()
        {
            columnFormation.GetComponent<Image>().sprite = columnFormationInactive;
            rowFormation.GetComponent<Image>().sprite = rowFormationInactive;
            wedgeFormation.GetComponent<Image>().sprite = wedgeFormationInactive;
        }

        private void Awake()
        {
            Messenger<int>.AddListener(Constants.Messages.CntChanged, ChangeUnitCount);
            Messenger.AddListener(Constants.Messages.SwitchRotationWheelState, SwitchRotationWheelState);
        }

        private void OnDestroy()
        {
            Messenger<int>.RemoveListener(Constants.Messages.CntChanged, ChangeUnitCount);
            Messenger.RemoveListener(Constants.Messages.SwitchRotationWheelState, SwitchRotationWheelState);
        }

        private void SwitchRotationWheelState()
        {
            rotationWheel.SetActive(!rotationWheel.activeSelf);
        }

        private void Update()
        {
            if (_uiActive)
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    DeactiveFormationPanel();
                    columnFormation.GetComponent<Image>().sprite = columnFormationActive;
                    Messenger<Constants.Formations>.Broadcast(Constants.Messages.FormationChanged, Constants.Formations.Column);
                }
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    DeactiveFormationPanel();
                    rowFormation.GetComponent<Image>().sprite = rowFormationActive;
                    Messenger<Constants.Formations>.Broadcast(Constants.Messages.FormationChanged, Constants.Formations.Row);
                }
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    DeactiveFormationPanel();
                    wedgeFormation.GetComponent<Image>().sprite = wedgeFormationActive;
                    Messenger<Constants.Formations>.Broadcast(Constants.Messages.FormationChanged, Constants.Formations.Wedge);
                }
            }
        }
    }
}
