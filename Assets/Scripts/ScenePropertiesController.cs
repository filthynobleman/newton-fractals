using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePropertiesController : MonoBehaviour
{
    private SceneProperty[] mProperties;
    private MeshRenderer mRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mProperties = FindObjectsOfType<SceneProperty>();
        mRenderer = FindObjectOfType<MeshRenderer>();

        for (int i = 0; i < mProperties.Length; i++)
            mProperties[i].Initialize(mRenderer);
    }
}
