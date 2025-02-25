﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
//[ExecuteInEditMode]
public class LiquidEffect : MonoBehaviour
{
    public float viscocity;
    public float delta_x = 0.1f;
    public float delta_t = 1.0f / 60;
    public float bound_size = 0.1f;

    public Material screenMat;
    public Material advectMat;
    public Material dyeMat;
    public Material jacobiMat;
    public Material forceMat;
    public Material divergenceMat;
    public Material gradientSubtractionMat;
    public Material boundaryMat;
    public Material vorticityMat;
    public Material impluseMat;

    RenderTexture[] pressureTex;
    RenderTexture[] velocityTex;
    RenderTexture[] densityTex;
    RenderTexture[] temperatureTex;
    RenderTexture divergenceTex;

    RenderTexture advectPass;
    RenderTexture divergencePass;
    RenderTexture densityPass;
    RenderTexture gradientPass;

    public bool isStart = false;

    private void Start()
    {
        advectPass = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        divergencePass = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        densityPass = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        gradientPass = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

        pressureTex = new RenderTexture[2];
        pressureTex[0] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RGFloat);
        pressureTex[1] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RGFloat);

        velocityTex = new RenderTexture[2];
        velocityTex[0] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RGFloat);
        velocityTex[1] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RGFloat);

        densityTex = new RenderTexture[2];
        densityTex[0] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        densityTex[1] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

        divergenceTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RGFloat);
        divergenceTex.filterMode = FilterMode.Point;

        temperatureTex = new RenderTexture[2];
        temperatureTex[0] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        temperatureTex[1] = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

        //Graphics.Blit(null, velocityTex[0], screenMat);

    }

    private void FixedUpdate()
    {
        if (isStart)
            RenderPass();
    }
    public bool isRun = false;
    float cout = 0;
    float time = 1;
    void StartRun()
    {
        if (isRun)
        {
            cout += Time.deltaTime;
            if (cout >= time)
                isRun = false;

            float i = cout / time;
            float size = 0.1f;

            float _x = i;

            float _y = Random.Range(0.0f, 0.1f);
            Dye(velocityTex[0], velocityTex[1], _x, _y, size);
            Swap(velocityTex);
            Dye(densityTex[0], densityTex[1], _x, _y, size);
            Swap(densityTex);

            Dye(velocityTex[0], velocityTex[1], _x, 1 - _y, size);
            Swap(velocityTex);
            Dye(densityTex[0], densityTex[1], _x, 1 - _y, size);
            Swap(densityTex);


        }
        else cout = 0;
    }
    void RenderPass()
    {
        if (!CheckAlreadyMaterials()) return;

        //advect
        Advect(velocityTex[0], velocityTex[0], velocityTex[1]);
        Advect(velocityTex[0], temperatureTex[0], temperatureTex[1]);
        Advect(velocityTex[0], densityTex[0], densityTex[1]);

        Swap(velocityTex);
        Swap(temperatureTex);
        Swap(densityTex);


        advectPass = velocityTex[0];
        //----------------------------------------------

        Force(velocityTex[0], velocityTex[1], 0.5f, 0.5f, 0.0f, 0.1f);

        float f = 0.01f;
        for (int i = 0; i < 10; i++)
        {
            float _x = (float)i * 0.1f;

            ApplyImpulse(temperatureTex[0], temperatureTex[1], new Vector2(_x, 0.0f), 0.05f, f);
            ApplyImpulse(densityTex[0], densityTex[1], new Vector2(_x, 0.0f), 0.05f, f);
            Swap(temperatureTex);
            Swap(densityTex);
        }

        //if(isStart)
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition;
            float _x = pos.x / (float)densityTex[0].width;
            float _y = pos.y / (float)densityTex[0].height;

            Dye(velocityTex[0], velocityTex[1], _x, _y, 0.05f);
            Swap(velocityTex);
            Dye(densityTex[0], densityTex[1], _x, _y, 0.05f);
            Swap(densityTex);

        }

        StartRun();

        //-------------------------------------------------

        //divergence
        Divergence(velocityTex[0], divergenceTex);
        divergencePass = divergenceTex;

        ClearSurface(pressureTex[0]);
        //jacobis
        for (int i = 0; i < 50; ++i)
        {
            Jacobi(pressureTex[0], divergencePass, pressureTex[1]);
            Swap(pressureTex);
        }

        //gradient
        GradienSubtract(pressureTex[0], velocityTex[0], velocityTex[1]);
        GradienSubtract(pressureTex[0], velocityTex[0], gradientPass);//debug
        Swap(velocityTex);

        //-------------------------------------------------

        ////boundary
        Boundary(velocityTex[0], velocityTex[1], bound_size);
        Swap(velocityTex);
        Boundary(densityTex[0], densityTex[1], bound_size);
        Swap(densityTex);

        ////Vorticity
        Vorticity(velocityTex[0], velocityTex[1]);
        Swap(velocityTex);

        densityPass = densityTex[0];

        //RenderTexture.ReleaseTemporary(pass);
    }
    void Vorticity(RenderTexture source, RenderTexture dest)
    {
        vorticityMat.SetTexture("_v", source);
        Graphics.Blit(null, dest, vorticityMat);
    }
    void ApplyImpulse(RenderTexture source, RenderTexture dest, Vector2 pos, float radius, float val)
    {
        impluseMat.SetVector("_Point", pos);
        impluseMat.SetFloat("_Radius", radius);
        impluseMat.SetFloat("_Fill", val);
        impluseMat.SetTexture("_Source", source);

        Graphics.Blit(null, dest, impluseMat);
    }
    void Dye(RenderTexture source, RenderTexture dest, float x, float y, float size)
    {
        dyeMat.SetTexture("_colorField", source);
        dyeMat.SetVector("_impulse_pos", new Vector4(x, y, 0, 0));
        dyeMat.SetFloat("_rho", size);
        Graphics.Blit(null, dest, dyeMat);
    }
    void Boundary(RenderTexture source, RenderTexture dest, float _offset)
    {
        boundaryMat.SetTexture("_x", source);
        boundaryMat.SetFloat("_dx", _offset);
        Graphics.Blit(null, dest, boundaryMat);
    }
    void GradienSubtract(RenderTexture p, RenderTexture v, RenderTexture dest)
    {
        gradientSubtractionMat.SetTexture("_p", p);
        gradientSubtractionMat.SetTexture("_w", v);
        Graphics.Blit(null, dest, gradientSubtractionMat);
    }
    void Jacobi(RenderTexture p, RenderTexture d, RenderTexture dest)
    {
        float alpha = Mathf.Pow(delta_x, 2) / viscocity * delta_t;
        float rbeta = 1.0f / (4.0f + alpha);

        jacobiMat.SetTexture("_x", p);
        jacobiMat.SetTexture("_b", d);
        jacobiMat.SetFloat("_dx", delta_x);
        jacobiMat.SetFloat("_alpha", alpha);
        jacobiMat.SetFloat("_rbeta", rbeta);
        Graphics.Blit(null, dest, jacobiMat);
    }
    void Divergence(RenderTexture source, RenderTexture dest)
    {
        divergenceMat.SetTexture("_w", source);
        Graphics.Blit(null, divergenceTex, divergenceMat);
    }
    void Advect(RenderTexture v, RenderTexture colorField, RenderTexture dest)
    {
        advectMat.SetTexture("_v", v);
        advectMat.SetTexture("_x", colorField);
        Graphics.Blit(null, dest, advectMat);
    }
    void Force(RenderTexture source, RenderTexture dest, float x, float y, float forcex, float forcey)
    {
        forceMat.SetTexture("_v", source);
        forceMat.SetVector("_impulse_pos", new Vector4(x, y, 0, 0));
        forceMat.SetVector("_force", new Vector4(forcex, forcey, 0, 0));
        Graphics.Blit(null, dest, forceMat);
    }
    bool CheckAlreadyMaterials()
    {
        if (screenMat == null) return false;
        if (advectMat == null) return false;
        if (dyeMat == null) return false;
        if (jacobiMat == null) return false;
        if (forceMat == null) return false;
        if (divergenceMat == null) return false;
        if (gradientSubtractionMat == null) return false;
        if (boundaryMat == null) return false;
        if (vorticityMat == null) return false;
        return true;
    }
    void Swap(RenderTexture[] texs)
    {
        RenderTexture temp = texs[0];
        texs[0] = texs[1];
        texs[1] = temp;
    }
    void ClearSurface(RenderTexture surface)
    {
        Graphics.SetRenderTarget(surface);
        GL.Clear(false, true, new Color(0, 0, 0, 0));
        Graphics.SetRenderTarget(null);
    }
    public bool isDebug = false;
    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), densityPass);
        if (isDebug)
        {
            GUI.DrawTexture(new Rect(0, 0, 100, 100), advectPass);
            GUI.DrawTexture(new Rect(0, 100, 100, 100), divergencePass);
            GUI.DrawTexture(new Rect(0, 200, 100, 100), gradientPass);
            GUI.DrawTexture(new Rect(0, 300, 100, 100), densityPass);
            GUI.Label(new Rect(0, 0, 100, 100), "advect");
            GUI.Label(new Rect(0, 100, 100, 100), "divergence");
            GUI.Label(new Rect(0, 200, 100, 100), "gradient");
            GUI.Label(new Rect(0, 300, 100, 100), "density");
        }
        if (!isStart)
            if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "Play"))
            {
                isStart = true;
                isRun = true;

            }

    }
}
