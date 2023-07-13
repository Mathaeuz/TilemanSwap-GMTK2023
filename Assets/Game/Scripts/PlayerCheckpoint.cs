using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    public static PlayerCheckpoint ActiveCheckpoint = null;
    public SharedParticles Flare, Burst;
    public AudioClip Get;
    public Transform SpritePrefab;
    Role[] TargetState;

    private void Awake()
    {
        Flare.Init();
        Burst.Init();
        var sprite = Instantiate(SpritePrefab);
        sprite.position = transform.position;
        TargetState = RoleManager.Instance.NewRoleState();
    }

    protected void SetCheckpoint(PlayerCheckpoint checkpoint)
    {
        if (ActiveCheckpoint != null)
        {
            ActiveCheckpoint.Hide();
        }
        if (ActiveCheckpoint != checkpoint)
        {
            SharedSoundEmiter.Instance.Play(Get);
            RoleManager.Instance.ReadCurrentSwaps(TargetState);
            ActiveCheckpoint = checkpoint;
            checkpoint.Show();
        }
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
        if (!col.CompareTag(nameof(Player)))
        {
            return;
        }
        SetCheckpoint(this);
        col.GetComponentInParent<Player>().Checkpoint = this;
    }

    public void Return()
    {
        RoleManager.Instance.RollbackSwaps(TargetState);
    }
}