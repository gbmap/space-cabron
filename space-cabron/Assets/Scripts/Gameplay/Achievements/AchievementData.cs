using System.Collections.Generic;
using SpaceCabron.Gameplay.Level;
using UnityEngine;

namespace SpaceCabron.Achievements
{
    [System.Serializable]
    public class AchievementLevelData {
        public BaseLevelConfiguration level;
        public string achievementId;
    }

    [CreateAssetMenu(menuName="Space Cabrón/Achievement Data")]
    public class AchievementData : ScriptableObject
    {
        public List<AchievementLevelData> Achievements;

        public AchievementLevelData Get(BaseLevelConfiguration level) {
            if (!Achievements.Exists(a => a.level == level)) {
                return null;
            }
            return Achievements.Find(a => a.level == level);
        }
    }
}