using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static TextToChatNode;
using Random = System.Random;

public class TextToChatNode : BaseNode<string,string>
{
    private string _tempChatResponse;
    private string _wholeChatResponse;
    public bool isSequencial = true;
    protected override IEnumerator ProcessData(string prompt)
    {
        yield return ChatAPI(prompt);
    }
    public IEnumerator ChatAPI(string prompt)
    {
        var form = new WWWForm();

        form.AddField("role_setting", "user");
        form.AddField("content_setting", _config.API_Configs.Chat.InitialPrompt);
        form.AddField("role", "user");
        form.AddField("content", prompt);

        var token = GenerateToken();
        form.AddField("temp_token", token);

        string url = "https://" + _config.AI_Server_IP_Port + _config.API_Configs.Chat.ChatWhole;

        // UnityWebRequestを作成し、FormDataを設定
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        if (isSequencial)
        {
            StartCoroutine(GetNewChat(token));
        }

        yield return request.SendWebRequest();
        //Send Data to the next nodes
        string chatResponseJsonText = request.downloadHandler.text;
        ChatResponse chatResponseJson = JsonUtility.FromJson<ChatResponse>(chatResponseJsonText);
        _wholeChatResponse = chatResponseJson.resulting_sentences;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (!isSequencial)
            {
                SendData(_wholeChatResponse);
            }
        }
    }
    [System.Serializable]
    public class ChatResponse
    {                    
        public string resulting_sentences;
        public string finish_reason;
    }
    private IEnumerator GetNewChat(string token)
    {
        var form = new WWWForm();
        form.AddField("token", token);
        while (true)
        {

            string url = "https://" + _config.AI_Server_IP_Port + _config.API_Configs.Chat.GetNewChat;

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();
            if(request.result == UnityWebRequest.Result.ConnectionError)
            {
                //Debug.Log(this.gameObject.name + ": Connection Error");
                break;
            }
            string _tempDeltaResponse = request.downloadHandler.text;
            var deltaJsonResponse = JsonUtility.FromJson<DeltaResponse>(_tempDeltaResponse);
            if(deltaJsonResponse.state == "Wait")
            {
                Debug.Log(deltaJsonResponse.content.response);
                yield return new WaitForSeconds(0.5f);
            }
            else if(deltaJsonResponse.state == "Success")
            {
                if (deltaJsonResponse.content.response == "END")
                {
                    break;
                }
                else
                {
                    _tempChatResponse = deltaJsonResponse.content.response;
                    SendData(_tempChatResponse);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
    [System.Serializable]
    public class DeltaResponse
    {
        public string state;
        public Content content;
    }
    [System.Serializable]
    public class Content
    {
        public string response;
        public int response_index;
    }
    public IEnumerator ChatAPI(List<string> messages,string prompt)
    {
        int message_num = 0;
        var form = new WWWForm();
        foreach (string content in messages)
        {
            message_num++;
            form.AddField("role"+ message_num, "user");
            form.AddField("content"+ message_num, content);
            form.AddField("role" + message_num, "assistant");
            form.AddField("content" + message_num, content);
        }
        form.AddField("role" + message_num+1, "user");
        form.AddField("content" + message_num+1, prompt);
        var token = GenerateToken();
        form.AddField("temp_token", token);
        if (isSequencial)
        {
            StartCoroutine(GetNewChat(token));
        }
        string url = "https://" + _config.AI_Server_IP_Port + _config.API_Configs.Chat.ChatWhole;
        // UnityWebRequestを作成し、FormDataを設定
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        // リクエストを送信し、レスポンスを待機
        yield return request.SendWebRequest();
        //Send Data to the next nodes
        _wholeChatResponse = request.downloadHandler.text;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (!isSequencial)
            {
                SendData(_wholeChatResponse);
            }
        }
    }

    public static string GenerateToken()
    {
        // 現在の時刻をミリ秒単位で取得
        long currentTimeMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // ランダムな2桁の数字を生成
        Random random = new Random();
        int randomDigits = random.Next(10, 99);

        // 10桁のトークンを生成
        string token = currentTimeMillis.ToString() + randomDigits.ToString();

        return token;
    }
}
