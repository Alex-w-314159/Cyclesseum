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
namespace Cyclesseum
{
    public partial class CyclesseumOnlineMethods {
        public static OnlinePlayer GetOwner<T>(T objeto) where T: Creature, Weapon
        {
            return objeto.abstractPhysicalObject.GetOnlineObject().owner
        }

        public static bool OwnerIsMe<T>(T objeto) where T : Creature, Weapon
        {
            return objeto.abstractPhysicalObject.GetOnlineObject().owner.isMe;
        }
    }
}