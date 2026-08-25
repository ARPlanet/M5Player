using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class AnchorAttachToTargetActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class AnchorDetachFromTargetActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }
}
