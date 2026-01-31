using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace intheclouds
{
    [AddComponentMenu(".Level Design/Event Sender")]
    public class EventSender : MonoBehaviour
    {
        [SerializeField]
        private bool _sendEventOnStart;
        [SerializeField]
        private float _sendEventOnStartDelay;
        [SerializeField]
        private UnityEvent _event;


        private IEnumerator Start()
        {
            if (_sendEventOnStart)
            {
                yield return new WaitForSeconds(_sendEventOnStartDelay);
                _event.Invoke();
            }
        }

        public void SendEvent()
        {
            InvokeEvent();
        }

        public void SendEventWithDelay(float delay)
        {
            Invoke(nameof(InvokeEvent), delay > 0f ? delay : 0f);
        }

        private void InvokeEvent()
        {
            _event?.Invoke();
        }
    }
}
