// MyCoolNewModPlugin.cs
using System;
using System.Security.Permissions;
using BepInEx;
using HarmonyLib;
using IL;
using MonoMod.RuntimeDetour;
using RainMeadow;
using RainMeadow.UI;
using UnityEngine;

//#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace Cyclesseum
{
    [BepInPlugin("alex3.14159.Cyclesseum", "Cyclesseum", "2026.04.23.1")]
    public partial class CyclesseumMod : BaseUnityPlugin
    {
        public static CyclesseumMod instance;
        private bool init;
        private bool fullyInit;
        private bool addedMod = false;
        public HideAndSeekGameMode hideAndSeekGameMode;
        public AscendedBalanceGameMode ascendedBalanceGameMode;

        public void OnEnable()
        {
            instance = this;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            var harmony = new Harmony("alex3.14159.Cyclesseum");
            harmony.PatchAll();
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (init) return;
            init = true;

            try
            {
                On.Menu.Menu.ctor += Menu_ctor;
                fullyInit = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                fullyInit = false;
            }
        }

        private void Menu_ctor(On.Menu.Menu.orig_ctor orig, Menu.Menu self, ProcessManager manager, ProcessManager.ProcessID ID)
        {
            orig(self, manager, ID);
            if (self is ArenaOnlineLobbyMenu)
            {
                AddNewModes();
            }
        }

        private void AddNewModes()
        {

            if (RainMeadow.RainMeadow.isArenaMode(out var arena))
            {
                hideAndSeekGameMode = new HideAndSeekGameMode();
                ascendedBalanceGameMode=new AscendedBalanceGameMode();
                //arena.AddExternalGameModes(AscendedBalanceGameMode.MyGameModeName, ascendedBalanceGameMode);
                arena.AddExternalGameModes(HideAndSeekGameMode.MyGameModeName, hideAndSeekGameMode);
            }

        }

    }
}