using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion
    DatabaseReference reference;
    public GameObject UsernamePromptPanel;
    public InputField inputtext;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("Username", "sad") == "sad")
        {
            UsernamePromptPanel.SetActive(true);
        }
        else
        {
            UserManager.instance.USERNAME = PlayerPrefs.GetString("Username", "sad");
        }
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://travemo.firebaseio.com");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    
    public void SetUsername()
    {
        PlayerPrefs.SetInt("FirstOpenn", 0);
        PlayerPrefs.SetString("Username", inputtext.text);
        UserManager.instance.USERNAME = inputtext.text;
    }
    public void NewMessage()
    {
        ChatListManager.instance.ChatListPanel.SetActive(false);
        ChatManager.instance.OpenChat(GameObject.Find("NewChatPrompt").transform.GetChild(0).GetChild(1).GetComponent<InputField>().text);
    }
    public void SendMsg(string to, string from, string type, string msg)
    {
        string key = reference.Child("Messages").Child(UserManager.instance.USERNAME).Push().Key;

        MessageItem user = new MessageItem(to, from, "received", msg, key);
        string json = JsonUtility.ToJson(user);
        reference.Child("Messages").Child(to).Child(key).SetRawJsonValueAsync(json);

        user = new MessageItem(to, from, "sent", msg, key);
        json = JsonUtility.ToJson(user);

        reference.Child("Messages").Child(from).Child(key).SetRawJsonValueAsync(json);
        ChatListItem clm = new ChatListItem(to, "sent", msg, DateTime.UtcNow.ToString());
        json = JsonUtility.ToJson(clm);
       
        reference.Child("MessageList").Child(to).Child(from).SetRawJsonValueAsync(json);
        reference.Child("MessageList").Child(to).Child(from).Child("time").SetValueAsync(Firebase.Database.ServerValue.Timestamp);

        ChatListItem clma = new ChatListItem(to, "received", msg, DateTime.UtcNow.ToString());
        json = JsonUtility.ToJson(clma);

        reference.Child("MessageList").Child(from).Child(to).SetRawJsonValueAsync(json);
        reference.Child("MessageList").Child(from).Child(to).Child("time").SetValueAsync(Firebase.Database.ServerValue.Timestamp);
    }

}
