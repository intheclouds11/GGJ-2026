using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace intheclouds
{
    [AddComponentMenu(".Level Design/Trigger Zone (Event)")]
    public class EventTriggerZone : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _eventToTrigger;
        [SerializeField]
        private bool _disableOnTrigger = true;

        private Collider _col;


        private void Awake()
        {
            if (_eventToTrigger.GetPersistentEventCount() == 0)
            {
                Debug.LogWarning($"[EventTriggerZone] No listeners assigned to {name}. Disabled GameObject", this);
                gameObject.SetActive(false);
                return;
            }

            _col = GetComponent<Collider>();
            if (!_col)
            {
                Debug.LogError($"EventTriggerZone] Need to add a collider to {name}", this);
                return;
            }

            _col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _eventToTrigger?.Invoke();
            if (_disableOnTrigger) _col.enabled = false;
        }

        [Button, PropertyOrder(-1)]
        private void TriggerEvent()
        {
            _eventToTrigger?.Invoke();
        }
    }
}