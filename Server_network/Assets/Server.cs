//Server
//하는일 : Client와 정보를 주고 받으며, 받은 정보의 차 위치를 나타내주고 전체적으로 관리하는 서버의 심장이다.
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;

class Server : MonoBehaviour
{
    /*struct carInfo 
    하는 일 : 차들의 정보 저장*/
    struct carInfo
    {
        bool isalive; //사용중인가를 알려주는 변수
        public float x, y; //lotation x와 y
        public float speed; //차의 speed.
        public float angle; //차의 각도 (direction)

        public void setInfo(bool aisalive, float ax, float ay, float aspeed, float aangle) //정보 세팅.
        {
            isalive = aisalive;
            x = ax;
            y = ay;
            speed = aspeed;
            angle = aangle;
        }

        public void setIsalive() //isalive 세팅.
        {
            isalive = true;
        }

        public void delInfo() //모든 정보를 지우는 것.
        {
            isalive = false;
        }

        public String toString() //쉽게 보내기 위해 만든 스트링 전환 함수
        {
            return isalive + " " + x + " " + y + " " + speed + " " + angle;
        }
    }

    //Server 클래스가 공유하는 변수들
    GameObject[] ServerCarObject; //Server카의 오브젝트
    GameObject[] CarObject; //Client카들의 오브젝트
    carInfo[] carArr; //Client카들의 data.
    carInfo[] ServerCarArr; //Server카들의 data.
    bool[] carNumberUsed; //현재 사용되고 있는 carNumber을 구분하기 위해 만든 변수.
    public static int clientCount = 0; //8명이상일때를 처리하기 위한 변수.
    private readonly object thisLock = new object(); //client count가 동시에 들어올 때를 생각한 rock변수
    public int Grade; //점수


    void Start()
    {
        //전부 초반 세팅 (initial)
        Grade = 0;
        ServerCarObject = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            ServerCarObject[i] = GameObject.Find("ServerCar" + i);
        }
        CarObject = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            CarObject[i] = GameObject.Find("OtherCar" + i);
        }
        carArr = new carInfo[8];
        ServerCarArr = new carInfo[2];
        carNumberUsed = new bool[8];
        for (int index = 0; index < 8; index++)//initalize
        {
            carNumberUsed[index] = false;
            carArr[index].delInfo();
        }
        //initial 끝.

        Thread acceptServer = new Thread(new ThreadStart(ListeningServer));//listening socket 생성 && 그것을 처리할 새로운 thread 만들어냄
        acceptServer.IsBackground = true;
        acceptServer.Start();//listening 시작
    }
    void Update()
    {
        //차들 나타내주기
        for (int i = 0; i < 8; i++)
        {
            if (carNumberUsed[i]) CarObject[i].SetActive(true);//지금사용하고 있으면 나타내주고
            else CarObject[i].SetActive(false); //사용하지 않으면 안나타낸다.
        }
        //서버카 정보 넣기
        for (int i = 0; i < 2; i++)
        {
            ServerCarArr[i].setInfo(true, ServerCarObject[i].transform.position.x, ServerCarObject[i].transform.position.y, ServerCarObject[i].GetComponent<ServerCar>().speed, ServerCarObject[i].transform.localEulerAngles.z);
        }
        //차들 모두 업뎃 및 서버차 정보 업뎃
        for (int i = 0; i < 8; i++)
        {
            if (carNumberUsed[i])
            {

                CarObject[i].transform.position = new Vector3(carArr[i].x, carArr[i].y, 0);
                CarObject[i].transform.localRotation = Quaternion.Euler(0, 0, carArr[i].angle);
                CarObject[i].GetComponentInChildren<OtherCarPrevent>().SetSpeed(carArr[i].speed);
            }
        }
    }

    void ListeningServer()//listing socket
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 5004); //tcp listener생성
        tcpListener.Start();

        while (true)
        {
            TcpClient client = tcpListener.AcceptTcpClient();//클라이언트가 접속하면
            Thread tcpThread = new Thread(new ParameterizedThreadStart(clientServer));//각 클라이언트들의 스레드생성
            tcpThread.Start(client);//각 클라이언트들의 스레드시작
        }
    }

    void clientServer(object newClient)
    {
        int carNumber;
        int i;
        TcpClient client = newClient as TcpClient;//소켓을불러옴
        NetworkStream stream = client.GetStream(); //네트워크 스트림
        StreamReader sr = new StreamReader(stream); //스트림 공유를 편하게 하기위한 도구
        StreamWriter sw = new StreamWriter(stream); //위와 같음


        if (clientCount >= 8) //클라이언트가 8명보다 많게되면 맵 특성상 넣을 공간이 없으므로
        {
            sw.WriteLine("Sorry"); //거절을 보냄.
            sw.Flush();
        }
        else
        {
            sw.WriteLine("Start"); //시작을 보냄.
            sw.Flush();
            lock (thisLock)//해당차의 번호를 할당하는 syncronization
            {
                for (i = 0; i < 8; i++)
                {
                    if (carNumberUsed[i] == false) break;
                }
                carNumberUsed[i] = true;
                carNumber = i;
                clientCount++;
            }

            //소켓 시작
            try
            {
                carArr[carNumber].setIsalive();
                sw.WriteLine(carNumber);  //차 넘버를 보내줌.
                sw.Flush();
                while (true)
                {
                    String msg = sr.ReadLine();//메세지 입력받기

                    if (string.Equals(msg, "End")) //만약 클라이언트가 도착지에 도착했음을 보내면
                    {
                        Grade++; //등수를 올려준뒤
                        sw.WriteLine(Grade); //보내줌.
                        sw.Flush();
                        break;
                    }
                    else
                    {
                        StringtoStructor(carNumber, msg); //메세지를 스트럭쳐로 쉽게 전환시킴

                        //받은 대가로 모든 차의 정보들을 보내주기.
                        for (int index = 0; index < 2; index++)
                        {
                            sw.WriteLine(index + ServerCarArr[index].toString());
                            sw.Flush();
                        }
                        for (int index = 0; index < 8; index++)
                        {
                            sw.WriteLine(index + carArr[index].toString());
                            sw.Flush();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            //통신이 종료되었을때
            lock (thisLock)//syncronization
            {
                carNumberUsed[carNumber] = false;//사용하지않는 카넘버로 지정
                carArr[carNumber].delInfo();//카정보 삭제
                clientCount--;//카운트 떨구기
            }
            sr.Close();
            sw.Close();
            stream.Close();
            client.Close();
        }
    }

    /*스트링을 스트럭쳐로 쉽게 전환하기 위한 함수*/
    void StringtoStructor(int index, String a)
    {
        String[] arr = a.Split(' ');
        carArr[index].setInfo(Boolean.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
    }
}