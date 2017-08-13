using UnityEngine;
using UnityEngine.AI;

public class SkeletonBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;
    private FieldOfView _fieldOfView;
    private GameObject _prey;
    private Animator _animator;
    private HealthPointsBar _hp;
    private const float AttackingRange = 3f;
    private const float ScaleFactor = 15;
    private Attack _attack;
    private bool _alive = true;

	void Start ()
	{	    
        _agent = gameObject.AddComponent<NavMeshAgent>();
	    _agent.radius = 0.5f / ScaleFactor;
        _agent.stoppingDistance = AttackingRange / 2;
	    _agent.speed = 2f;
                
        _animator = GetComponent<Animator>();
	    _fieldOfView = GetComponentInChildren<FieldOfView>();
	    _attack = GetComponent<Attack>();
	    _hp = GetComponent<HealthPointsBar>();
	}    
	
	void Update ()
	{
        if (!_alive)
        {
            return;
        }
        if (_hp.HealthPoints <= 0)
        {
            _alive = false;
            return;
        }

        _prey = null;
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("player"))
	    {
	        if (gameObj.GetComponent<HealthPointsBar>().HealthPoints > 0 && _fieldOfView.IsInField(gameObj.transform.position))
	        {                
                if (!_prey || Utils.SqrDistance(this.gameObject, gameObj) < Utils.SqrDistance(this.gameObject.gameObject, _prey)) {
	                _prey = gameObj;
                }
	        }
	    }
	    if (_prey)
	    {
	        _animator.SetBool("EnemyIsWithinFieldOfView", true);
	        if (Utils.SqrDistance(this.gameObject, _prey) < AttackingRange)
	        {
	            _animator.SetBool("EnemyIsWithinAttackingRange", true);
	            if (!_attack.IsAttacking)
	            {
	                _attack.IsAttacking = true;
                    StartCoroutine(_attack.BringDamage(_prey));
	            }
	        }
	        else
	        {
	            _animator.SetBool("EnemyIsWithinAttackingRange", false);
	            _agent.destination = _prey.transform.position;
	        }
	    }
	    else
	    {
            _animator.SetBool("EnemyIsWithinFieldOfView", false);
            _animator.SetBool("EnemyIsWithinAttackingRange", false);
        }	    
	}
}
