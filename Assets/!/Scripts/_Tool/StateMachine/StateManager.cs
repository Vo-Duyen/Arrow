using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace DanLangA
{
    public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
        protected BaseState<EState>                     CurrentState;

        protected bool IsTransitioningState = false;

        void Start()
        {
            CurrentState.EnterState();
        }

        void Update()
        {
            EState nextStateKey = CurrentState.GetNextState();
            if (!IsTransitioningState && !nextStateKey.Equals(CurrentState.StateKey))
            {
                StartCoroutine(TransitionToStateCoroutine(nextStateKey));
            }
            else if (!IsTransitioningState)
            {
                CurrentState.UpdateState();
            }
        }

        private IEnumerator TransitionToStateCoroutine(EState nextStateKey)
        {
            if (!States.ContainsKey(nextStateKey))
            {
                Debug.LogError($"State {nextStateKey} not found in States dictionary!");
                yield break;
            }

            IsTransitioningState = true;
            Debug.Log($"Transitioning from {CurrentState.StateKey} to {nextStateKey}");

            // Đợi ExitState hoàn thành
            yield return StartCoroutine(CurrentState.ExitState());

            CurrentState = States[nextStateKey];
            CurrentState.EnterState();
            IsTransitioningState = false;
        }

        void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }

        void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }

        void OnCollisionEnter(Collision collision)
        {
            CurrentState.OnCollisionEnter(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            CurrentState.OnCollisionStay(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            CurrentState.OnCollisionExit(collision);
        }

        private void OnMouseDown()
        {
            CurrentState.OnMouseDown();
        }
    }
}