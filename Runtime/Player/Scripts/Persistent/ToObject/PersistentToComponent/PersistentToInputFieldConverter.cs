using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    public class PersistentToInputFieldConverter : PersistentToComponenetConverter<InputField>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if(comp == null)  return Task.CompletedTask;
            PersistentInputField persistentInputField = (PersistentInputField)persistent;
            comp.textComponent = dataBase.GetObject<Text>(persistentInputField.textComp);
            comp.text = persistentInputField.text;
            comp.characterLimit = persistentInputField.characterLimit;
            comp.lineType = persistentInputField.lineType;
            comp.placeholder = dataBase.GetObject<Graphic>(persistentInputField.placeholder);

            return Task.CompletedTask;
        }
    }
}
