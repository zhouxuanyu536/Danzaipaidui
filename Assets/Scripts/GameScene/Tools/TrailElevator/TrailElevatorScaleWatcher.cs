using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrailElevatorScaleWatcher : MonoBehaviour
{
    private Material ShaderMaterial;

    private void Start()
    {
        ShaderMaterial = GetComponent<Renderer>().sharedMaterials.FirstOrDefault();
    }
}
