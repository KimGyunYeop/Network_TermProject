using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCarPrevent : MonoBehaviour {

    private int car_number, canControl;
    private double x;
    Vector3 initScale, initpos;
    public float speed, pre_brakingDistance, brakingDistance, brake_Speed, initScaleY;
    // Use this for initialization
    void Start() //initial
    {
        speed = 0f;
        pre_brakingDistance = 0f;
        brakingDistance = 0f;
        x = 0;
        initScale = transform.localScale;
        initScaleY = initScale.y;
        initpos = transform.localPosition;
        brake_Speed = 0.8f;
    }
    void Update()
    {
        brakingDistance = Calculate_BrakingDistance(); //현재 브레이크 거리 세팅
        if (speed < 0)
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, -(brakingDistance / 45), 0);
        }
        else
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, brakingDistance / 45, 0);
        }
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localPosition = initpos;
        transform.localScale = initScale;
    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }
    private float Calculate_BrakingDistance() //제동거리 구하기
    {
        if (Math.Abs(speed) < 0.5) return 0;
        x = Math.Log(1 / Math.Abs(speed), brake_Speed);
        return (float)(Math.Abs(speed) * ((Math.Pow((double)brake_Speed, x) / Math.Log((double)brake_Speed)) - (1 / Math.Log((double)brake_Speed))));
    }
}
