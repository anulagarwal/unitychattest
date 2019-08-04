using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
public class ChatManager : MonoBehaviour
{
    #region Singleton
    public static ChatManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Found multiple TileManagers! Destroying instance.");
            Destroy(gameObject);
        }
    }

    #endregion
    [SerializeField]
    GameObject MessageReceived;

    [SerializeField]
    InputField inputtext;

    [SerializeField]
    GameObject MessageSent;

    [SerializeField]
    GameObject ChatPanel;
    bool IsDataReady;
    bool IsIncomingDataReady;
    IEnumerable<DataSnapshot> data;
    IEnumerable<DataSnapshot> data2;
    string UserNameChat;
    public Text ChatHeader;
    public InputField textField;
    public List<MessageItem> msgList = new List<MessageItem>();
    public List<GameObject> ml;
    public ScrollRect scroll;
    TouchScreenKeyboard keyboard;
    // Start is called before the first frame update
    void Start()
    {
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        FirebaseDatabase.DefaultInstance
     .GetReference("Messages").Child(UserManager.instance.USERNAME)
     .GetValueAsync().ContinueWith(task =>
     {
         if (task.IsFaulted)
         {
             // Handle the error...
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             IsIncomingDataReady = true;
             data2 = snapshot.Children;
         }
     });

    }
    public void DecryptIncomingData()
    {
        List<MessageItem> temp = new List<MessageItem>();
        foreach (DataSnapshot user in data2)
        {
                temp.Add(new MessageItem(user.Child("to").Value.ToString(), user.Child("from").Value.ToString(), user.Child("type").Value.ToString(), user.Child("msg").Value.ToString(), user.Child("key").Value.ToString()));
        }
        if (temp.Count > 0)
        {
            if (temp[temp.Count - 1].type == "received")
            {
                AddMsgItem(temp[temp.Count - 1].type, temp[temp.Count - 1].msg);
            }
          //  SetOrder();
        }
    }

    void OnGUI()
    {
       
    }
    // Update is called once per frame
    void Update()
    {

        if (scroll.normalizedPosition.y < 1)
        {
            scroll.normalizedPosition = new Vector2(0, 1);
        }
        if (IsDataReady)
        {
            DecryptData();
            IsDataReady = false;
        }
        if (IsIncomingDataReady)
        {
            DecryptIncomingData();
            IsIncomingDataReady = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseChat();
        }
    }
 
    public void DecryptData()
    {
        foreach (DataSnapshot user in data)
        {
            if(user.Child("from").Value.ToString()== UserNameChat || user.Child("to").Value.ToString() == UserNameChat)
            {
                AddMsgItem(user.Child("type").Value.ToString(), user.Child("msg").Value.ToString());
            }
        }
      //  SetOrder();
    }

    public void SetOrder()
    {
        ml.Reverse();
        int i = 0;
        foreach(GameObject go in ml)
        {
            go.transform.SetSiblingIndex(i);
            i++;
        }
    }
    public void PopulateChat( string username)
    {
        UserNameChat = username;
        FirebaseDatabase.DefaultInstance
      .GetReference("Messages").Child(UserManager.instance.USERNAME)
      .GetValueAsync().ContinueWith(task => {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Dictionary<string, string> map = snapshot.Value as Dictionary<string, string>;
              data = snapshot.Children;
              IsDataReady = true;
              // Do something with snapshot...
          }
      });

    }
    public void AddMsgItem(string type, string text)
    {
        if (type == "received")
        {
            GameObject go = Instantiate(MessageReceived, transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0)) as GameObject;
            go.GetComponentInChildren<Text>().text = text;
            ml.Add(go);
            go.transform.SetSiblingIndex(0);
        }

        else if(type == "sent")
        {
            GameObject go = Instantiate(MessageSent, transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0)) as GameObject;
            go.GetComponentInChildren<Text>().text = text;
            ml.Add(go);
            go.transform.SetSiblingIndex(0);
        }
    }
    public void OpenChat(string name)
    {
        UserNameChat = name;
        ChatHeader.text = name;
        FirebaseDatabase.DefaultInstance
        .GetReference("MessageList").Child(UserManager.instance.USERNAME).Child(name)
        .ValueChanged += HandleValueChanged;
        ChatPanel.SetActive(true);
        PopulateChat(name);
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

    }

    public void CloseChat()
    {
        foreach(GameObject go in ml)
        {
            Destroy(go);
        }
        ml.Clear();
        ChatPanel.SetActive(false);
        ChatListManager.instance.OpenList();
    }
    public void ReceiveMessage(string text)
    {
        AddMsgItem("received", text);
    }
    public void Send(string text)
    {
        string s = inputtext.text;
        if (s != null && s != "")
        {
            AddMsgItem("sent", s);
            GameManager.instance.SendMsg(UserNameChat, UserManager.instance.USERNAME, "sent", inputtext.text);
            inputtext.text = "";
        }
    }
}
