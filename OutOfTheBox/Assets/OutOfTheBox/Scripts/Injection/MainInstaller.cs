using System;
using Assets.OutOfTheBox.Scripts.Entities;
using Assets.OutOfTheBox.Scripts.Audio;
using Assets.OutOfTheBox.Scripts.Inputs;
using Assets.OutOfTheBox.Scripts.Navigation;
using Assets.OutOfTheBox.Scripts.Timers;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.OutOfTheBox.Scripts.Injection
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private Instances _instances;
        [SerializeField] private Prefabs _prefabs;

        public override void InstallBindings()
        {
            Container.Bind<Navigator>().ToSingle();
            Container.Bind<ITickable>().ToSingle<Ticker>();
            Container.Bind<Ticker>().ToSingle();

            Container.Bind<AppSettings>().ToSingleInstance(_prefabs.AppSettings);

            Container.Bind<AudioManager>().ToSingleInstance(_instances.AudioManager);
            Container.Bind<AudioClips>().ToSingleInstance(_instances.AudioClips);
            Container.Bind<CatStats>().ToSingleInstance(_instances.CatStats);
            Container.Bind<Controller>().ToSingleInstance(_instances.Controller);
            Container.Bind<EventSystem>().ToSingleInstance(_instances.EventSystem);
            Container.Bind<InControlInputModule>().ToSingleInstance(_instances.InControlInputModule);
        }

        [Serializable]
        public class Instances
        {
            public AudioClips AudioClips;
            public AudioManager AudioManager;
            public CatStats CatStats;
            public Controller Controller;
            public EventSystem EventSystem;
            public InControlInputModule InControlInputModule;
        }

        [Serializable]
        public class Prefabs
        {
            public AppSettings AppSettings;
        }
    }
}