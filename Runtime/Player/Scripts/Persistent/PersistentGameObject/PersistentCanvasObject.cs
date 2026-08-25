using System;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Serializable]
    public class PersistentCanvasObject : PersistentGameObject
    {
        public override string Type => GameObjectTypes.Canvas;

        [JsonProperty("uiCanvas")]
        public PersistentUICanvas uiCanvas;
    }
}