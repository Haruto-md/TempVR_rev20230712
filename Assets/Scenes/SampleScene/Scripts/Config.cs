[System.Serializable]
public class ConfigData
{
    public string AI_Server_IP_Port;
    public API_Configs API_Configs;
}
[System.Serializable]
public class API_Configs
{
    public string AudioToText;
    public Chat Chat;
    public string TextToAudio;
    public string AudioCharacter;

}
[System.Serializable]
public class Chat
{
    public string ChatWhole;
    public string GetNewChat;
    public string InitialPrompt;
}