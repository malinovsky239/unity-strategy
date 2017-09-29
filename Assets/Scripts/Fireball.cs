using UnityEngine;

namespace Assets.Scripts
{
    public class Fireball : MonoBehaviour
    {
        private float _speed = 10.0f;
        private const float ExplodingDistance = 1f;
        private const int Damage = 30;
        public GameObject Creator;

        private void Update()
        {
            transform.Translate(transform.forward * _speed * Time.deltaTime, Space.World);
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
