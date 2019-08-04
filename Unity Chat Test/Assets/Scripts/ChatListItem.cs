using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatListItem 
{
    public string username;
    public string msg;
    public string type;
    public string timestamp;
    
    public ChatListItem()
    {
    }

    public ChatListItem(string username, string type, string msg, string timestamp)
    {
        this.username = username;
        this.msg = msg;
        this.type = type;
        this.timestamp = timestamp;
    }

    public ChatListItem(string username, string type, string msg)
    {
        this.username = username;
        this.msg = msg;
        this.type = type;
    }
}
