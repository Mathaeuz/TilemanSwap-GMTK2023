using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawn : MonoBehaviour
{
    public Player Prefab;
    Player player;
    public UnityEvent<Player> OnSpawn;

    public void AddListener(UnityAction<Player> callback)
    {
        if (player != null)
        {
            callback(player);
            return;
        }
        OnSpawn.AddListener(callback);
    }

    // Start is called before the first frame update
    void Awake()
    {
        player = Instantiate(Prefab);
        player.transform.position = transform.position;
        player.SpawnPosition = transform.position;
        OnSpawn.Invoke(player);
        PlayerCheckpoint.ActiveCheckpoint = null;
    }
}
