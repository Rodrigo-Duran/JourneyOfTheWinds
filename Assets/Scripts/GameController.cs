using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{

    #region Components

    //-----------------------------------------------------/
    [Header("Player")]

    [SerializeField] private GameObject playerObject;
    private Player player;

    //-----------------------------------------------------/
    [Header("Enemy")]

    [SerializeField] private GameObject enemyObject;
    private Enemy enemy;

    //-----------------------------------------------------/
    [Header("Game")]
    private int energy;
    private int actualRound;
    private int nextRound;
    private int enemyDamage;
    private bool enemyCanAttack;
    private bool buttonsReady;
    private bool gameCanStart;
    private bool gameIsPaused;
    private string lastEnemyAttack;

    //-----------------------------------------------------/
    [Header("UI")]
    [Header("InGameScreen")]
    [SerializeField] private GameObject inGameScreen;

    //ActionsButtons
    [SerializeField] private GameObject button1, button2, button3, button4;
    private UnityEngine.UI.Button attack1Button, attack2Button, attack3Button, blockButton;

    //Round
    [SerializeField] private GameObject round;
    private TextMeshProUGUI roundLabel;

    //Health Points
    [SerializeField] private GameObject playerLife, enemyLife;
    private TextMeshProUGUI playerLifeLabel, enemyLifeLabel;

    //Energy
    [SerializeField] UnityEngine.UI.Image energyBarImage;
    [SerializeField] private List<Sprite> spritesList;
    [SerializeField] private GameObject energyBar;
    private TextMeshProUGUI energyBarLabel;

    //Player HitMark and Block
    [SerializeField] private GameObject playerHitMark, playerBlocked, playerDamageSuffer;
    private TextMeshProUGUI playerDamageSufferLabel;

    //Enemy HitMark and Blocked
    [SerializeField] private GameObject enemyHitMark, enemyBlocked, enemyDamageSuffer;
    private TextMeshProUGUI enemyDamageSufferLabel;

    //Attacking Order
    [SerializeField] private UnityEngine.UI.Image attackingOrderImage;
    [SerializeField] private Sprite enemyTurn;
    [SerializeField] private Sprite playerTurn;

    //-----------------------------------------------------/
    [Header("PausedGameScreen")]
    [SerializeField] private GameObject pauseGameScreen;

    //-----------------------------------------------------/
    [Header("GameOverScreen")]
    [SerializeField] private GameObject gameOverScreen;

    //-----------------------------------------------------/
    [Header("YouWinScreen")]
    [SerializeField] private GameObject youWinScreen;

    //-----------------------------------------------------/
    [Header("DialogueScreen")]
    
    //Speaks
    [SerializeField] private List<string> charactersSpeaks;
    [SerializeField] private GameObject enemySpeak, playerSpeak;
    private TextMeshProUGUI enemySpeakLabel, playerSpeakLabel;

    //Baloons
    [SerializeField] private GameObject playerBaloon, enemyBaloon;

    //-----------------------------------------------------/
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource, soundEffectsSource;
    [SerializeField] private AudioClip music, swordAttack, hurricaneAttack, windCutAttack, enemyBasicAttack, enemyMagicAttack1, enemyMagicAttack2, blockSound;


    #endregion

    #region MainMethods
    //Awake
    void Awake()
    {
        //Player
        player = playerObject.GetComponent<Player>();

        //Enemy
        enemy = enemyObject.GetComponent<Enemy>();

        //Game
        Debug.Log("COMEÇOU O JOGO");
        energy = 1;
        actualRound = 1;
        nextRound = 2;
        enemyDamage = 0;
        enemyCanAttack = true;
        buttonsReady = true;
        gameCanStart = false;
        gameIsPaused = false;
        lastEnemyAttack = null;
        if (Time.timeScale == 0f) Time.timeScale = 1f;

        //UI
        attack1Button = button1.GetComponent<UnityEngine.UI.Button>();
        attack2Button = button2.GetComponent<UnityEngine.UI.Button>();
        attack3Button = button3.GetComponent<UnityEngine.UI.Button>();
        blockButton = button4.GetComponent<UnityEngine.UI.Button>();
        roundLabel = round.GetComponent<TextMeshProUGUI>();
        playerLifeLabel = playerLife.GetComponent<TextMeshProUGUI>();
        enemyLifeLabel = enemyLife.GetComponent<TextMeshProUGUI>();
        energyBarLabel = energyBar.GetComponent<TextMeshProUGUI>();
        enemySpeakLabel = enemySpeak.GetComponent<TextMeshProUGUI>();
        playerSpeakLabel = playerSpeak.GetComponent<TextMeshProUGUI>();
        playerDamageSufferLabel = playerDamageSuffer.GetComponent<TextMeshProUGUI>();
        enemyDamageSufferLabel = enemyDamageSuffer.GetComponent<TextMeshProUGUI>();

        //Scene Animator
        if (GameObject.Find("SceneController1") == null)
        {
            GameObject sceneTransition = Instantiate(Resources.Load<GameObject>("SceneController1"));
        }

        //StartingGame
        StartCoroutine(StartingGame());
        //gameCanStart = true;
        //inGameScreen.SetActive(true);

    }

    //Update
    void Update()
    {
        if (gameCanStart) 
        { 
            CheckingRound();
            CheckingButtons();
            UpdatingLabels();
            UpdatingEnergyBarImage();
            PauseGame();
            CheckingMusic();
        }

    }
    #endregion

    #region CombatHandler

    //PlayerBasicAttack
    public void PlayerBasicAttack()
    {
        buttonsReady = false;
        Debug.Log("YOUR CHOICE WAS BASIC ATTACK");
        StartCoroutine(DoingTheCombat(false, player.BasicAttack(), 1, enemy.Block(1)));
        if (energy < 4 ) energy++;
    }

    //PlayerHurricaneAttack
    public void PlayerHurricaneAttack()
    {
        buttonsReady = false;
        Debug.Log("YOUR CHOICE WAS HURRICANE ATTACK");
        StartCoroutine(DoingTheCombat(false, player.HurricaneAttack(), 2, enemy.Block(2)));
        energy -= 2;
    }

    //PlayerWindCutAttack
    public void PlayerWindCutAttack()
    {
        buttonsReady = false;
        Debug.Log("YOUR CHOICE WAS WINDCUT ATTACK");
        StartCoroutine(DoingTheCombat(false, player.WindCutAttack(), 3, enemy.Block(3)));
        energy -= 3;
    }

    //PlayerBlock
    public void PlayerBlock()
    {
        buttonsReady = false;
        Debug.Log("YOUR CHOICE WAS BLOCK");
        StartCoroutine(DoingTheCombat(player.Block(), 0, 0, false));
    }

    //DoingTheCombat
    IEnumerator DoingTheCombat(bool isABlockPlay, int playerDamage, int playerAttackType, bool enemyCanBlock)
    {
        Debug.Log("DOING THE COMBAT !!!");
        //ESCOLHENDO ATAQUE DO INIMIGO CASO SEJA RODADA QUE ELE POSSA ATACAR        
        string enemyAttack = null;

        attackingOrderImage.sprite = enemyTurn;

        if (enemyCanAttack) 
        {

            enemyAttack = enemy.ChooseAttack(); // Options -  "basic" / "magic1" / "magic2"
            switch (enemyAttack)
            {
                case "basic":
                    enemyDamage = enemy.BasicAttack();
                    //TOCAR ANIMAÇÃO DE ATAQUE DO INIMIGO

                    enemyBaloon.SetActive(true);
                    enemySpeakLabel.SetText("ATAQUE BÁSICO");
                    yield return new WaitForSeconds(1f);
                    enemyBaloon.SetActive(false);
                    enemySpeakLabel.SetText("");

                    soundEffectsSource.clip = enemyBasicAttack;
                    soundEffectsSource.Play();
                    Debug.Log("ENEMY - ATAQUE BASICO");
                    break;
                case "magic1":
                    enemyDamage = enemy.MagicAttack1();
                    //TOCAR ANIMAÇÃO DE PREPARO DE ATAQUE DO INIMIGO

                    enemyBaloon.SetActive(true);
                    enemySpeakLabel.SetText("PREPARAR ATAQUE MAGICO 1");
                    yield return new WaitForSeconds(1f);
                    enemyBaloon.SetActive(false);
                    enemySpeakLabel.SetText("");

                    lastEnemyAttack = "magic1";
                    Debug.Log("ENEMY - PREPARAR ATAQUE MAGIC1");
                    break;

                case "magic2":
                    enemyDamage = enemy.MagicAttack2();
                    //TOCAR ANIMAÇÃO DE PREPARO DE ATAQUE DO INIMIGO

                    enemyBaloon.SetActive(true);
                    enemySpeakLabel.SetText("PREPARAR ATAQUE MAGICO 2");
                    yield return new WaitForSeconds(1f);
                    enemyBaloon.SetActive(false);
                    enemySpeakLabel.SetText("");

                    lastEnemyAttack = "magic2";
                    Debug.Log("ENEMY - PREPARAR ATAQUE MAGIC2");
                    break;
            }
        }

        //SE FOR A RODADA QUE ELE NÃO PODE ATACAR, O ATAQUE SERÁ O MAGIC1 OU MAGIC2
        else
        {
            if (lastEnemyAttack == "magic1")
            {
                Debug.Log("ENEMY - ATAQUE MAGIC 1");

                enemyBaloon.SetActive(true);
                enemySpeakLabel.SetText("ATAQUE MAGICO 1");
                yield return new WaitForSeconds(1f);
                enemyBaloon.SetActive(false);

                enemySpeakLabel.SetText("");
                soundEffectsSource.clip = enemyMagicAttack1;
                soundEffectsSource.Play();
            }
            else if (lastEnemyAttack == "magic2")
            {
                Debug.Log("ENEMY - ATAQUE MAGIC 2");

                enemyBaloon.SetActive(true);
                enemySpeakLabel.SetText("ATAQUE MAGICO 2");
                yield return new WaitForSeconds(1f);
                enemyBaloon.SetActive(false);
                enemySpeakLabel.SetText("");

                soundEffectsSource.clip = enemyMagicAttack2;
                soundEffectsSource.Play();
            }
            
            //Tocar animação de ataque magic1 ou 2 do inimigo
            enemyCanAttack = true;
        }

        //ESCOLHA DE BLOQUEIO DO PLAYER
        if (isABlockPlay)
        {
            yield return new WaitForSeconds(1f);
            playerBaloon.SetActive(true);
            playerSpeakLabel.SetText("BLOQUEAR ATAQUE");
            yield return new WaitForSeconds(1f);
            playerBaloon.SetActive(false);
            playerSpeakLabel.SetText("");

            //Player Chose Block
            yield return new WaitForSeconds(1f);
            playerBlocked.SetActive(true);

            soundEffectsSource.clip = blockSound;
            soundEffectsSource.Play();
            Debug.Log("PLAYER - ATAQUE BLOQUEADO COM SUCESSO");
            yield return new WaitForSeconds(1f);
            playerBlocked.SetActive(false);
            //Tocar animação de bloqueio do player
        }

        //ESCOLHA DE ATAQUE DO PLAYER
        else
        {
            //SE FOR A RODADA DE PREPARAÇÃO DO ATAQUE MAGIC1 OU 2 NÃO ATACAR O PLAYER
            if (!(enemyCanAttack && enemyAttack == "magic1" || enemyAttack == "magic2"))
            {
                //PLAYER SOFRENDO O ATAQUE
                yield return new WaitForSeconds(1);
                //tocar animação de sofrer dano do player
                playerHitMark.SetActive(true);
                playerDamageSufferLabel.SetText(enemyDamage.ToString());
                
                //SE O DANO SOFRIDO PELO PLAYER FOR MAIOR QUE A VIDA QUE ELE TEM, ENTÃO A VIDA É SETADA PARA 0
                if(player._playerLife - enemyDamage > 0)player._playerLife -= enemyDamage;
                else player._playerLife = 0;    

                Debug.Log("PLAYER - FOI ATACADO, SOFREU " + enemyDamage + " DE DANO");
                yield return new WaitForSeconds(1f);
                playerHitMark.SetActive(false);
                playerDamageSufferLabel.SetText("");

                //SE A VIDA DO PLAYER CHEGAR A 0, CHAMAR A FUNÇÃO GAME OVER
                if (player._playerLife <= 0) GameOver();
            }

            //PLAYER ATACANDO
            yield return new WaitForSeconds(1);
            switch (playerAttackType)
            {
                case 1:
                    //tocar animação de ataque basico do player

                    playerBaloon.SetActive(true);
                    playerSpeakLabel.SetText("ATAQUE BASICO");
                    yield return new WaitForSeconds(1f);
                    playerBaloon.SetActive(false);
                    playerSpeakLabel.SetText("");

                    soundEffectsSource.clip = swordAttack;
                    soundEffectsSource.Play();
                    Debug.Log("PLAYER - ATAQUE BASICO");
                    break;

                case 2:
                    //tocar animação de ataque furacao do player

                    playerBaloon.SetActive(true);
                    playerSpeakLabel.SetText("ATAQUE FURACAO");
                    yield return new WaitForSeconds(1f);
                    playerBaloon.SetActive(false);
                    playerSpeakLabel.SetText("");

                    soundEffectsSource.clip = hurricaneAttack;
                    soundEffectsSource.Play();
                    Debug.Log("PLAYER - ATAQUE FURACAO");
                    break;

                case 3:
                    //tocar animação de ataque corte de vento do player

                    playerBaloon.SetActive(true);
                    playerSpeakLabel.SetText("ATAQUE CORTE DE VENTO");
                    yield return new WaitForSeconds(1f);
                    playerBaloon.SetActive(false);
                    playerSpeakLabel.SetText("");

                    soundEffectsSource.clip = windCutAttack;
                    soundEffectsSource.Play();
                    Debug.Log("PLAYER - ATAQUE CORTE DE VENTO");
                    break;
            }

            if (enemyCanBlock)
            {
                yield return new WaitForSeconds(1);
                enemyBlocked.SetActive(true);
                soundEffectsSource.clip = blockSound;
                soundEffectsSource.Play();
                Debug.Log("ENEMY - ATAQUE BLOQUEADO COM SUCESSO");
                yield return new WaitForSeconds(1);
                enemyBlocked.SetActive(false);
                //tocar animação de bloqueio do inimigo
            }
            else
            {
                yield return new WaitForSeconds(1);
                //tocar animação de sofrer dano do inimigo
                enemyHitMark.SetActive(true);
                enemyDamageSufferLabel.SetText(playerDamage.ToString());

                //SE O DANO SOFRIDO PELO INIMIGO FOR MAIOR QUE A VIDA QUE ELE TEM, ENTÃO A VIDA É SETADA PARA 0
                if (enemy._life - playerDamage > 0)enemy._life -= playerDamage;
                else enemy._life = 0;

                Debug.Log("ENEMY - FOI ATACADO, SOFREU " + playerDamage + " DE DANO");
                yield return new WaitForSeconds(1f);
                enemyHitMark.SetActive(false);
                enemyDamageSufferLabel.SetText("");

                //SE A VIDA DO INIMIGO CHEGAR A 0, CHAMAR A FUNÇÃO PROXIMA FASE
                if (enemy._life <= 0) NextPhase();
            }
        }

        if (enemyAttack == "magic1" || enemyAttack == "magic2") enemyCanAttack = false;
        actualRound += 1 ;
        if (enemy._life > 0 && player._playerLife > 0) buttonsReady = true;

        attackingOrderImage.sprite = playerTurn;
    }

    #endregion

    #region GameHandler

    //CheckingRound
    void CheckingRound()
    {
        if (nextRound == actualRound)
        {
            nextRound++;
        }
    }

    //CheckingButtons
    void CheckingButtons()
    {
        if (energy >= 0 && buttonsReady && gameCanStart)
        {
            blockButton.interactable = true;
            attack1Button.interactable = true;
        }
        else
        {
            blockButton.interactable = false;
            attack1Button.interactable = false;
        }
        if (energy >= 2 && buttonsReady && gameCanStart)
        {
            attack2Button.interactable = true;
        }
        else
        {
            attack2Button.interactable = false;
        }

        if (energy >= 3 && buttonsReady && gameCanStart)
        {
            attack3Button.interactable = true;
        }
        else
        {
            attack3Button.interactable = false;
        }
    }

    //UpdatingLabels
    void UpdatingLabels()
    {
        roundLabel.SetText(actualRound.ToString());
        playerLifeLabel.SetText(player._playerLife.ToString() + " HP");
        enemyLifeLabel.SetText(enemy._life.ToString() + " HP");
        energyBarLabel.SetText(energy.ToString());

    }

    //UpdatingEnergyBarImage
    void UpdatingEnergyBarImage()
    {
        switch (energy)
        {
            case 0:
                energyBarImage.sprite = spritesList[0];
                break;

            case 1:
                energyBarImage.sprite = spritesList[1];
                break;

            case 2:
                energyBarImage.sprite = spritesList[2];
                break;

            case 3:
                energyBarImage.sprite = spritesList[3];
                break;

            case 4:
                energyBarImage.sprite = spritesList[4];
                break;
        }
    }
    
    //PauseGame
    void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameIsPaused)
            {
                gameIsPaused = true;
                inGameScreen.SetActive(false);
                pauseGameScreen.SetActive(true);
                Time.timeScale = 0f;
                musicSource.Pause();
            }
            else
            {
                gameIsPaused = false;
                pauseGameScreen.SetActive(false);
                inGameScreen.SetActive(true);
                Time.timeScale = 1f;
                musicSource.UnPause();
            }
        }
    }

    //ResumeGame
    public void ResumeGame()
    {
        gameIsPaused = false;
        pauseGameScreen.SetActive(false);
        inGameScreen.SetActive(true);
        Time.timeScale = 1f;
        musicSource.UnPause();
    }

    //QuitGame
    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Time.timeScale = 1f;
        SceneController.instance.NextScene(0);
    }

    //GameOver
    void GameOver()
    {
        StopAllCoroutines();
        Debug.Log("GAME OVER !!!");
        //tocar animação do player morrendo
        inGameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        StartCoroutine(FinishingTheGame());

    }

    //FinishingTheGame
    IEnumerator FinishingTheGame()
    {
        yield return new WaitForSeconds(3);
        gameOverScreen.SetActive(false);
        SceneController.instance.NextScene(5);
    }

    //NextPhase
    void NextPhase()
    {
        StopAllCoroutines();
        Debug.Log("YOU PASSED !!!");
        //tocar animação do inimigo morrendo
        inGameScreen.SetActive(false);
        youWinScreen.SetActive(true);
        StartCoroutine(MovingForward());
    }

    //MovingForward
    IEnumerator MovingForward()
    {
        yield return new WaitForSeconds(5f);
        youWinScreen.SetActive(false);
        SceneController.instance.NextScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator StartingGame()
    {
       
        musicSource.clip = music;
        musicSource.Play();

        yield return new WaitForSeconds(3);

        enemyBaloon.SetActive(true);
        enemySpeakLabel.SetText(charactersSpeaks[0]);

        yield return new WaitForSeconds(3);

        enemyBaloon.SetActive(false);
        enemySpeakLabel.SetText("");
        playerBaloon.SetActive(true);
        playerSpeakLabel.SetText(charactersSpeaks[1]);

        yield return new WaitForSeconds(3);

        playerBaloon.SetActive(false);
        playerSpeakLabel.SetText("");
        enemyBaloon.SetActive(true);
        enemySpeakLabel.SetText(charactersSpeaks[2]);

        yield return new WaitForSeconds(3);

        enemyBaloon.SetActive(false);
        enemySpeakLabel.SetText("");

        gameCanStart = true;
        
        //Playing Player and Enemy idle animation
        player.SetIdleAnimation();
        enemy.SetIdleAnimation();

        //Setting the InGame Screen active
        inGameScreen.SetActive(true);

        attackingOrderImage.sprite = playerTurn;
    }

    //CheckingMusic
    void CheckingMusic()
    {
        if ((!gameIsPaused) && (!musicSource.isPlaying))
        {
            musicSource.clip = music;
            musicSource.Play();
        }
    }

    #endregion

}
