using RainMeadow;
using static RainMeadow.OnlineEntity.EntityData;
using static RainMeadow.OnlineState;
using System;
using Cyclesseum;

public class HideAndSeekClientData: OnlineEntity.EntityData
{
    public bool isSeeker;
    public float lerp;

    public HideAndSeekClientData() { }

    public override EntityDataState MakeState(OnlineEntity entity, OnlineResource inResource)
    {
        return new State(this);
    }

    public class State : EntityDataState
    {
        [OnlineField(group = "HideAndSeekChecks")]
        public bool isSeeker;

        [OnlineField(group = "Lerp")]
        public float lerp;
        public State() { }

        public State(HideAndSeekClientData onlineEntity) : base()
        {
            if (RainMeadow.RainMeadow.isArenaMode(out var arena) && arena != null && HideAndSeekGameMode.isHideAndSeekGameMode(arena, out var hideAndSeekGameMode) && hideAndSeekGameMode != null)
            {
                isSeeker = onlineEntity.isSeeker;
                lerp = onlineEntity.lerp;
            }
        }

        public override void ReadTo(OnlineEntity.EntityData entityData, OnlineEntity onlineEntity)
        {
            var hideAndSeekClientData = (HideAndSeekClientData)entityData;
            hideAndSeekClientData.isSeeker = isSeeker;
            hideAndSeekClientData.lerp = lerp;
        }

        public override Type GetDataType() => typeof(HideAndSeekClientData);
    }
}