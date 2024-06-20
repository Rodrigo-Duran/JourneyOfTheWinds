using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameWinController : MonoBehaviour
{

    #region Components

    [SerializeField] private GameObject counting;
    private TextMeshProUGUI countingLabel;

    #endregion

    #region MainMethods

    //Awake
    private void Awake()
    {
        if (GameObject.Find("SceneController1") == null)
        {
            GameObject sceneTransition = Instantiate(Resources.Load<GameObject>("SceneController1"));
        }

        countingLabel = counting.GetComponent<TextMeshProUGUI>();
        countingLabel.SetText("");
        StartCoroutine(BackToMainMenu());
    }

    #endregion

    #region Methods

    //BackToMainMenu
    IEnumerator BackToMainMenu()
    {
        yield return new WaitForSecondsRealtime(1f);
        countingLabel.SetText("3");
        yield return new WaitForSecondsRealtime(1f);
        countingLabel.SetText("2");
        yield return new WaitForSecondsRealtime(1f);
        countingLabel.SetText("1");
        yield return new WaitForSecondsRealtime(1f);
        SceneController.instance.NextScene(0);
    }

    #endregion
}
