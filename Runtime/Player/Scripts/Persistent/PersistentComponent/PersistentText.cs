using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Serializable]
    public class PersistentText : PersistentGraphic
    {
        public string text;
        public FontStyle fontStyle;
        public int fontSize;
        public TextAnchor alignment;
        public Color color;
        //public bool maskable;
    }
}