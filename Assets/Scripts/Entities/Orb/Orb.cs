﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Deirin.StateMachine;

namespace Sangaku
{
    public class Orb : StateMachineBase, IEntity, IContext
    {

        #region SM

        public override void SetUpSM()
        {
            context = new OrbSMContext(this);
            base.SetUpSM();
        }

        protected override void OnStateChange(IState _endedState)
        {
            GoToNext();
        }

        public void GoToNext()
        {
            StateMachine.SetTrigger("GoToNext");
        }

        #endregion

        #region Entity

        public List<IBehaviour> Behaviours
        {
            get;
            private set;
        }

        public void SetUpEntity()
        {
            Behaviours = GetComponentsInChildren<IBehaviour>().ToList();
            foreach (IBehaviour behaviour in Behaviours)
            {
                behaviour.Setup(this);
            }
        }

        #endregion

    }

    public class OrbSMContext : IContext
    {
        public IEntity OrbEntity;

        public OrbSMContext(IEntity _orbEntity)
        {
            OrbEntity = _orbEntity;
        }
    }
}
