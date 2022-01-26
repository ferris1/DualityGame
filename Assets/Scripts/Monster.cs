using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private float attackRemainTime = 0.0f;
    Rigidbody monsterrig;

    #region Monster
    public Animator monsterAnimator;
    #endregion

    #region AI Agent
    public NavMeshAgent monsterAgent;
    #endregion

    void Start()
    {
        monsterrig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist0 = (DataCenter.instance.players[0].transform.position - transform.position).magnitude;
        float dist1 = (DataCenter.instance.players[1].transform.position - transform.position).magnitude;

        var targetPlayer = dist0 < dist1 ? DataCenter.PlayerEnum.Player1 : DataCenter.PlayerEnum.Player2;

        if (DataCenter.instance.playerDead[0] && DataCenter.instance.playerDead[1])
        {
            targetPlayer = DataCenter.PlayerEnum.None;
        }
        else
        {
            if (targetPlayer == DataCenter.PlayerEnum.Player1 && DataCenter.instance.playerDead[0])
            {
                targetPlayer = DataCenter.PlayerEnum.Player2;
            }
            else if (targetPlayer == DataCenter.PlayerEnum.Player2 && DataCenter.instance.playerDead[1])
            {
                targetPlayer = DataCenter.PlayerEnum.Player1;
            }
        }
        if (targetPlayer == DataCenter.PlayerEnum.None)
        {
            monsterAgent.speed = 0.0f;
            return;
        }

        float targetDist = targetPlayer == 0 ? dist0 : dist1;
        var target = DataCenter.instance.players[(int)targetPlayer - 1];


        monsterAgent.stoppingDistance = DataCenter.instance.monsterAttackRange;
        monsterAgent.destination = target.transform.position;

        if (targetDist < DataCenter.instance.monsterAttackRange)
        {
            attackRemainTime -= Time.deltaTime;
            if (attackRemainTime <= 0.0f)
            {
                monsterAnimator.SetTrigger("Attack.");
                attackRemainTime = DataCenter.instance.monsterAttackInterval;
                target.GetComponent<Health>()?.HealthChange(-2);
            }
            monsterAgent.speed = 0.0f;
        }
        else
        {
            monsterAgent.speed = DataCenter.instance.monsterMoveSpeed;
        }
    }
}
