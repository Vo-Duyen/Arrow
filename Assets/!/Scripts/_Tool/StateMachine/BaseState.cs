// Author: DanlangA

using System;
using UnityEngine;
using System.Collections;

namespace DanLangA
{
    public abstract class BaseState<EState> where EState : Enum
    {
        protected BaseState(EState key)
        {
            StateKey = key;
        }

        public          EState      StateKey { get; private set; }
        public abstract void        EnterState();
        public abstract void        UpdateState();
        public abstract IEnumerator ExitState();
        public abstract EState      GetNextState();
        public abstract void        OnTriggerEnter(Collider    other);
        public abstract void        OnTriggerStay(Collider     other);
        public abstract void        OnTriggerExit(Collider     other);
        public abstract void        OnCollisionEnter(Collision collision);
        public abstract void        OnCollisionStay(Collision  collision);
        public abstract void        OnCollisionExit(Collision  collision);
        public abstract void        OnMouseDown();
    }
}