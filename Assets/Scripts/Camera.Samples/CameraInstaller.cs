using BeardedPlatypus.Camera.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeardedPlatypus.Camera.Samples
{
    /// <summary>
    /// <see cref="CameraInstaller"/> provides the Samples specific dependency to
    /// construct the camera logic.
    /// </summary>
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] private Button resetViewButton;
        [SerializeField] private ResetDataScriptableObject resetData;
        
        public override void InstallBindings()
        {
            Container.Bind<Button>()
                     .FromInstance(resetViewButton)
                     .AsSingle()
                     .WhenInjectedInto<ResetViewController>();
            Container.Bind<ResetDataScriptableObject>()
                     .FromInstance(resetData)
                     .AsSingle();
            Container.Bind<ResetViewController>()
                     .To<ResetViewController>()
                     .AsSingle();
            Container.Bind<IBindings>()
                     .To<Bindings>()
                     .FromNewComponentOnNewGameObject()
                     .AsSingle();
        }
    }
}