using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Attack : MonoBehaviour
    {
        public bool IsAttacking;
        [SerializeField] private int Damage = 10;

        public IEnumerator BringDamage(GameObject prey, float waitingTime = 1f)
        {
            yield return new WaitForSeconds(waitingTime);
            prey.GetComponent<HealthPointsBar>().HealthPoints -= Damage;
            IsAttacking = false;
        }
    }
}
