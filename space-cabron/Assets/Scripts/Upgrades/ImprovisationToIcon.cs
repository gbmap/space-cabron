using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;


namespace SpaceCabron
{
    [CreateAssetMenu(menuName="Space Cabrón/Improvisation To Icon")]
    public class ImprovisationToIcon : ScriptableObject
    {
        public Sprite BreakNoteIcon;
        public Sprite MordentIcon;
        public Sprite TremoloIcon;

        public Sprite GetIcon(Improvisation type)
        {
            if (type is BreakNoteImprovisation)
                return BreakNoteIcon;
            if (type is MordentImprovisation)
                return MordentIcon;
            if (type is TremoloImprovisation)
                return TremoloIcon;
            return null;
        }
    }
}