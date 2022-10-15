using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using Steamworks;
using UnityEngine;
using SpaceCabron.Achievements;

namespace SpaceCabron.Gameplay
{
    public class AchievementsManager : MonoBehaviour
    {
        public AchievementData AchievementData;

        void Awake() {
            if (!SteamManager.Initialized) {
                Steamworks.SteamAPI.Init();
            }
        }

        void OnEnable() {
            MessageRouter.AddHandler<Messages.MsgLevelWon>(Callback_OnLevelWon);
        }

        void OnDisable() {
            MessageRouter.RemoveHandler<Messages.MsgLevelWon>(Callback_OnLevelWon);
        }

        private void Callback_OnLevelWon(MsgLevelWon msg)
        {
            var achievement = AchievementData.Get(LevelLoader.CurrentLevelConfiguration);
            if (achievement == null) {
                return;
            }

            UnlockAchievement(achievement.achievementId);
        }

        // Update is called once per frame
        void Update()
        {
            if (!SteamManager.Initialized) {
                return;
            }

            Steamworks.SteamAPI.RunCallbacks();
        }

        private void UnlockAchievement(string achievementId) {
            if (SteamUserStats.GetAchievement(achievementId, out bool achieved)) {
                if (!achieved) {
                    Debug.Log("Achieving: " + achievementId);
                    SteamUserStats.SetAchievement(achievementId);
                }
            }
        }
    }
}