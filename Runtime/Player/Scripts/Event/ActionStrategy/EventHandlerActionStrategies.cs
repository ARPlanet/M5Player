using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module5.Player
{
    public class EventHandlerEnableActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamEvent = "event";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamEvent, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            SetEventHandlerEnabled(command, true);
            yield break;
        }

        private void SetEventHandlerEnabled(M5Command command, bool enabled)
        {
            var eventComponent = (EventComponent)command.Parameters[ParamTarget];
            if (eventComponent != null)
            {
                object eventVal = command.Parameters[ParamEvent];
                Guid eventGuid = Guid.Empty;
                if (eventVal is Guid g) eventGuid = g;
                else if (eventVal is string s && Guid.TryParse(s, out Guid parsed)) eventGuid = parsed;

                if (eventGuid != Guid.Empty && eventComponent.eventHandlers != null)
                {
                    var handler = eventComponent.eventHandlers.FirstOrDefault(h => h.Guid == eventGuid);
                    if (handler != null)
                    {
                        Debug.Log($"[Action {command.CommandType}][target={eventComponent.gameObject.name}, handler={handler.Name}, enabled={enabled}]");
                        handler.Enabled = enabled;
                    }
                }
            }
        }
    }

    public class EventHandlerDisableActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamEvent = "event";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() },
            { ParamEvent, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            SetEventHandlerEnabled(command, false);
            yield break;
        }

        private void SetEventHandlerEnabled(M5Command command, bool enabled)
        {
            var eventComponent = (EventComponent)command.Parameters[ParamTarget];
            if (eventComponent != null)
            {
                object eventVal = command.Parameters[ParamEvent];
                Guid eventGuid = Guid.Empty;
                if (eventVal is Guid g) eventGuid = g;
                else if (eventVal is string s && Guid.TryParse(s, out Guid parsed)) eventGuid = parsed;

                if (eventGuid != Guid.Empty && eventComponent.eventHandlers != null)
                {
                    var handler = eventComponent.eventHandlers.FirstOrDefault(h => h.Guid == eventGuid);
                    if (handler != null)
                    {
                        Debug.Log($"[Action {command.CommandType}][target={eventComponent.gameObject.name}, handler={handler.Name}, enabled={enabled}]");
                        handler.Enabled = enabled;
                    }
                }
            }
        }
    }
}
