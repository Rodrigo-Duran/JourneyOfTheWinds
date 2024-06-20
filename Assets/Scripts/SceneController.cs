using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Components

    //PRIVATE
    private Animator animator;
    //private bool isLoading = false;
    //private float progress = 0f;

    //PUBLIC
    public static SceneController instance;

    #endregion

    #region MainMethods

    //Awake
    private void Awake()
    {
        //if (instance == null) 
        //{ 
            instance = this;

            animator = gameObject.GetComponentInChildren<Animator>();
            animator.gameObject.SetActive(false);

            DontDestroyOnLoad(gameObject);

        //SÓ PODE TOCAR A ANIMAÇÃO DE FADE IN QUANDO INICIAR A CENA, SE NÃO FOR A PRIMEIRA VEZ DO MAIN MENU (QUANDO O PLAYER ABRE O JOGO)
        if (!GameManager.firstTimeOpenTheGame)
        {
            StartCoroutine(Starting());
        }
        //}
        //else
        //{
        //    Destroy(gameObject); 
        //}

    }
    #endregion

    #region ScenesHandler

    //Starting
    IEnumerator Starting()
    {
        animator.gameObject.SetActive(true);
        animator.SetInteger("transition", 2);
        Debug.Log("INITIATING TRANSITION 2");
        yield return new WaitForSecondsRealtime(2f);
        animator.gameObject.SetActive(false);
    }

    //NextScene
    public void NextScene(int sceneIndex)
    {
        Debug.Log("NEXT SCENE");
        /*
        if (!isLoading)
        {
            isLoading = true;
            progress = 0f;
            Time.timeScale = 0f;

            //play animação
            animator.gameObject.SetActive(true);
            animator.SetInteger("transition", 1);

            //carrega nova cena
            StartCoroutine(LoadScene(sceneIndex));
        }*/
        
        StartCoroutine(LoadScene(sceneIndex));
    }

    //LoadScene
    IEnumerator LoadScene(int sceneIndex)
    {
        /*
        //yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        //enquanto não terminar de carregar a nova cena
        while (!operation.isDone)
        {
            progress = operation.progress;

            if (operation.progress.Equals(.9f))
            {
                //espera tempo da primeira transição
                yield return new WaitForSecondsRealtime(0.8f);

                operation.allowSceneActivation = true;
            }

            yield return new WaitForEndOfFrame();
        }

        //terminou de carregar
        Time.timeScale = 1f;
        isLoading = false;
        progress = 0f;

        //finaliza animação
        animator.SetInteger("transition", 2);

        //esconde transição
        yield return new WaitForSeconds(0.8f);
        animator.gameObject.SetActive(false);
        */



        
        //Ativa o gameObject com o Canva
        animator.gameObject.SetActive(true);
        Debug.Log("ANIMATOR ACTIVATED");

        //Toca animação FadeOut
        animator.SetInteger("transition", 1);
        Debug.Log("PLAY FADE OUT ANIMATION");

        yield return new WaitForSecondsRealtime(1f);
        //PROBLEMA COM O QUITGAME() DO PAUSE GAME SCREEN
        SceneManager.LoadSceneAsync(sceneIndex);
        //AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        //operation.allowSceneActivation = false;
        //animator.SetInteger("transition", 2);
        //yield return new WaitForSecondsRealtime(0.3f);
        //operation.allowSceneActivation = true;

        yield return new WaitForSecondsRealtime(0.2f);
        //Desativa o gameObject
        animator.gameObject.SetActive(false);
        Debug.Log("ANIMATOR DESACTIVATED");
        Destroy(gameObject);
        
    }

    #endregion
}
