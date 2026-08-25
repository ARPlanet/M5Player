using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentEventComponent : PersistentComponent
    {
        public PersistentEventHandler[] eventHandlers;


    }
}