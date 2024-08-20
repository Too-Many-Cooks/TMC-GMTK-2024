using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerBehavior : WalkerBehaviour
{
    [SerializeField][Range(0, 10)] protected float runningSpeed = 3;
    [SerializeField][Range(0, 10)] protected float durationOfWalk = 2;
    [SerializeField][Range(0, 10)] protected float durationOfRun = 2;

    bool running = false;
    Timer _myTimer;
    Timer MyTimer
    {
        get
        {
            if (_myTimer == null)
                _myTimer = new Timer(durationOfWalk);

            return _myTimer;
        }

        set => MyTimer = value;
    }

    protected override void Update()
    {
        base.Update();

        MyTimer.Update(Time.deltaTime);

        if (MyTimer.IsComplete)
        {
            running = !running;
            MyTimer.Reset();

            if (running)
                MyTimer.TargetTime = durationOfRun;
            else
                MyTimer.TargetTime = durationOfWalk;
        }
    }

    protected override void Move()
    {
        if (running)
            characterController.Move((-transform.position + playerTransform.position).normalized * runningSpeed * Time.deltaTime);
        else
            characterController.Move((-transform.position + playerTransform.position).normalized * walkingSpeed * Time.deltaTime);
    }
}
