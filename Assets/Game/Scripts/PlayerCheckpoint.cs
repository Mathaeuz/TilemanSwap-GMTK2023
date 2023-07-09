using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    static PlayerCheckpoint ActiveCheckpoint;
    public SharedParticles Flare, Burst;
    public Transform Sprite;

    protected static void SetCheckpoint(PlayerCheckpoint checkpoint)
    {
        if (ActiveCheckpoint != null)
        {
            ActiveCheckpoint.Hide();
        }
        if (ActiveCheckpoint != checkpoint)
        {
            RoleManager.Instance.SaveSwaps();
            ActiveCheckpoint = checkpoint;
            checkpoint.Show();
        }
    }

    private void Awake()
    {
        Flare.Init();
        Burst.Init();
        Sprite.SetParent(null);
        Sprite.localScale = Vector3.one;
    }

    private void Show()
    {
        Flare.Play(transform.position);
        Burst.Emit(transform.position);
    }

    private void Hide()
    {
    }

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
        {
            return;
        }
        SetCheckpoint(this);
        col.GetComponentInParent<Player>().RespawnAnchor = transform;
    }
}