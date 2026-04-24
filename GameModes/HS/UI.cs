using Cyclesseum;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using Newtonsoft.Json.Linq;
using RainMeadow.UI.Components.Patched;
using RainMeadow.UI.Pages;
using UnityEngine;
using ArenaMode = RainMeadow.ArenaOnlineGameMode;

namespace RainMeadow.UI.Components
{
    public class HideAndSeekInterface : RectangularMenuObject
    {
        public MenuTabWrapper tabWrapper;

        /*
         * 
         * Lobby data
         * 
         */

        public MenuLabel hidersLabel;
        public MenuLabel seekersLabel;

        public MenuLabel minTimeSecondsLabel;
        public MenuLabel timePerCameraSecondsLabel;
        public MenuLabel maxTimeSecondsLabel;

        public OpTextBox hidersMinTimeSecondsTextBox;
        public OpTextBox hidersTimePerCameraSecondsTextBox;
        public OpTextBox hidersMaxTimeSecondsTextBox;

        public OpTextBox seekersMinTimeSecondsTextBox;
        public OpTextBox seekersTimePerCameraSecondsTextBox;
        public OpTextBox seekersMaxTimeSecondsTextBox;

        public MenuLabel disableCollissionsLabel;
        public OpCheckBox disableCollisionsCheckBox;

        /*
         * 
         * Client data
         * 
         */

        public MenuLabel isSeekerLabel;
        public OpCheckBox isSeekerCheckBox;

        /*
         * 
         * Local vars
         * 
         */
        public ArenaMode arena => OnlineManager.lobby.gameMode as ArenaOnlineGameMode;
        public HideAndSeekGameMode hideAndSeekGameMode;

        public bool AllSettingsDisabled =>
            arena.initiateLobbyCountdown && arena.arenaClientSettings.ready;
        public bool OwnerSettingsDisabled =>
            !(OnlineManager.lobby?.isOwner == true) || AllSettingsDisabled;

        public Vector2 size;

        public float xMargin = 30f;
        public float yMargin = 30f;

