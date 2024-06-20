using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{

    #region MainMethods

    //Awake
    private void Awake()
    {
        if (GameObject.Find("SceneController1") == null)
        {
            GameObject sceneTransition = Instantiate(Resources.Load<GameObject>("SceneController1"));
        }

        StartCoroutine(BackToMainMenu());
    }

    #endregion

    #region Methods

    //BackToMainMenu
    IEnumerator BackToMainMenu()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneController.instance.NextScene(0);
    }

    #endregion
}
