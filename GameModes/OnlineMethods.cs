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
using System;
using System.Reflection;
namespace Cyclesseum
{
    public partial class CyclesseumOnlineMethods {

        public delegate bool GameModeGetter<T>(ArenaOnlineGameMode arena, out T gameMode) where T : ExternalArenaGameMode;
        public static bool IsMyArenaGameMode<T>(
            GameModeGetter<T> getter,
            out ArenaOnlineGameMode arena,
            out T gameMode)
            where T : ExternalArenaGameMode
        {
            gameMode = default;

            if (!RainMeadow.RainMeadow.isArenaMode(out arena))
                return false;

            return getter(arena, out gameMode);
        }
        public static OnlinePlayer GetPhysicalObjectOwner<T>(T physicalObject) where T: PhysicalObject
        {
            return physicalObject.abstractPhysicalObject.GetOnlineObject().owner;
        }

        public static bool PhysicalObjecOwnerIsMe<T>(T physicalObject) where T: PhysicalObject
        {
            return physicalObject.abstractPhysicalObject.GetOnlineObject().owner.isMe;
        }

        public static bool TryGetPhysicalObjectData<T, U>(T physicalObject, out U physicalObjectData) where T : PhysicalObject where U : OnlineEntity.EntityData
        {
            return physicalObject.abstractPhysicalObject.GetOnlineObject().TryGetData<U>(out physicalObjectData);
        }
        public static bool TryGetPhysicalObjectOwnerData<T,U>(T physicalObject, out U ownerData) where T: PhysicalObject where U : OnlineEntity.EntityData
        {
            return OnlineManager.lobby.clientSettings[GetPhysicalObjectOwner<T>(physicalObject)].TryGetData<U>(out ownerData);
        }
    }
}