        public HideAndSeekInterface(
            ArenaOnlineGameMode arena,  
            HideAndSeekGameMode hideAndSeekGameMode,
            Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size
        ) : base(menu, owner, pos, size)
        {
            
            tabWrapper = new(menu, this);

            this.hideAndSeekGameMode = hideAndSeekGameMode;

            /*
             * 
             * isSeeker
             * 
             */
            isSeekerCheckBox = new(new Configurable<bool>(true), intuitivePosition(1f,5.5f)) {
                description = menu.Translate("Enable seeker mode")
            };

            isSeekerCheckBox.OnChange += () => {
                this.hideAndSeekGameMode.isSeeker = isSeekerCheckBox.GetValueBool();
            };
            isSeekerLabel = new(menu, this, menu.Translate("I am a seeker"),
                intuitivePosition(1.5f, 5.5f), new(50f, 20f), false);
            isSeekerLabel.label.alignment = FLabelAlignment.Left;

            /*
             * 
             * hiders and seekers labels
             * 
             */

            hidersLabel = new(menu, this, menu.Translate("Hiders"),
                intuitivePosition(0.5f, 4f), new(50f, 20f), false);
            hidersLabel.label.alignment = FLabelAlignment.Left;

            seekersLabel = new(menu, this, menu.Translate("Seekers"),
                intuitivePosition(4.5f, 4f), new(50f, 20f), false);
            seekersLabel.label.alignment = FLabelAlignment.Left;

            /*
             * 
             * minTimeSeconds
             * 
             */
            hidersMinTimeSecondsTextBox = new(new Configurable<int>(30),
                intuitivePosition(1f, 3f), 60)
            {
                alignment = FLabelAlignment.Center,
                description = menu.Translate("Minimum amount of time for hiding"),
                accept = OpTextBox.Accept.Int,
                greyedOut = OwnerSettingsDisabled
            };
            hidersMinTimeSecondsTextBox.OnValueUpdate += (config, value, oldValue) =>
            {
                if (hidersMinTimeSecondsTextBox.valueInt < 0) hidersMinTimeSecondsTextBox.valueInt = 0;
                this.hideAndSeekGameMode.hidersMinTimeSeconds = hidersMinTimeSecondsTextBox.valueInt;
            };


            seekersMinTimeSecondsTextBox = new(new Configurable<int>(30),
                intuitivePosition(5f, 3f), 60)
            {
                alignment = FLabelAlignment.Center,
                description = menu.Translate("Minimum amount of time for seeking"),
                accept = OpTextBox.Accept.Int,
                greyedOut = OwnerSettingsDisabled
            };
            seekersMinTimeSecondsTextBox.OnValueUpdate += (config, value, oldValue) =>
            {
                if (seekersMinTimeSecondsTextBox.valueInt < 0) seekersMinTimeSecondsTextBox.valueInt = 0;
                this.hideAndSeekGameMode.seekersMinTimeSeconds = seekersMinTimeSecondsTextBox.valueInt;
            };


            minTimeSecondsLabel = new(menu, this, menu.Translate("Min time"),
                intuitivePosition(7f, 3f), new(100f, 20f), false);
            minTimeSecondsLabel.label.alignment = FLabelAlignment.Left;

            /*
             * 
             * timePerCameraSeconds
             * 
             */

            hidersTimePerCameraSecondsTextBox = new(new Configurable<int>(30),
                intuitivePosition(1f, 2f), 60)
            {
                alignment = FLabelAlignment.Center,
                description = menu.Translate("Total bonus for hiders increases with level size (number of cameras)"),
                accept = OpTextBox.Accept.Int,
                greyedOut = OwnerSettingsDisabled
            };
            hidersTimePerCameraSecondsTextBox.OnValueUpdate += (config, value, oldValue) =>
            {
                if (hidersTimePerCameraSecondsTextBox.valueInt < 0) hidersTimePerCameraSecondsTextBox.valueInt = 0;
                this.hideAndSeekGameMode.hidersTimePerCameraSeconds = hidersTimePerCameraSecondsTextBox.valueInt;
            };


            seekersTimePerCameraSecondsTextBox = new(new Configurable<int>(30),
                intuitivePosition(5f, 2f), 60)
            {
                alignment = FLabelAlignment.Center,
                description = menu.Translate("Total bonus for seekers increases with level size (number of cameras)"),
                accept = OpTextBox.Accept.Int,
                greyedOut = OwnerSettingsDisabled
            };
            seekersTimePerCameraSecondsTextBox.OnValueUpdate += (config, value, oldValue) =>
            {
                if (seekersTimePerCameraSecondsTextBox.valueInt < 0) seekersTimePerCameraSecondsTextBox.valueInt = 0;
                this.hideAndSeekGameMode.seekersTimePerCameraSeconds = seekersTimePerCameraSecondsTextBox.valueInt;
            };


            timePerCameraSecondsLabel = new(menu, this, menu.Translate("Bonus time per camera"),
                intuitivePosition(7f, 2f), new(100f, 20f), false);
            timePerCameraSecondsLabel.label.alignment = FLabelAlignment.Left;

            /*
             * 
             * maxTimeSeconds
             * 
             */

            hidersMaxTimeSecondsTextBox = new(new Configurable<int>(30),
                intuitivePosition(1f, 1f), 60)
            {
                alignment = FLabelAlignment.Center,
                description = menu.Translate("Maximum amount of time for hiding. If lower than minimum time, there's no limit."),
                accept = OpTextBox.Accept.Int,
                greyedOut = OwnerSettingsDisabled
            };
            hidersMaxTimeSecondsTextBox.OnValueUpdate += (config, value, oldValue) =>
            {
                if (hidersMaxTimeSecondsTextBox.valueInt < 0) hidersMaxTimeSecondsTextBox.valueInt = 0;
                this.hideAndSeekGameMode.hidersMaxTimeSeconds = hidersMaxTimeSecondsTextBox.valueInt;
            };


            seekersMaxTimeSecondsTextBox = new(new Configurable<int>(30),
                intuitivePosition(5f, 1f), 60)
            {
                alignment = FLabelAlignment.Center,
                description = menu.Translate("Maximum amount of time for seeking. If lower than minimum time, there's no limit."),
                accept = OpTextBox.Accept.Int,
                greyedOut = OwnerSettingsDisabled
            };
            seekersMaxTimeSecondsTextBox.OnValueUpdate += (config, value, oldValue) =>
            {
                if (seekersMaxTimeSecondsTextBox.valueInt < 0) seekersMaxTimeSecondsTextBox.valueInt = 0;
                this.hideAndSeekGameMode.seekersMaxTimeSeconds = seekersMaxTimeSecondsTextBox.valueInt;
            };


            maxTimeSecondsLabel = new(menu, this, menu.Translate("Max time"),
                intuitivePosition(7f, 1f), new(100f, 20f), false);
            maxTimeSecondsLabel.label.alignment = FLabelAlignment.Left;

            var isSeekerWrapper = new PatchedUIelementWrapper(tabWrapper, isSeekerCheckBox);
            var hidersMinTimeSecondsWrapper = new PatchedUIelementWrapper(tabWrapper, hidersMinTimeSecondsTextBox);
            var hidersTimePerCameraSecondsWrapper = new PatchedUIelementWrapper(tabWrapper, hidersTimePerCameraSecondsTextBox);
            var hidersMaxTimeSecondsWrapper = new PatchedUIelementWrapper(tabWrapper, hidersMaxTimeSecondsTextBox);
            var seekersMinTimeSecondsWrapper = new PatchedUIelementWrapper(tabWrapper, seekersMinTimeSecondsTextBox);
            var seekersTimePerCameraSecondsWrapper = new PatchedUIelementWrapper(tabWrapper, seekersTimePerCameraSecondsTextBox);
            var seekersMaxTimeSecondsWrapper = new PatchedUIelementWrapper(tabWrapper, seekersMaxTimeSecondsTextBox);

            this.SafeAddSubobjects(
                tabWrapper,
                isSeekerWrapper,
                isSeekerLabel,
                hidersLabel,
                seekersLabel,
                minTimeSecondsLabel,
                timePerCameraSecondsLabel,
                maxTimeSecondsLabel,
                hidersMinTimeSecondsWrapper,
                hidersTimePerCameraSecondsWrapper,
                hidersMaxTimeSecondsWrapper,
                seekersMinTimeSecondsWrapper,
                seekersTimePerCameraSecondsWrapper,
                seekersMaxTimeSecondsWrapper
                );
        }

