using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace BeardedPlatypus.Camera.Core
{
    /// <summary>
    /// <see cref="Controller"/> the controller is responsible for controlling the
    /// provided <see cref="VirtualCameraTransform"/> based upon the provided
    /// <see cref="IBindings"/> and behaviours.
    /// </summary>
    public sealed class Controller : IDisposable
    {
        // TODO: Should we decompose the Orbit, Zoom, and Translation further?
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        // Currently, we assume the bindings and behaviours do never change.
        private readonly IBindings _bindings;
        private readonly IOrbitCenter _orbitCenter;

        [CanBeNull] private readonly IOrbitBehaviour _orbitBehaviour;
        [CanBeNull] private readonly ITranslateBehaviour _translateBehaviour;
        [CanBeNull] private readonly IZoomBehaviour _zoomBehaviour;
        
        /// <summary>
        /// Creates a new <see cref="Controller"/> with the given dependencies.
        /// </summary>
        /// <param name="bindings">The bindings containing the reactive streams.</param>
        /// <param name="orbitCenter">The orbit center around which we orbit.</param>
        /// <param name="orbitBehaviour">Optional orbit behaviour.</param>
        /// <param name="translateBehaviour">Optional translate behaviour</param>
        /// <param name="zoomBehaviour">Optional zoom behaviour</param>
        /// <remarks>
        /// If any of the behaviours is set to <c>null</c>, the reactive streams
        /// will be ignored.
        /// </remarks>
        public Controller(IBindings bindings, 
                          IOrbitCenter orbitCenter,
                          [CanBeNull] IOrbitBehaviour orbitBehaviour,
                          [CanBeNull] ITranslateBehaviour translateBehaviour,
                          [CanBeNull] IZoomBehaviour zoomBehaviour)
        {
            _bindings = bindings;
            _orbitCenter = orbitCenter;
            
            _orbitBehaviour = orbitBehaviour;
            _translateBehaviour = translateBehaviour;
            _zoomBehaviour = zoomBehaviour;
            
            ConfigureSubscriptions();
        }

        /// <summary>
        /// Gets the <see cref="Transform"/> of the Virtual Camera that is currently
        /// controlled by this <see cref="Controller"/>.
        /// </summary>
        /// <remarks>
        /// If set to <c>null</c>, the controller will not update anything.
        /// </remarks>
        [CanBeNull] public Transform VirtualCameraTransform { get; set; } = null;

        private void ConfigureSubscriptions()
        {
            _bindings.Orbit.Subscribe(OnOrbit).AddTo(_disposable);
            _bindings.Translate.Subscribe(OnTranslate).AddTo(_disposable);
            _bindings.Zoom.Subscribe(OnZoom).AddTo(_disposable);
        }

        private void OnOrbit(Vector2 inputTranslation)
        {
            if (!IsOrbitEnabled() || VirtualCameraTransform is null) return;
            _orbitBehaviour!.OnOrbit(inputTranslation, 
                                    _orbitCenter, 
                                    VirtualCameraTransform);
        }

        private bool IsOrbitEnabled() => !(_orbitBehaviour is null);
        
        private void OnTranslate(Vector3 inputTranslation)
        {
            if (!IsTranslateEnabled() || VirtualCameraTransform is null) return;
            _translateBehaviour!.OnTranslate(inputTranslation, 
                                            _orbitCenter, 
                                            VirtualCameraTransform);
        }
        
        private bool IsTranslateEnabled() => !(_translateBehaviour is null);

        private void OnZoom(float zoomTranslation)
        {
            if (!IsZoomEnabled() || VirtualCameraTransform is null) return;
            _zoomBehaviour!.OnZoom(zoomTranslation, 
                                  _orbitCenter, 
                                  VirtualCameraTransform);
        }
        
        private bool IsZoomEnabled() => !(_zoomBehaviour is null);

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => _disposable?.Dispose();
    }
}
