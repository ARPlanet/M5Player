using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentPanelObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.Panel;
    }
}