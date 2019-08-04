using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MessageItem
{ 
    public string to;
    public string msg;
    public string type;
    public string key;
    public string from;
    public string timestamp;

    public MessageItem()
    {
    }

    public MessageItem( string to,string from, string type, string msg, string key)
    {
        this.to = to;
        this.from = from;
        this.msg = msg;
        this.type = type;
        this.key = key;
    }
}
