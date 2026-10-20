using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module5.Player
{
    [Command("Event Handler", "Enable")]
    public class EventHandlerEnableActionStrategy : IActionStrategy
    {
        public const string CommandKey = "event_handler_enable";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamEvent = "event";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<EventComponent>() },
            { ParamEvent, ParameterDefinition.CreateGuid() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            SetEventHandlerEnabled(command, context, true);
            yield break;
        }

        private void SetEventHandlerEnabled(M5Command command, ActionExecutionContext context, bool enabled)
        {
            var eventComponent = (EventComponent)command.Parameters[ParamTarget];
            if (eventComponent != null)
            {
                Guid eventVal = (Guid)command.Parameters[ParamEvent];

                if (eventVal != Guid.Empty && eventComponent.eventHandlers != null)
                {
                    var handler = eventComponent.eventHandlers.FirstOrDefault(h => h.Guid == eventVal);
                    if (handler != null)
                    {
                        Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={eventComponent.gameObject.name}, handler={handler.Name}, enabled={enabled}]");
                        handler.Enabled = enabled;
                    }
                }
            }
        }
    }

    [Command("Event Handler", "Disable")]
    public class EventHandlerDisableActionStrategy : IActionStrategy
    {
        public const string CommandKey = "event_handler_disable";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamEvent = "event";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<EventComponent>() },
            { ParamEvent, ParameterDefinition.CreateGuid() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            SetEventHandlerEnabled(command, context, false);
            yield break;
        }

        private void SetEventHandlerEnabled(M5Command command, ActionExecutionContext context, bool enabled)
        {
            var eventComponent = (EventComponent)command.Parameters[ParamTarget];
            if (eventComponent != null)
            {
                Guid eventVal = (Guid)command.Parameters[ParamEvent];

                if (eventVal != Guid.Empty && eventComponent.eventHandlers != null)
                {
                    var handler = eventComponent.eventHandlers.FirstOrDefault(h => h.Guid == eventVal);
                    if (handler != null)
                    {
                        Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={eventComponent.gameObject.name}, handler={handler.Name}, enabled={enabled}]");
                        handler.Enabled = enabled;
                    }
                }
            }
        }
    }
}
