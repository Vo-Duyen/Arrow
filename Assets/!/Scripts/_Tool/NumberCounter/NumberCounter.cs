// Author: DanlangA

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _.Scripts._Tool.NumberCounter
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class NumberCounter : MonoBehaviour
    {
        public  TextMeshProUGUI text;
        public  int             counterFPS   = 60;
        public  float           duration     = 1.5f;
        public  string          numberFormat = "N0";
        private int             _value;

        public int Value
        {
            get => _value;
            set
            {
                UpdateText(value);
                _value = value;
            }
        }

        private Coroutine _countingCoroutine;

        private void Awake()
        {
            text                  = GetComponent<TextMeshProUGUI>();
            text.enableAutoSizing = true;
            text.alignment        = TextAlignmentOptions.CaplineRight;
        }

        private void UpdateText(int newValue)
        {
            if (_countingCoroutine != null)
            {
                StopCoroutine(_countingCoroutine);
            }

            _countingCoroutine = StartCoroutine(CountText(newValue));
        }

        private IEnumerator CountText(int newValue)
        {
            WaitForSeconds wait          = new WaitForSeconds(1f / counterFPS);
            int            previousValue = _value;
            int stepAmount = newValue > previousValue
                                 ? Mathf.CeilToInt((newValue  - previousValue) / (counterFPS * duration))
                                 : Mathf.FloorToInt((newValue - previousValue) / (counterFPS * duration));

            if (newValue > previousValue)
            {
                while (newValue > previousValue)
                {
                    previousValue += stepAmount;
                    if (newValue < previousValue)
                    {
                        previousValue = newValue;
                    }

                    text.SetText(previousValue.ToString(numberFormat));
                    yield return wait;
                }
            }
            else
            {
                while (newValue < previousValue)
                {
                    previousValue += stepAmount;
                    if (newValue > previousValue)
                    {
                        previousValue = newValue;
                    }

                    text.SetText(previousValue.ToString(numberFormat));
                    yield return wait;
                }
            }
        }
        // Call method
        public void SetValue(int newValue)
        {
            Value = newValue;
        }
    }
}