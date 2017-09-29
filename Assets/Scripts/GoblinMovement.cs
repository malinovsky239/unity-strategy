using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class GoblinMovement : MonoBehaviour
    {
        public GameObject Leader;
        private CharacterController _characterController;
        private Animator _animator;
        private NavMeshAgent _agent;
        private Attack _attack;
        private FieldOfView _fieldOfView;
        private bool _alive = true;
        public float Speed = 0.0f;
        private float _attackingDistance = 3f;

        private Vector3 _targetCoordinate;
        private Vector3 _lastFramePosition;
        public float ForwardCoeff, RightCoeff;
        private GameObject _target;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _attack = GetComponent<Attack>();
            _fieldOfView = GetComponent<FieldOfView>();
        }

        public void MoveTo(Vector3 destination)
        {
            _animator.SetFloat(Constants.AnimatorParameters.Speed, 10);
            if (Leader == null)
            {
                _targetCoordinate = destination;
                _agent.destination = destination;
            }
            else
            {
                _targetCoordinate = destination + Leader.transform.forward * ForwardCoeff + Leader.transform.right * RightCoeff;
            }
        }

        private const float Eps = 1.0f;

        private void Update()
        {
            float deltaHeight = _characterController.isGrounded ? 0 : -1;
            _characterController.Move(new Vector3(0, deltaHeight * Time.deltaTime, 0));

            if (!_alive)
            {
                return;
            }
            if (GetComponent<HealthPointsBar>().HealthPoints <= 0)
            {
                _alive = false;
                return;
            }

            if (Leader != null)
            {
                _agent.destination = Leader.transform.position + Leader.transform.forward * ForwardCoeff + Leader.transform.right * RightCoeff;

                // avoid stupid regrouping when the leader turns by 180 degrees instantly
                Vector3 curPos = transform.position;
                Vector3 curDest = _agent.destination;
                Vector3 newDest = Leader.transform.position + Leader.transform.forward * ForwardCoeff - Leader.transform.right * RightCoeff;
                if ((newDest - curPos).sqrMagnitude < (curDest - curPos).sqrMagnitude && (curDest - curPos).sqrMagnitude > 1)
                {
                    RightCoeff = -RightCoeff;
                    _agent.destination = newDest;
                }
            }

            if (Utils.HeightIgnoringDistance(_targetCoordinate, transform.position) > Eps && !(Utils.HeightIgnoringDistance(_lastFramePosition, transform.position) < 0.001f))
            {
                _animator.SetFloat(Constants.AnimatorParameters.Speed, 10);
            }
            else
            {
                _animator.SetFloat(Constants.AnimatorParameters.Speed, 0);
                _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, false);
                foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag(Constants.Tags.Enemy))
                {
                    if (gameObj.GetComponent<HealthPointsBar>().HealthPoints > 0)
                    {
                        if (Utils.SqrDistance(this.gameObject, gameObj) < _attackingDistance)
                        {
                            transform.rotation = Quaternion.LookRotation(gameObj.transform.position - transform.position);
                            _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, true);
                            if (!_attack.IsAttacking)
                            {
                                _attack.IsAttacking = true;
                                StartCoroutine(_attack.BringDamage(gameObj));
                            }
                            break;
                        }
                        else if (_fieldOfView.IsInField(gameObj.transform.position))
                        {
                            _agent.SetDestination(gameObj.transform.position);
                            _animator.SetFloat(Constants.AnimatorParameters.Speed, 10);
                            Leader = null; // TODO: break formation only temporarily
                        }
                    }
                }
            }

            _lastFramePosition = transform.position;
        }
    }
}
