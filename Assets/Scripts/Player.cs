using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Player : MonoBehaviour
{

    #region Components

    //Private
    private int playerLife;
    private Animator animator;

    //Public
    public int _playerLife
    {
        get { return playerLife; }
        set { playerLife = value; }
    }

    #endregion

    #region MainMethods

    //Awake
    void Awake()
    {
        playerLife = 500;
        animator = GetComponent<Animator>();
    }

    //Update
    void Update()
    {

    }

    #endregion

    #region CombatHandler

    //BasicAttack
    public int BasicAttack()
    {
        return Random.Range(100, 200);
    }

    //HurricaneAttack
    public int HurricaneAttack()
    {
        return Random.Range(200, 600);
    }

    //WindCutAttack
    public int WindCutAttack()
    {
        return Random.Range(400, 800);
    }

    //Block
    public bool Block()
    {
        return true;
    }

    #endregion

    #region AnimationHandler

    //SetIdleAnimation
    public void SetIdleAnimation()
    {
        animator.SetInteger("transition", 1);
    }

    #endregion


}
