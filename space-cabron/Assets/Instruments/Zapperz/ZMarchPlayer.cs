using System;
using QFSW.QC;
using UnityEngine;

namespace Z
{
    public abstract class ZBaseMarchPlayer : MonoBehaviour
    {
        [Command]
        [Range(30, 320)]
        public static int BPM = 80;

        public int CurrentBar { get; set; }
        public int CurrentBeat
        {
            get { return March.CurrentNoteIndex; }
        }
        public int NotesInBar
        {
            get { return March.Beats.Length; }
        }

        public float CurrentBeatTime
        {
            get
            {
                return ZMarchGenerator.GetBeatType(BPM, CurrentBeat);
            }
        }

        private ZMarch _baseMarch = ZMarch.Empty;
        public ZMarch BaseMarch
        {
            get { return _baseMarch; }
            set
            {
                _baseMarch = value;
                if (Improviser != null && Improviser.HasImprovisers)
                {
                    _improvisedMarchCache = Improviser.Improvise(value, value.CurrentNoteIndex);
                }
            }
        }

        public ZMarch March
        {
            get
            {
                if (Improviser != null && Improviser.HasImprovisers)
                {
                    return _improvisedMarchCache;
                }
                return BaseMarch;
            }
            set
            {
                BaseMarch = value;
            }
        }

        private ZMarch _improvisedMarchCache = ZMarch.Empty;

        public ZMarchImproviser Improviser { get; set; }

        public System.Action<int> OnBeat; // param = beatIndex;
        public System.Action<int> OnBar; // param = barIndex
        public System.Action<int> OnNewBeat;


        protected virtual void Awake()
        {
            Improviser = new ZMarchImproviser();
        }

        protected virtual void OnEnable()
        {
            OnBar += _Cb_OnBar;
        }

        protected virtual void OnDisable()
        {
            OnBar -= _Cb_OnBar;
        }

        private void _Cb_OnBar(int barIndex)
        {
            if (Improviser != null)
            {
                _improvisedMarchCache = Improviser.Improvise(BaseMarch, barIndex);
            }
        }
    }

    public class ZMarchPlayer : ZBaseMarchPlayer
    {
        float t;

        private void Update()
        {
            if (March.IsEmpty) return;

            float bt = ZMarchGenerator.GetBeatType(BPM, March.CurrentNoteType);

            t = Mathf.Min(bt, t + Time.deltaTime);
            if (t >= bt)
            {
                OnBeat?.Invoke(March.CurrentNoteType);

                bool ended;
                March.Advance(out ended);

                CurrentBar += System.Convert.ToInt32(ended);
                if (ended)
                {
                    OnBar?.Invoke(CurrentBar);
                }

                t = 0f;
            }
        }
    }
}