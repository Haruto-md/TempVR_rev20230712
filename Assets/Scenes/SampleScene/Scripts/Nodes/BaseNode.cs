using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

public class BaseNode<IInput,IOutput> : MonoBehaviour, IDataReceiver<IInput>
{
    public List<GameObject > connectingNodes;
    public string _fileName = "test_config.json";
    protected ConfigData _config;
    protected string _filePath;
    public bool _debug = false;
    private void Start()
    {
        StartCoroutine(LoadConfig());
    }
    private IEnumerator LoadConfig()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        _filePath = Path.Combine(streamingAssetsPath, _fileName);
        using (UnityWebRequest www = UnityWebRequest.Get(_filePath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonContent = www.downloadHandler.text;
                _config = JsonUtility.FromJson<ConfigData>(jsonContent);
            }
            else
            {
                Debug.LogError("Failed to load Config.: " + www.error);
            }
        }
    }
    public void Connect(GameObject node)
    {
        if(node != null)
        {
            connectingNodes.Add(node);
            if (_debug)
            {
                Debug.Log(this.gameObject.name +": Connects to :" + node.name);
            }
        }
    }

    public void Disconnect()
    {
        connectingNodes.Clear();
    }
    public void ReceiveData(IInput data)
    {
        if (_debug)
        {
            Debug.Log(this.gameObject.name + " :Receive Data\n" + data);
        }
        StartCoroutine(ProcessData(data));
    }
    protected virtual IEnumerator ProcessData(IInput data)
    {
        return null;
    }
    protected void SendData(IOutput data)
    {
        if (_debug)
        {
            Debug.Log(this.gameObject.name + ": Send Data\n" + data);
        }
        foreach (GameObject node in connectingNodes)
        {
            var receiver = node.GetComponent<IDataReceiver<IOutput>>();
            receiver.ReceiveData(data);
        }
    }
}