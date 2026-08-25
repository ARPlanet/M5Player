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
        [Inject] IEventManager EventManager { get; set; }
        [Inject] IProgressBarManager ProgressBarManager {  get; set; }

        [Header("功能開關")]
        [Tooltip("是否啟用長壓功能。若關閉，則任何按壓放開後皆視為一般點擊。")]
        public bool enableLongTap = true;

        [Header("時間設定")]
        public float longTapThreshold = 0.8f;
        public float delayBeforeShow = 0.2f;

        [Header("事件")]
        public UnityEvent onClick;
        public UnityEvent onLongTap;

        private Coroutine tapCoroutine;
        private float currentTimer = 0f;
        private bool isPointerDown = false;

        // 當手指移動時，同步更新進度條位置
        public void OnDrag(PointerEventData eventData)
        {
            // 只有在啟用長壓且正在按壓時才更新進度條位置
            if (enableLongTap && isPointerDown)
            {
                ProgressBarManager.UpdatePosition(eventData.position);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopExistingCoroutine();
            isPointerDown = true;

            // 只有在啟用長壓時才初始化進度條位置
            if (enableLongTap)
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
                    M5Event m5Event = ObjectEvents.CreateLongTapEvent(this, this);
                    EventManager.TriggerEvent(m5Event);

                    onLongTap.Invoke();
                }
                else
                {
                    M5Event m5Event = ObjectEvents.CreateClickEvent(this, this);
                    EventManager.TriggerEvent(m5Event);

                    onClick.Invoke();
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
                if (enableLongTap && currentTimer >= delayBeforeShow)
                {
                    ProgressBarManager.Show();
                    float range = longTapThreshold - delayBeforeShow;
                    float progress = (currentTimer - delayBeforeShow) / range;
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