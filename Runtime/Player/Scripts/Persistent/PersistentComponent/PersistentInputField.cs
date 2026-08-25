using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    [Serializable]
    public class PersistentInputField : PersistentComponent
    {
        public string textComp;
        public string text;
        public int characterLimit;
        public InputField.LineType lineType;
        public string placeholder;
    }
}