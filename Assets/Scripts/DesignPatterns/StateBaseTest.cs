using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class Bee : Monster
{
    public enum State { Idle, Trace, Returning, Die, Size }
    StateMachine<State, Bee> stateMachine;

    private Transform target;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float range;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine<State, Bee>(this);
        stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
        stateMachine.AddState(State.Trace, new TraceState(this, stateMachine));
        stateMachine.AddState(State.Returning, new ReturningState(this, stateMachine));
        stateMachine.AddState(State.Die, new DieState(this, stateMachine));
    }

    private void Start()
    {
        rigidbody.gravityScale = 0f;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        stateMachine.SetUp(State.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    protected override void Die()
    {
        stateMachine.ChangeState(State.Die);
    }


    #region State
    private abstract class BeeState : StateBase<State, Bee>
    {
        protected GameObject gameObject => owner.gameObject;
        protected Transform transform => owner.transform;
        protected Rigidbody2D rigidbody => owner.rigidbody;
        protected SpriteRenderer renderer => owner.renderer;
        protected Animator animator => owner.animator;
        protected Collider2D collider => owner.collider;

        protected BeeState(Bee owner, StateMachine<State, Bee> stateMachine) : base(owner, stateMachine)
        {
        }
    }

    private class IdleState : BeeState
    {
        private Transform target;
        private float range;

        public IdleState(Bee owner, StateMachine<State, Bee> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Setup()
        {
            target = owner.target;
            range = owner.range;
        }

        public override void Enter()
        {
            rigidbody.velocity = Vector3.zero;
        }

        public override void Update()
        {

        }

        public override void Transition()
        {
            if ((target.position - transform.position).sqrMagnitude < range * range)
            {
                stateMachine.ChangeState(State.Trace);
            }
        }

        public override void Exit()
        {

        }
    }

    private class TraceState : BeeState
    {
        private Transform target;
        private float speed;
        private float range;

        public TraceState(Bee owner, StateMachine<State, Bee> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Setup()
        {
            target = owner.target;
            speed = owner.moveSpeed;
            range = owner.range;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rigidbody.velocity = dir * speed;
            renderer.flipX = rigidbody.velocity.x > 0 ? true : false;
        }

        public override void Transition()
        {
            if ((target.position - transform.position).sqrMagnitude > range * range)
            {
                stateMachine.ChangeState(State.Returning);
            }
        }

        public override void Exit()
        {

        }
    }

    private class ReturningState : BeeState
    {
        private Vector3 returnPosition;
        private float speed;

        public ReturningState(Bee owner, StateMachine<State, Bee> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Setup()
        {
            returnPosition = transform.position;
            speed = owner.moveSpeed;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            Vector2 dir = (returnPosition - transform.position).normalized;
            rigidbody.velocity = dir * speed;
            renderer.flipX = rigidbody.velocity.x > 0 ? true : false;
        }

        public override void Transition()
        {
            if ((returnPosition - transform.position).sqrMagnitude < 0.01f)
            {
                stateMachine.ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {

        }
    }

    private class DieState : BeeState
    {
        public DieState(Bee owner, StateMachine<State, Bee> stateMachine) : base(owner, stateMachine)
        {
        }

        public override void Setup()
        {

        }

        public override void Enter()
        {
            rigidbody.gravityScale = 1.0f;
            rigidbody.velocity = Vector2.up * 3;
            animator.SetBool("Die", true);
            collider.enabled = false;

            Destroy(gameObject, 3f);
        }

        public override void Update()
        {

        }

        public override void Transition()
        {

        }

        public override void Exit()
        {

        }
    }
    #endregion
}