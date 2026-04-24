using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Xml;
using HarmonyLib;
using IL.MoreSlugcats;
using IL.Music;
using MoreSlugcats;
using RainMeadow;
using RainMeadow.Arena.ArenaOnlineGameModes.TeamBattle;
using RainMeadow.UI.Components;
using Rewired.ControllerExtensions;
using UnityEngine;
using Watcher;
namespace Cyclesseum
{
    [HarmonyPatch(typeof(PlayerGraphics), "DrawSprites")]
    public static class Harmony_PlayerGraphics_DrawSprites
    {
        static void Prefix(PlayerGraphics __instance, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
        {
            CyclesseumMethods.PlayerGraphics_DrawSprites(__instance, sLeaser, rCam, timeStacker, camPos);
        }
    }

    /*[HarmonyPatch(typeof(Player), "ThrownSpear")]
    public static class Harmony_Player_ThrownSpear
    {
        static bool Prefix(Player __instance, Spear spear)
        {
            CyclesseumMethods.Player_ThrownSpear(__instance, spear);
            return true;
        }
    }*/

    [HarmonyPatch(typeof(Weapon), "Update")]
    public static class Harmony_Weapon_Update
    {
        static void Postfix(Weapon __instance,bool eu)
        {
            CyclesseumMethods.Weapon_Update(__instance, eu);
        }
    }

    [HarmonyPatch(typeof(RainMeadow.RainMeadow), "Player_UpdateMSC")]
    public static class Harmony_Player_UpdateMSC
    {
        static void Postfix(On.Player.orig_UpdateMSC orig, Player self)
        {
            HideAndSeekMethods.Player_UpdateMSC(self);
        }
    }

    [HarmonyPatch(typeof(ArenaOnlineGameMode), "AddClientData")]
    public static class Harmony_ArenaOnlineGameMode_AddClientData
    {
        static void Postfix(ArenaOnlineGameMode __instance)
        {
            __instance.clientSettings.AddData(new HideAndSeekClientData());
        }
    }

    [HarmonyPatch(typeof(ArenaOnlineGameMode), "ResourceAvailable")]
    public static class Harmony_ArenaOnlineGameMode_ResourceAvailable
    {
        static void Postfix(OnlineResource onlineResource)
        {
            if (onlineResource is Lobby lobby)
            {
                lobby.AddData(new HideAndSeekLobbyData());
            }
        }
    }

    [HarmonyPatch(typeof(GameplayExtensions), "FriendlyFireSafetyCandidate")]
    public static class Harmony_GameplayExtensions_FriendlyFireSafetyCandidate
    {

        static bool Postfix(bool __result, Creature creature, Creature? friend)
        {
            return CyclesseumMethods.checkFriendlyFire(__result, creature, friend);
        }
    }

    /*[HarmonyPatch(typeof(Music.PlayerThreatTracker), "Update")]
    public static class Harmony_A
    {

        static bool Prefix(Music.PlayerThreatTracker __instance)
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode))
            {
                __instance.currentThreat = 1f;
                __instance.currentMusicAgnosticThreat = 1f;
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(MeadowMusic), "PlayerThreatTracker_Update")]
    public static class Harmony_B
    {

        static bool Prefix(On.Music.PlayerThreatTracker.orig_Update orig, Music.PlayerThreatTracker self)
        {
            return false;
        }
    }*/

}