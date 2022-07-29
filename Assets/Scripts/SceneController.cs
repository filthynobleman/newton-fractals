using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Cancel") > 0)
            SceneManager.LoadSceneAsync(0);
    }
}
