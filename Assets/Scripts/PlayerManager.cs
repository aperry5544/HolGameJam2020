using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerManager
{
    private static Dictionary<KeyCode, GameObject> playerList = null;

    public static Dictionary<KeyCode, GameObject> PlayerList
    {
        get
        {
            if (playerList == null)
            {
                //playerList = new Dictionary<KeyCode, Action>();
            }
            return playerList;
        }
        set
        {
            playerList = value;
        }
    }

    public static void AddPlayer(KeyCode key, GameObject obj)
    {
        PlayerList.Add(key, obj);
    }

    public static void RemovePlayer(KeyCode key)
    {

    }

    public static void UpdatePlayers()
    {
        foreach (KeyValuePair<KeyCode, GameObject> player in PlayerList)
        {
            if (Input.GetKeyDown(player.Key))
            {
                // Call activate on player controller
            }

            if (Input.GetKeyUp(player.Key))
            {
                // Call deactivate on player controller
            }
        }
    }

}
