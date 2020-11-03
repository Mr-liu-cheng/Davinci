using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Image))]
public class UGUIAnimation : MonoBehaviour
{
    private Image ImageSource;
    private Sprite _lastImage;
    private int mCurFrame = 0;
    private float mDelta = 0;

    public float FPS = 5;
    public List<Sprite> SpriteFrames;
    public bool IsPlaying = false;
    public bool Foward = true;
    public bool AutoPlay = false;
    public bool Loop = false;

    #region 属性方法
    public int FrameCount
    {
        get
        {
            return SpriteFrames.Count;
        }
    }
    public Sprite LastImage
    {
        get
        {
            return _lastImage;
        }

        set
        {
            _lastImage = value;
        }
    }
    #endregion

    void Awake()
    {
        ImageSource = GetComponent<Image>();
    }

    void Start()
    {
        if (AutoPlay)
        {
            if(Foward)
            {
              Play();
            }
            else
            {
              PlayReverse();
            }
          
        }
    }
    void Update()
    {
        if (!IsPlaying || 0 == FrameCount)
        {
            return;
        }
        mDelta += Time.deltaTime;

        if (mDelta > 1 / FPS)
        {
            mDelta = 0;

            SetSprite(mCurFrame);

            if (Foward)
            {
                mCurFrame++;
            }
            else
            {
                mCurFrame--;
            }

            if (mCurFrame >= FrameCount)
            {
                if (Loop)
                {
                    mCurFrame = 0;
                }
                else
                {
                    Completion();
                    return;
                }
            }
            else if (mCurFrame < 0)
            {
                if (Loop)
                {
                    mCurFrame = FrameCount - 1;
                }
                else
                {
                    Completion();
                    return;
                }
            }
        }
    }
    private void SetSprite(int idx)
    {
        ImageSource.sprite = SpriteFrames[idx];
        //该部分为设置成原始图片大小，如果只需要显示Image设定好的图片大小，注释掉该行即可。
        //ImageSource.SetNativeSize();
    }
    public void Play()
    {
        mCurFrame = 0;
        IsPlaying = true;
        Foward = true;
    }

    public void PlayReverse()
    {
        mCurFrame = SpriteFrames.Count - 1;
        IsPlaying = true;
        Foward = false;
    }
    public void Pause()
    {
        IsPlaying = false;
    }

    public void Resume()
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
        }
    }

    public void Stop()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        IsPlaying = false;
    }

    public void Rewind()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        Play();
    }

    public void Completion()
    {
        IsPlaying = false;
        if(LastImage!=null)
        {
            ImageSource.sprite = LastImage;
        }
    }
}