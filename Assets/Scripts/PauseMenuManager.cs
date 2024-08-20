using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField]
    SceneReference pauseMenu;

    public void TogglePauseMenu(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;

        if(!SceneManager.GetSceneByName(pauseMenu.SceneName).IsValid())
        {
            Time.timeScale = 0f;
            SceneManager.LoadScene(pauseMenu.SceneName, LoadSceneMode.Additive);
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(pauseMenu.SceneName);
        }
    }
}
