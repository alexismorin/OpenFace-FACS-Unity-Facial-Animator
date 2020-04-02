//https://github.com/valkjsaaa/Unity-ZeroMQ-Example
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;
using UnityEngine;

// connect with ZeroMQ
public class OpenFaceAnimatorListener {
    public string sub_to_ip;
    public string sub_to_port;
    private readonly Thread _listenerWorker;
    private bool _listenerCancelled;
    public delegate void MessageDelegate (List<string> msg_list);
    private readonly MessageDelegate _messageDelegate;
    private readonly ConcurrentQueue<List<string>> _messageQueue = new ConcurrentQueue<List<string>> ();
    //private string csv_folder = "Assets/Logging/";
    private string csv_path = "Assets/Logging/unity_timestamps_sub.csv";
    private StreamWriter csv_writer;
    private long msg_count;
    public OpenFaceAnimatorListener (string sub_to_ip, string sub_to_port) {
        this.sub_to_ip = sub_to_ip;
        this.sub_to_port = sub_to_port;
    }

    private void ListenerWork () {
        Debug.Log ("Setting up subscriber sock");
        AsyncIO.ForceDotNet.Force ();
        using (var subSocket = new SubscriberSocket ()) {
            // set limit on how many messages in memory
            subSocket.Options.ReceiveHighWatermark = 1000;
            // socket connection
            // subSocket.Connect("tcp://localhost:5572");
            subSocket.Connect ("tcp://" + sub_to_ip + ":" + sub_to_port);
            // subscribe to topics; "" == all topics
            subSocket.Subscribe ("");
            Debug.Log ("sub socket initiliased");

            string topic;
            //string frame;
            string timestamp;
            //string blend_shapes;
            //string head_pose;
            string facsvatar_json;
            while (!_listenerCancelled) {
                //string frameString;
                // wait for full message
                //if (!subSocket.TryReceiveFrameString(out frameString)) continue;
                //Debug.Log(frameString);
                //_messageQueue.Enqueue(frameString);

                List<string> msg_list = new List<string> ();
                if (!subSocket.TryReceiveFrameString (out topic)) continue;
                //if (!subSocket.TryReceiveFrameString(out frame)) continue;
                if (!subSocket.TryReceiveFrameString (out timestamp)) continue;
                //if (!subSocket.TryReceiveFrameString(out blend_shapes)) continue;
                //if (!subSocket.TryReceiveFrameString(out head_pose)) continue;
                if (!subSocket.TryReceiveFrameString (out facsvatar_json)) continue;

                //Debug.Log("Received messages:");
                //Debug.Log(frame);
                //Debug.Log(timestamp);
                //Debug.Log(facsvatar_json);

                // check if we're not done; timestamp is empty
                if (timestamp != "") {
                    msg_list.Add (topic);
                    msg_list.Add (timestamp);
                    msg_list.Add (facsvatar_json);
                    long timeNowMs = UnixTimeNowMillisec ();
                    msg_list.Add (timeNowMs.ToString ()); // time msg received; for unity performance

                    msg_count++;

                    _messageQueue.Enqueue (msg_list);
                }
                // done
                else {
                    Debug.Log ("Received all messages");
                }
            }
            subSocket.Close ();
        }
        NetMQConfig.Cleanup ();
    }

    public static long UnixTimeNowMillisec () {
        DateTime unixStart = new DateTime (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (DateTime.UtcNow - unixStart).Ticks;
        long timeNowMs = unixTimeStampInTicks / (TimeSpan.TicksPerMillisecond / 10000); // 100ns
        //Debug.Log(timeNowMs);
        return timeNowMs;
    }

    // check queue for messages
    public void Update () {
        while (!_messageQueue.IsEmpty) {
            List<string> msg_list;
            if (_messageQueue.TryDequeue (out msg_list)) {
                _messageDelegate (msg_list);
            } else {
                break;
            }
        }
    }

    // threaded message listener
    public OpenFaceAnimatorListener (MessageDelegate messageDelegate) {
        _messageDelegate = messageDelegate;
        _listenerWorker = new Thread (ListenerWork);
    }

    public void Start () {

        _listenerCancelled = false;
        _listenerWorker.Start ();
    }

    public void Stop () {
        _listenerCancelled = true;
        _listenerWorker.Join ();

    }
}

// act on messages received
public class OpenFaceAnimator : MonoBehaviour {
    private OpenFaceAnimatorListener oFListener;
    public string sub_to_ip = "127.0.0.1";
    public string sub_to_port = "5572";

    // logging
    private long msg_count;
    private string csv_folder = "Assets/Logging/";
    private string csv_path = "Assets/Logging/unity_timestamps_sub.csv";
    private StreamWriter csv_writer;
    private string csv_path_total = "Assets/Logging/unity_timestamps_total.csv";
    private StreamWriter csv_writer_total;

    public ActionUnitAnimator actionUnitAnimator;

    // Receive data from FACSvatar/OpenFace.
    private void HandleMessage (List<string> msg_list) {

        JObject facsvatar = JObject.Parse (msg_list[2]);
        // Action Unit Data
        JObject blend_shapes = facsvatar["au_r"].ToObject<JObject> ();

        UnityMainThreadDispatcher.Instance ().Enqueue (actionUnitAnimator.RequestBlendshapes (blend_shapes));

        msg_count++;
    }

    public static long UnixTimeNowMillisec () {
        DateTime unixStart = new DateTime (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (DateTime.UtcNow - unixStart).Ticks;
        long timeNowMs = unixTimeStampInTicks / (TimeSpan.TicksPerMillisecond / 10000); // 100ns
        //Debug.Log(timeNowMs);
        return timeNowMs;
    }

    private void Start () {

        oFListener = new OpenFaceAnimatorListener (HandleMessage);
        oFListener.sub_to_ip = sub_to_ip;
        oFListener.sub_to_port = sub_to_port;
        oFListener.Start ();
    }

    private void Update () {
        oFListener.Update ();
    }

    private void OnDestroy () {
        oFListener.Stop ();
    }
}