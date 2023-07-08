public class SpikeBehaviour : RoleBehaviour
{
    private void Awake()
    {
        SetupColliderManipulator();
    }

    private void OnEnable()
    {
        if (Object == null)
        {
            return;
        }
        manipulator.SwapPhysicsMaterial(Object.ActiveRole.Material);
        manipulator.SwapTags(nameof(SpikeRole));
    }

    private void OnDisable()
    {
        manipulator.RestorePhysicsMaterial();
        manipulator.RestoreTags();
    }
}