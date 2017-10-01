using UnityEngine;

namespace Assets.Scripts
{
    public class Fireball : MonoBehaviour
    {
        private const float Speed = 10.0f;
        private const float ExplodingDistance = 1.0f;
        private const int Damage = 30;
        public GameObject Creator { get; set; }

        private void Update()
        {
            transform.Translate(transform.forward * Speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != Creator)
            {
                foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag(Constants.Tags.Player))
                {
                    if (Utils.HeightIgnoringDistance(gameObj.transform.position, transform.position) < ExplodingDistance)
                    {
                        gameObj.GetComponent<HealthPointsBar>().HealthPoints -= Damage;
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
