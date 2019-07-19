using MenuSystemWithZenject;
using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller<GlobalInstaller>
{
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _loadingMenu;
    [SerializeField] private GameObject _playMenu;

    public override void InstallBindings()
    {
        // Bind MenuSystemWithZenject
        Container.Bind<MenuManager>()
            .AsSingle();

        Container.Bind<GameObject>()
            .FromInstance(_playMenu)
            .WhenInjectedInto<PlayMenu.CustomMenuFactory>();
        Container.BindFactory<PlayMenu, PlayMenu.Factory>()
            .FromFactory<PlayMenu.CustomMenuFactory>();

        Container.Bind<GameObject>()
            .FromInstance(_loadingMenu)
            .WhenInjectedInto<LoadingMenu.CustomMenuFactory>();
        Container.BindFactory<LoadingMenu, LoadingMenu.Factory>()
            .FromFactory<LoadingMenu.CustomMenuFactory>();

        Container.Bind<GameObject>()
            .FromInstance(_gameOverMenu)
            .WhenInjectedInto<GameOverMenu.CustomMenuFactory>();
        Container.BindFactory<GameOverMenu, GameOverMenu.Factory>()
            .FromFactory<GameOverMenu.CustomMenuFactory>();
    }
}