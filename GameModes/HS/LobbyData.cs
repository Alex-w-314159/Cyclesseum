using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using Menu;
using RainMeadow;
using RainMeadow.UI;
using RainMeadow.UI.Components;
using UnityEngine;
using System;
using Cyclesseum;

internal class HideAndSeekLobbyData : OnlineResource.ResourceData
{
	public int hidersMinTimeSeconds;
	public int hidersTimePerCameraSeconds;
	public int hidersMaxTimeSeconds;

	public int seekersMinTimeSeconds;
	public int seekersTimePerCameraSeconds;
	public int seekersMaxTimeSeconds;

	public bool disableCollisions;

	public string proceduralMusicRegion="";

	public HideAndSeekLobbyData() { }

	public override ResourceDataState MakeState(OnlineResource resource)
	{
		return new State(this, resource);
	}

	internal class State : ResourceDataState
	{
		[OnlineField(group = "Time")]
		public int hidersMinTimeSeconds;

		[OnlineField(group = "Time")]
		public int hidersTimePerCameraSeconds;

		[OnlineField(group = "Time")]
		public int hidersMaxTimeSeconds;

		[OnlineField(group = "Time")]
		public int seekersMinTimeSeconds;

		[OnlineField(group = "Time")]
		public int seekersTimePerCameraSeconds;

		[OnlineField(group = "Time")]
		public int seekersMaxTimeSeconds;

        [OnlineField(group = "Options")]
        public bool disableCollisions;

		[OnlineField(group = "Internal")]
		public string proceduralMusicRegion="";

        public State() { }

		// takes the current value in the State
		public State(HideAndSeekLobbyData data, OnlineResource onlineResource)
		{
			if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode))
			{
				/*
				 * 
				 * Time
				 * 
				 */

				this.hidersMinTimeSeconds = hideAndSeekGameMode.hidersMinTimeSeconds;
				this.hidersTimePerCameraSeconds = hideAndSeekGameMode.hidersTimePerCameraSeconds;
				this.hidersMaxTimeSeconds = hideAndSeekGameMode.hidersMaxTimeSeconds;

				this.seekersMinTimeSeconds = hideAndSeekGameMode.seekersMinTimeSeconds;
				this.seekersTimePerCameraSeconds = hideAndSeekGameMode.seekersTimePerCameraSeconds;
				this.seekersMaxTimeSeconds = hideAndSeekGameMode.seekersMaxTimeSeconds;
				
				/*
				 * 
				 * Options
				 * 
				 */
				this.disableCollisions = hideAndSeekGameMode.disableCollisions;

				/*
				 * 
				 * Internal
				 * 
				 */
				this.proceduralMusicRegion = hideAndSeekGameMode.proceduralMusicRegion ?? "";
			}
		}

		// read the value in the State and applies it
		public override void ReadTo(OnlineResource.ResourceData data, OnlineResource resource)
		{
			var lobby = (resource as Lobby);
			if (RainMeadow.RainMeadow.isArenaMode(out var arena) && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode))
			{
				/*
				 * 
				 * Time
				 * 
				 */
				hideAndSeekGameMode.hidersMinTimeSeconds = this.hidersMinTimeSeconds;
				hideAndSeekGameMode.hidersTimePerCameraSeconds = this.hidersTimePerCameraSeconds;
				hideAndSeekGameMode.hidersMaxTimeSeconds = this.hidersMaxTimeSeconds;

				hideAndSeekGameMode.seekersMinTimeSeconds = this.seekersMinTimeSeconds;
				hideAndSeekGameMode.seekersTimePerCameraSeconds = this.seekersTimePerCameraSeconds;
				hideAndSeekGameMode.seekersMaxTimeSeconds = this.seekersMaxTimeSeconds;

				/*
				 * 
				 * Options
				 * 
				 */
				hideAndSeekGameMode.disableCollisions = this.disableCollisions;

				/*
				 * 
				 * Internal
				 * 
				 */
				hideAndSeekGameMode.proceduralMusicRegion = this.proceduralMusicRegion ?? "";
			}
		}

		public override Type GetDataType() => typeof(HideAndSeekLobbyData);
	}
}