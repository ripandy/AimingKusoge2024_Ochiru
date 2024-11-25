using Doinject;
using Kusoge.Entities;
using Kusoge.GameStates;
using Kusoge.Interfaces;
using UnityEngine;

namespace Contents.Gameplay
{
    public class GameplayBindingInstaller : MonoBehaviour, IBindingInstaller
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private BeanLauncherData beanLauncherData;
        
        [SerializeField] private BeanPresenter beanPresenter;
        [SerializeField] private IntroPresenter introPresenter;
        [SerializeField] private GameOverPresenter gameOverPresenter;

        [SerializeField] private PlayerDirectionVariable playerPlayerDirection;
        [SerializeField] private FaceDirectionConverterVectorVariable faceDirectionConverterVectorVariable;
        [SerializeField] private BittenBeanGameEvent bittenBeanGameEvent;

        public void Install(DIContainer container, IContextArg contextArg)
        {
            // Domain
            container.BindFromInstance(playerData.Value);
            container.BindFromInstance(beanLauncherData.Value);
            container.BindSingleton<IntroGameState>();
            container.BindSingleton<PlayGameState>();
            container.BindSingleton<GameOverGameState>();

            // Presenters
            container.BindFromInstance<IPlayerDirectionPresenter>(playerPlayerDirection);
            container.BindFromInstance<IBeanPresenter>(beanPresenter);
            container.BindFromInstance<IIntroPresenter>(introPresenter);
            container.BindFromInstance<IGameOverPresenter>(gameOverPresenter);

            // Input Providers
            container.BindFromInstance<IPlayerDirectionInputProvider>(faceDirectionConverterVectorVariable);
            container.BindFromInstance<IPlayerBiteInputProvider>(bittenBeanGameEvent);
        }
    }
}