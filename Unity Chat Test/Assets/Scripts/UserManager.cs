using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{

    #region Singleton
    public static UserManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (PlayerPrefs.GetInt("FirstOpenn", 1) == 1)
        {
           GameManager.instance.UsernamePromptPanel.SetActive(true);
        }
        else
        {
            UserManager.instance.USERNAME = PlayerPrefs.GetString("Username", "sad");
        }
    }

    #endregion

    public string USERNAME;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
