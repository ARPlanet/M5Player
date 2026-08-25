using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToButtonConverter : PersistentToComponenetConverter<Button>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            return Task.CompletedTask;
        }
    }
}
