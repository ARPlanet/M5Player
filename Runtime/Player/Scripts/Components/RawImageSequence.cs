using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Module5.Player
{
    public enum SequenceMode
    {
        Once,
        Loop,
        pingpong
    }

    public class RawImageSequence : MonoBehaviour
    {
        [SerializeField] protected RawImage rawImage;
        public RawImage RawImage
        {
            get
            {
                if(rawImage == null)
                {
                    rawImage = GetComponent<RawImage>();
                }
                return rawImage;
            }
        }

        [SerializeField] protected Vector2Int size;
        public Vector2Int Size
        {
            get => size;
            set
            {
                size = value;
                if(enabled) UVAnimate();
            }
        }

        public int frameLength;
        public float speed;
        public SequenceMode mode;
        public bool playAuto = true;

        public bool isPlay = false;
        [SerializeField] protected float timer;
        [SerializeField] protected int frame;

        protected virtual void Start()
        {
            if (playAuto)
            {
                Play();
            }
        }
        private void OnEnable()
        {
            UVAnimate();
        }
        private void OnDisable()
        {
            RawImage.uvRect = new Rect(0, 0, 1, 1);
        }

        protected virtual void Update()
        {
            if (!isPlay) return;
            timer += Time.deltaTime;
            UVAnimate();
        }

        protected virtual void UVAnimate()
        {
            int count = Mathf.FloorToInt(timer / speed);
            int x = 1;
            int y = 1;
            float w = 1f / size.x;
            float h = 1f / size.y;
            switch (mode)
            {
                case SequenceMode.Once:
                {
                    frame = count >= frameLength ? frameLength : count % frameLength;
                    if (frame >= frameLength)
                    {
                        Stop();
                        return;
                    }
                    x = frame % size.x;
                    y = frame / size.x;
                    break;
                }
                case SequenceMode.Loop:
                {
                    frame = count % frameLength;
                    x = frame % size.x;
                    y = frame / size.x;
                    break;
                }
                case SequenceMode.pingpong:
                {
                    frame = count % (frameLength * 2);
                    bool reverse = frame >= frameLength;
                    frame = reverse ? frameLength * 2 - frame - 1 : frame;
                    x = frame % size.x;
                    y = frame / size.x;
                    break;
                }
            }
            RawImage.uvRect = new Rect(x * w, 1 - h - (y * h), w, h);
        }

        public virtual void Play()
        {
            timer -= Time.deltaTime;
            isPlay = true;
        }

        public virtual void Pause()
        {
            isPlay = false;
        }

        public virtual void Stop()
        {
            isPlay = false;
            timer = 0;
        }

        protected virtual void EndPaly()
        {

        }
    }
}