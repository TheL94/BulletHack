﻿using UnityEngine;
using UnityEngine.Events;

namespace Deirin.Utility
{
    public class Timer : MonoBehaviour
    {
        [Multiline] public string description;
        public float time;
        public bool startCountingOnAwake = false;
        public bool repeat = true;

        public UnityEvent OnTimerStart, OnTimerEnd;

        bool countTime = false;
        [HideInInspector] public float timer;

        #region MonoBehaviour methods

        private void OnEnable()
        {
            if(repeat)
                OnTimerEnd.AddListener(ResetTimer);
        }

        private void OnDisable()
        {
            if(repeat)
                OnTimerEnd.RemoveListener(ResetTimer);
        }

        private void Awake()
        {
            if (startCountingOnAwake)
                StartTimer(time);
        }

        void Update()
        {
            if (countTime)
                timer += Time.deltaTime;
            if (timer >= time && countTime)
            {
                OnTimerEnd.Invoke();
                print(name + " ha finito");
            }
        }

        #endregion

        #region API

        public void StartTimer(float _time)
        {
            time = _time;
            countTime = true;
            print(name + " è iniziato");
        }

        public void StartTimer()
        {
            timer = 0;
            countTime = true;
            OnTimerStart.Invoke();
        }

        public void StopTimer()
        {
            timer = 0;
            countTime = false;
        }

        public void PauseTimer()
        {
            countTime = false;
        }

        public void ResetTimer()
        {
            timer = 0;
            if (repeat)
                countTime = true;
            else
                countTime = false;
        }

        public void Say(string _msg)
        {
            Debug.Log(_msg);
        }

        #endregion

    }
}