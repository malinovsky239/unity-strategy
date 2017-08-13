using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NecromancerBehaviour : MonoBehaviour
{
    private const int MinDist = 3;
    private const int MaxDist = 7;
    private const int SafeDist = 5;
    private NavMeshAgent _agent;
    private Animator _animator;        
    private FieldOfView _fieldOfView;
    private HealthPointsBar _hp;
    [SerializeField] private GameObject _fireballPrefab;
    private bool _fireballSingleton = false;
    private bool _alive = true;
    
    void Start ()
    {        
        _agent = GetComponent<NavMeshAgent>();        
        _fieldOfView = GetComponentInChildren<FieldOfView>();
        _hp = GetComponent<HealthPointsBar>();
        _animator = GetComponent<Animator>();
        _animator.SetInteger("speed", 2);
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

        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("player");
	    GameObject closestTarget = null;
	    foreach (var target in potentialTargets)
	    {
	        if (_fieldOfView.IsInField(target.transform.position))
	        {
	            if (closestTarget == null || Utils.SqrDistance(this.gameObject, target) < Utils.SqrDistance(this.gameObject, closestTarget))
	            {
	                closestTarget = target;
	            }
            }
	    }

	    if (closestTarget)
	    {
	        if (!_fireballSingleton)
	        {
	            _fireballSingleton = true;
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
	        float sumX = 0, sumY = 0;
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("enemy"))
            {
                if (unit != this.gameObject)
                {
                    cnt++;
                    sumX += unit.transform.position.x;
                    sumY += unit.transform.position.y;
                }
            }
	        float avgX = sumX / cnt, avgY = sumY / cnt;
            Vector3 center = new Vector3(avgX, 0, avgY);
	        center.y = Terrain.activeTerrain.SampleHeight(center);

	        if ((transform.position - center).magnitude > SafeDist)
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
        _animator.SetInteger("speed", 0);
        _agent.isStopped = true;
        _animator.SetBool("castingMagic", true);
        StartCoroutine(CastingSpell(target));
    }

    private IEnumerator CastingSpell(GameObject target)
    {
        yield return new WaitForSeconds(3);

        GameObject fireball = Instantiate(_fireballPrefab) as GameObject;
        fireball.GetComponent<Fireball>().Creator = this.gameObject;        
        fireball.transform.position = transform.position + Vector3.up * 0.5f;                
        fireball.transform.LookAt(target.transform.position);
        fireball.transform.forward = new Vector3(fireball.transform.forward.x, 0, fireball.transform.forward.z);

        _animator.SetInteger("speed", 2);
        _animator.SetBool("castingMagic", false);
        _agent.isStopped = false;

        StartCoroutine(UnblockFireball());        
    }

    private IEnumerator UnblockFireball()
    {
        yield return new WaitForSeconds(5);
        _fireballSingleton = false;
    }

    Vector3 MoveInRandomDirection(Vector3 start)
    {
        while (true)
        {
            float range = Random.Range(MinDist, MaxDist);
            Vector2 movementAttempt = Random.insideUnitCircle * range;
            Vector3 destinationPoint = start + new Vector3(movementAttempt.x, 0, movementAttempt.y);
            destinationPoint.y = Terrain.activeTerrain.SampleHeight(destinationPoint);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destinationPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                return destinationPoint;
            }
        }
    }
}
