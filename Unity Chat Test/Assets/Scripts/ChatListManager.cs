using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class ChatListManager : MonoBehaviour
{
    #region Singleton
    public static ChatListManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion
    [SerializeField]
    List<MessageItem> list;
    List<MessageItem> tempList;
    [SerializeField]
    GameObject item;
    public GameObject ChatListPanel;
    DatabaseReference reference;
    bool IsDataReady;
    IEnumerable<DataSnapshot> data;
    List<GameObject> objList = new List<GameObject>();
    public List<ChatListItem> chatItem = new List<ChatListItem>();
    bool IsFirstData;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://travemo.firebaseio.com");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        SetListener();
        IsFirstData = true;

    }
    public void SetListener()
    {
        FirebaseDatabase.DefaultInstance
       .GetReference("MessageList").Child(UserManager.instance.USERNAME)
       .ValueChanged += HandleValueChanged;
    }
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
         IsDataReady = true;
         data = args.Snapshot.Children;
        // Do something with the data in args.Snapshot
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDataReady)
        {
            DecryptData();
            IsDataReady = false;
        }
    }
    public void DecryptData()
    {
        chatItem.Clear();
        foreach(DataSnapshot user in data)
        {
            if (user != null)
                chatItem.Add(new ChatListItem( user.Key, user.Child("type").Value.ToString(), user.Child("msg").Value.ToString(),( user.Child("timestamp").Value.ToString())));
        }
        chatItem = chatItem.OrderBy(w => w.timestamp).ToList();
        chatItem.Reverse();
        SpawnList(chatItem);
    }
    public void SetOrder()
    {
        objList.Reverse();
        int i = 0;
        foreach (GameObject go in objList)
        {
            go.transform.SetSiblingIndex(i);
            i++;
        }
    }

    public void SpawnList(List<ChatListItem> cm)
    {
        foreach (GameObject go in objList)
        {
            Destroy(go);
        }
        objList.Clear();
        int i = 0;
        foreach (ChatListItem c in cm)
        {
            Transform tm = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            GameObject go = Instantiate(item, tm) as GameObject;
            go.transform.GetChild(1).GetComponent<Text>().text = c.username;
            go.transform.GetChild(2).GetComponent<Text>().text = c.msg;
            go.GetComponent<Button>().onClick.AddListener(delegate () { OpenChat(c.username); });
            objList.Add(go);
            i++;
        }
    }
    public void OpenChat(string UserName)
    {
        ChatListPanel.SetActive(false);
        ChatManager.instance.OpenChat(UserName);
    }
    public void OpenList()
    {
        ChatListPanel.SetActive(true);
    }

}
