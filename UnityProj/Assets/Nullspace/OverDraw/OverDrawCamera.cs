using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{
    public class OverDrawCamera : MonoBehaviour
    {
        public Shader replaceShder;
        public class DataState
        {
            public int sceneOn;
            public int uiOn;
            public int anaOn;

            public DataState()
            {
                sceneOn = 0;
                uiOn = 0;
                anaOn = 0;
            }
        }

        public class DataDisplays
        {
            public static string[] scenesDisplayse = new string[] { "开启 场景", "关闭 场景" };
            public static string[] uiDisplayes = new string[] { "开启 UI", "关闭 场景" };
            public static string[] anaDisplayes = new string[] { "开启 统计", "关闭 场景" };
        }

        private float m_unit = 0.01f;
        private int m_propertyId = -1;
        private OverDrawAnalysis m_uiAna = null;
        private DataState m_state = new DataState();

        private Camera mSceneCamera = null;
        private Camera mUICamera = null;
        private Rect windowRect;
        private ComputeShader mComputeShader;
        private bool isShaderValid;
        private void Awake()
        {
            isShaderValid = replaceShder != null;
            m_propertyId = Shader.PropertyToID("_OverdrawUnit");
            mComputeShader = Resources.Load<ComputeShader>("OverdrawCount");
            windowRect = new Rect(Screen.width / 2 - 300, Screen.height / 2 - 100, 600, 200);
        }

        public void ToggleScaneReplaceShader(bool toggle)
        {
            if (mSceneCamera == null)
            {
                mSceneCamera = gameObject.GetComponent<Camera>();
            }
            if (mSceneCamera != null)
            {
                if (toggle)
                {
                    mSceneCamera.SetReplacementShader(replaceShder, "");
                }
                else
                {
                    mSceneCamera.SetReplacementShader(null, null);
                }
            }
        }

        public void ToggleUIReplaceShader(bool toggle)
        {
            if (mUICamera == null)
            {
                Camera[] cams = Camera.allCameras;
                foreach (Camera cam in cams)
                {
                    if (cam.name == "UICamera")
                    {
                        mUICamera = cam;
                        break;
                    }
                }
                if (m_uiAna == null && mUICamera != null)
                {
                    m_uiAna = mUICamera.gameObject.AddComponent<OverDrawAnalysis>();
                }
            }
            if (mUICamera != null)
            {
                if (toggle)
                {
                    mUICamera.SetReplacementShader(replaceShder, "");
                }
                else
                {
                    mUICamera.SetReplacementShader(null, null);
                }
            }
        }

        public void Analysis()
        {
            m_state.anaOn = m_state.anaOn == 0 ? 1 : 0;
            bool f = m_state.anaOn == 1;
            ToggleScaneReplaceShader(f);
            ToggleUIReplaceShader(f);
            if (m_uiAna != null)
            {
                m_uiAna.EnableAnalysis(f);
            }
            DataAna.Reset();
        }


        public void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, DoMyWindow, "OverDraw检测");
            if (m_state.anaOn == 0)
            {
                if (GUILayout.Button(DataDisplays.scenesDisplayse[m_state.sceneOn], GUILayout.Width(200), GUILayout.Height(40)))
                {
                    m_state.sceneOn = m_state.sceneOn == 0 ? 1 : 0;
                    ToggleScaneReplaceShader(m_state.sceneOn == 1);
                }
                if (GUILayout.Button(DataDisplays.uiDisplayes[m_state.uiOn], GUILayout.Width(200), GUILayout.Height(40)))
                {
                    m_state.uiOn = m_state.uiOn == 0 ? 1 : 0;
                    ToggleUIReplaceShader(m_state.uiOn == 1);
                }
            }

        }

        private void DoMyWindow(int windowID)
        {
            if (GUILayout.Button("关闭此窗口", GUILayout.Width(100), GUILayout.Height(40)))
            {
                if (m_uiAna != null)
                {
                    m_state.anaOn = 1;
                    Analysis();
                    m_uiAna.EnableAnalysis(false);
                }
                this.enabled = false;
            }
            if (GUILayout.Button(DataDisplays.anaDisplayes[m_state.anaOn], GUILayout.Width(200), GUILayout.Height(40)))
            {
                Analysis();
            }
            float test = GUILayout.HorizontalSlider(m_unit, 0.01f, 0.1f);
            test = (float)Math.Round((double)test, 2);
            if (m_unit != test)
            {
                m_unit = test;
                DataAna.Count = (int)(1.0f / m_unit);
                Shader.SetGlobalFloat(m_propertyId, m_unit);
            }
            var oldColor = GUI.skin.label.normal.textColor;
            var oldSize = GUI.skin.label.fontSize;
            GUI.skin.label.normal.textColor = Color.red;
            GUI.skin.label.fontSize = 24;
            GUILayout.Label("shader is " + (isShaderValid ? "valid" : "invalid"), GUILayout.Width(200), GUILayout.Height(40));
            GUILayout.Label(string.Format("unit={4},avg={0},max={1},ps={2},pix={3}", DataAna.Avg, DataAna.Max, DataAna.PS, DataAna.Pixels, m_unit), GUILayout.Width(600), GUILayout.Height(40));
            GUI.skin.label.normal.textColor = oldColor;
            GUI.skin.label.fontSize = oldSize;
        }
    }

}
