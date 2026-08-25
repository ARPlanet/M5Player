using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToTextConverter : PersistentToGraphicConverter<Text>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (comp == null) return Task.CompletedTask;

            base.PersistentToObjectAsync(dataBase);

            PersistentText persistentText = (PersistentText)persistent;

            //comp.text = persistentText.text;
            comp.fontStyle = persistentText.fontStyle;
            comp.fontSize = persistentText.fontSize;
            comp.alignment = persistentText.alignment;
            comp.color = persistentText.color;
            //comp.maskable = persistentText.maskable;

            DynamicTextReplacer textReplacer = comp.GetComponent<DynamicTextReplacer>();
            if(textReplacer != null)
            {
                textReplacer.RawText = persistentText.text;
            }

            return Task.CompletedTask;
        }

    }
}
