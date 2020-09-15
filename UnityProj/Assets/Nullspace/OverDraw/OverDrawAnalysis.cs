
using UnityEngine;

namespace Nullspace
{
    public class DataAna
    {
        public static int Avg = 0;
        public static int Max = 0;
        public static int PS = 0;
        public static int Pixels = 0;
        public static int Count = 100;

        public static void Reset()
        {
            Avg = 0;
            Max = 0;
            PS = 0;
            Pixels = 0;
        }
    }
    public class OverDrawAnalysis : MonoBehaviour
    {
        private Rect m_rectReadPicture = new Rect(0, 0, 0, 0);
        private Texture2D m_cache = null;
        public void EnableAnalysis(bool enabled)
        {
            this.enabled = enabled;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            int w = source.width;
            int h = source.height;
            if (m_rectReadPicture.width != w || m_rectReadPicture.height != h)
            {
                m_rectReadPicture = new Rect(0, 0, w, h);
                m_cache = new Texture2D(w, h, TextureFormat.RGB24, false);
            }
            RenderTexture.active = source;
            m_cache.ReadPixels(m_rectReadPicture, 0, 0);
            m_cache.Apply();
            RenderTexture.active = null;
            int accumute = 0;
            int total = w * h;
            int max = -1;
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    int r = (int)(m_cache.GetPixel(i, j).r * DataAna.Count + 0.5);
                    accumute += r;
                    if (r > max)
                    {
                        max = r;
                    }
                }
            }
            int avg = (int)(accumute * 1.0f / total + 0.5f);
            DataAna.Avg = avg;
            DataAna.Max = max;
            DataAna.PS = accumute;
            DataAna.Pixels = total;
            Graphics.Blit(source, destination);
        }
    }
}
