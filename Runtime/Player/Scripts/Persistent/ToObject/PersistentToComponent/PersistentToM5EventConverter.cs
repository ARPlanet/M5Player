using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Module5.Player
{
    public class PersistentToM5EventConverter : PersistentToComponenetConverter<EventComponent>
    {
        public override Task PersistentToObjectAsync(PersistentToObjectDataBase dataBase)
        {
            if (persistent is not PersistentEventComponent persistentEvent) return Task.CompletedTask;

            if (persistentEvent.eventHandlers == null)
            {
                comp.eventHandlers = new M5EventHandler[0];
                return Task.CompletedTask;
            }

            comp.eventHandlers = new M5EventHandler[persistentEvent.eventHandlers.Length];
            for (int i = 0; i < persistentEvent.eventHandlers.Length; i++)
            {
                var pEvent = persistentEvent.eventHandlers[i];
                var m5Event = new M5EventHandler
                {
                    Name = pEvent.Name,
                    Enabled = pEvent.Enabled,
                    EventType = string.IsNullOrEmpty(pEvent.EventType) ? NoneCommands.KeyNone : pEvent.EventType,
                    SourceFilter = pEvent.SourceFilter?.Select(s => {
                        if (Guid.TryParse(s, out Guid g))
                        {
                            // 嘗試映射新 ID
                            if (dataBase.TryGetInstance(g, out var inst)) return inst.GUID;
                            return g;
                        }
                        return Guid.Empty;
                    }).Where(g => g != Guid.Empty).ToArray(),
                    Delay = pEvent.Delay,
                    Priority = pEvent.Priority
                };

                // Map Conditions
                if (pEvent.Conditions != null)
                {
                    m5Event.Conditions = pEvent.Conditions?.Select(c => {
                        string targetIdStr = c.TargetId;
                        if (Guid.TryParse(c.TargetId, out var g) && g != Guid.Empty && dataBase.TryGetInstance(g, out var inst))
                        {
                            targetIdStr = inst.GUID.ToString();
                        }

                        return new M5Condition
                        {
                            TargetType = c.TargetType,
                            TargetId = targetIdStr,
                            TargetProperty = c.TargetProperty,
                            Operator = c.Operator,
                            ExpectedValue = RemapValue(c.ExpectedValue, dataBase)
                        };
                    }).ToArray();
                }

                // Map Commands
                if (pEvent.Commands != null)
                {
                    m5Event.Commands = new M5Command[pEvent.Commands.Length];
                    for (int k = 0; k < pEvent.Commands.Length; k++)
                    {
                        var pCmd = pEvent.Commands[k];
                        var cmdType = string.IsNullOrEmpty(pCmd.Type) ? NoneCommands.KeyNone : pCmd.Type;
                        m5Event.Commands[k] = new M5Command
                        {
                            Delay = pCmd.Delay,
                            CommandType = cmdType,
                            Parameters = RemapParameters(pCmd.Parameters, dataBase, cmdType)
                        };
                    }
                }
                
                // Basic mapping for ID if available in base class
                if (Guid.TryParse(pEvent.Guid, out Guid guid))
                {
                    m5Event.Guid = guid;
                }
                else
                {
                    m5Event.Guid = Guid.NewGuid();
                }

                comp.eventHandlers[i] = m5Event;
            }
            return Task.CompletedTask;
        }

        private Dictionary<string, object> RemapParameters(Dictionary<string, object> parameters, PersistentToObjectDataBase dataBase, string cmdType = null)
        {
            if (parameters == null) return null;
            var result = new Dictionary<string, object>();
            foreach (var kvp in parameters)
            {
                if (kvp.Key == "event" && (cmdType == EventHandlerCommands.KeyDisable || cmdType == EventHandlerCommands.KeyEnable))
                {
                    result[kvp.Key] = kvp.Value;
                }
                else
                {
                    result[kvp.Key] = RemapValue(kvp.Value, dataBase, cmdType);
                }
            }
            return result;
        }

        private object RemapValue(object value, PersistentToObjectDataBase dataBase, string cmdType = null)
        {
            if (value == null) return null;

            if (value is Guid guid)
            {
                if (dataBase.TryGetInstance(guid, out var inst)) return inst.GUID;
                return guid;
            }

            if (value is string s && Guid.TryParse(s, out Guid g))
            {
                if (dataBase.TryGetInstance(g, out var inst)) return inst.GUID.ToString();
            }
            else if (value is JObject jo)
            {
                var dict = jo.ToObject<Dictionary<string, object>>();
                if (dict != null) return RemapParameters(dict, dataBase, cmdType);
            }
            else if (value is IList list)
            {
                var newList = new List<object>();
                foreach (var item in list)
                {
                    newList.Add(RemapValue(item, dataBase, cmdType));
                }
                return newList;
            }
            return value;
        }
    }
}
