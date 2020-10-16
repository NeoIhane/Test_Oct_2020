using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
//[ExecuteInEditMode]
public class LiquidEffect : MonoBehaviour
{
    public float viscocity;
    public float delta_x = 0.1f;
    public float delta_t = 1.0f / 60;

    public Material screenMat;
    public Material advectMat;
    public Material dyeMat;
    public Material jacobiMat;
    public Material forceMat;
    public Material divergenceMat;
    public Material gradientSubtractionMat;
    public Material boundaryMat;
    public Material vorticityMat;
    public Material flowMat;
    public Material impluseMat;
    public Material obstacleMat;

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

        pressureTex = new RenderTexture[2];//RenderTexture.GetTemporary(source.width, source.height);
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
    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    RenderTexture pass = RenderTexture.GetTemporary(source.width, source.height);
 
    //    Graphics.Blit(source, pass, screenMat);
    //    screenMat.SetTexture("_MainTex", velocityTex[0]);
    //    Graphics.Blit(pass, destination, screenMat);
    //    RenderTexture.ReleaseTemporary(pass);
    //}
    private void FixedUpdate()
    {
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
            float force = 1.5f;


            //ApplyImpulse(temperatureTex[0], temperatureTex[1], new Vector2(0.5f, 0.0f), 0.1f, 10);
            ApplyImpulse(densityTex[0], densityTex[1], new Vector2(0.5f, 0.0f), 0.1f, 1);
            //Swap(temperatureTex);
            Swap(densityTex);

            //ApplyImpulse(densityTex[0], densityTex[1], new Vector2(i, Mathf.Sin(cout)*0.1f), 0.1f, 1);
            //Swap(densityTex);

            Force(velocityTex[0], velocityTex[1], i, 0, 0.0f, -1 * force);
            Swap(velocityTex);

            Force(velocityTex[0], velocityTex[1], 1 - i, 1, 0.0f, 1 * force);
            Swap(velocityTex);


            Dye(densityTex[0], densityTex[1], i, 0, 0.1f);
            Swap(densityTex);

            Dye(densityTex[0], densityTex[1], 1 - i, 1, 0.1f);
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
        Buoyancy(velocityTex[0], temperatureTex[0], densityTex[0], velocityTex[1]);
        Swap(velocityTex);

        //ApplyImpulse(temperatureTex[0], temperatureTex[1], new Vector2(0.5f, 0.0f), 0.1f, 10);
        //ApplyImpulse(densityTex[0], densityTex[1], new Vector2(0.5f, 0.0f), 0.1f, 1);
        //Swap(temperatureTex);
        //Swap(densityTex);

        if(isStart)
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition;
            float _x = pos.x / (float)densityTex[0].width;
            float _y = pos.y / (float)densityTex[0].height;
            /*
            Force(temperatureTex[0], temperatureTex[1], _x, _y);
            Swap(temperatureTex);

            Force(densityTex[0], densityTex[1], _x, _y);
            Swap(densityTex);*/

            //Force(velocityTex[0], velocityTex[1], _x, _y);
            //Swap(velocityTex);

            Dye(velocityTex[0], velocityTex[1], _x, _y, 0.02f);
            Swap(velocityTex);
            Dye(densityTex[0], densityTex[1], _x, _y, 0.02f);
            Swap(densityTex);


            //Force(velocityTex[0], velocityTex[1], _x, _y);
            //Swap(velocityTex);
            //Force(densityTex[0], densityTex[1], _x, _y);
            //Swap(densityTex);


        }

        StartRun();

        //densityPass = densityTex[0];
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
        Swap(velocityTex);
        gradientPass = velocityTex[0];
        //-------------------------------------------------

        //boundary
        Boundary(velocityTex[0], velocityTex[1]);
        Swap(velocityTex);
        Boundary(densityTex[0], densityTex[1]);
        Swap(densityTex);

        //Vorticity(velocityTex[0], velocityTex[1]);
        //Swap(velocityTex);
        //Vorticity(densityTex[0], densityTex[1]);
        //Swap(densityTex);

        Vorticity(velocityTex[0], velocityTex[1]);
        Swap(velocityTex);

        //Buoyancy(velocityTex[0], temperatureTex[0], densityTex[0], velocityTex[1]);
        //Swap(velocityTex);

        //Advect(densityTex[0], densityTex[1]);
        //Swap(densityTex);
        densityPass = densityTex[0];

        //Graphics.Blit(densityTex[0], destination, screenMat);
      
        //RenderTexture.ReleaseTemporary(pass);
    }
    void Obstacle(float r, Vector2 point, RenderTexture dest)
    {
        obstacleMat.SetFloat("_Radius", r);
        obstacleMat.SetVector("_Point", point);
        Graphics.Blit(null, dest, obstacleMat);
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

        Graphics.Blit(null, dest,  impluseMat);
    }
    void Dye(RenderTexture source, RenderTexture dest, float x, float y, float size)
    {
        dyeMat.SetTexture("_colorField", source);
        dyeMat.SetVector("_impulse_pos", new Vector4(x, y, 0, 0));
        dyeMat.SetFloat("_rho", size);
        Graphics.Blit(null, dest, dyeMat);
    }
    void Boundary(RenderTexture source, RenderTexture dest)
    {
        boundaryMat.SetTexture("_x", source);
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
        //float alpha = Mathf.Pow(delta_x, 2) / viscocity * delta_t;
        //float rbeta = 1.0f / (4.0f + alpha);

        jacobiMat.SetTexture("_x", p);
        jacobiMat.SetTexture("_b", d);
        //jacobiMat.SetFloat("_dx", delta_x);
        //jacobiMat.SetFloat("_alpha", alpha);
        //jacobiMat.SetFloat("_rbeta", rbeta);
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
    void Buoyancy(RenderTexture v, RenderTexture t, RenderTexture d, RenderTexture dest)
    {
        flowMat.SetTexture("_Velocity", v);
        flowMat.SetTexture("_Temperature", t);
        flowMat.SetTexture("_Density", d);
        flowMat.SetFloat("_AmbientTemperature", 0);
        flowMat.SetFloat("_TimeStep", 0.25f);
        flowMat.SetFloat("_Sigma", 1);
        flowMat.SetFloat("_Kappa", 0.05f);
        Graphics.Blit(null, dest, flowMat);
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
    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), densityPass);
        //GUI.DrawTexture(new Rect(0, 0, 100, 100), advectPass);
        //GUI.DrawTexture(new Rect(0, 100, 100, 100), divergencePass);
        //GUI.DrawTexture(new Rect(0, 200, 100, 100), gradientPass);
        //GUI.DrawTexture(new Rect(0, 300, 100, 100), densityPass);
        //GUI.Label(new Rect(0, 0, 100, 100), "advect");
        //GUI.Label(new Rect(0, 100, 100, 100), "divergence");
        //GUI.Label(new Rect(0, 200, 100, 100), "gradient");
        //GUI.Label(new Rect(0, 300, 100, 100), "density");

        if (!isStart)
            if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "Play"))
            {
                isStart = true;
                isRun = true;
                
            }

    }
}
