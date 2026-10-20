using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Anchor", "Attach To Target")]
    public class AnchorAttachToTargetActionStrategy : IActionStrategy
    {
        public const string CommandKey = "anchor_attach_to_target";
        string IActionStrategy.CommandKey => CommandKey;

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Anchor", "Detach From Target")]
    public class AnchorDetachFromTargetActionStrategy : IActionStrategy
    {
        public const string CommandKey = "anchor_detach_from_target";
        string IActionStrategy.CommandKey => CommandKey;

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }
}
