using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;


namespace Midbaryom.Core
{
    public class NetworkManager : MonoBehaviour
    {
        private string LocalIPAddress = "localhost";
        private int ListeningPort = 13000;
        private int SendingPort = 40001;
        Thread listener;
        static Queue pQueue = Queue.Synchronized(new Queue()); //this is the message queue, it is thread safe
        static UdpClient udp;
        private IPEndPoint endPoint;

        private void Start()
        {
            StartUDP();
        }

        private void Update()
        {
            //in the main thread, read the message and update the game manager
            lock (pQueue.SyncRoot)
            {
                if (pQueue.Count > 0)
                {
                    object o = pQueue.Dequeue(); //Take the olders message out of the queue
                    ZedInputHandler.HandleInput((string)o);
                }

                else
                {
                    //ZedInputHandler.HandleNoInput();
                }
            }
        }

        private void OnDestroy()
        {
            EndUDP();
        }

        public void StartUDP()
        {
            endPoint = new IPEndPoint(IPAddress.Any, ListeningPort); //this line will listen to all IP addresses in the network
                                                                     //endPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress), ListeningPort); //this line will listen to a specific IP address
            udp = new UdpClient(endPoint);
            Debug.Log("Listening for Data...");
            listener = new Thread(new ThreadStart(MessageHandler));
            listener.IsBackground = true;
            listener.Start();
        }

        void MessageHandler()
        {
            Byte[] data = new byte[0];
            while (true)
            {
                try
                {
                    //Did we get a new message?
                    data = udp.Receive(ref endPoint);
                }
                catch (Exception err)
                {
                    //If there's a problem
                    Debug.Log("Communication error, recieve data error " + err);
                    udp.Close();
                    return;
                }
                //Treat the new message
                string msg = Encoding.ASCII.GetString(data);
                //Debug.Log("UDP incoming " + msg);
                pQueue.Enqueue(msg);
            }
        }

        private void EndUDP()
        {
            if (udp != null)
            {
                udp.Close();
            }
            if (listener != null)
            {
                listener.Abort();
            }
        }

        public void SendMessage(string message)
        {
            UdpClient send_client = new UdpClient();
            IPEndPoint send_endPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress), SendingPort);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            send_client.Send(bytes, bytes.Length, send_endPoint);
            send_client.Close();
            Debug.Log("Sent message: " + message);
        }
    }
}
