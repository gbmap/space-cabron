using System.Linq;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using SpaceCabron.Gameplay.Interactables;
using SpaceCabron.Messages;
using UnityEngine;

namespace Gmap
{
    public class UpdateBackgroundShaderGlobalVariables : MonoBehaviour
    {
        const int nNotes = 100;
        float[] LastNoteTimes = new float[nNotes];
        float[] NoteTimes = new float[nNotes];
        float[] NextNoteTimes = new float[nNotes];
        int noteTimesCursor = 0;

        float[] EnemyKillTimes = new float[10];
        Vector4[] EnemyKillPositions = new Vector4[10];
        int enemyKillCursor = 0;

        float expectedNextNote = 0f;


        int Index
        {
            get { return Mathf.Clamp(gameObject.name[gameObject.name.Length - 1] - '0', 0, 0); }
        }

        void Awake() {
            MessageRouter.AddHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
            MessageRouter.AddHandler<MsgOnUpgradeTaken>(Callback_OnUpgradeTaken);
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
            UpdateBuffers();
            UpdateEnemyKillBuffers();
        }

        private void UpdateEnemyKillBuffers()
        {
            System.Array.ForEach(EnemyKillTimes, (x) => x = -10f);
            Shader.SetGlobalFloatArray("_EnemyKillTimes", EnemyKillTimes);
        }

        void OnDestroy() {
            MessageRouter.RemoveHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
            MessageRouter.RemoveHandler<MsgOnUpgradeTaken>(Callback_OnUpgradeTaken);
            MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        }

        private void Callback_OnUpgradeTaken(MsgOnUpgradeTaken obj){
        }

        private void Callback_OnPlayerSpawned(MsgOnPlayerSpawned obj){
        }

        void Update()
        {
            Shader.SetGlobalFloat("_EngineTime", Time.time);
        }

        public void OnNote(OnNoteArgs n)
        {
            float delta = Time.time - expectedNextNote;
            System.Array.ForEach(NoteTimes, (x) => x += delta);
            UpdateBuffers();

            Shader.SetGlobalFloat("_Beat", Time.time);
            Shader.SetGlobalFloat("_LastNoteDuration", n.Duration);

            expectedNextNote = Time.time + n.Duration;
        }

        public void OnBar(OnBarArgs args)
        {
            float startTime = Time.time + args.Turntable.LastNote.GetDuration(args.Turntable.BPS);

            MelodySwitcher ms = GetComponentInChildren<MelodySwitcher>();
            if (ms != null) {
                var improviser = args.Turntable.Improviser;

                int i = args.BarIndex % ms.Structure.Length;
                var melody = ms.GetMelody(ms.Structure[i]-'0'-1);
                startTime = UpdateNoteTimes(args, melody, improviser, startTime, args.Turntable.BarIndex, NoteTimes);
            }
            else {
                startTime = UpdateNoteTimes(args, args.Turntable.Melody, args.Turntable.Improviser, startTime, args.Turntable.BarIndex, NoteTimes);
                UpdateNoteTimes(args, args.Turntable.Melody, args.Turntable.Improviser, startTime, args.Turntable.BarIndex + 1, NoteTimes);
            }

            UpdateBuffers();
        }

        private void UpdateBuffers()
        {
            Shader.SetGlobalInteger("_NoteCount" + Index.ToString(), nNotes);
            Shader.SetGlobalFloatArray("_NoteTimes" + Index.ToString(), NoteTimes);
        }

        private void CacheLastNoteTimes()
        {
            System.Array.Copy(NoteTimes, 0, LastNoteTimes, 0, NoteTimes.Length);
        }

        private float UpdateNoteTimes(
            OnBarArgs args,
            Melody melody,
            Improviser improviser,
            float startTime,
            int barIndex,
            float[] noteTimes
        ) {
            var turntable = args.Turntable;
            // var melody = args.Turntable.Melody;
            // var improviser = args.Turntable.Improviser;
            var notes = Enumerable.Range(0, melody.Length)
                      .SelectMany(i => improviser.Improvise(melody, barIndex, melody.GetNote(i), i, false))
                      .ToArray();

            float value = startTime;
            noteTimes[noteTimesCursor] = value;
            for (int i = 0; i < notes.Length; i++)
            {
                value += notes[i].GetDuration(args.Turntable.BPS);
                noteTimes[noteTimesCursor] = value;
                noteTimesCursor = (noteTimesCursor + 1) % noteTimes.Length;
            }
            return value;
        }

        private void Callback_OnObjectDestroyed(MsgOnObjectDestroyed obj)
        {
            if (Camera.main == null) {
                return;
            }

            Vector3 viewportPos = Camera.main.WorldToScreenPoint(obj.health.transform.position);
            viewportPos.x/=Screen.width;
            viewportPos.y/=Screen.height;
            float time = Time.time;

            EnemyKillPositions[enemyKillCursor] = viewportPos;
            EnemyKillTimes[enemyKillCursor] = time;
            enemyKillCursor = (enemyKillCursor + 1) % EnemyKillTimes.Length;

            Shader.SetGlobalFloatArray("_EnemyKillTimes", EnemyKillTimes);
            Shader.SetGlobalVectorArray("_EnemyKillPositions", EnemyKillPositions);
        }

    }
}