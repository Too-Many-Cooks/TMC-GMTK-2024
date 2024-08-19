using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    SceneReference sceneToLoad;

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad.SceneName, LoadSceneMode.Single);
    }

    public void UnloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(sceneToLoad.SceneName);
    }
}
