using Gmap.CosmicMusicUtensil;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCabron.UI
{
    public class UIUpgradeInfoItem : MonoBehaviour
    {
        Image icon;
        ITurntable turntable;
        Improvisation improvisation;
        ImprovisationToIcon improvisationToIcon;

        void Awake()
        {
            improvisationToIcon = Resources.Load<ImprovisationToIcon>("ImprovisationToIcon");
            icon = GetComponent<Image>();
        }

        void OnDisable()
        {
            turntable.OnNote -= OnNote;
        }

        public void Configure(
            ITurntable turntable, 
            Improvisation improvisation
        ) {
            this.improvisation = improvisation;
            this.turntable = turntable;

            Sprite iconSprite = improvisationToIcon.GetIcon(improvisation);
            icon.sprite = iconSprite;
            turntable.OnNote += OnNote;
        }

        private void OnNote(OnNoteArgs obj)
        {
            if (icon == null)
                return;

            bool shouldApply = improvisation.ShouldApply(
                turntable.Melody, 
                turntable.BarIndex, 
                turntable.Melody.NoteArray, 
                turntable.NoteIndex
                // Turntable advances index when queueing new notes, hence -1.
            );

            icon.color = shouldApply ? Color.white : Color.gray;
        }
    }
}