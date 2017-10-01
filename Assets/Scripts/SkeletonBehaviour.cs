using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class SkeletonBehaviour : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private FieldOfView _fieldOfView;
        private Animator _animator;
        private HealthPointsBar _hp;
        private GameObject _prey;
        private Attack _attack;

        private const float AttackingRange = 3f;
        private const float AgentRadius = 1f / 30;

        private void Start()
        {
            _agent = gameObject.AddComponent<NavMeshAgent>();
            _agent.radius = AgentRadius;
            _agent.stoppingDistance = AttackingRange / 2;
            _agent.speed = Constants.Speed.Walk;

            _animator = GetComponent<Animator>();
            _fieldOfView = GetComponentInChildren<FieldOfView>();
            _attack = GetComponent<Attack>();
            _hp = GetComponent<HealthPointsBar>();
        }

        private void Update()
        {
            if (_hp.HealthPoints <= 0)
            {
                return;
            }

            _prey = null;
            foreach (GameObject potentialPrey in GameObject.FindGameObjectsWithTag(Constants.Tags.Player))
            {
                if (potentialPrey.GetComponent<HealthPointsBar>().HealthPoints > 0 && _fieldOfView.IsInField(potentialPrey.transform.position))
                {
                    if (!_prey || Utils.SqrDistance(gameObject, potentialPrey) < Utils.SqrDistance(gameObject, _prey))
                    {
                        _prey = potentialPrey;
                    }
                }
            }
            if (_prey != null)
            {
                _animator.SetBool(Constants.AnimatorParameters.EnemyWithinFieldOfView, true);
                if (Utils.SqrDistance(gameObject, _prey) < AttackingRange)
                {
                    _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, true);
                    if (!_attack.IsAttacking)
                    {
                        _attack.IsAttacking = true;
                        StartCoroutine(_attack.BringDamage(_prey));
                    }
                }
                else
                {
                    _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, false);
                    _agent.destination = _prey.transform.position;
                }
            }
            else
            {
                _animator.SetBool(Constants.AnimatorParameters.EnemyWithinFieldOfView, false);
                _animator.SetBool(Constants.AnimatorParameters.EnemyWithinAttackingRange, false);
            }
        }
    }
}
