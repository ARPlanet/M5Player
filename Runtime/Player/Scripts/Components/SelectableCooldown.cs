using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module5.Player
{
    /// <summary>
    /// 通用 UI 點擊防連點 / 冷卻保護元件。
    /// 可掛載於任何 Selectable (如 Button, Toggle 等) 上。
    /// 點擊後會短暫將 interactable 設為 false，防止高頻連點造成事件暴衝或邏輯衝突。
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class SelectableCooldown : MonoBehaviour, IPointerClickHandler, ISubmitHandler
    {
        [Tooltip("點擊後的冷卻時間 (秒)")]
        [SerializeField] private float cooldown = 0.3f;
        public float Cooldown { get => cooldown; set => cooldown = value; }

        private Selectable _selectable;
        private Coroutine _cooldownCoroutine;
        private Button _targetButton;

        public Selectable TargetSelectable
        {
            get
            {
                if (_selectable == null) _selectable = GetComponent<Selectable>();
                return _selectable;
            }
            set
            {
                if (_selectable != value)
                {
                    UnbindButtonListener();
                    _selectable = value;
                    BindButtonListener();
                }
            }
        }

        private void Awake()
        {
            if (_selectable == null)
            {
                _selectable = GetComponent<Selectable>();
            }
            BindButtonListener();
        }

        private void OnEnable()
        {
            BindButtonListener();
            // 物件重新啟用時重置，確保絕不永久卡在 false
            ResetCooldown();
        }

        private void OnDisable()
        {
            UnbindButtonListener();
            // 物件停用時中止協程並恢復狀態
            ResetCooldown();
        }

        private void OnDestroy()
        {
            UnbindButtonListener();
        }

        private void BindButtonListener()
        {
            if (TargetSelectable is Button button && _targetButton != button)
            {
                UnbindButtonListener();
                _targetButton = button;
                _targetButton.onClick.AddListener(OnButtonClicked);
            }
        }

        private void UnbindButtonListener()
        {
            if (_targetButton != null)
            {
                _targetButton.onClick.RemoveListener(OnButtonClicked);
                _targetButton = null;
            }
        }

        private void OnButtonClicked()
        {
            if (!isActiveAndEnabled || TargetSelectable == null || cooldown <= 0f) return;
            TriggerCooldown(waitForNextFrame: false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 若為 Button，已由 onClick 事件精確觸發，此處略過以防元件執行順序干擾
            if (TargetSelectable is Button) return;

            // 僅處理滑鼠左鍵或觸控 (eventData 為 null 時放行，以支援測試與基本觸發)
            if (eventData != null && eventData.button != PointerEventData.InputButton.Left) return;

            // 若目前不可互動或已在冷卻中，則不重複觸發
            if (!isActiveAndEnabled || TargetSelectable == null || !TargetSelectable.IsInteractable() || cooldown <= 0f) return;

            TriggerCooldown(waitForNextFrame: true);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (TargetSelectable is Button) return;
            if (!isActiveAndEnabled || TargetSelectable == null || !TargetSelectable.IsInteractable() || cooldown <= 0f) return;

            TriggerCooldown(waitForNextFrame: true);
        }

        /// <summary>
        /// 觸發冷卻鎖定（亦可供外部代碼主動調用）
        /// </summary>
        public void TriggerCooldown(bool waitForNextFrame = false)
        {
            if (!isActiveAndEnabled || TargetSelectable == null || cooldown <= 0f) return;

            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
            }
            _cooldownCoroutine = StartCoroutine(CooldownRoutine(waitForNextFrame));
        }

        private IEnumerator CooldownRoutine(bool waitForNextFrame)
        {
            if (waitForNextFrame)
            {
                yield return null;
            }

            if (TargetSelectable != null)
            {
                TargetSelectable.interactable = false;
            }

            // 使用 Realtime，即使遊戲暫停 (Time.timeScale = 0) 也能準確解鎖
            yield return new WaitForSecondsRealtime(cooldown);

            if (TargetSelectable != null)
            {
                TargetSelectable.interactable = true;
            }
            _cooldownCoroutine = null;
        }

        /// <summary>
        /// 重置冷卻狀態並恢復可互動
        /// </summary>
        public void ResetCooldown()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }
            if (TargetSelectable != null)
            {
                TargetSelectable.interactable = true;
            }
        }
    }
}
