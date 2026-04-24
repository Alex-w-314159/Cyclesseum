using System;
using DevInterface;
using IL;
using IL.MoreSlugcats;
using Music;
using On;
using RainMeadow;
using UnityEngine;


namespace Cyclesseum
{
    public class HideAndSeekMethods
    {
        public static void PlayerGraphics_DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode))
            {
                try
                {
                    // dev nightsky skin

                    if (self.player.abstractPhysicalObject.GetOnlineObject(out var oe) == true && ArenaHelpers.GetArenaClientSettings(oe!.owner).slugcatColor != null)
                    {
                        var realPlayerVelPerSecond = (self.player.mainBodyChunk.pos - self.player.mainBodyChunk.lastPos) * 40f;
                        var hidingVelMagnitude = (realPlayerVelPerSecond.magnitude > 5f) ? realPlayerVelPerSecond.magnitude - 5f : 0f;
                        var maxVelMagnitude = 10f;
                        var timeSinceSpawnedSeconds = 0.025f * ((float)self.player.timeSinceSpawned);
                        Color playerQuietColor;
                        Color playerMovingColor;
                        var hiderMovingH = Mathf.Repeat((1f / 8f) * timeSinceSpawnedSeconds, 1f);//Rainbow
                        var hiderMovingV = Mathf.Clamp(Mathf.PingPong(timeSinceSpawnedSeconds, 4.5f) / 4.5f, 0.01f, 0.99f);
                        var playerHiderMovingColor = Color.HSVToRGB(hiderMovingH, 1f, hiderMovingV);
                        var playerSeekerMovingColor = Color.HSVToRGB(0f, 1f, 0.25f + 0.5f * hiderMovingV);
                        if (OnlineManager.lobby.clientSettings[oe!.owner].TryGetData<HideAndSeekClientData>(out var hideAndSeekClientData))
                        {

                            if (oe.owner.isMe) hideAndSeekClientData.lerp = (self.player.lastCoord == self.player.coord) ? Mathf.Lerp(hideAndSeekClientData.lerp, Mathf.Clamp01(hidingVelMagnitude), 0.05f) : Mathf.Lerp(hideAndSeekClientData.lerp, Mathf.Clamp01(hidingVelMagnitude), 0.001f);
                            if (!hideAndSeekClientData.isSeeker)
                            {
                                Color.RGBToHSV(ArenaHelpers.GetArenaClientSettings(oe!.owner).slugcatColor.SafeColorRange(), out var h, out var s, out var v);
                                playerQuietColor = Color.HSVToRGB(h, s, v);
                                playerMovingColor = playerHiderMovingColor;
                            }
                            else
                            {
                                Color.RGBToHSV(playerQuietColor = ArenaHelpers.GetArenaClientSettings(oe!.owner).slugcatColor.SafeColorRange(), out var seekerH, out _, out _);
                                playerQuietColor = Color.HSVToRGB(seekerH, 1f, 1f);
                                playerMovingColor = Color.HSVToRGB(seekerH, 1f, 1f);
                            }
                            Color color = Color.Lerp(playerQuietColor, playerMovingColor, hideAndSeekClientData.lerp).SafeColorRange();
                            if (arena.trackSetupTime >= arena.setupTime || oe.owner.isMe)
                            {
                                color.a = 1;
                            }
                            else
                            {
                                color.a = 0;
                            }
                            for (int i = 0; i < sLeaser.sprites.Length; i++) // 9 is face, 10 is Mark light
                            {
                                sLeaser.sprites[i]._color = color;
                            }
                            if (self.gills != null)
                            {
                                self.gills.effectColor = color;
                                self.gills.baseColor = color;
                            }
                        }

                    }
                }
                finally
                {
                }
                return;
            }
        }

        public static void Player_ThrownSpear(Player player, Spear spear)
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode))
            {
                var playerOnlineEntity = player.abstractPhysicalObject.GetOnlineObject();
                OnlineManager
                       .lobby.clientSettings[playerOnlineEntity.owner]
                       .TryGetData<HideAndSeekClientData>(out var hideAndSeekClientData);
                if (!hideAndSeekClientData.isSeeker)
                {
                    if (!spear.abstractPhysicalObject.GetOnlineObject().TryGetData<SpearColorData>(out var spearColorData))
                    {
                        var spearColorDataInstance = new SpearColorData();
                        spear.abstractPhysicalObject.GetOnlineObject().AddData<SpearColorData>(spearColorDataInstance);
                    }
                    else
                    {

                        spearColorData.color = arena.avatarSettings.bodyColor;
                    }
                    spear.throwModeFrames = 18;
                    spear.spearDamageBonus = 0.1f + 0.1f * Mathf.Pow(UnityEngine.Random.value, 4f);
                    spear.firstChunk.vel.x *= 0.77f;
                    spear.color = spearColorData.color;

                }
            }
        }

        public static void Player_UpdateMSC(Player self)
        {
            // if (RainMeadow.RainMeadow.isArenaMode(out var arena))// && Cyclesseum.HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var gamemode))
            //{ }
            if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode) && self.timeSinceSpawned > 0)
            {
                /*
                 * 
                 * Basic controls
                 * 
                 */
                if (!self.abstractPhysicalObject.GetOnlineObject(out var playerOnlineEntity))
                    return;
                if (OnlineManager
                       .lobby.clientSettings[playerOnlineEntity!.owner]
                       .TryGetData<HideAndSeekClientData>(out var hideAndSeekClientData))
                {
                    if (!hideAndSeekClientData.isSeeker)
                    {
                        self.Template.lungCapacity = 10f;
                    }
                    else
                    {
                        self.Template.lungCapacity = 100f;
                    }
                    if (self.input[0].mp)
                    {
                        var inputX = (float)self.input[0].x;
                        var inputY = (float)self.input[0].y;

                        var inputH = (self.input[0].pckp ? 1f : 0f);
                        var inputS = (self.input[0].jmp ? 1f : 0f);
                        var inputV = (self.input[0].thrw ? 1f : 0f);
                        Color color;
                        Color newColor;
                        color = arena.avatarSettings.bodyColor;
                        Color.RGBToHSV(color, out var h, out var s, out var v);

                        var newH = Mathf.Repeat(h + (inputH * (inputX * 0.01f + inputY * 0.001f)), 1f);
                        var newS = Mathf.Clamp(s + (inputS * (inputX * 0.01f + inputY * 0.001f)), 0.01f, 0.99f);
                        var newV = Mathf.Clamp(v + (inputV * (inputX * 0.01f + inputY * 0.001f)), 0.01f, 0.99f);

                        newColor = Color.HSVToRGB(newH, newS, newV);
                        if (playerOnlineEntity.apo.GetOnlineObject().owner.isMe) ArenaHelpers.GetArenaClientSettings(playerOnlineEntity.apo.GetOnlineObject().owner).slugcatColor = newColor;
                        arena.avatarSettings.bodyColor = newColor;
                        arena.arenaClientSettings.slugcatColor = newColor;
                        arena.avatarSettings.eyeColor = newColor;
                        self.input[0].x = 0;
                        self.input[0].y = 0;
                        self.input[0].jmp = false;
                        self.input[0].thrw = false;
                    }
                    else
                    {
                        if (playerOnlineEntity.apo.GetOnlineObject().owner.isMe) arena.avatarSettings.eyeColor = Color.HSVToRGB(Mathf.PingPong(((float)self.timeSinceSpawned) / 40.0f, 1f), 1f, 1f);
                    }

                    /*
                     * 
                     * Objects
                     * 
                     */

                    for (int i = 0; i < self.grasps?.Length; i++)
                    {
                        var obj = self.grasps[i]?.grabbed as Weapon;
                        if (obj is Spear)
                        {
                            ((Spear)obj).color = arena.arenaClientSettings.slugcatColor;
                        }
                        if (obj is Rock)
                        {
                            ((Rock)obj).color = arena.arenaClientSettings.slugcatColor;
                        }
                    }

                    /*
                     * 
                     * Music
                     * 
                     * 
                     */
                    var players = arena.session.Players;
                    var distance = 100f;
                    foreach (AbstractCreature player in players)
                    {
                        var playerOnlineObject = player.GetOnlineObject();
                        if (playerOnlineObject.owner != self.abstractPhysicalObject.GetOnlineObject().owner)
                        {
                            distance = Mathf.Min(Vector2.Distance(new Vector2(playerOnlineObject.apo.pos.x, playerOnlineObject.apo.pos.y), new Vector2(self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y)), distance);
                        }
                    }

                    if (self.room.game.manager.musicPlayer != null)
                    {
                        if (self.room.game.manager.musicPlayer.song != null) self.room.game.manager.musicPlayer.song.StopAndDestroy();
                        self.room.game.manager.musicPlayer.song = null;
                        self.room.game.manager.musicPlayer.nextSong = null;
                        /*
                         * 
                         * Initialize threatTracker
                         * 
                         */
                        if (self.room.game.manager.musicPlayer.threatTracker == null)
                        {
                            self.room.game.manager.musicPlayer.threatTracker = new PlayerThreatTracker(self.room.game.manager.musicPlayer, 10);
                        }
                        /*
                         * 
                         * 
                         * 
                         */
                        self.room.game.manager.musicPlayer.threatTracker.currentThreat = 1f * (hideAndSeekClientData.isSeeker ? 0f : 1f);//amenaza
                        self.room.game.manager.musicPlayer.threatTracker.region = hideAndSeekGameMode.proceduralMusicRegion;
                        if (self.room.game.manager.musicPlayer.proceduralMusic == null)
                        {
                            self.room.game.manager.musicPlayer.proceduralMusic = new ProceduralMusic(self.room.game.manager.musicPlayer, hideAndSeekGameMode.proceduralMusicRegion);
                            self.room.game.manager.musicPlayer.proceduralMusic.volume = 0.5f;
                            self.room.game.manager.musicPlayer.proceduralMusic.StartPlaying();
                        }
                        self.room.game.manager.musicPlayer.proceduralMusic.volume = 0.5f;
                        if (self.room.game.manager.musicPlayer.threatTracker.region != hideAndSeekGameMode.proceduralMusicRegion)
                        {
                            self.room.game.manager.musicPlayer.proceduralMusic.StopAndDestroy();
                            self.room.game.manager.musicPlayer.proceduralMusic = null;
                        }
                        //self.room.game.manager.musicPlayer.droneGoalMix = 1f;
                        //self.room.game.manager.musicPlayer.proceduralMusic.silentCounter = 0;
                        //self.room.game.manager.musicPlayer.proceduralMusic.audibleCounter = 0;
                        //self.room.game.manager.musicPlayer.proceduralMusic.reScrambleOnNextSilence = false;

                    }
                    else
                    {
                        if (self.timeSinceSpawned % 120 == 0) ChatLogManager.LogSystemMessage("null");
                    }
                }
            }
        }

        public static bool checkHideAndSeekFriendlyFire(
            ArenaOnlineGameMode arena,
            OnlinePlayer? A,
            OnlinePlayer? B,
            Creature creature,
            Creature friend,
            bool result
        )
        {
            if (creature.abstractCreature.GetOnlineCreature() is not OnlineCreature oc)
            {
                return false;
            }

            if (!oc.isAvatar)
            {
                return false;
            }
            if (A is not null && B is not null)
            {
                if (
                    OnlineManager
                        .lobby.clientSettings[A]
                        .TryGetData<HideAndSeekClientData>(out var hideAndSeekClientDataA)
                )
                {
                    if (
                        OnlineManager
                            .lobby.clientSettings[B]
                            .TryGetData<HideAndSeekClientData>(out var hideAndSeekClientDataB)
                    )
                    {
                        bool isFriendlyFire = (((hideAndSeekClientDataA.isSeeker == hideAndSeekClientDataB.isSeeker) || (!hideAndSeekClientDataA.isSeeker && !hideAndSeekClientDataB.isSeeker))); //&& !arena.friendlyFire);
                        if (isFriendlyFire)
                        {
                            RainMeadow.RainMeadow.Debug("Same team! No hits");
                        }
                        return isFriendlyFire && creature.State.alive && friend.State.alive;
                    }
                }
            }
            return result;
        } 

        public static void Weapon_Update(Weapon weapon, bool eu)
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode))
            {
                if (!weapon.abstractPhysicalObject.GetOnlineObject().TryGetData<SpearColorData>(out var spearColorData))
                {
                    var spearColorDataInstance = new SpearColorData();
                    weapon.abstractPhysicalObject.GetOnlineObject().AddData<SpearColorData>(spearColorDataInstance);
                }
                else
                {
                    try
                    {
                        if (weapon.grabbedBy[0].grabber is Player)
                        {
                            OnlineManager.lobby.clientSettings[weapon.grabbedBy[0].grabber.abstractPhysicalObject.GetOnlineObject().owner]
                                   .TryGetData<ArenaClientSettings>(out var arenaClientData);
                            var playerColor = arenaClientData.slugcatColor;
                            weapon.color = playerColor;
                            try
                            {

                                if (weapon is MoreSlugcats.ElectricSpear)
                                {
                                    (weapon as MoreSlugcats.ElectricSpear).electricColor = playerColor;
                                    (weapon as MoreSlugcats.ElectricSpear).blackColor = playerColor;
                                }
                            }
                            catch (Exception e) { }

                            try
                            {

                                if (weapon is ExplosiveSpear)
                                {
                                    (weapon as ExplosiveSpear).redColor = playerColor;
                                    (weapon as ExplosiveSpear).explodeColor = playerColor;
                                }
                            }
                            catch (Exception e) { }
                            try
                            {

                                if (weapon is MoreSlugcats.LillyPuck)
                                {
                                    (weapon as MoreSlugcats.LillyPuck).flowerColor = playerColor;
                                }
                            }
                            catch (Exception e) { }

                            try
                            {

                                if (weapon is FlareBomb)
                                {
                                    (weapon as PlayerCarryableItem).color = playerColor; ;
                                }
                            }
                            catch (Exception e) { }
                        }
                    }
                    catch (Exception e) { }
                    ;
                }
            }
        }

        /*void ColorToCMYK(Color rgb, out float c, out float m, out float y, out float k)
        {
            float r = rgb.r;
            float g = rgb.g;
            float b = rgb.b;

            k = Mathf.Clamp(1f - Mathf.Max(r, g, b), 0.01f, 0.99f);

            if (k >= 1f)
            {
                c = m = y = 0f;
                return;
            }

            c = (1f - r - k) / (1f - k);
            m = (1f - g - k) / (1f - k);
            y = (1f - b - k) / (1f - k);
        }

        Color CMYKToColor(float c, float m, float y, float k)
        {
            return new Color(
                (1f - c) * (1f - k),
                (1f - m) * (1f - k),
                (1f - y) * (1f - k)
            );
        }

        //ColorUtility para convertirlo a HTML
        //newColor = Color.Lerp(color, newColor, 0.5f);
        ColorToCMYK(arena.avatarSettings.bodyColor, out var c, out var m, out var y, out var k);

            if (self.input[0].pckp)
            {
                c = Mathf.Clamp01(c + (input * 0.005f));
            }
            else if (self.input[0].jmp)
            {
                m = Mathf.Clamp01(m + (input * 0.005f));
            }
            else if(self.input[0].thrw)
            {
                y = Mathf.Clamp01(y + (input * 0.005f));
            }
        else
        {
            k = Mathf.Clamp01(k + (input * 0.005f));
        }
        var newColor = CMYKToColor(c, m, y, k);*/

    }
}
