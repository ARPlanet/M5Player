using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Module5.Player
{
    public class PersistentToLightConverter : PersistentToComponenetConverter<Light>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if(comp == null)  return Task.CompletedTask;
            PersistentLight persistentlight = (PersistentLight)persistent;
            comp.type = persistentlight.type;
            comp.range = persistentlight.range;
            comp.spotAngle = persistentlight.spotAngle;
            comp.color = persistentlight.color;
            comp.intensity = persistentlight.intensity;
            comp.shadows = persistentlight.shadows;
            comp.shadowStrength = persistentlight.shadowStrength;
            return Task.CompletedTask;
        }
    }
}
