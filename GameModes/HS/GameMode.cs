// ExternalCoolGame.cs
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using Menu;
using RainMeadow;
using RainMeadow.UI;
using RainMeadow.UI.Components;
using UnityEngine;
using Unity.Mathematics;

namespace Cyclesseum
{
    public class HideAndSeekGameMode : ExternalArenaGameMode
    {
        public static ArenaSetup.GameTypeID MyGameModeName = new ArenaSetup.GameTypeID("Hide and Seek", register: false);

        private int _timerDuration;

        /*
         * 
         * Lobby data
         * 
         */
        public int hidersMinTimeSeconds;
        public int hidersTimePerCameraSeconds;
        public int hidersMaxTimeSeconds;

        public int seekersMinTimeSeconds;
        public int seekersTimePerCameraSeconds;
        public int seekersMaxTimeSeconds;

        public bool disableCollisions;

        /*
         * 
         * Client data
         * 
         */

        public bool isSeeker;

        public TabContainer.Tab? hideAndSeekTab;
        public HideAndSeekInterface? hideAndSeekInterface;


        /*
         * 
         * Internal
         * 
         */

        public int hidersTotalTimeSeconds;
        public int seekersTotalTimeSeconds;
        public int totalCameras;
        public int totalTimeSeconds;
        public string[] proceduralMusicRegionList = [
            "su","hi"
            ];
        public string proceduralMusicRegion="";
        public float lerp=1f;

        public HideAndSeekGameMode()
        {
            /*
             * 
             * Lobby data
             * 
             */

            hidersMinTimeSeconds = 45;
            hidersTimePerCameraSeconds = 15;
            hidersMaxTimeSeconds = 0;

            seekersMinTimeSeconds = 180;
            seekersTimePerCameraSeconds = 30;
            seekersMaxTimeSeconds = 0;

            disableCollisions = true;

            /*
             * 
             * Client data
             * 
             */

            isSeeker = false;

            /*
             * 
             * Internal
             * 
             */
            proceduralMusicRegion = proceduralMusicRegionList[0];
        }
        public static bool isHideAndSeekGameMode(ArenaOnlineGameMode arena, out HideAndSeekGameMode gameMode)
        {
            gameMode = null;
            if (arena.currentGameMode == MyGameModeName.value)
            {
                gameMode = (arena.registeredGameModes.FirstOrDefault(x => x.Key == MyGameModeName.value).Value as HideAndSeekGameMode);
                return true;
            }
            return false;
        }

        public override void ArenaSessionCtor(ArenaOnlineGameMode arena, On.ArenaGameSession.orig_ctor orig, ArenaGameSession self, RainWorldGame game)
        {
            base.ArenaSessionCtor(arena, orig, self, game);

            OnlineManager.lobby.clientSettings.TryGetValue(OnlineManager.mePlayer, out var clientSettings);
                if (clientSettings != null)
                {

                    clientSettings.TryGetData<HideAndSeekClientData>(out var hideAndSeekClientData);
                    if (hideAndSeekClientData != null)
                    {
                        hideAndSeekClientData.isSeeker = isSeeker;
                    }
                }
        }


        public override void OnUIEnabled(ArenaOnlineLobbyMenu menu)
        {
            base.OnUIEnabled(menu);
            if(RainMeadow.RainMeadow.isArenaMode(out var arena))
            {
                hideAndSeekTab = new(menu, menu.arenaMainLobbyPage.tabContainer);
                hideAndSeekTab.AddObjects(hideAndSeekInterface = new HideAndSeekInterface(arena,
                    this, hideAndSeekTab.menu, hideAndSeekTab,
                    new Vector2(0f, 0f),
                    menu.arenaMainLobbyPage.tabContainer.size
                ));
                menu.arenaMainLobbyPage.tabContainer.AddTab(hideAndSeekTab, "Hide and Seek");
            }
        }

        public override void OnUIDisabled(ArenaOnlineLobbyMenu menu)
        {
            base.OnUIDisabled(menu);
            hideAndSeekInterface?.OnShutdown();
            if (hideAndSeekTab != null) menu.arenaMainLobbyPage.tabContainer.RemoveTab(hideAndSeekTab);
            hideAndSeekTab = null;
        }

        public override void OnUIShutDown(ArenaOnlineLobbyMenu menu)
        {
            base.OnUIShutDown(menu);
            hideAndSeekInterface?.OnShutdown();
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

        // OPTIONAL

        public override bool HoldFireWhileTimerIsActive(ArenaOnlineGameMode arena)
        {
            return arena.countdownInitiatedHoldFire = false;
        }

        public override int SetTimer(ArenaOnlineGameMode arena)
        {
            ArenaGameSession arenaGameSession = (
                    RWCustom.Custom.rainWorld.processManager.currentMainLoop as RainWorldGame
                ).GetArenaGameSession;
            AbstractRoom absRoom = arenaGameSession.game.world.abstractRooms[0];
            Room room = absRoom.realizedRoom;
            totalCameras = arenaGameSession.room.cameraPositions.Count();
            hidersTotalTimeSeconds = (hidersMaxTimeSeconds >= hidersMinTimeSeconds) ? Mathf.Min(hidersMaxTimeSeconds, hidersMinTimeSeconds + (totalCameras * hidersTimePerCameraSeconds)) : hidersMinTimeSeconds + (totalCameras * hidersTimePerCameraSeconds);
            seekersTotalTimeSeconds = (seekersMaxTimeSeconds >= seekersMinTimeSeconds) ? Mathf.Min(seekersMaxTimeSeconds, seekersMinTimeSeconds + (totalCameras * seekersTimePerCameraSeconds)) : seekersMinTimeSeconds + (totalCameras * seekersTimePerCameraSeconds);
            totalTimeSeconds = hidersTotalTimeSeconds + seekersTotalTimeSeconds;
            arenaGameSession.game.world.rainCycle.cycleLength = (totalTimeSeconds+10)*40;
            var rand = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
            proceduralMusicRegion = proceduralMusicRegionList[rand.NextInt(0,proceduralMusicRegionList.Length)];//[Random.Range(0, proceduralMusicRegionList.Length)];
            return arena.setupTime =hidersTotalTimeSeconds;
        }

        public override Dialog AddGameModeInfo(ArenaOnlineGameMode arena, Menu.Menu menu)
        {
            return new DialogNotify(
                menu.LongTranslate("Hide and Seek"),
                new Vector2(500f, 400f),
                menu.manager,
                () =>
                {
                    menu.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                }
            );
        }

        public virtual string TimerText()
        {
            return "";
        }
    }
}

