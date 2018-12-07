using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prevent : MonoBehaviour
{
    public int car_number;
    private bool canControl;
    private double x;
    Vector3 initScale, initpos;
    public float speed, brakingDistance, brake_Speed, initScaleY;
    // Use this for initialization
    void Start() //initial
    {
        speed = 0f;
        brakingDistance = 0f;
        x = 0;
        initScale = transform.localScale;
        initScaleY = initScale.y;
        initpos = transform.localPosition;
        brake_Speed = 0.9f;
        canControl = true;
    }
    void Update()
    {
        GetComponentInParent<ServerCar>().SetControl(canControl);
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

    void OnTriggerExit2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차랑 부딪히면 speed를 줄임
        {
            canControl = true;
        }
    }
}
