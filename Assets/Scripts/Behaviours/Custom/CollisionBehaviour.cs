﻿using UnityEngine;

namespace Sangaku
{
    /// <summary>
    /// Behaviour che permette di interfacciarsi con le collisoni di unity in modo generico
    /// </summary>
    public class CollisionBehaviour : BaseBehaviour
    {
        [Header("Collision Events", order = 2)]
        [SerializeField] UnityCollisionEvent OnCollisionEnterEvent;
        [SerializeField] UnityCollisionEvent OnCollisionStayEvent;
        [SerializeField] UnityCollisionEvent OnCollisionExitEvent;

        protected virtual void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterEvent.Invoke(collision);
            CollisionEnter(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnCollisionStayEvent.Invoke(collision);
            CollisionStay(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnCollisionExitEvent.Invoke(collision);
            CollisionExit(collision);
        }

        protected virtual void CollisionEnter(Collision _collision) { }
        protected virtual void CollisionStay(Collision _collision) { }
        protected virtual void CollisionExit(Collision _collision) { }
    }
}
