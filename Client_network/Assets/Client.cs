//Client 
//하는 일 : 서버와 정보를 주고 받고 받은 정보로 차들을 배치시키는 전체적인 역할을 함.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    /*struct carInfo 
    하는 일 : 차들의 정보 저장*/
    struct carInfo
    {
        public bool isalive; //사용중인가를 알려주는 변수
        public float x, y; //lotation x와 y
        public float speed; //차의 speed.
        public float angle; //차의 각도 (direction)

        public void setInfo(bool aisalive, float ax, float ay, float aspeed, float aangle)
        {
            this.isalive = aisalive;
            this.x = ax;
            this.y = ay;
            this.speed = aspeed;
            this.angle = aangle;
        }

        public void setIsalive()
        {
            isalive = true;
        }

        public void delInfo()
        {
            isalive = false;
        }

        public String toString() //쉽게 보내기 위해 만든 스트링 전환 함수
        {
            return isalive + " " + x + " " + y + " " + speed + " " + angle;
        }
    }
    TcpClient client; //tcp클라이언트
    carInfo carStat; //자신의 차 정보
    NetworkStream stream; //네트워크 스트림
    GameObject[] CarObject; //다른 차의 오브젝트들 (자신포함)
    GameObject[] ServerCarObject; //서버카 오브젝트
    int Carnumber, start_first; //자신의 차 number과 처음 시작을 알려주는 변수
    public static GameObject car; //자신의 차 오브젝트
    Thread clientthr;
    StreamWriter sw;//쉽게 쓰기위한 변수
    StreamReader sr; //쉽게 읽기위한 변수
    carInfo[] ServerCarArr;//서버카 정보
    carInfo[] carArr; //다른 차의 정보들 (자신포함)
    public int Grade; //점수
    public bool isEnd, ChangeEndScene, ChangeNoScene; //끝났을 경우, endscence를 바꿔야하는 경우, 들어가지 못하는 상황을 위한 변수들
    // Use this for initialization
    void Start()
    {
        //모두 initial
        ChangeNoScene = false;
        ChangeEndScene = false;
        isEnd = false;
        ServerCarArr = new carInfo[2];
        for (int index = 0; index < 2; index++)//initalize
        {
            ServerCarArr[index].delInfo();
            ServerCarArr[index].isalive = false;
        }
        carArr = new carInfo[8];
        carStat.setIsalive();
        for (int index = 0; index < 8; index++)//initalize
        {
            carArr[index].delInfo();
        }
        CarObject = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            CarObject[i] = GameObject.Find("OtherCar" + i);
        }
        ServerCarObject = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            ServerCarObject[i] = GameObject.Find("ServerCar" + i);
        }
        car = GameObject.Find("car_main");
        start_first = 0;
        //initial끝.

        //클라이언트 생성
        client = new TcpClient("127.0.0.1", 5004);//일단 시험용으로 자신에게 접속(소켓새성)
        clientthr = new Thread(new ParameterizedThreadStart(clientThread)); //서버에게 계속 정보를 받기위해 스레드 생성.
        clientthr.Start(client);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnd && ChangeEndScene) //만약 골했다면
        {
            GameObject.Find("Grade").GetComponent<Grade>().grade = Grade; //점수를 넣고
            SceneManager.LoadScene("End"); //끝 화면으로 바꿔줌.
        }
        if (ChangeNoScene) //만약 게임화면으로 들어갈수 없다면,
        {
            SceneManager.LoadScene("endScence"); //못들어가는 씬으로 바꿔줌.
        }
        //처음 시작 판단 후 넘버에 따른 첫 위치 지정해주기.
        if (start_first == 1)
        {
            start_first = 0;
            SetFirstLocation();
        }

        //사용중인 차들 나타내주기
        for (int i = 0; i < 8; i++)
        {
            if (carArr[i].isalive && i != Carnumber) CarObject[i].SetActive(true);
            else CarObject[i].SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            ServerCarObject[i].SetActive(true);
        }
        //현재차 정보 넣기
        carStat.setInfo(true, car.transform.position.x, car.transform.position.y, car.GetComponent<Car>().speed, car.transform.localEulerAngles.z);
        //서버카 위치 수정
        for (int i = 0; i < 2; i++)
        {
            if (ServerCarArr[i].isalive)
            {
                ServerCarObject[i].transform.position = new Vector3(ServerCarArr[i].x, ServerCarArr[i].y, 0);
                ServerCarObject[i].transform.localRotation = Quaternion.Euler(0, 0, ServerCarArr[i].angle);
                ServerCarObject[i].GetComponentInChildren<OtherCarPrevent>().SetSpeed(ServerCarArr[i].speed);
            }
        }
        //다른카 위치 수정
        for (int i = 0; i < 8; i++)
        {
            if (carArr[i].isalive && i != Carnumber)
            {
                CarObject[i].transform.position = new Vector3(carArr[i].x, carArr[i].y, 0);
                CarObject[i].transform.localRotation = Quaternion.Euler(0, 0, carArr[i].angle);
                CarObject[i].GetComponentInChildren<OtherCarPrevent>().SetSpeed(carArr[i].speed);
            }
        }
    }

    //서버와 정보를 주고받는 함수이자 스레드.
    void clientThread(object temp)
    {
        client = temp as TcpClient;
        stream = client.GetStream();
        sw = new StreamWriter(stream);
        sr = new StreamReader(stream);

        String CanGoString = sr.ReadLine(); //허락을 받고
        if (String.Equals(CanGoString, "Sorry")) { ChangeNoScene = true; } //못들어가면 변수를 바꿈
        else if (String.Equals(CanGoString, "Start"))
        {
            Carnumber = int.Parse(sr.ReadLine()); //자신의 번호를 받음.
            start_first = 1;

            while (true)
            {
                if (isEnd) //만약 끝났다면
                {
                    sw.WriteLine("End"); //서버에게 끝을 보고함.
                    sw.Flush();
                    Grade = int.Parse(sr.ReadLine()); //점수를 받고
                    ChangeEndScene = true; //씬을 바꿔줌.
                    break;
                }
                else
                {
                    //자신의 차 정보를 서버에게 보냄
                    sw.WriteLine(carStat.toString());
                    sw.Flush();

                    //그 대가로 다른 차들의 정보를 받음.
                    for (int i = 0; i < 2; i++)
                    {
                        String msg = sr.ReadLine();//입력받기
                        int num = int.Parse(msg.Substring(0, 1));
                        StringtoStructor(num, msg.Substring(1), 0);
                    }
                    for (int index = 0; index < 8; index++)
                    {
                        String msg = sr.ReadLine();//입력받기
                        int num = int.Parse(msg.Substring(0, 1));
                        StringtoStructor(num, msg.Substring(1), 1);
                    }
                    Thread.Sleep(20);
                }
            }
        }
        stream.Close();
        client.Close();
        sw.Close();
        sr.Close();
    }
    //차의 첫 위치를 결정해주는 함수.
    void SetFirstLocation()
    {
        if (Carnumber == 0) { car.transform.localPosition = new Vector3(-8269, 4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, -90); }
        else if (Carnumber == 1) { car.transform.localPosition = new Vector3(-3160, 6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 2) { car.transform.localPosition = new Vector3(3160, 6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 3) { car.transform.localPosition = new Vector3(8269, 4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 90); }
        else if (Carnumber == 4) { car.transform.localPosition = new Vector3(-8269, -4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, -90); }
        else if (Carnumber == 5) { car.transform.localPosition = new Vector3(-3160, -6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 6) { car.transform.localPosition = new Vector3(3160, -6369, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        else if (Carnumber == 7) { car.transform.localPosition = new Vector3(8269, -4590, 0); car.transform.localRotation = Quaternion.Euler(0, 0, 90); }
    }

    //쉽게 스트링을 스트럭쳐로 바꿔주기위한 함수.
    void StringtoStructor(int index, String a, int inputArr)
    {
        String[] arr = a.Split(' ');
        if (inputArr == 0) //0은 서버카 정리
            ServerCarArr[index].setInfo(Boolean.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
        if (inputArr == 1) //1은 다른 차들 위치 정리
            carArr[index].setInfo(Boolean.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
    }


    void Application_ApplicationExit(object sender, EventArgs e)
    {
        try
        {
            clientthr.Abort();
        }
        catch { }
    }

    void OnApplicationQuit()//어플리케이션이 꺼지면 소켓 ,스트림, 스레드 모두종료
    {
        carStat.delInfo();
        stream.Close();
        client.Close();
        sw.Close();
        sr.Close();
        clientthr.Abort();
    }
    public String getCarinfo(int carNumber)
    {
        return carArr[carNumber].toString();
    }
}