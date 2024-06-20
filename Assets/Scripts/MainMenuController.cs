using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pressStartScreen;
    [SerializeField] private GameObject optionsScreen;
    private bool pressStartScreenIsActive;

    #region MainMethods

    //Awake
    private void Awake()
    {
        pressStartScreenIsActive = true;

        if (GameObject.Find("SceneController1") == null)
        {
            GameObject sceneTransition = Instantiate(Resources.Load<GameObject>("SceneController1"));
        }
    }

    //Update
    private void Update()
    {
        if (pressStartScreenIsActive)
        {
            if (Input.anyKeyDown)
            {
                pressStartScreen.SetActive(false);
                optionsScreen.SetActive(true);
                pressStartScreenIsActive = false;
            }
        }

    }

    #endregion

    #region Methods
    //StartGame
    public void StartGame()
    {
        SceneController.instance.NextScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.firstTimeOpenTheGame = false;
    }

    //QuitGame
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

}
