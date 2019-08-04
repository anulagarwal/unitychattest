using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public string username;
    public string name;
    public string id;


    public User()
    {
    }

    public User(string username, string id, string name)
    {
        this.username = username;
        this.id = id;
        this.name = name;
    }
}