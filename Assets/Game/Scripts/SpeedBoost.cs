using LDtkUnity;
using UnityEditor;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public Color GizmoColor = new Color(1, .5f, 0);
    public float Power;

    private void Awake()
    {
        Process();
        if (Power == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(nameof(Player)))
        {
            return;
        }
        var player = other.GetComponentInParent<Player>();
        var spd = player.Body.velocity;
        var boost = (transform.position - player.transform.position).normalized * Power;
        spd.x = Mathf.Max(boost.x, Mathf.Abs(player.Body.velocity.x)) * Mathf.Sign(player.Body.velocity.x);
        spd.y = Mathf.Max(boost.y, Mathf.Abs(player.Body.velocity.y)) * Mathf.Sign(player.Body.velocity.y);

        player.Body.velocity = spd;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        var maxHeight = (Power * Power) / (-4 * Physics2D.gravity.y);
        var pos = transform.position + Vector3.up * maxHeight;
        Gizmos.DrawLine(pos, transform.position);
        Gizmos.DrawLine(pos + Vector3.left, pos);
        Gizmos.DrawLine(pos + Vector3.right, pos);
    }

    private void Process()
    {
        var fields = GetComponent<LDtkFields>();
        Power = fields.GetFloat(nameof(Power));
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpeedBoost))]
    [CanEditMultipleObjects]
    public class _Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Process"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    (targets[i] as SpeedBoost).Process();
                    EditorUtility.SetDirty(targets[i]);
                }
            }
            base.OnInspectorGUI();
        }
    }
#endif
}