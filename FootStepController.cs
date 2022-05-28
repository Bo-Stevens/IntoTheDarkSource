using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class FootStepController : MonoBehaviour
{

    public int framesToSkip = 3;
    int frameCounter = 0;

    [HideInInspector]
    public AudioClip footStepSound;
    // Start is called before the first frame update
    void Start()
    {
        frameCounter = Random.Range(0, framesToSkip);
        footStepSound = FootstepSoundManger.soundManager.footstepSounds["Floor_Checker_Mat"];
    }

    // Update is called once per frame
    void Update()
    {
        frameCounter += 1;
        if (framesToSkip <= frameCounter)
        {
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if(meshCollider == null)
            {
                return;
            }
            if (meshCollider.sharedMesh != null)
            {
                //Mesh mesh = meshCollider.sharedMesh;
                if(meshCollider.sharedMesh.triangles.Length <= hit.triangleIndex * 3 + 3 || hit.triangleIndex < 0)
                {
                    return;
                }
                int[] hitTriangle = new int[]
                {
                        meshCollider.sharedMesh.triangles[hit.triangleIndex * 3],
                        meshCollider.sharedMesh.triangles[hit.triangleIndex * 3 + 1],
                        meshCollider.sharedMesh.triangles[hit.triangleIndex * 3 + 2]
                };
                for (int i = 0; i < meshCollider.sharedMesh.subMeshCount; i++)
                {
                    int[] subMeshTris = meshCollider.sharedMesh.GetTriangles(i);
                    for (int j = 0; j < subMeshTris.Length; j += 3)
                    {
                        if (subMeshTris[j] == hitTriangle[0] &&
                            subMeshTris[j + 1] == hitTriangle[1] &&
                            subMeshTris[j + 2] == hitTriangle[2])
                        {
                            //mat = hit.collider.GetComponent<MeshRenderer>().materials[i];
                            AudioClip newFootstepSound;
                            if (FootstepSoundManger.soundManager != null && FootstepSoundManger.soundManager.footstepSounds.TryGetValue(hit.collider.GetComponent<MeshRenderer>().sharedMaterials[i].name, out newFootstepSound))
                            {
                                footStepSound = newFootstepSound;
                            }
                            frameCounter = 0;
                        }
                    }
                }
            }
        }
    }


}
