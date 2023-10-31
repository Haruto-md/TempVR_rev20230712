using UnityEngine;
public class BaseNode<IInput,IOutput> : MonoBehaviour, IDataReceiver<IInput>
{
    public ConfigData _config;
    void Start()
    {
        if(_config == null){
            _config = GameObject.Find("InitializationManager").GetComponent<InitializationManager>().config_data;
        }
    }
    public List<IDataReceiver<IOutput>> _connectTo;
    public bool _debug = false;
    public void Connect(IDataReceiver<IOutput> node)
    {
        _connectTo.Add(node);
    }
    public void Connect(List<IDataReceiver<IOutput>> nodes)
    {
        _connectTo.AddRange(nodes);
    }

    public void Disconnect()
    {
        _connectTo.Clear();
    }
    public void ReceiveData(IInput data)
    {
        StartCoroutine(ProcessData(data));
    }
    protected vertual IEnumerator ProcessData(IInput data)
    {
        // var processedData = data
        // //例えば TextToAudio string dataなど。継承クラスで記述
        // yield return null;//requestなど
        // const clip = AudioClip.Create();
        // SendData(clip);
    }
    protected void SendData(IOutput data)
    {
        foreach(node in _connectTo)
        {
            node.ReceiveData(data);
        }
    }
}