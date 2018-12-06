using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prevent : MonoBehaviour
{
    GameObject Car;
    private int car_number, canControl;
    private double x;
    Vector3 initScale, initpos;
    public float speed, pre_brakingDistance, brakingDistance, brake_Speed, initScaleY;
    float result;
    // Use this for initialization
    void Start() //initial
    {
        Car = GameObject.Find("car_main");
        speed = 0f;
        result = 0f;
        pre_brakingDistance = 0f;
        brakingDistance = 0f;
        x = 0;
        initScale = transform.localScale;
        initScaleY = initScale.y;
        initpos = transform.localPosition;
        brake_Speed = Car.GetComponent<Carspeed>().getBrakeSpeed();
        brake_Speed = 0.9f;
    }
    void Update()
    {
        brakingDistance = Calculate_BrakingDistance(); //현재 브레이크 거리 세팅
        if (speed < 0)
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, -(brakingDistance / 50), 0);
        }
        else
        {
            initScale.Set(initScale.x, initScaleY + Math.Abs(speed) / 20, initScale.z);
            initpos.Set(0, brakingDistance / 50, 0);
        }
        transform.localPosition = initpos;
        transform.localScale = initScale;
    }
    public void SetSpeed(float Carspeed)
    {
        this.speed = Carspeed;
    }
    private float Calculate_BrakingDistance() //제동거리 구하기
    {
        if (0 <= speed && speed < 1) return 0;
        x = Math.Log(1 / Math.Abs(speed), brake_Speed);
        return (float)(Math.Abs(speed) * ((Math.Pow((double)brake_Speed, x) / Math.Log((double)brake_Speed)) - (1 / Math.Log((double)brake_Speed))));
    }
    void OnTriggerEnter2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차랑 부딪히면 speed를 줄임
        {
            speed *= brake_Speed;
            canControl = 0;
        }
        if (others.gameObject.tag == "Line") //선이랑 부딪히면 
        {

            if (Math.Abs(speed) > 0.001f) //만약 거의 0이 아니면
            {
                speed *= brake_Speed; //속도를 줄임
            }
            else speed = 0;
            canControl = 0;
        }
        Car.GetComponent<Carspeed>().SetSpeed(speed);//차의 속도를 바꿔줌
    }
    void OnTriggerStay2D(Collider2D others) //enter이랑 똑같
    {
        if (others.gameObject.tag == "Car")
        {
            speed *= brake_Speed;
            canControl = 0;
        }
        if (others.gameObject.tag == "Line")
        {
            if (Math.Abs(speed) > 0.001f)
            {
                speed *= brake_Speed;
            }
            else speed = 0;
            canControl = 0;
        }
        Car.GetComponent<Carspeed>().SetSpeed(speed);
    }

    void OnTriggerExit2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차랑 부딪히면 speed를 줄임
        {
            canControl = 1;
        }
        if (others.gameObject.tag == "Line") //선이랑 부딪히면 
        {
            canControl = 1;
        }
        Car.GetComponent<Carspeed>().SetSpeed(speed);//차의 속도를 바꿔줌
    }
}