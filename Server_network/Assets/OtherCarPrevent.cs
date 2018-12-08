//OtherCarPrevent
//하는일 : 자신 외에 다른 차들의 레드레인지를 계산하기 위한 함수.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCarPrevent : MonoBehaviour {

    private double x; //제동거리를 계산하기 위한 변수
    Vector3 initScale, initpos; //레드레인지를 위치 시키기위한 도구
    public float speed, brakingDistance, brake_Speed, initScaleY; 
    //speed는 스피드. brakingDistance는 제동거리, brake_speed는 브레이크를 밟을 때 줄어들도록 만든 일정의 float값, initScaleY는 y의 크기를 변환 시키기위한 값.
    // Use this for initialization
    void Start() //initial
    {
        speed = 0f;
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
        if (speed < 0) //스피드가 0보다 아래일 때는, 뒤로 가고 있으므로,
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z); //속도를 늘릴 수록 크기를 크게 해준다.
            initpos.Set(0, -(brakingDistance / 45), 0); //제동거리를 뒷부분에 위치.
        }
        else
        {
            //그반대.
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, brakingDistance / 45, 0);
        }

        //위치와 크기모두 바꿔줌.
        transform.localRotation = Quaternion.Euler(0, 0, 0); //rotation이 변하지 않도록 고정해주는 것.
        transform.localPosition = initpos;
        transform.localScale = initScale;
    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }
    private float Calculate_BrakingDistance() //제동거리 구하기 (적분 사용해서 구함)
    {
        if (Math.Abs(speed) < 0.5) return 0;
        x = Math.Log(1 / Math.Abs(speed), brake_Speed);
        return (float)(Math.Abs(speed) * ((Math.Pow((double)brake_Speed, x) / Math.Log((double)brake_Speed)) - (1 / Math.Log((double)brake_Speed))));
    }
}
