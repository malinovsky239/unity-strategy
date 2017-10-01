using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Attack : MonoBehaviour
    {
        public bool IsAttacking { get; set; }
        [SerializeField] private int _damage;

        public IEnumerator BringDamage(GameObject prey, float pauseBeforeAttack = Constants.Intervals.DefaultPauseBeforeAttack)
        {
            yield return new WaitForSeconds(pauseBeforeAttack);
            prey.GetComponent<HealthPointsBar>().HealthPoints -= _damage;
            IsAttacking = false;
        }
    }
}
