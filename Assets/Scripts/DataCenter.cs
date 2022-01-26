using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenter : MonoBehaviour
{
    public enum GameStage
    {
        GameStage0 = 0,
        GameStage1 = 1,
        GameStage2 = 2,
        GameStage3 = 3,
        GameStage4 = 4
    }
    public enum PlayerEnum
    {
        Player1 = 1,
        Player2 = 2,
        None = 0
    }
    public static DataCenter instance { get; private set; }
    public PlayerEnum whichMove = PlayerEnum.None;
    public GameObject[] players;
    public bool[] playerDead = new bool[2];
    public GameObject boss;
    public GameStage currentGameStage = GameStage.GameStage0;
    public int playerDamageToBoss = 5;
    public int playerHealToBoss = 1;
    public int BossInitHealth = 100;
    public int PlayerInitHealth = 100;
    public int bossToPlayerDamage = -2;

    public float poisonHealthPercentage = 0.8f;
    public float poisonDamagePertentage = 1.5f;

    public StagePanel stagePanel;
    public ChoosePanel choosePanel;
    public GameObject posionPanel;

    #region Pause
    private bool _PauseGame = false;
    public bool pauseGame
    {
        get => _PauseGame;
        set
        {
            _PauseGame = value;
            if(_PauseGame || _StagePause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        }
    }

    private bool _StagePause = false;
    public bool stagePause
    {
        get => _StagePause;
        set
        {
            _StagePause = value;
            if (_PauseGame || _StagePause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        }
    }
    #endregion

    #region Start Game
    public GameObject startGameRoot;
    public WordWindows subline;
    #endregion

    #region StageChange  
    public float ChaseDist =100;
    public float[] BossHealthPer;
    #endregion
    #region Monsters
    public float monsterMoveSpeed = 5.0f;
    public float monsterAttackRange = 5.0f;
    public float monsterAttackInterval = 0.5f;
    public int playerDamageToMonster = 5;
    #endregion

    #region Game Stage 2
    public float stage2SwapTime = 5.0f;
    public int[] rageTowardPlayers = new int[] { 0, 0 };
    public int monsterRage = 2;
    public int bossRage = 1;
    float stage2StateCountDown = 0.0f;
    #endregion

    #region Game Stage 4
    public int bossDamageAdd = 0;
    public int bossSpeedAdd = 0;
    public bool takenDrug { get; set; } = false;
    #endregion

    #region Beginner
    public BeginnerWindows beginner;
    #endregion
    public void AddRage(PlayerEnum player, int Val)
    {
        if(currentGameStage == GameStage.GameStage2)
        {
            rageTowardPlayers[(int)player - 1] += Val;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }

    public void InitGameStatus()
    {
        currentGameStage = GameStage.GameStage0;
        stage2StateCountDown = 0.0f;
        rageTowardPlayers = new int[] { 0, 0 };
        whichMove = PlayerEnum.None;
        bossDamageAdd = 0;
        bossSpeedAdd = 0;
        boss.GetComponent<BossControl>().ResetData();
        players[0].GetComponent<CharacterControl>().ResetData();
        players[1].GetComponent<CharacterControl>().ResetData();
        takenDrug = false;
        posionPanel.SetActive(false);
        playerDead[0] = false;
        playerDead[1] = false;
    }

    public void InitMainMenu()
    {
        stagePause = true;
        startGameRoot.SetActive(true);
        choosePanel.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        startGameRoot.SetActive(false);
        InitGameStatus();
        subline.StartShow(() =>
        {
            stagePause = false;
        });
    }

    private void Start()
    {
        whichMove = 0;
        InitMainMenu();
    }

    public PlayerEnum GetWitchMove()
    {
        return whichMove;
    }
    public void TrySetWitchMove(PlayerEnum which)
    {
        if(whichMove == PlayerEnum.None && currentGameStage != GameStage.GameStage1)
        {
            whichMove = which;
        }
    }

    public void TryResetWhichMove(PlayerEnum which)
    {
        if(whichMove == which && currentGameStage != GameStage.GameStage1)
        {
            whichMove = PlayerEnum.None;
        }
    }

    public void TakePosion()
    {
        if(!takenDrug)
        {
            takenDrug = true;
            players[0].GetComponent<Health>().health = (int)Mathf.Max(1.0f, (float)players[0].GetComponent<Health>().health * 0.8f);
            players[1].GetComponent<Health>().health = (int)Mathf.Max(1.0f, (float)players[1].GetComponent<Health>().health * 0.8f);
            posionPanel.SetActive(false);
        }
    }

    void StageChange()
    {
        switch (currentGameStage)
        {
            case DataCenter.GameStage.GameStage0:
                {
                    var p1 = players[0].GetComponent<CharacterControl>();
                    var p2 = players[1].GetComponent<CharacterControl>();
                    if(p1.isStage0 && p2.isStage0)
                    {
                        stagePause = true;
                        stagePanel.ShowPanel(currentGameStage, () =>
                        {   
                            stagePause = false;
                            OnStageChange(currentGameStage);
                        });
                        
                        currentGameStage = DataCenter.GameStage.GameStage1;
                        print("Change to GameStage1");
                    } 
                };
                break;
            case DataCenter.GameStage.GameStage1:
                {
                    if (boss.GetComponent<Health>()?.health<=BossInitHealth*BossHealthPer[1])
                    {
                        stagePause = true;
                        stagePanel.ShowPanel(currentGameStage, () =>
                        {
                            stagePause = false;
                            OnStageChange(currentGameStage);
                            posionPanel.SetActive(true);
                        });
                        currentGameStage = DataCenter.GameStage.GameStage2;
                        print("Change to GameStage2");
                    }
                };
                break;
            case DataCenter.GameStage.GameStage2:
                {
                    if (boss.GetComponent<Health>()?.health <= BossInitHealth * BossHealthPer[2])
                    {
                        stagePause = true;
                        stagePanel.ShowPanel(currentGameStage, () =>
                        {
                            stagePause = false;
                            OnStageChange(currentGameStage);
                        });
                        currentGameStage = DataCenter.GameStage.GameStage3;
                        print("Change to GameStage3");
                    }
                }
                break;
            case DataCenter.GameStage.GameStage3:
                {
                    if (boss.GetComponent<Health>()?.health <= BossInitHealth * BossHealthPer[3])
                    {
                        stagePause = true;
                        choosePanel.gameObject.SetActive(true);
                        currentGameStage = DataCenter.GameStage.GameStage4;
                        OnStageChange(currentGameStage);
                        print("Change to GameStage4");
                    }
                }
                break;
        }
    }

    private void OnStage4()
    {
        bossDamageAdd = 2;
        bossSpeedAdd = 2;
    }

    private void OnStageChange(GameStage stage)
    {
        if(stage == GameStage.GameStage4)
        {
            OnStage4();
        }
        var be = beginner.GetComponent<BeginnerWindows>();
        be.ShowStageText(stage);
    }

    private void Update()
    {
        StageChange();
        if(currentGameStage == GameStage.GameStage1)
        {
            stage2StateCountDown -= Time.deltaTime;
            if(stage2StateCountDown <= 0.0f)
            {
                stage2StateCountDown = stage2SwapTime;
                whichMove = (whichMove == PlayerEnum.Player1) ? PlayerEnum.Player2 : PlayerEnum.Player1;
            }
        }
    }
}
