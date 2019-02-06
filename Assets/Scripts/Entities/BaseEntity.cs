﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Sangaku
{
    public abstract class BaseEntity : MonoBehaviour, IEntity  
    {
        /// <summary>
        /// List of IBehaviours that describe this Entity.
        /// </summary>
        public List<IBehaviour> Behaviours { get; private set; }

        /// <summary>
        /// Basic Entity setup. Every Entity needs to implement this to function.
        /// </summary>
        public void SetUpEntity()
        {
            Behaviours = GetComponentsInChildren<IBehaviour>().ToList();
            foreach (IBehaviour behaviour in Behaviours)
            {
                behaviour.Setup(this);
            }
            CustomSetup();
        }

        /// <summary>
        /// Additional Entity setup. Unique to every Entity that implements it.
        /// </summary>
        public virtual void CustomSetup() { }
    } 
}