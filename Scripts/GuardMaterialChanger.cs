using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMaterialChanger : MonoBehaviour
{
    public Material[] mats;
    public SkinnedMeshRenderer armor_low, body_low, clothe_low;
    void Start()
    {
        int matIndex = Random.Range(0, mats.Length);
        armor_low.material = mats[matIndex];
        body_low.material = mats[matIndex];
        clothe_low.material = mats[matIndex];
    }
}
