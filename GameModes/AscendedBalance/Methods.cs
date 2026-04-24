using System.Linq;
using System.Xml.Schema;
using IL.MoreSlugcats;
using RainMeadow;
using UnityEngine;

namespace Cyclesseum
{
    public class AscendedBalanceMethods
    {
        public static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena))
            {
                if (!arena.countdownInitiatedHoldFire)
                {
                    if (self.timeSinceSpawned % 60 <= 10)
                    {
                       // Color color = Color.HSVToRGB(Mathf.Abs(Mathf.Cos((float)(self.timeSinceSpawned / 60f))), 1f, 0.5f);
                        //arena.avatarSettings.bodyColor = color;
                        //arena.arenaClientSettings.slugcatColor = color;
                             }
                    if (Cyclesseum.AscendedBalanceGameMode.isAscendedBalanceGameMode(arena, out var ab))
                    {
                        if (self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint)
                        {

                            if (!arena.sainot)
                            {
                                self.slugcatStats.throwingSkill = 0;
                                if (!arena.countdownInitiatedHoldFire)
                                {

                                    /*
                                    * Initialize self attributes
                                    */
                                    self.maxGodTime = arena.arenaSaintAscendanceTimer * 40f + 1f;
                                    self.killWait = self.maxGodTime / 2f;
                                    /*
                                     * Local vars
                                     */
                                    float timeStep = self.maxGodTime / 20.0f; //The amount of time used as a unit to increase or decrease self.godTimer
                                    float ascensionCooldown = self.maxGodTime * 0.4f;
                                    float maxSaintTongueTolerance = Mathf.Min(240f * 240f / self.maxGodTime, 120f) * 1.5f;
                                    bool saintAscensionCooldownCheck = self.godDeactiveTimer > ascensionCooldown;

                                    bool saintMovementBalanceNormal = (self.input[0].x != 0 || (self.input[0].y != 0)) && self.dynamicRunSpeed[0] > 0f && self.tongue.mode == Player.Tongue.Mode.Retracted && self.goIntoCorridorClimb == 0 && ((self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.BeamTip && self.animation != Player.AnimationIndex.HangUnderVerticalBeam) || self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.RocketJump || self.animation == Player.AnimationIndex.Roll || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.StandUp || self.animation == Player.AnimationIndex.None || (self.animation == Player.AnimationIndex.ClimbOnBeam && self.lastCoord.x != self.coord.x));
                                    bool saintMovementBalanceSwim = self.animation == Player.AnimationIndex.SurfaceSwim && self.goIntoCorridorClimb == 0 && Mathf.Abs(self.gravity) > 0.1f;
                                    bool saintMovementBalanceLowG = Mathf.Abs(self.gravity) <= 0.1f && self.animation != Player.AnimationIndex.ZeroGPoleGrab && self.goIntoCorridorClimb == 0 && self.animation == Player.AnimationIndex.ZeroGSwim && self.wantToJump != 0;

                                    bool saintMovementBalanceMassiveTunnelZone = self.goIntoCorridorClimb > 3 * self.maxGodTime && (self.animation == Player.AnimationIndex.CorridorTurn || self.corridorDrop || self.horizontalCorridorSlideCounter != 0);

                                    bool saintMovementBalance = self.Consious && !self.Stunned & (saintMovementBalanceNormal || saintMovementBalanceLowG || saintMovementBalanceSwim || saintMovementBalanceMassiveTunnelZone);

                                    bool isDirectionPersistant = (self.wiggleDirectionCounters.x > 64 && self.wiggleDirectionCounters.y > 64);
                                    bool isDirectionChanging = ((self.wiggleDirectionCounters.x != 0 || self.wiggleDirectionCounters.y != 0) || self.animation == Player.AnimationIndex.BellySlide || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump) && !saintMovementBalanceLowG && !saintMovementBalanceMassiveTunnelZone;

                                    bool saintCanUseAscension =
                                        (!self.monkAscension && self.wantToJump > 0 && self.input[0].pckp && self.canJump <= 0 && self.Consious && !self.Stunned)
                                        &&
                                        (
                                        (!self.tongue.Attached && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab && self.animation != Player.AnimationIndex.CorridorTurn && self.goIntoCorridorClimb == 0)
                                        ||
                                        saintMovementBalanceMassiveTunnelZone || (self.godTimer > 0f & self.godDeactiveTimer > 20));
                                    /*
                                     * Saint meets conditions to start ascension
                                     */
                                    if (saintCanUseAscension)
                                    {
                                        self.godWarmup = 20f;
                                        self.ActivateAscension();
                                        self.forceBurst = true;
                                    }

                                    /*
                                     * Saint is in ascension  mode
                                     */
                                    if (self.monkAscension)
                                    {
                                        self.tongue.Release();
                                        self.mainBodyChunk.vel = new Vector2(0, 0);
                                        self.mainBodyChunk.pos = self.mainBodyChunk.pos;
                                        if (self.input[0].pckp != false)
                                        {
                                            if (self.input[0].x != 0)
                                            {
                                                self.burstVelX = Mathf.Clamp(self.burstVelX + (float)self.input[0].x * 0.01f, -50f, 50f);
                                                self.burstVelX += self.burstVelX;

                                            }
                                            if (self.input[0].y != 0)
                                            {
                                                self.burstVelY = Mathf.Clamp(self.burstVelY + (float)self.input[0].y * 0.01f, -50f, 50f);
                                                self.burstVelY += self.burstVelY;
                                            }
                                        }
                                        if (self.killPressed)
                                        {
                                            self.DeactivateAscension();
                                            self.godTimer = 0.0f;
                                            self.godDeactiveTimer = 0f;
                                        }
                                        if (self.wantToJump > 0)
                                        {
                                            self.burstX = 0f;
                                            self.burstY = 0f;
                                            self.DeactivateAscension();
                                            if (self.godTimer > timeStep)
                                            {
                                                self.godDeactiveTimer = ascensionCooldown * 0.5f;
                                                self.godTimer = Mathf.Max(0f, self.godTimer - 4f * timeStep);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (self!.tongue.Attached)
                                        {
                                            if (self.input[0].jmp && !self.input[1].jmp && self!.tongueAttachTime >= 2)
                                            {
                                                float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                                                if (self.grasps[0] != null && self.HeavyCarry(self.grasps[0].grabbed) && !(self.grasps[0].grabbed is Cicada))
                                                {
                                                    num += Mathf.Min(Mathf.Max(0f, self.grasps[0].grabbed.TotalMass - 0.2f) * 1.5f, 1.3f);
                                                }

                                                self.bodyChunks[0].vel.y = 8f * num;
                                                self.bodyChunks[1].vel.y = 7f * num;
                                                self.jumpBoost = 8f;
                                                self!.tongue.Release();
                                            }


                                            if (self.input[0].y > 0)
                                            {
                                                self!.tongue.decreaseRopeLength(3f);
                                            }

                                            if (self!.input[0].x == 0)
                                            {
                                                self!.input[0].x = self.input[0].x;
                                            }

                                            if (self.input[0].y < 0)
                                            {
                                                self!.tongue.increaseRopeLength(3f);
                                            }
                                        }
                                        /*if (!self.tongue.Attached && !self.input[0].jmp && self.input[1].jmp && !self.input[0].pckp && self.canJump <= 0)
                                        {
                                            ChatLogManager.LogMessage("", "Shoot");
                                            self.tongue.Shoot(new Vector2(self.mainBodyChunk.vel.x, Mathf.Abs(self.mainBodyChunk.vel.y)+0.001f).normalized);
                                            self.TongueUpdate();
                                        }*/
                                        self.burstX = 0f;
                                        self.burstY = 0f;
                                    }

                                    /*
                                     * godTimer Increase or Decrease
                                     */
                                    if (!self.monkAscension && saintAscensionCooldownCheck)
                                    {
                                        float balanceFactorIncrease = 1f;
                                        float balanceFactorDecrease = 1f;
                                        if (saintMovementBalance)
                                        {
                                            if (saintMovementBalanceNormal)
                                            {
                                                balanceFactorIncrease = 1.5f;
                                                if (self.animation == Player.AnimationIndex.RocketJump)
                                                {
                                                    balanceFactorIncrease = 8f;
                                                }
                                                balanceFactorDecrease = 0.6f;
                                            }
                                            if (saintMovementBalanceSwim)
                                            {
                                                balanceFactorIncrease = 0.003f;
                                                balanceFactorDecrease = 1f;
                                            }
                                            if (saintMovementBalanceLowG)
                                            {
                                                balanceFactorIncrease = 3f;
                                                balanceFactorDecrease = 1.5f;
                                            }
                                            if (saintMovementBalanceMassiveTunnelZone)
                                            {
                                                if (self.animation == Player.AnimationIndex.CorridorTurn)
                                                {

                                                    balanceFactorIncrease = 8f;
                                                }
                                                if (self.horizontalCorridorSlideCounter != 0)
                                                {
                                                    balanceFactorIncrease = 6f;
                                                }
                                                if (self.corridorDrop)
                                                {
                                                    balanceFactorIncrease = 8f;
                                                }
                                                balanceFactorDecrease = 0.0015f;
                                            }

                                            if (isDirectionPersistant)
                                            {
                                                //self.godTimer = Mathf.Min(Mathf.Max(1f, self.maxGodTime - 1f), self.godTimer + .15f * balanceFactorIncrease * timeStep * Mathf.Pow(self.godTimer / self.maxGodTime, 0.3f));
                                                self.godTimer = Mathf.Min(Mathf.Max(1f, self.maxGodTime - 1f), self.godTimer + .15f * balanceFactorIncrease * timeStep * self.mainBodyChunk.vel.magnitude);
                                            }
                                            else
                                            {

                                                self.godTimer = Mathf.Min(Mathf.Max(1f, self.maxGodTime - 1f), self.godTimer + .10f * balanceFactorIncrease * timeStep * self.mainBodyChunk.vel.magnitude);
                                            }
                                            if (isDirectionChanging)
                                            {

                                                self.godTimer = Mathf.Min(Mathf.Max(1f, self.maxGodTime - 1f), self.godTimer - .15f * balanceFactorIncrease * timeStep * (1f / (self.mainBodyChunk.vel.magnitude + 1f)));
                                            }

                                        }
                                        else
                                        {
                                            self.godTimer = Mathf.Max(0.0f, self.godTimer - 9f * balanceFactorDecrease * timeStep * Mathf.Pow(Mathf.Max(0.1f, 1f - self.godTimer) / self.maxGodTime, 0.5f));
                                        }
                                    }

                                    /*
                                     * Tongue balance
                                     */
                                    if (self.tongueAttachTime > 0 && !saintMovementBalanceLowG)
                                    {
                                        if (UnityEngine.Mathf.FloorToInt(self.godWarmup) % 40 == 0 && self.tongueAttachTime > 10)
                                        {
                                            self.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, self.firstChunk);
                                            for (int i = 0; i < 15 * self.godWarmup / maxSaintTongueTolerance; i++)
                                            {
                                                UnityEngine.Vector2 vector = RWCustom.Custom.DegToVec(360f * UnityEngine.Random.value);
                                                self.room.AddObject(new MouseSpark(self.firstChunk.pos + vector * 9f, self.firstChunk.vel + vector * 36f * UnityEngine.Random.value, 20f, Color.magenta));
                                            }
                                            self.room.AddObject(new Explosion.ExplosionLight(self.firstChunk.pos, 50f + 70f * self.godWarmup / maxSaintTongueTolerance, self.godWarmup / maxSaintTongueTolerance, 3, Color.magenta));
                                        }
                                        if (self.tongueAttachTime == 1)
                                        {
                                            self.godWarmup = Mathf.Max(0f, self.godWarmup + 20f);
                                        }
                                        self.godWarmup = Mathf.Min(maxSaintTongueTolerance, self.godWarmup + 1f);
                                    }
                                    else
                                    {
                                        if (saintMovementBalance)
                                        {
                                            self.godWarmup = Mathf.Max(0f, self.godWarmup - 1f);
                                        }
                                        else
                                        {
                                            self.godWarmup = Mathf.Max(0f, self.godWarmup - 0.1f);
                                        }
                                    }
                                    if (Mathf.Abs(self.godWarmup - maxSaintTongueTolerance) < 1f && (self.tongue.attachedTime > 0))
                                    {
                                        //ChatLogManager.LogMessage("", "YOU ARE EXHAUSTED!!!");
                                        self.godWarmup = 0f;
                                        int exhaustedTime = UnityEngine.Random.Range(120, 240);
                                        self.Stun(exhaustedTime);
                                        self.SaintStagger(exhaustedTime);
                                        self.bodyChunks[0].vel.x = UnityEngine.Random.Range(10f, 25f) * (UnityEngine.Mathf.FloorToInt(UnityEngine.Random.Range(0f, 2f)) * 2 - 1);
                                        self.bodyChunks[1].vel.x = 0.75f * self.bodyChunks[0].vel.y;
                                        self.bodyChunks[0].vel.y = UnityEngine.Random.Range(15f, 40f) * (UnityEngine.Mathf.FloorToInt(UnityEngine.Random.Range(0f, 2f)) * 2 - 1);
                                        self.bodyChunks[1].vel.y = 0.75f * self.bodyChunks[0].vel.y;

                                        self.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, self.firstChunk);
                                        self.room.PlaySound(SoundID.Fire_Spear_Explode, self.firstChunk);
                                        self.room.AddObject(new Explosion(self.room, self, self.firstChunk.pos, 7, 250f, 30f, 0f, 0f, 0.35f, self, 0f, 160f, 0.5f));
                                        self.room.AddObject(new Explosion.ExplosionLight(self.firstChunk.pos, 300f, 1f, 4, Color.magenta));
                                        for (int i = 0; i < 30; i++)
                                        {
                                            UnityEngine.Vector2 vector = RWCustom.Custom.DegToVec(360f * UnityEngine.Random.value);
                                            self.room.AddObject(new MouseSpark(self.firstChunk.pos + vector * 9f, self.firstChunk.vel + vector * 36f * UnityEngine.Random.value, 20f, Color.magenta));
                                        }
                                    }

                                    if (!saintAscensionCooldownCheck && self.timeSinceSpawned % 8 <= 4)
                                    {
                                        self.hideGodPips = true;
                                    }
                                    else
                                    {
                                        self.hideGodPips = false;
                                    }

                                }

                            }
                            else
                            {
                                self.slugcatStats.throwingSkill = 1;
                            }
                        }

                        if (OnlineManager.lobby != null && RainMeadow.RainMeadow.HasSlugcatClassOnBack(self, MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint, out Player saint_player))
                        {
                            if (self.tongue is not null && self.tongue.Attached)
                            {
                                self.tongue.Release();
                            }


                            if (self.IsLocal() && self.onBack == null)
                            {
                                if (!saint_player!.tongue.Attached)
                                {
                                    if (!MoreSlugcats.MMF.cfgOldTongue.Value && self.input[0].jmp && !self.input[1].jmp && !self.input[0].pckp && self.canJump <= 0 && self.bodyMode != Player.BodyModeIndex.Crawl && self.animation != Player.AnimationIndex.ClimbOnBeam && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.HangFromBeam
                                            && saint_player!.SaintTongueCheck() &&
                                            self.bodyMode != Player.BodyModeIndex.CorridorClimb && !self.corridorDrop &&
                                            self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.bodyMode != Player.BodyModeIndex.WallClimb &&
                                            self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.VineGrab &&
                                            self.animation != Player.AnimationIndex.ZeroGPoleGrab)
                                    {
                                        Vector2 vector = new Vector2((float)self.flipDirection, 0.7f);
                                        Vector2 normalized = vector.normalized;
                                        if (self.input[0].y > 0)
                                        {
                                            normalized = new Vector2(0f, 1f);
                                        }
                                        normalized = (normalized + self.mainBodyChunk.vel.normalized * 0.2f).normalized;
                                        saint_player!.tongue.Shoot(normalized);
                                    }
                                }


                                if (saint_player!.tongue.Attached)
                                {
                                    if (self.input[0].jmp && !self.input[1].jmp && saint_player!.tongueAttachTime >= 2)
                                    {
                                        float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                                        if (self.grasps[0] != null && self.HeavyCarry(self.grasps[0].grabbed) && !(self.grasps[0].grabbed is Cicada))
                                        {
                                            num += Mathf.Min(Mathf.Max(0f, self.grasps[0].grabbed.TotalMass - 0.2f) * 1.5f, 1.3f);
                                        }

                                        self.bodyChunks[0].vel.y = 8f * num;
                                        self.bodyChunks[1].vel.y = 7f * num;
                                        self.jumpBoost = 8f;
                                        saint_player!.tongue.Release();
                                    }


                                    if (self.input[0].y > 0)
                                    {
                                        saint_player!.tongue.decreaseRopeLength(3f);
                                    }

                                    if (saint_player!.input[0].x == 0)
                                    {
                                        saint_player!.input[0].x = self.input[0].x;
                                    }

                                    if (self.input[0].y < 0)
                                    {
                                        saint_player!.tongue.increaseRopeLength(3f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
