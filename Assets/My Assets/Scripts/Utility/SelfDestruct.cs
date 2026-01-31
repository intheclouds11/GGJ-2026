using System.Collections;
using UnityEngine;

namespace intheclouds
{
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField]
        private float _lifetime = 2f;
        [SerializeField]
        private bool _useLifetimeRange;
        [SerializeField]
        private float _lifetimeMin = 15f;
        [SerializeField]
        private float _lifetimeMax = 25f;

        private bool _destroy = true;


        private IEnumerator Start()
        {
            var particles = GetComponent<ParticleSystem>();
            var lifeTime = particles ? particles.main.startLifetime.constantMax :
                _useLifetimeRange ? Random.Range(_lifetimeMin, _lifetimeMax) : _lifetime;

            yield return new WaitForSeconds(lifeTime);

            if (_destroy)
            {
                Destroy(gameObject);
            }
        }

        public void CancelDestroy()
        {
            _destroy = false;
        }
    }
}