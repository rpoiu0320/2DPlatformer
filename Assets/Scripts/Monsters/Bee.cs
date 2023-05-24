using BeeState;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bee : MonoBehaviour
{
    // public enum State { Idle, Trace, Return, Attack, Patrol, Size }
    private StateBase[] states;
    private State curState;

    [SerializeField] public float detectRange;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float attackRange;
    [SerializeField] public Transform[] patrolPoints;

    public Transform player;
    public Vector3 returnPosition;
    // private float lastAttackTime;
    // private float idleTime = 0;
    public int patrolIndex = 0;

    private void Awake()
    {
        states = new StateBase[(int)State.Size];
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Patrol] = new PatrolState(this);
    }

    private void Start()
    {
        curState = State.Idle;
        states[(int)State.Trace].Enter();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        returnPosition = transform.position;
    }

    private void Update()
    {
        states[(int)curState].Update();
    }
    
    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Update();
    }
}

    /*private void Update()
    {
        switch (curState)
        {
            case State.Idle:    // 1. 플레이어가 멀리 있을때는 가만히 있기
                IdleUpdate();
                break;
            case State.Trace:   // 2. 플레이어가 어느정도 가까워지면 공격
                TraceUpdate();
                break;
            case State.Return:  // 2-1. 추적 중에 너무 멀어지면 원위치
                ReturnUpdate(); 
                break;
            case State.Attack:
                AttackUpdate(); // 2-2. 추적 중에 가까워지면 공격
                break;
            case State.Patrol:
                PatrolUpdate();
                break;
        }
    }

    private void IdleUpdate()
    {
        idleTime += Time.deltaTime;

        if (idleTime > 2)
        {
            idleTime = 0;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            curState = State.Patrol;
        }
        
        if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }

    private void TraceUpdate()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(player.position, transform.position) > detectRange)
        {
            curState = State.Return;
        }
        else if (Vector2.Distance(player.position, transform.position) < attackRange)
        {
            curState = State.Attack;
        }
    }

    private void ReturnUpdate()
    {
        // 원래 자리로 돌아가기
        Vector2 dir = (returnPosition - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        // 원래 자리로 돌아갔으면
        if (Vector2.Distance(transform.position, returnPosition) < 0.02f)
        {
            curState = State.Idle;
        }
        else if(Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }

    private void AttackUpdate()
    {
        if(lastAttackTime > 1)
        {
            // 공격
            lastAttackTime = 0;
        }

        lastAttackTime += Time.deltaTime;

        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Trace;
        }
    }

    private void PatrolUpdate()
    {
        // 순찰 진행
        Vector2 dir = (patrolPoints[patrolIndex].position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.02f)
        {
            curState = State.Idle;
        }
        else if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }*/

namespace BeeState
{
    public enum State { Idle, Trace, Return, Attack, Patrol, Size }

    public class IdleState : StateBase
    {
        private Bee bee;
        private float idleTime;

        public IdleState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Update()
        {
            idleTime += Time.deltaTime;

            if (idleTime > 2)
            {
                idleTime = 0;
                bee.ChangeState(State.Patrol);
            }

            if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }
    }

    public class TraceState : StateBase
    {
        private Bee bee;

        public TraceState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Update()
        {
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(State.Return);
            }
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.attackRange)
            {
                bee.ChangeState(State.Attack);
            }
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }
    }

    public class ReturnState : StateBase
    {
        private Bee bee;

        public ReturnState(Bee bee)
        {
            this.bee = bee;
        }

        public Vector3 returnPosition;

        public override void Update()
        {
            // 원래 자리로 돌아가기
            Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // 원래 자리로 돌아갔으면
            if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.02f)
            {
                bee.ChangeState(State.Idle);
            }
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }
    }

    public class AttackState : StateBase
    {
        private Bee bee;

        public AttackState(Bee bee)
        {
            this.bee = bee;
        }

        private float lastAttackTime = 0;

        public override void Update()
        {
            if (lastAttackTime > 1)
            {
                // 공격
                lastAttackTime = 0;
            }
            lastAttackTime += Time.deltaTime;

            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.attackRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }
    }

    public class PatrolState : StateBase
    {
        private Bee bee;

        public PatrolState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Update()
        {
            // 순찰 진행
            Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(bee.transform.position, bee.patrolPoints[bee.patrolIndex].position) < 0.02f)
            {
                bee.ChangeState(State.Idle);
            }
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Enter()
        {
            bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;
        }

        public override void Exit()
        {

        }
    }
}

