// ExternalCoolGame.cs
using System.Linq;
using System.Text.RegularExpressions;
using Menu;
using RainMeadow;
using HarmonyLib;

namespace Cyclesseum
{
    public partial class AscendedBalanceGameMode : ExternalArenaGameMode
    {
        public static ArenaSetup.GameTypeID MyGameModeName = new ArenaSetup.GameTypeID("Ascended Balance EXP", register: false);


        private int _timerDuration;

        public int tongueTimer;
        public static bool isAscendedBalanceGameMode(ArenaOnlineGameMode arena, out AscendedBalanceGameMode gameMode)
        {
            gameMode = null;
            if (arena.currentGameMode == MyGameModeName.value)
            {
                gameMode = (arena.registeredGameModes.FirstOrDefault(x => x.Key == MyGameModeName.value).Value as AscendedBalanceGameMode);
                return true;
            }
            return false;
        }

        // in the class MyCoolNewGameMode
        public override ArenaSetup.GameTypeID GetGameModeId
        {
            get
            {
                return MyGameModeName; // Set to YOUR cool game mode
            }
            set { GetGameModeId = value; }
        }

        public override bool IsExitsOpen(ArenaOnlineGameMode arena, On.ArenaBehaviors.ExitManager.orig_ExitsOpen orig, ArenaBehaviors.ExitManager self)
        {
            int playersStillStanding = self.gameSession.Players?.Count(player =>
                player.realizedCreature != null &&
                (player.realizedCreature.State.alive)) ?? 0;

            if (playersStillStanding == 1 && arena.arenaSittingOnlineOrder.Count > 1)
            {
                return true;
            }

            if (self.world.rainCycle.TimeUntilRain <= 100)
            {
                return true;
            }

            orig(self);

            return orig(self);
        }

        public override int TimerDuration
        {
            get { return _timerDuration; }
            set { _timerDuration = value; }
        }

        public override bool SpawnBatflies(FliesWorldAI self, int spawnRoom)
        {
            return false;
        }


    }
}

