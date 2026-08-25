using UnityEngine;
using UnityEngine.InputSystem;
using Module5.DI;
namespace Module5.Player
{
    /// <summary>
    /// Player 端 Console 開關觸發器。
    /// 支援兩種觸發方式：
    ///   1. BackQuote (`) 鍵（桌面端）
    ///   2. 三指同時觸控螢幕並持續 1 秒（行動裝置）
    /// Console 預設隱藏，每次觸發切換顯示/隱藏狀態。
    /// </summary>
    public class ConsoleToggle : MonoBehaviour
    {
        [Header("參照")]
        [SerializeField] ConsolePanel consolePanel;

        [Header("設定")]
        [Tooltip("三指觸控需持續幾秒才觸發")]
        [SerializeField] float touchHoldDuration = 1f;

        float _touchHoldTimer;
        bool  _touchTriggered;

        void Start()
        {
            // 預設隱藏
            if (consolePanel != null)
                consolePanel.gameObject.SetActive(false);
        }

        void Update()
        {
            HandleKeyboard();
            HandleTouch();
        }

        // ── 私有方法 ──────────────────────────────────────────────────────────

        void HandleKeyboard()
        {
            // 使用新 Input System 偵測 BackQuote 鍵
            if (Keyboard.current != null && Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                Toggle();
            }
        }

        void HandleTouch()
        {
            if (Touchscreen.current == null) return;

            int touchCount = 0;
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                    touchCount++;
            }

            if (touchCount >= 3)
            {
                _touchHoldTimer += Time.deltaTime;

                if (!_touchTriggered && _touchHoldTimer >= touchHoldDuration)
                {
                    _touchTriggered = true;
                    Toggle();
                }
            }
            else
            {
                _touchHoldTimer  = 0f;
                _touchTriggered  = false;
            }
        }

        void Toggle()
        {
            if (consolePanel == null) return;

            bool next = !consolePanel.gameObject.activeSelf;
            consolePanel.gameObject.SetActive(next);
        }
    }
}
