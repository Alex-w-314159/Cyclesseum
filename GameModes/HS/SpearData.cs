using System;
using System.Data.SqlTypes;
using RainMeadow;
using UnityEngine;
using static RainMeadow.OnlineEntity;
using static RainMeadow.OnlineEntity.EntityData;
using static RainMeadow.OnlineState;
using System.Collections.Generic;
using System.Linq;
public class ObjectColorData : OnlineEntity.EntityData
{
    public Color color;

    public ObjectColorData() { }

    public override EntityDataState MakeState(OnlineEntity entity, OnlineResource resource)
    {
        return new State(this, entity);
    }

    public class State : EntityDataState
    {
        [OnlineFieldColorRgb]
        public Color color;

        public State() { }

        public State(ObjectColorData data, OnlineEntity entity)
        {
            if (entity is OnlineSpear spear && spear.AbstractSpear.realizedObject is Spear sp)
            {
                color = sp.color;
            }
        }

        public override void ReadTo(EntityData data, OnlineEntity entity)
        {
            if (entity is OnlineSpear spear && spear.AbstractSpear.realizedObject is Spear sp)
            {
                if (color != null)
                {
                    sp.color = color;
                }
            }
        }

        public override Type GetDataType() => typeof(SpearColorData);
    }
}