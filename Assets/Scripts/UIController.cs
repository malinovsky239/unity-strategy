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
        private bool _UIActive;

        private void Start()
        {
            _UIActive = false;
            rotationWheel.SetActive(false);
            ChangeUnitCount(0);
            DeactiveFormationPanel();
        }

        public void ChangeUnitCount(int newCount)
        {
            goblinCount.text = newCount.ToString();
            if (newCount == 0)
            {
                goblinAvatar.SetActive(false);
                goblinCountDisplay.SetActive(false);
                formationsPanel.SetActive(false);
                _UIActive = false;
            }
            else
            {
                goblinAvatar.SetActive(true);
                goblinCountDisplay.SetActive(true);
                formationsPanel.SetActive(true);
                _UIActive = true;
            }
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
            Messenger.RemoveListener(Constants.Messages.SwitchRotationWheelState, SwitchRotationWheelState);
        }

        private void SwitchRotationWheelState()
        {
            rotationWheel.SetActive(!rotationWheel.activeSelf);
        }

        private void Update()
        {
            if (_UIActive)
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    DeactiveFormationPanel();
                    columnFormation.GetComponent<Image>().sprite = columnFormationActive;
                    Messenger<int>.Broadcast(Constants.Messages.FormationChanged, 1);
                }
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    DeactiveFormationPanel();
                    rowFormation.GetComponent<Image>().sprite = rowFormationActive;
                    Messenger<int>.Broadcast(Constants.Messages.FormationChanged, 2);
                }
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    DeactiveFormationPanel();
                    wedgeFormation.GetComponent<Image>().sprite = wedgeFormationActive;
                    Messenger<int>.Broadcast(Constants.Messages.FormationChanged, 3);
                }
            }
        }
    }
}
