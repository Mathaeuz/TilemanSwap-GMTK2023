using UnityEngine;

public class SwapSilenceListener : MonoBehaviour
{
    private void Awake()
    {
        RoleManager.Instance.AllowEffects.AddListener(gameObject.SetActive);
    }
}