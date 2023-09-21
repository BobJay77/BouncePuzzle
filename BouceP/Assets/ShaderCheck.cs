using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShaderCheck : MonoBehaviour
{
    public GameObject vfx;
    public TMP_Text text;
    private void Update()
    {
        Material material = new Material(Shader.Find("GAP/AdditiveMobileDistortionScroll"));

        if (material)
        {
            text.text = "SHADER IS HERE";
        }
        else
        {
            text.text = "NULL";
        }
    }
}
