﻿using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MiniWeChat
{
    [RequireComponent(typeof(RectTransform))]
    public class GIFImage : MonoBehaviour
    {
        public string loadingGifPath;
        public float speed = 1;

        private RectTransform _rectTrans;
        private static Dictionary<string, List<Texture2D>> _gifFrameDict = new Dictionary<string, List<Texture2D>>();
        private static HashSet<string> _finishGIFs = new HashSet<string>();

        void Awake()
        {
            StartCoroutine(LoadGifFrames());
        }

        private IEnumerator LoadGifFrames()
        {
            if (_gifFrameDict.ContainsKey(loadingGifPath))
            {
                yield break;
            }

            List<Texture2D> gifFrames = new List<Texture2D>();
            _gifFrameDict[loadingGifPath] = gifFrames;

            _rectTrans = GetComponent<RectTransform>();
            var gifImage = System.Drawing.Image.FromFile(Application.dataPath + "/Raw/Image/" + loadingGifPath);
            var dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);
            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);
                var frameTexture = new Texture2D(gifImage.Height, gifImage.Height);

                var frame = new Bitmap(gifImage.Width, gifImage.Height);
                System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);
                ImageConverter converter = new ImageConverter();
                frameTexture.LoadImage((byte[])converter.ConvertTo(frame, typeof(byte[])));

                gifFrames.Add(frameTexture);
                //yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.06f)) ;
                yield return null;
            }

            _finishGIFs.Add(loadingGifPath);
        }

        void OnGUI()
        {
            List<Texture2D> gifFrames = _gifFrameDict[loadingGifPath];

            if (IsFinishedLoading())
            {
                GUI.DrawTexture(new Rect(transform.position.x, Screen.height - transform.position.y, gifFrames[0].width, gifFrames[0].height), gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count]);
            }
            else
            {
                GUI.DrawTexture(new Rect(transform.position.x, Screen.height - transform.position.y, gifFrames[0].width, gifFrames[0].height), gifFrames[0]);
            }
        }

        private bool IsFinishedLoading()
        {
            return _finishGIFs.Contains(loadingGifPath);
        }
    }
}

