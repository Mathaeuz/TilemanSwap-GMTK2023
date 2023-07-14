using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawn : Singleton<PlayerSpawn>
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
        PlayerCheckpoint.ActiveCheckpoint = null;
    }

    public void SpawnPlayer()
    {
        SpawnPlayer(transform.position);
    }

    public void SpawnPlayer(Vector3 position)
    {
        player = Instantiate(Prefab);
        player.transform.position = position;
        player.SpawnPosition = position;
        OnSpawn.Invoke(player);
    }
}
