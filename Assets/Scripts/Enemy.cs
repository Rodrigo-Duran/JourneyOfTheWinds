using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Components

    //PRIVATE

    //Life
    [SerializeField] private int life;

    //Type
    [SerializeField] private int type; //1 Devil , 2 Tiger, 3 Dragon

    //BasicAttack Damage
    [SerializeField] private int basicAttackMinimumDamage;
    [SerializeField] private int basicAttackMaximumDamage;

    //MagicAttack1 Damage
    [SerializeField] private int magicAttack1MinimumDamage;
    [SerializeField] private int magicAttack1MaximumDamage;

    //MagicAttack2 Damage
    [SerializeField] private int magicAttack2MinimumDamage;
    [SerializeField] private int magicAttack2MaximumDamage;

    //Blocking Chance
    [SerializeField] private int basicAttackBlockingChance;
    [SerializeField] private int hurricaneAttackBlockingChance;
    [SerializeField] private int windCutAttackBlockingChance;

    //Animator
    private Animator animator;

    //PUBLIC
    public int _life
    {
        get { return life; }
        set { life = value; }
    }

    #endregion

    #region MainMethods
    //Awake
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //Update
    void Update()
    {
        
    }
    #endregion

    #region AnimationHandler
    //SetIdleAnimation
    public void SetIdleAnimation()
    {
        if (type == 1) animator.SetInteger("transition", 1);
        else if (type == 2) animator.SetInteger("transition", 2);
        else if (type == 3) animator.SetInteger("transition", 3);
    }
    #endregion

    #region CombatHandler

    //BasicAttack
    public int BasicAttack()
    {
        return Random.Range(basicAttackMinimumDamage, basicAttackMaximumDamage);
    }

    //MagicAttack1
    public int MagicAttack1()
    {
        return Random.Range(magicAttack1MinimumDamage, magicAttack1MaximumDamage);
    }

    //MagicAttack2
    public int MagicAttack2()
    {
        return Random.Range(magicAttack2MinimumDamage, magicAttack2MaximumDamage);
    }

    //ChooseAttack
    public string ChooseAttack()
    {
        int i = Random.Range(1, 100);
        string chosenAttack;

        if (i <= 20) // 20% chance
        {
            chosenAttack = "magic2";
        }
        else if (i > 20 && i < 54) // 33% chance
        {
            chosenAttack = "magic1";
        }
        else // 47% chance
        {
            chosenAttack = "basic";
        }

        return chosenAttack;
    }

    //Block
    public bool Block(int playerAttack)
    {
        int i;

        switch (playerAttack)
        {
            case 1: //Chance Of Blocking the Player Basic Attack 

                i = Random.Range(1, 100);
                if (i < basicAttackBlockingChance + 1) return true;
                break;

            case 2: //Chance Of Blocking the Player Hurricane Attack 
                i = Random.Range(1, 100);
                if (i < hurricaneAttackBlockingChance + 1) return true;
                break;

            case 3: //Chance Of Blocking the Player WindCut Attack 
                i = Random.Range(1, 100);
                if (i < windCutAttackBlockingChance + 1) return true;
                break;
        }

        return false;
    }

    #endregion
}
