using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Module5.DI;
namespace Module5.Player
{
    [RequireComponent(typeof(Text))]
    public class DynamicTextReplacer : MonoBehaviour
    {
        [SerializeField, TextArea(3, 10)]
        [Tooltip("設定包含變數的字串版型，例如：'Score: ${Score : 000}'")]
        private string rawText;

        public string RawText
        {
            get => rawText;
            set
            {
                if (rawText != value)
                {
                    rawText = value;
                    if (isActiveAndEnabled)
                    {
                        UnsubscribeEvents();
                        ParseRawText();
                        SubscribeEvents();
                        UpdateText();
                    }
                }
            }
        }

        [Tooltip("延遲更新時間（秒），用以限制更新頻率")]
        public float updateDelay = 0.05f;

        [Inject] IVariableManager VariableManager { get; set; }

        private Text targetText;
        private List<TextPart> parts = new List<TextPart>();

        private bool isCoolingDown = false;
        private bool isUpdatePending = false;
        private Coroutine cooldownCoroutine;

        private struct TextPart
        {
            public bool isDynamic;
            public string text;
            public string variableName;
            public string format;
        }

        private void Awake()
        {
            targetText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            ParseRawText();
            SubscribeEvents();
            UpdateText(); // Initial update
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
                cooldownCoroutine = null;
            }
            isCoolingDown = false;
            isUpdatePending = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && isActiveAndEnabled)
            {
                UnsubscribeEvents();
                ParseRawText();
                SubscribeEvents();
                UpdateText();
            }
        }
#endif

        private void ParseRawText()
        {
            parts.Clear();
            if (string.IsNullOrEmpty(rawText)) return;

            string pattern = @"\$\{(?<name>[^:}]+)(?:\s*:\s*(?<format>[^}]+))?\}";
            int lastIndex = 0;

            foreach (Match match in Regex.Matches(rawText, pattern))
            {
                if (match.Index > lastIndex)
                {
                    parts.Add(new TextPart { isDynamic = false, text = rawText.Substring(lastIndex, match.Index - lastIndex) });
                }

                parts.Add(new TextPart
                {
                    isDynamic = true,
                    variableName = match.Groups["name"].Value.Trim(),
                    format = match.Groups["format"].Success ? match.Groups["format"].Value.Trim() : null,
                    text = match.Value // Keep the original text just in case variable is not found
                });

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < rawText.Length)
            {
                parts.Add(new TextPart { isDynamic = false, text = rawText.Substring(lastIndex) });
            }
        }

        private void SubscribeEvents()
        {
            if (VariableManager == null) return;
            HashSet<string> subscribedVars = new HashSet<string>();

            foreach (var part in parts)
            {
                if (part.isDynamic && !subscribedVars.Contains(part.variableName))
                {
                    VariableManager.Subscribe(part.variableName, HandleVariableChanged);
                    subscribedVars.Add(part.variableName);
                }
            }
        }

        private void UnsubscribeEvents()
        {
            if (VariableManager == null) return;
            HashSet<string> unsubscribedVars = new HashSet<string>();

            foreach (var part in parts)
            {
                if (part.isDynamic && !unsubscribedVars.Contains(part.variableName))
                {
                    VariableManager.Unsubscribe(part.variableName, HandleVariableChanged);
                    unsubscribedVars.Add(part.variableName);
                }
            }
        }

        private void HandleVariableChanged(object newValue)
        {
            if (!isCoolingDown)
            {
                UpdateText();
                if (updateDelay > 0)
                {
                    isCoolingDown = true;
                    cooldownCoroutine = StartCoroutine(CooldownRoutine());
                }
            }
            else
            {
                isUpdatePending = true;
            }
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(updateDelay);

            if (isUpdatePending)
            {
                UpdateText();
                isUpdatePending = false;
                cooldownCoroutine = StartCoroutine(CooldownRoutine());
            }
            else
            {
                isCoolingDown = false;
                cooldownCoroutine = null;
            }
        }

        private void UpdateText()
        {
            if (targetText == null) return;
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (var part in parts)
            {
                if (!part.isDynamic)
                {
                    sb.Append(part.text);
                }
                else
                {
                    if (VariableManager != null && VariableManager.TryGetValue(part.variableName, out object value))
                    {
                        if (value == null)
                        {
                            // If the variable exists but is explicitly null, we just ignore or append empty string.
                        }
                        else if (!string.IsNullOrEmpty(part.format) && value is IFormattable formattable)
                        {
                            sb.Append(formattable.ToString(part.format, null));
                        }
                        else
                        {
                            sb.Append(value.ToString());
                        }
                    }
                    else
                    {
                        // Variable not found, keep original text
                        sb.Append(part.text);
                    }
                }
            }

            targetText.text = sb.ToString();
        }
    }
}
