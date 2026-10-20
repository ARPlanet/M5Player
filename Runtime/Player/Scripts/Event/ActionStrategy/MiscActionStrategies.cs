using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Module5.Player
{
    [Command("Remote Function", "Call")]
    public class RemoteFunctionCallActionStrategy : IActionStrategy
    {
        public const string CommandKey = "remote_function_call";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamUri = "uri";
        public const string ParamHeader = "header";
        public const string ParamContent = "content";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamUri, ParameterDefinition.Create<string>(string.Empty) },
            { ParamHeader, ParameterDefinition.Create<string>(string.Empty) },
            { ParamContent, ParameterDefinition.Create<string>(string.Empty) }
        };

        private readonly IVariableManager _variableManager;
        private IEventManager _eventManager;

        public RemoteFunctionCallActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public void SetEventManager(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string uri = (string)command.Parameters[ParamUri];
            string header = (string)command.Parameters[ParamHeader];
            string content = (string)command.Parameters[ParamContent];

            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][uri={uri}]");

            using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
            {
                if (!string.IsNullOrEmpty(content))
                {
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(content);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                request.downloadHandler = new DownloadHandlerBuffer();

                // 標頭處理
                Dictionary<string, string> headerDict = null;
                if (!string.IsNullOrEmpty(header))
                {
                    try
                    {
                        headerDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(header);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"{context.LogPrefix}[RemoteFunctionCall] Failed to parse header JSON: {e.Message}");
                    }
                }

                if (headerDict == null) headerDict = new Dictionary<string, string>();

                // 如果沒有 Content-Type，預設為 application/json
                if (!headerDict.ContainsKey("Content-Type"))
                {
                    headerDict["Content-Type"] = "application/json";
                }

                foreach (var kvp in headerDict)
                {
                    request.SetRequestHeader(kvp.Key, kvp.Value);
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log($"{context.LogPrefix}[RemoteFunctionCall] Success: {responseText}");

                    // 嘗試反序列化為指令陣列並執行
                    try
                    {
                        var remoteCommands = JsonConvert.DeserializeObject<M5Command[]>(responseText);
                        if (remoteCommands != null && remoteCommands.Length > 0)
                        {
                            if (_eventManager != null)
                            {
                                _eventManager.ExecuteCommands(remoteCommands, new ActionExecutionContext { RuleName = "Remote" });
                            }
                            else
                            {
                                Debug.LogWarning($"{context.LogPrefix}[RemoteFunctionCall] EventManager not set, cannot execute remote commands.");
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // 若不是 JSON 或格式不符則忽略（可能是單純的字串回傳）
                        Debug.Log($"{context.LogPrefix}[RemoteFunctionCall] Response is not a command array or failed to parse: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"{context.LogPrefix}[RemoteFunctionCall] Error: {request.error}\nResponse: {request.downloadHandler.text}");
                }
            }
        }
    }

    [Command("Native Function", "Call")]
    public class NativeFunctionCallActionStrategy : IActionStrategy
    {
        public const string CommandKey = "native_function_call";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Uri", "Open")]
    public class UriOpenActionStrategy : IActionStrategy
    {
        public const string CommandKey = "uri_open";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamUri = "uri";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamUri, ParameterDefinition.Create<string>(string.Empty) }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string uri = (string)command.Parameters[ParamUri];
            if (!string.IsNullOrEmpty(uri))
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][uri={uri}]");
                Application.OpenURL(uri);
            }
            yield break;
        }
    }

    [Command("Misc", "Rand Exec")]
    public class RandExecActionStrategy : IActionStrategy
    {
        public const string CommandKey = "misc_rand_exec";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Misc", "Delay Exec")]
    public class DelayExecActionStrategy : IActionStrategy
    {
        public const string CommandKey = "misc_delay_exec";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamSeconds = "seconds";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamSeconds, ParameterDefinition.Create<float>(0f) }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }
}
