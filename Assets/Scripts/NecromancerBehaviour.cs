using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class NecromancerBehaviour : MonoBehaviour
    {
        private const int PathElementMinLength = 3;
        private const int PathElementMaxLength = 7;
        private const int SafeWalkAwayDist = 5;
        private const float FireballTrajectoryHeight = 0.5f;
        private const float MaxRandomPointDeviation = 1.0f;

        private NavMeshAgent _agent;
        private Animator _animator;
        private FieldOfView _fieldOfView;
        private HealthPointsBar _hp;

        [SerializeField] private GameObject _fireballPrefab;
        private bool _canCastFireball;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _fieldOfView = GetComponentInChildren<FieldOfView>();
            _hp = GetComponent<HealthPointsBar>();
            _animator = GetComponent<Animator>();
            _animator.SetInteger(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Walk);
            _canCastFireball = true;
        }

        private void Update()
        {
            if (_hp.HealthPoints <= 0)
            {
                return;
            }

            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(Constants.Tags.Player);
            GameObject closestTarget = null;
            foreach (var target in potentialTargets)
            {
                if (_fieldOfView.IsInField(target.transform.position))
                {
                    if (closestTarget == null || Utils.SqrDistance(gameObject, target) < Utils.SqrDistance(gameObject, closestTarget))
                    {
                        closestTarget = target;
                    }
                }
            }

            if (closestTarget)
            {
                if (_canCastFireball)
                {
                    _canCastFireball = false;
                    CastFireball(closestTarget);
                }
                else
                {
                    // run away since the enemy can hit us!
                    Vector3 runAwayDirection = (transform.position - closestTarget.transform.position).normalized;
                    _agent.destination = transform.position + runAwayDirection;
                }
                return;
            }

            if (!_agent.hasPath)
            {
                // necromancer is patrolling the surroundings 
                // but trying not to go too far away from being surrounded by friendly units

                int cnt = 0;
                Vector3 sumPos = Vector3.zero;
                foreach (GameObject unit in GameObject.FindGameObjectsWithTag(Constants.Tags.Enemy))
                {
                    if (unit != gameObject)
                    {
                        cnt++;
                        sumPos += unit.transform.position;
                    }
                }

                Vector3 center = cnt > 0 ? sumPos / cnt : transform.position;
                center.y = Terrain.activeTerrain.SampleHeight(center);

                if ((transform.position - center).magnitude > SafeWalkAwayDist)
                {
                    _agent.destination = center;
                }
                else
                {
                    _agent.destination = MoveInRandomDirection(transform.position);
                }
            }
        }

        private void CastFireball(GameObject target)
        {
            _animator.SetInteger(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Stand);
            _agent.isStopped = true;
            _animator.SetBool(Constants.AnimatorParameters.CastingMagic, true);
            StartCoroutine(CastingSpell(target));
        }

        private IEnumerator CastingSpell(GameObject target)
        {
            yield return new WaitForSeconds(Constants.Intervals.FireballCastingPreparations);

            GameObject fireball = Instantiate(_fireballPrefab);
            fireball.GetComponent<Fireball>().Creator = gameObject;
            fireball.transform.position = transform.position + Vector3.up * FireballTrajectoryHeight;
            fireball.transform.LookAt(target.transform.position);
            fireball.transform.forward = Utils.IgnoreHeight(fireball.transform.forward);

            _animator.SetInteger(Constants.AnimatorParameters.SpeedParam, Constants.Speed.Walk);
            _animator.SetBool(Constants.AnimatorParameters.CastingMagic, false);
            _agent.isStopped = false;

            StartCoroutine(UnblockFireball());
        }

        private IEnumerator UnblockFireball()
        {
            yield return new WaitForSeconds(Constants.Intervals.FireballRecharging);
            _canCastFireball = true;
        }

        private Vector3 MoveInRandomDirection(Vector3 start)
        {
            while (true)
            {
                float range = Random.Range(PathElementMinLength, PathElementMaxLength);
                Vector2 movementAttempt = Random.insideUnitCircle * range;
                Vector3 destinationPoint = start + Utils.IgnoreHeight(movementAttempt);
                destinationPoint.y = Terrain.activeTerrain.SampleHeight(destinationPoint);
                NavMeshHit validDestination;
                if (NavMesh.SamplePosition(destinationPoint, out validDestination, MaxRandomPointDeviation, NavMesh.AllAreas))
                {
                    return validDestination.position;
                }
            }
        }
    }
}
