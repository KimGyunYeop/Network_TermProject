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
    struct carInfo
    {
        TcpClient clientSocket;
        bool isalive;
        public float x, y;
        public float speed;
        public float angle;

        public void setInfo(bool aisalive, float ax, float ay, float aspeed, float aangle)
        {
            isalive = aisalive;
            x = ax;
            y = ay;
            speed = aspeed;
            angle = aangle;
        }

        public void setSocket(TcpClient temp)
        {
            clientSocket = temp;
            isalive = true;
        }

        public void delInfo()
        {
            clientSocket = null;
            isalive = false;
        }

        public String toString()
        {
            return isalive + " " + x + " " + y + " " + speed + " " + angle;
        }
    }
    GameObject[] ServerCarObject;
    GameObject[] CarObject;
    carInfo[] carArr;
    carInfo[] ServerCarArr;
    bool[] carNumberUsed;
    public static int clientCount = 0;
    private readonly object thisLock = new object();
    public int Grade;


    void Start()
    {
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

        Thread acceptServer = new Thread(new ThreadStart(ListeningServer));//listening socket 생성
        acceptServer.IsBackground = true;
        acceptServer.Start();//listening 시작
    }
    void Update()
    {
        //차들 나타내주기
        for(int i=0; i<8; i++)
        {
            if (carNumberUsed[i]) CarObject[i].SetActive(true);//지금사용하고 있으면 나타내주고
            else CarObject[i].SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            //서버카 정보 넣기
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
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 5004);
        tcpListener.Start();

        while (true)
        {
            TcpClient client = tcpListener.AcceptTcpClient();//클라이언트가 접속하면
            Thread tcpThread = new Thread(new ParameterizedThreadStart(clientServer));//스레드생성
            tcpThread.Start(client);//스레드시작
        }
    }

    void clientServer(object newClient)
    {
        int carNumber;
        int i;
        TcpClient client = newClient as TcpClient;//소켓을불러옴
        NetworkStream stream = client.GetStream();
        StreamReader sr = new StreamReader(stream);
        StreamWriter sw = new StreamWriter(stream);


        if (clientCount >= 8)
        {
            sw.WriteLine("Sorry");
            sw.Flush();
        }
        else
        {
            sw.WriteLine("Start");
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

            try
            {
                carArr[carNumber].setSocket(client);
                sw.WriteLine(carNumber);
                sw.Flush();
                while (true)
                {
                    String msg = sr.ReadLine();//입력받기
                    if (string.Equals(msg, "End"))
                    {
                        Grade++;
                        sw.WriteLine(Grade);
                        sw.Flush();
                        break;
                    }
                    else
                    {
                        StringtoStructor(carNumber, msg);
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

    void StringtoStructor(int index, String a)
    {
        String[] arr = a.Split(' ');
        carArr[index].setInfo(Boolean.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
    }

    public String getCarinfo(int carNumber)
    {
        return carArr[carNumber].toString();
    }
}