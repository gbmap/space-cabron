using Gmap.CosmicMusicUtensil;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCabron.UI
{
    public class UIUpgradeInfoItem : MonoBehaviour
    {
        Image icon;
        public ITurntable Turntable { get; private set; }
        public Improvisation Improvisation { get; private set; }

        static ImprovisationToIcon improvisationToIcon;
        static ImprovisationToIcon ImprovisationToIcon
        {
            get
            {
                if (improvisationToIcon == null)
                    improvisationToIcon = Resources.Load<ImprovisationToIcon>("ImprovisationToIcon");
                return improvisationToIcon;
            }
        }

        public static bool HasIcon(Improvisation improv)
        {
            return ImprovisationToIcon.GetIcon(improv) != null;
        }

        void Awake()
        {
            icon = GetComponent<Image>();
        }

        void OnDisable()
        {
            Turntable.OnNote -= OnNote;
        }

        public void Configure(
            ITurntable turntable, 
            Improvisation improvisation
        ) {
            this.Improvisation = improvisation;
            this.Turntable = turntable;

            Sprite iconSprite = improvisationToIcon.GetIcon(improvisation);
            icon.sprite = iconSprite;
            turntable.OnNote += OnNote;
        }

        private void OnNote(OnNoteArgs obj)
        {
            if (icon == null)
                return;

            bool shouldApply = Improvisation.ShouldApply(
                Turntable.Melody, 
                Turntable.BarIndex, 
                Turntable.Melody.NoteArray, 
                Turntable.NoteIndex
                // Turntable advances index when queueing new notes, hence -1.
            );

            icon.color = shouldApply ? Color.white : Color.gray;
        }
    }
}