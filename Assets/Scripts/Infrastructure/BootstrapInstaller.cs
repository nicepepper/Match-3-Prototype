using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public InputMouse InputMousePrefab;
    
    public override void InstallBindings()
    {
        BindInputService();
    }

    private void BindInputService()
    {
        Container
            .Bind<IInputService>()
            .To<InputMouse>()
            .FromComponentInNewPrefab(InputMousePrefab)
            .AsSingle();
    }
}
