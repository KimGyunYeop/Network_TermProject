//Prevent
//하는 일: 서버카가 차와 만낫을 때, 멈출 수 있도록 도와주는 , 레드레인지를 계산해주는 클래스.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prevent : MonoBehaviour
{
    private bool canControl; //컨트롤을 막기 위한 변수
    private double x;  //제동거리를 계산하기 위한 변수
    Vector3 initScale, initpos;//레드레인지를 위치 시키기위한 도구
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
        canControl = true;
    }
    void Update()
    {
        GetComponentInParent<ServerCar>().SetControl(canControl); //매번 canControl을 변환시켜준다.
        brakingDistance = Calculate_BrakingDistance(); //현재 브레이크 거리 세팅
        if (speed < 0)
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, brakingDistance / 45, 0);
        } //차가 뒤로 가고 있으면, 변화량을 음수로 둬야한다
        else
        {
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
    private float Calculate_BrakingDistance() //제동거리 구하기 (적분을 이용함)
    {
        if (speed < 1) return 0;
        x = Math.Log(1 / speed, brake_Speed);
        return (float)(speed * ((Math.Pow((double)brake_Speed, x) / Math.Log((double)brake_Speed)) - (1 / Math.Log((double)brake_Speed))));
    }
    void OnTriggerEnter2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차랑 부딪히면 speed를 줄임
        {
            if (Math.Abs(speed) > 0.1f) //만약 거의 0이 아니면
            {
                speed *= (float)brake_Speed; //속도를 줄임
            }
            else speed = 0;
            canControl = false;
        }

        this.GetComponentInParent<ServerCar>().SetSpeed(speed);//차의 속도를 바꿔줌
    }
    void OnTriggerStay2D(Collider2D others) //enter이랑 똑같
    {
        if (others.gameObject.tag == "Car")
        {
            if (Math.Abs(speed) > 0.1f) //만약 거의 0이 아니면
            {
                speed *= (float)brake_Speed; //속도를 줄임
            }
            else speed = 0;
            canControl = false;
        }
        this.GetComponentInParent<ServerCar>().SetSpeed(speed);
    }

    void OnTriggerExit2D(Collider2D others) //만약 게임 오브젝트가 부딪힌걸 나오면
    {
        if (others.gameObject.tag == "Car")
        {
            canControl = true;//다시 컨트롤할수 있게해줌.
        }
    }
}
