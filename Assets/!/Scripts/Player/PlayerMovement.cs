// Author: DanlangA

using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace _.Scripts.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly  int      IsWalking = Animator.StringToHash("IsWalking");
        [SerializeField] private Animator animator;
        
        [Header("Navigation Movement")]
        [SerializeField] private NavMeshAgent agent;
        
        [Header("Joystick Movement")]
        [SerializeField] private RectTransform joystick;
        [SerializeField] private RectTransform floatingJoystick;
        [SerializeField] private RectTransform knob;
        [SerializeField] private Vector2 joystickSize;
        private Finger _movementFinger;
        [SerializeField] private Vector2 movementAmount;
        
        private void Awake()
        {
            agent    = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            floatingJoystick = joystick.GetChild(0).GetComponent<RectTransform>();
            knob             = floatingJoystick.GetChild(0).GetComponent<RectTransform>();
            joystickSize     = joystick.sizeDelta;
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerMove += OnFingerMove;
            Touch.onFingerUp += OnFingerUp;
        }

        private void OnDisable()
        {
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerMove -= OnFingerMove;
            Touch.onFingerUp -= OnFingerUp;
        }

        private void Update()
        {
            Vector3 scaleMovement = new Vector3(movementAmount.x, 0f, movementAmount.y) * Time.deltaTime * agent.speed;
            agent.transform.LookAt(agent.transform.position + scaleMovement, Vector3.up);
            agent.Move(scaleMovement);
        }

        private void OnFingerDown(Finger finger)
        {
            if (_movementFinger == null && finger.screenPosition.x < Screen.width / 2f)
            {
                _movementFinger = finger;
                movementAmount = Vector2.zero;
                joystick.gameObject.SetActive(true);
                joystick.anchoredPosition = ClampStartPosition(finger.screenPosition);
            }
        }

        private Vector2 ClampStartPosition(Vector2 screenPosition)
        {
            screenPosition.x = screenPosition.x > joystickSize.x / 2f ? screenPosition.x : joystickSize.x / 2f;
            screenPosition.y = screenPosition.y > joystickSize.y / 2f ? (screenPosition.y > Screen.height - joystickSize.y / 2f ? Screen.height - joystickSize.y / 2f : screenPosition.y) : joystickSize.y / 2f;
            return screenPosition;
        }

        private void OnFingerMove(Finger finger)
        {
            if (_movementFinger == finger)
            {
                float   maxMovement  = joystickSize.x / 2f;
                Touch   currentTouch = finger.currentTouch;
                knob.anchoredPosition = Vector2.Distance(currentTouch.screenPosition, joystick.anchoredPosition) > maxMovement ? (currentTouch.screenPosition - joystick.anchoredPosition).normalized * maxMovement : currentTouch.screenPosition - joystick.anchoredPosition;
                movementAmount = knob.anchoredPosition / maxMovement;
                animator.SetBool(id:IsWalking, true);
            }
        }

        private void OnFingerUp(Finger finger)
        {
            if (_movementFinger == finger)
            {
                _movementFinger = null;
                knob.anchoredPosition = Vector2.zero;
                movementAmount = Vector2.zero;
                joystick.gameObject.SetActive(false);
                animator.SetBool(id:IsWalking, false);
            }
        }

        // private void OnGUI()
        // {
        //     GUIStyle style = new GUIStyle()
        //                      {
        //                          fontSize  = 15,
        //                          fontStyle = FontStyle.Bold,
        //                          normal = new GUIStyleState()
        //                                   {
        //                                       textColor = Color.red
        //                                   }
        //                      };
        //     if (_movementFinger != null)
        //     {
        //         GUI.Label(new Rect(10, 35, 500, 20), $"Finger Start Position: {_movementFinger.currentTouch.startScreenPosition}", style);
        //         GUI.Label(new Rect(10, 65, 500, 20), $"Finger Current Position: {_movementFinger.currentTouch.screenPosition}", style);
        //     }
        //     else
        //     {
        //         GUI.Label(new Rect(10, 35, 500, 20), "No Current Movement Touch", style);
        //     }
        //
        //     GUI.Label(new Rect(10, 10, 500, 20), $"Screen Size ({Screen.width}, {Screen.height})", style);
        // }
    }
}