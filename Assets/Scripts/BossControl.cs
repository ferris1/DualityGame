using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{
    public float[] moveSpeeds;
    public float speedReduce = 0.5f;
    public GameObject monsterObj; 
    public float attackRange = 5.0f;
    public float attackInterval = 0.5f;
    private float attackRemainTime = 0.0f;
    public int MonsterNum = 8;
    public int MonsterGenNum = 0; 
    private DataCenter.PlayerEnum lastTargetPlayer = DataCenter.PlayerEnum.None;
    Rigidbody bossrig;

    public float MonsterGenInterval = 5f;
    float MonsterGenRemainTime = 0.0f;
    public float speedReducingTime { get; set; } = 0.0f;
    private AudioSource m_AudioSouce;
    bool startSound = false;
    float soundTimer = 0.0f;
    #region Animator
    public Animator bossAminator;
    #endregion
    #region AI
    public NavMeshAgent bossAgent;
    #endregion

    public void ResetData()
    {
        lastTargetPlayer = DataCenter.PlayerEnum.None;
        attackRemainTime = 0.0f;
        MonsterGenRemainTime = 0.0f;
        MonsterGenNum = 0;
        GetComponent<Health>().health = DataCenter.instance.BossInitHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        bossrig = GetComponent<Rigidbody>();
        GetComponent<Health>().health = DataCenter.instance.BossInitHealth;
        m_AudioSouce =GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startSound)
        {
            soundTimer += Time.deltaTime;
        }
        if (soundTimer > 0.9f)
        {
            soundTimer = 0;
            startSound = false;
            m_AudioSouce.Stop();
        }

        var moveEnum = DataCenter.instance.GetWitchMove();
        DataCenter.PlayerEnum targetPlayer = DataCenter.PlayerEnum.Player1;
        float dist0 = (DataCenter.instance.players[0].transform.position - transform.position).magnitude;
        float dist1 = (DataCenter.instance.players[1].transform.position - transform.position).magnitude;
        bool doDefaultTargetJudge = true;
        if(DataCenter.instance.currentGameStage == DataCenter.GameStage.GameStage2)
        {
            doDefaultTargetJudge = false;
            if(DataCenter.instance.rageTowardPlayers[0] == DataCenter.instance.rageTowardPlayers[1])
            {
                doDefaultTargetJudge = true;
            }
            else
            {
                lastTargetPlayer = targetPlayer = DataCenter.instance.rageTowardPlayers[0] > DataCenter.instance.rageTowardPlayers[1] ?
                DataCenter.PlayerEnum.Player1 : DataCenter.PlayerEnum.Player2;
            }
        }
        if(doDefaultTargetJudge)
        {
            if (moveEnum == DataCenter.PlayerEnum.Player1 || moveEnum == DataCenter.PlayerEnum.Player2)
            {
                targetPlayer = moveEnum;
                lastTargetPlayer = targetPlayer;
            }
            else
            {
                if (lastTargetPlayer == DataCenter.PlayerEnum.None)
                {
                    lastTargetPlayer = dist0 < dist1 ? DataCenter.PlayerEnum.Player1 : DataCenter.PlayerEnum.Player2;
                }
                targetPlayer = lastTargetPlayer;
            }
        }
        if(DataCenter.instance.playerDead[0] && DataCenter.instance.playerDead[1])
        {
            targetPlayer = DataCenter.PlayerEnum.None;
        }
        else
        {
            if (targetPlayer == DataCenter.PlayerEnum.Player1 && DataCenter.instance.playerDead[0])
            {
                targetPlayer = DataCenter.PlayerEnum.Player2;
            }
            else if(targetPlayer == DataCenter.PlayerEnum.Player2 && DataCenter.instance.playerDead[1])
            {
                targetPlayer = DataCenter.PlayerEnum.Player1;
            }
        }
        if(targetPlayer == DataCenter.PlayerEnum.None)
        {
            bossAgent.speed = 0.0f;
            return;
        }

        {
            var target = DataCenter.instance.players[(int)targetPlayer - 1];
            float targetDist = targetPlayer == 0 ? dist0 : dist1;

            bossAgent.stoppingDistance = attackRange;
            bossAgent.destination = target.transform.position;
            //transform.LookAt(target.transform, Vector3.up);
            if (targetDist < attackRange)
            {
                attackRemainTime -= Time.deltaTime;
                if(attackRemainTime <= 0.0f)
                {
                    bossAminator.SetTrigger("Attack");
                    attackRemainTime = attackInterval;
                    if(targetPlayer == DataCenter.instance.GetWitchMove())
                    {

                        target.GetComponent<Health>()?.HealthChange(DataCenter.instance.bossToPlayerDamage+DataCenter.instance.bossDamageAdd);

                        //play audio
                        if (!startSound) {
                         
                            m_AudioSouce.Play();
                            startSound = true;
                        }
                    
                    }



                }
            }
            else
            {
                float reduce = speedReducingTime > 0.0f ? speedReduce : 1.0f;
                bossAgent.speed = moveSpeeds[(int)DataCenter.instance.currentGameStage] * reduce;
            }
            speedReducingTime = Mathf.Max(0.0f, speedReducingTime - Time.deltaTime);
        }
        if(DataCenter.instance.currentGameStage == DataCenter.GameStage.GameStage2)
        {
            MonsterGenRemainTime -= Time.deltaTime;
            if(MonsterGenRemainTime <= 0.0f && MonsterGenNum < MonsterNum)
            {
                MonsterGenNum += 1;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
                {
                    MonsterGenRemainTime = MonsterGenInterval;
                    var monster = GameObject.Instantiate(monsterObj);
                    monster.transform.position = hit.position;
                    monster.layer = gameObject.layer;
                    monster.GetComponent<NavMeshAgent>().Warp(hit.position);
                }
            } 
        }
    }
}
