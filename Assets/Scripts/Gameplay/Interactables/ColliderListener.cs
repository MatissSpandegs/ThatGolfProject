using System;
using UnityEngine;

namespace Gameplay.Interactables
{
    public class ColliderListener : MonoBehaviour
    {
        public Action<Collision> CollisionEnter;
        public Action<Collision> CollisionStay;
        public Action<Collision> CollisionExit;
        
        public Action<Collider> TriggerEnter;
        public Action<Collider> TriggerStay;
        public Action<Collider> TriggerExit;
        
        private void OnCollisionEnter(Collision collision)
        {
            CollisionEnter?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            CollisionStay?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            CollisionExit?.Invoke(collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other);
        }
    }
}
