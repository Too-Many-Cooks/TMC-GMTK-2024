using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float _time;
    float _targetTime;

    public float Time
    {
        get { return _time; }

        set
        {
            if (value < 0)
            {
                Debug.LogWarning("Trying to set Timer's Time to a negative value (newTime == " + value + " secs)." +
                    "\nSetting Time value to 0.");
                _time = 0;
                return;
            }

            if(value > TargetTime)
            {
                Debug.LogWarning("Trying to set Timer's Time to a value bigger than the TargetTime " +
                    "(newTime == " + value + " secs; TargetTime == " + TargetTime + " secs)." +
                    "\nSetting Time value to the TargetTime.");
                _time = TargetTime;
                return;
            }

            _time = value;
        }
    }
    public float TargetTime
    {
        get { return _targetTime; }

        set
        {
            if (value <= 0)
            {
                Debug.LogWarning("Cannot set TargetTime to desired value (newTargetTime == " + value + " secs)." +
                    "\nTargetTime was not changed, and remains to be " + TargetTime + " secs.");
                return;
            }

            if(value < Time)
            {
                Debug.LogWarning("Setting TargetTime to a value lower than the current Time value." +
                    "\nTime value was also lowered to comply with the new TargetTime.");
                Time = value;
            }

            _targetTime = value;
        }
    }


        // Constructor.   

    public Timer(float targetTime, float initialTime = 0)
    {
        _targetTime = targetTime;
        _time = initialTime;


        if (initialTime < 0)
        {
            Debug.LogError("Timer cannot be created with an initialTime of " + initialTime + " secs." +
            "\nThe initialTime must be positive.");
            initialTime = 0;
        }

        if (targetTime <= 0 || targetTime < initialTime)
        {
            Debug.LogError("Timer cannot be created with a targetTime of " + targetTime + " secs." +
            "\nThe targetTime must be positive and bigger than the initialTime (or, in its absence, bigger than 0).");

            if (targetTime == 0)
                _targetTime = 0.1f;
            else
                _targetTime = initialTime;

            Debug.LogWarning("Timer was created with TargetTime == " + _targetTime + " secs.");
        }
    }


        // Methods.

    /// <summary>
    /// Updates the internal Time by ADDING Time.deltaTime. 
    /// If the Time value surpases TargetTime, it sets its value to the TargetTime.
    /// </summary>
    public void Update()
    {
        if (IsComplete)
            return;
        
        _time += UnityEngine.Time.deltaTime;

        if (_time > _targetTime)
            _time = _targetTime;
    }

    /// <summary>
    /// Updates the internal Time by ADDING the desired ammount of time. 
    /// If the Time value surpases TargetTime, it sets its value to the TargetTime.
    /// </summary>
    /// <param name="timeToAdd">Ammount of time to be ADDED to the MyTimer.</param>
    public void Update(float timeToAdd)
    {
        if (IsComplete)
            return;

        _time += timeToAdd;

        if (_time > _targetTime)
            _time = _targetTime;
    }

    /// <summary>
    /// Updates the internal Time by SUBSTRACTING Time.deltaTime. 
    /// If the Time value becomes less than 0, it sets its value to 0.
    /// </summary>
    public void NegativeUpdate()
    {
        if (Time == 0)
            return;

        _time -= UnityEngine.Time.deltaTime;

        if (_time < 0)
            _time = 0;
    }

    /// <summary>
    /// Updates the internal Time by SUBSTRACTING the desired ammount of time. 
    /// If the Time value becomes less than 0, it sets its value to 0.
    /// </summary>
    /// <param name="timeToSubstract">Ammount of time to be SUBSTRACTED from the MyTimer.</param>
    public void NegativeUpdate(float timeToSubstract)
    {
        if (Time == 0)
            return;

        _time -= timeToSubstract;

        if (_time < 0)
            _time = 0;
    }

    /// <summary>
    /// Sets the timer's Time value to 0.
    /// </summary>
    public void Reset()
    {
        Time = 0;
    }


        // Properties

    public bool IsComplete
    {
        get
        {
            if (Time == TargetTime)
                return true;
            else
                return false;
        }
    }

    public float PercentageComplete { get { return Time / TargetTime; } }
}
