﻿using UnityEngine;

namespace Sangaku
{
    public class OrbInteractionBehaviour : MonoBehaviour, IBehaviour
    {
        /// <summary>
        /// Riferimento all'entitià che controlla il Behaviour
        /// </summary>
        public IEntity Entity { get; private set; }
        /// <summary>
        /// True se il Behaviour è stato setuppato, false altrimenti
        /// </summary>
        public bool IsSetupped { get; private set; }

        /// <summary>
        /// Eseguo il setup del behaviour
        /// </summary>
        /// <param name="_entity"></param>
        public void Setup(IEntity _entity)
        {
            Entity = _entity;
            IsSetupped = true;
        }
    } 
}