using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToRawImageSequenceConverter : PersistentToBehaviourConverter<RawImageSequence>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            base.PersistentToObjectAsync(dataBase);
            if (comp == null) return Task.CompletedTask;
            PersistentRawImageSequence persistentRawImageSequence = (PersistentRawImageSequence)persistent;

            comp.Size = persistentRawImageSequence.size;
            comp.frameLength = persistentRawImageSequence.frameLength;
            comp.speed = persistentRawImageSequence.speed;
            comp.mode = persistentRawImageSequence.mode;
            comp.playAuto = persistentRawImageSequence.playAuto;

            return Task.CompletedTask;
        }

    }
}
