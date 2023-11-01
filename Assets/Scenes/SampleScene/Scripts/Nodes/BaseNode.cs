using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using System.Net;

public class BaseNode<IInput,IOutput> : MonoBehaviour, IDataReceiver<IInput>
{
    public List<GameObject > _connectingNodes;
    public List<string> _characterList;
    private string _configFileName = "Chameba.json";
    int _characterCount = 0;

    protected ConfigData _config;
    protected string _filePath;
    public bool _debug = false;
    private void Start()
    {
        StartCoroutine(LoadConfig());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_characterList.Count > 0)
            {
                _characterCount++;
                if (_characterCount == _characterList.Count)
                {
                    _characterCount = 0;
                }
                _configFileName = _characterList[_characterCount]+".json";
                StartCoroutine(LoadConfig());
                if (_debug)
                {
                    Debug.Log("Change Config to "+ _characterList[_characterCount]);
                }
            }
        }
    }
    private IEnumerator LoadConfig()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        _filePath = Path.Combine(streamingAssetsPath, _configFileName);
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
            _connectingNodes.Add(node);
            if (_debug)
            {
                Debug.Log(this.gameObject.name +": Connects to :" + node.name);
            }
        }
    }

    public void Disconnect()
    {
        _connectingNodes.Clear();
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
        foreach (GameObject node in _connectingNodes)
        {
            var receiver = node.GetComponent<IDataReceiver<IOutput>>();
            receiver.ReceiveData(data);
        }
    }
}