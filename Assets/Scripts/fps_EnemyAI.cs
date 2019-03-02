using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class fps_EnemyAI : MonoBehaviour {

    public float patrolSpeed = 2f; //巡逻速度.
    public float chaseSpeed = 5f;  //追击速度.
    public float chaseWaitTime = 5f;  //追击丢失等待时间.
    public float patrolWaitTime = 1f; //抵达巡逻点等待时间.
    public Transform[] patrolWayPoint; //巡逻点.

    private fps_EnemySight enemySight;
    private NavMeshAgent nav;
    private Transform player;
    private fps_PlayerHealth playerHealthl;
    private float chaseTimer;
    private float patrolTimer;
    private int wayPointIndex;

    void Start()
    {
        enemySight = gameObject.GetComponent<fps_EnemySight>();
        nav = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealthl = player.GetComponent<fps_PlayerHealth>();
    }

    void Update()
    {
        if (enemySight.playerInSight && playerHealthl.hp > 0)
            Shooting();
        else if (enemySight.playerPosition != enemySight.resetPosition && playerHealthl.hp > 0)
            Chasing();
        else
            Parolling();

    }

    private void Shooting()
    {
        nav.SetDestination(transform.position);
    }

    private void Chasing()
    {
        Vector3 sightingDeltaPos = enemySight.playerPosition - transform.position;
        if (sightingDeltaPos.sqrMagnitude > 4f)
            nav.destination = enemySight.playerPosition;
        nav.speed = chaseSpeed;
        if (nav.remainingDistance < nav.stoppingDistance)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseWaitTime)
            {
                enemySight.playerPosition = enemySight.resetPosition;
                chaseTimer = 0f;
            }
        }
        else
            chaseTimer = 0f;
    }

    private void Parolling()
    {
        nav.speed = patrolSpeed;
        if (nav.destination == enemySight.resetPosition || nav.remainingDistance < nav.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                if (wayPointIndex == patrolWayPoint.Length - 1)
                    wayPointIndex = 0;
                else
                    wayPointIndex++;
                patrolTimer = 0;
            }
        }
        else
            patrolTimer = 0;
        nav.destination = patrolWayPoint[wayPointIndex].position;
    }

}
