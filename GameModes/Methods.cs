using System;
using System.Linq;
using System.Xml.Schema;
using HarmonyLib;
using IL.MoreSlugcats;
using RainMeadow;
using UnityEngine;

namespace Cyclesseum
{
	public partial class CyclesseumMethods
	{
        public static void PlayerGraphics_DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            HideAndSeekMethods.PlayerGraphics_DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
        }

        private static void Player_checkInput(RainMeadow.RainMeadow __instance, On.Player.orig_checkInput orig, Player self)

        {
            if (OnlineManager.lobby != null)
            {
                var onlineEntity = self.abstractCreature?.GetOnlineObject();
                if (onlineEntity is not null)
                {
                    if (onlineEntity.isMine)
                    { // If we own the player we don't need a controller
                        if (self.controller is OnlineController)
                        {
                            self.controller = null;
                        }

                    }
                    else
                    {
                        if (self.controller is null)
                        { // If we don't own the player we need a controller
                            self.controller = new OnlineController(onlineEntity, self);
                        }

                        // If we're being held by a local player. they should request ownership of us
                        if (self.isNPC)
                        {
                            if (self.onBack is not null)
                            {
                                if (self.onBack.IsLocal() && onlineEntity.isTransferable && !onlineEntity.isPending)
                                {
                                    try
                                    {
                                        onlineEntity.Request();
                                    }
                                    catch (Exception except)
                                    {
                                        RainMeadow.RainMeadow.Debug(except);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            orig(self);
            if (OnlineManager.lobby != null)
            {
                if (self.controller is null && self.room.world.game.cameras[0]?.hud is HUD.HUD hud
                    && (hud.textPrompt?.pausedMode is true || hud.parts.OfType<ChatHud>().Any(x => x.chatInputActive) || (hud.parts.OfType<SpectatorHud>().Any(x => x.isActive) && RainMeadow.RainMeadow.rainMeadowOptions.StopMovementWhileSpectateOverlayActive.Value)))
                {
                    RainMeadow.GameplayOverrides.StopPlayerMovement(self);
                }

                if (RainMeadow.RainMeadow.isArenaMode(out var arena))
                {
                    if (arena.countdownInitiatedHoldFire)
                    {
                        //GameplayOverrides.HoldFire(self);
                    }
                }

                if (ModManager.MSC)
                {
                    /*if (RainMeadow.RainMeadow.BottomPlayerUsingSpearmasterAbility(self, out _))
                    {
                        self.input[0].pckp = true;
                    }*/
                }

                if (!self.isNPC)
                {
                    Player? grabbingplayer = self.grabbedBy.FirstOrDefault(x => x.grabber is Player)?.grabber as Player;
                    if (grabbingplayer != null)
                    {
                        if (!self.input[0].AnyDirectionalInput && !self.input[0].jmp)
                        {
                            self.input[0].x = grabbingplayer.input[0].x;
                            self.input[0].y = grabbingplayer.input[0].y;
                            if (grabbingplayer.bodyMode == Player.BodyModeIndex.Crawl && self.standing)
                            {
                                self.input[0].y = -1;
                            }
                            if (grabbingplayer.bodyMode == Player.BodyModeIndex.Stand && !self.standing)
                            {
                                self.input[0].y = 1;
                            }

                            self.input[0].jmp = grabbingplayer.input[0].jmp;
                        }
                    }

                }
            }
        }

        public static bool checkFriendlyFire(bool __result, Creature creature, Creature? friend)
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena))
            {
                if (creature.abstractCreature.GetOnlineCreature() is not OnlineCreature oc)
                {
                    return false;
                }
                if (friend is not null)
                {
                    if (HideAndSeekGameMode.isHideAndSeekGameMode(arena, out _))
                    {
                        return HideAndSeekMethods.checkHideAndSeekFriendlyFire(
                            arena,
                            oc.owner,
                            friend.abstractCreature.GetOnlineCreature()?.owner,
                            creature,
                            friend,__result
                        );
                    }
                }
            }
            return __result;
        }

        public static void Player_ThrownSpear(Player player, Spear spear)
        {
            HideAndSeekMethods.Player_ThrownSpear(player, spear);
        }

        public static void Weapon_Update(Weapon weapon, bool eu)
        {
            HideAndSeekMethods.Weapon_Update(weapon, eu);
        }
    }
}