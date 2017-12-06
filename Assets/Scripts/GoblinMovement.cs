using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class GoblinMovement : MonoBehaviour
    {
        public GameObject GroupLeader { get; set; }
        public float ForwardCoeff { get; set; }
        public float RightCoeff { get; set; }

        private CharacterController _characterController;
        private Animator _animator;
        private NavMeshAgent _agent;
        private Attack _attack;
        private FieldOfView _fieldOfView;
        private HealthPointsBar _hp;

        private const float AttackingDistance = 3f;

        private Vector3 _targetCoordinate;
        private Vector3 _lastFramePosition;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _attack = GetComponent<Attack>();
            _fieldOfView = GetComponentInChildren<FieldOfView>();
            _hp = GetComponent<HealthPointsBar>();
        }

        public void MoveTo(Vector3 destination)
        {
            _animator.SetFloat(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Run);
            if (GroupLeader == null)
            {
                _targetCoordinate = destination;
                _agent.destination = destination;
            }
            else
            {
                _targetCoordinate = destination + GroupLeader.transform.forward * ForwardCoeff + GroupLeader.transform.right * RightCoeff;
            }
        }

        private void Update()
        {
            Vector3 deltaHeight = _characterController.isGrounded ? Vector3.zero : Vector3.down;
            _characterController.Move(deltaHeight * Time.deltaTime);

            if (_hp.HealthPoints <= 0)
            {
                return;
            }

            if (GroupLeader != null)
            {
                _agent.destination = GroupLeader.transform.position + GroupLeader.transform.forward * ForwardCoeff + GroupLeader.transform.right * RightCoeff;

                // avoid stupid regrouping when the leader turns by 180 degrees instantly
                Vector3 curPos = transform.position;
                Vector3 curDest = _agent.destination;
                Vector3 newDest = GroupLeader.transform.position + GroupLeader.transform.forward * ForwardCoeff - GroupLeader.transform.right * RightCoeff;
                if ((newDest - curPos).sqrMagnitude < (curDest - curPos).sqrMagnitude && (curDest - curPos).sqrMagnitude > 1)
                {
                    RightCoeff = -RightCoeff;
                    _agent.destination = newDest;
                }
            }

            if (Utils.HeightIgnoringDistance(_targetCoordinate, transform.position) > Constants.LargeEps && !(Utils.HeightIgnoringDistance(_lastFramePosition, transform.position) < Constants.SmallEps))
            {
                _animator.SetFloat(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Run);
            }
            else
            {
                _animator.SetFloat(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Stand);
                _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, false);
                GameObject closestEnemyInTheFieldOfView = null;
                bool enemyWithinAttackingRange = false;
                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Constants.Tags.Enemy))
                {
                    if (enemy.GetComponent<HealthPointsBar>().HealthPoints > 0)
                    {
                        if (Utils.SqrDistance(gameObject, enemy) < AttackingDistance)
                        {
                            transform.rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
                            _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, true);
                            if (!_attack.IsAttacking)
                            {
                                _attack.IsAttacking = true;
                                enemyWithinAttackingRange = true;
                                StartCoroutine(_attack.BringDamage(enemy));
                            }
                            break;
                        }
                        if (_fieldOfView.IsInField(enemy.transform.position))
                        {
                            if (closestEnemyInTheFieldOfView == null ||
                                Utils.SqrDistance(gameObject, enemy) < Utils.SqrDistance(gameObject, closestEnemyInTheFieldOfView))
                            {
                                closestEnemyInTheFieldOfView = enemy;
                            }
                        }
                    }
                }
                if (!enemyWithinAttackingRange && closestEnemyInTheFieldOfView != null)
                {
                    _agent.SetDestination(closestEnemyInTheFieldOfView.transform.position);
                    _animator.SetFloat(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Run);
                    GroupLeader = null; // TODO: break formation only temporarily
                }
            }

            _lastFramePosition = transform.position;
        }
    }
}
