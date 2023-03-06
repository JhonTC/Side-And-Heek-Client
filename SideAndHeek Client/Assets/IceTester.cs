using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTester : MonoBehaviour
{
    public Material mat;

    Texture2D generatedTex;

    public Vector3[] icePositions;

    private void Start()
    {
        generatedTex = new Texture2D(1, icePositions.Length);
    }

    void Update()
    {
        for (int i = 0; i < icePositions.Length; i++)
        {
            generatedTex.SetPixel(0, i, new Color(icePositions[i].x / 2550, icePositions[i].y / 2550, icePositions[i].z / 2550));
            generatedTex.SetPixel(1, i, new Color((icePositions[i].x < 0) ? 0 : 1, 
                (icePositions[i].y / 10000 < 0) ? 0 : 1, 
                (icePositions[i].z / 10000 < 0) ? 0 : 1));
        }

        mat.SetTexture("IcePositions", generatedTex);
    }
}
