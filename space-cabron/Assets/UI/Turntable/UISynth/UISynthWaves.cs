using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UISynthWaves : MonoBehaviour
{
    /*
    public Synth synth;
    public Material lineRendering;

    RectTransform[] cells;

    private void Awake()
    {
        cells = new RectTransform[transform.childCount];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }

        RenderPipelineManager.endCameraRendering += OnEndRender;
    }

    
    // Update is called once per frame
    void Update()
    {
        double a = synth.Envelope.GetAmplitude(Time.time);
        float t = 0f;
        float tIncrement = 1f / cells.Length;
        
        for (int i = 0; i < cells.Length; i++)
        {
            float ss = synth.Instrument.Sample(t) * (float)a;

            RectTransform c = cells[i];

            bool b0 = ss >= 0f;
            c.pivot = new Vector2(0.5f, ss >= 0f ? 0f : 1f);
            c.localPosition = new Vector2(c.localPosition.x, 0f);
            c.sizeDelta = new Vector2(c.sizeDelta.x, 0.5f * 100f * Mathf.Abs(ss));

            t += tIncrement;
        }
    }

    private void OnEndRender(ScriptableRenderContext arg1, Camera arg2)
    {
    }


    private void OnRenderObject()
    {
        var buffer = synth.PrevData;

        GL.PushMatrix();
        lineRendering.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINE_STRIP);
        GL.Color(Color.red);

        for (int i = 0; i < buffer.Length; i++)
        {
            float y = buffer[i];
            float x = ((float)i) / buffer.Length;
            y += 0.5f;
            GL.Vertex(new Vector3(x, y, 0f));
        }
        GL.End();
        GL.PopMatrix();
    }
    */
}
