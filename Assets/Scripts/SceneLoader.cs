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
        SceneManager.LoadScene(sceneToLoad.SceneName);
    }
}
