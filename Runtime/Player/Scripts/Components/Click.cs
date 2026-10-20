using Module5.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Module5.DI;
namespace Module5.Player
{
    public class Click : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler
    {
        //[Inject] IAssetDataBaseManager AssetDataBaseManager { get; set; }
        [Inject] public IEventManager EventManager { get; set; }
        [Inject] public IProgressBarManager ProgressBarManager {  get; set; }

        [Header("功能開關")]
        [Tooltip("是否啟用長壓功能。若關閉，則任何按壓放開後皆視為一般點擊。")]
        public bool enableLongTap = true;

        [Header("時間設定")]
        public float longTapThreshold = 0.8f;
        public float delayBeforeShow = 0.2f;

        [Header("防連點設定")]
        [Tooltip("點擊冷卻時間 (秒)，防止使用者過快連點觸發事件。若設為 0 則不限制。")]
        public float clickCooldown = 0.3f;

        [Header("事件")]
        public UnityEvent onClick;
        public UnityEvent onLongTap;

        private Coroutine tapCoroutine;
        private float currentTimer = 0f;
        private bool isPointerDown = false;
        private float lastClickTime = -100f;

        public void ResetCooldown()
        {
            lastClickTime = -100f;
        }

        // 當手指移動時，同步更新進度條位置
        public void OnDrag(PointerEventData eventData)
        {
            // 只有在啟用長壓且正在按壓時才更新進度條位置
            if (enableLongTap && isPointerDown && ProgressBarManager != null)
            {
                ProgressBarManager.UpdatePosition(eventData.position);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopExistingCoroutine();
            isPointerDown = true;

            // 只有在啟用長壓時才初始化進度條位置
            if (enableLongTap && ProgressBarManager != null)
            {
                ProgressBarManager.UpdatePosition(eventData.position);
            }

            tapCoroutine = StartCoroutine(TrackPressTime());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPointerDown)
            {
                // 判斷邏輯：
                // 1. 如果長壓功能被關閉 -> 執行點擊
                // 2. 如果時間超過閾值 -> 執行長壓
                // 3. 其他情況 -> 執行點擊
                if (enableLongTap && currentTimer >= longTapThreshold)
                {
                    lastClickTime = Time.unscaledTime;
                    M5Event m5Event = new ObjectEvents.LongTapEvent(this, this);
                    if (EventManager != null) EventManager.TriggerEvent(m5Event);

                    onLongTap.Invoke();
                }
                else
                {
                    float now = Time.unscaledTime;
                    if (clickCooldown <= 0f || now - lastClickTime >= clickCooldown)
                    {
                        lastClickTime = now;
                        M5Event m5Event = new ObjectEvents.ClickEvent(this, this);
                        if (EventManager != null) EventManager.TriggerEvent(m5Event);

                        onClick.Invoke();
                    }
                    else
                    {
                        Debug.Log($"[Click] 連點被忽略 (Cooldown: {clickCooldown}s): {gameObject.name}");
                    }
                }
            }
            StopExistingCoroutine();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopExistingCoroutine();
        }

        private IEnumerator TrackPressTime()
        {
            currentTimer = 0f;
            while (isPointerDown)
            {
                currentTimer += Time.deltaTime;

                // 只有在啟用長壓時，才處理 UI 進度條邏輯
                if (enableLongTap && currentTimer >= delayBeforeShow && ProgressBarManager != null)
                {
                    ProgressBarManager.Show();
                    float range = longTapThreshold - delayBeforeShow;
                    float progress = range > 0 ? (currentTimer - delayBeforeShow) / range : 1f;
                    ProgressBarManager.SetProgress(progress);
                }

                yield return null;
            }
        }

        private void StopExistingCoroutine()
        {
            isPointerDown = false;
            if (tapCoroutine != null)
            {
                StopCoroutine(tapCoroutine);
                tapCoroutine = null;
            }

            // 統一呼叫 Hide，ProgressBarManager 內部會處理
            if (ProgressBarManager != null) ProgressBarManager.Hide();
        }
    }
}