        public void OnShutdown()
        {
            if (!(OnlineManager.lobby?.isOwner == true))
                return;
            RainMeadow.rainMeadowOptions.config.Save();
        }

        public override void RemoveSprites()
        {
            base.RemoveSprites();
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }

        public override void Update()
        {
            base.Update();

            /*
             * 
             * Lobby data
             * 
             */

            updateTextBoxInt(hidersMinTimeSecondsTextBox, hideAndSeekGameMode.hidersMinTimeSeconds);
            updateTextBoxInt(hidersTimePerCameraSecondsTextBox, hideAndSeekGameMode.hidersTimePerCameraSeconds);
            updateTextBoxInt(hidersMaxTimeSecondsTextBox, hideAndSeekGameMode.hidersMaxTimeSeconds);

            updateTextBoxInt(seekersMinTimeSecondsTextBox, hideAndSeekGameMode.seekersMinTimeSeconds);
            updateTextBoxInt(seekersTimePerCameraSecondsTextBox, hideAndSeekGameMode.seekersTimePerCameraSeconds);
            updateTextBoxInt(seekersMaxTimeSecondsTextBox, hideAndSeekGameMode.seekersMaxTimeSeconds);

            updateCheckBox(disableCollisionsCheckBox, hideAndSeekGameMode.disableCollisions);

            /*
             * 
             * Client data
             * 
             */

            updateCheckBox(isSeekerCheckBox, hideAndSeekGameMode.isSeeker);
        }

        public void updateCheckBox(OpCheckBox checkBox, bool value)
        {
            if (checkBox != null && !checkBox.held)
            {
                checkBox.SetValueBool(value);
            }
        }

        public void updateTextBoxInt(OpTextBox textBox, int value)
        {
            if (textBox != null)
            {
                textBox.greyedOut = OwnerSettingsDisabled;
                textBox.held = hidersTimePerCameraSecondsTextBox._KeyboardOn;
                if (!textBox.held)
                {
                    textBox.valueInt = value;
                }
            }
        }

        public Vector2 intuitivePosition(float x, float y)
        {
            return new Vector2(x*xMargin, 2*y*yMargin);
        }
    }
}