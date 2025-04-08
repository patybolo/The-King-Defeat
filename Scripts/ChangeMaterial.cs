using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public Material []materials;
    public SkinnedMeshRenderer skinnedMeshRendererBody,skinnedMeshRendererHead,skinnedMeshRendererJaw;

    void Start()
    {
        int randomIndex = Random.Range(0, materials.Length);
        skinnedMeshRendererBody.material = materials[randomIndex];
        skinnedMeshRendererHead.material = materials[randomIndex];
        skinnedMeshRendererJaw.material = materials[randomIndex];
    }
}
