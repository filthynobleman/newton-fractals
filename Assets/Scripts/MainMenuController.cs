using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadScene(int i)
    {
        SceneManager.LoadSceneAsync(i, LoadSceneMode.Single);
    }

    public void Update()
    {
        if (Input.GetAxis("Cancel") > 0)
            Application.Quit();
    }
}
