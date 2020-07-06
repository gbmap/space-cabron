using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SC
{

    [CustomEditor(typeof(Shitesizer))]
    public class ShitesizerEditor : Editor
    {
        Material mat;

        Shitesizer s;
        private void OnEnable()
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            s = target as Shitesizer;
        }

        private void OnDisable()
        {
            DestroyImmediate(mat);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            //DrawWave(Vector2.one * 200f, s.Buffer, mat);

            EditorGUILayout.Space();

            //DrawWave(Vector2.one * 200f, Mathf.Sin, mat);
        }

        public static void DrawWave(Vector2 size, float[] data, Material mat)
        {
            Rect rect = GUILayoutUtility.GetRect(10, size.x, size.y, size.y);
            //if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();
                GL.Clear(true, false, Color.black);

                mat.SetPass(0);
                GLDrawBackground(rect);
                GLDrawWaveData(rect, data);

                GL.PopMatrix();
                GUI.EndClip();
            }
        }

        public static  void DrawWave(Vector2 size, Func<double, double, double> Func, Material mat)
        {
            Rect rect = GUILayoutUtility.GetRect(10, size.x, size.y, size.y);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();
                GL.Clear(true, false, Color.black);

                mat.SetPass(0);
                GLDrawBackground(rect);

                GLDrawWaveFunc(rect, Func);

                GL.PopMatrix();
                GUI.EndClip();
            }
        }

        private static void GLDrawWaveData(Rect rect, float[] data)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.gray);
            GL.Vertex3(0f, rect.height * 0.5f, 0f);
            GL.Vertex3(rect.width, rect.height * 0.5f, 0f);
            GL.End();
            if (data != null && data.Length > 0)
            {
                float yMax = data.OrderByDescending(d => d).First();
                //data = data.Select(d => ((d / yMax) - 0.5f) * yMax ).ToArray();

                float wh = rect.height / 2f;

                GL.Begin(GL.LINE_STRIP);
                GL.Color(Color.green);

                for (int i = 0; i < data.Length; i++)
                {
                    float x = ((float)i) / (data.Length - 1);
                    GL.Vertex3(x * rect.width,
                        rect.height * 0.5f - data[i] * wh,
                        0f);
                }

                GL.End();
            }
        }

        private static void GLDrawBackground(Rect rect)
        {
            // background
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(rect.width, 0, 0);
            GL.Vertex3(rect.width, rect.height, 0);
            GL.Vertex3(0, rect.height, 0);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(Color.gray);

            for (float x = 0; x < rect.width; x+=rect.width/10f)
            {
                GL.Vertex3(x, 0f, 0f);
                GL.Vertex3(x, rect.height, 0f);
            }

            for (float y = 0; y < rect.height; y += rect.height / 10f)
            {
                GL.Vertex3(0f, y, 0f);
                GL.Vertex3(rect.width, y, 0f);
            }

            GL.End();
        }

        private static void GLDrawWaveFunc(Rect rect, Func<double, double, double> WaveFunc)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.gray);
            GL.Vertex3(0f, rect.height * 0.5f, 0f);
            GL.Vertex3(rect.width, rect.height * 0.5f, 0f);
            GL.End();

            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.green);

            for (float t = 0; t < rect.width; t+=rect.width/200f)
            {
                GL.Vertex3(t,
                    rect.height*0.5f-((float)WaveFunc(1.0, (t / rect.width))*rect.height/2.0f),
                    0f);
            }

            GL.End();
        }

    }
}