using System;
using UnityEngine;
using System.Collections.Generic;
using AdvancedUtilities.Cameras.Components.Events;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A component that can rotate the view of a camera.
    /// </summary>
    [Serializable]
    public class RotationComponent : CameraComponent
    {
        #region Public Fields

        /// <summary>
        /// Whether or not the RotationComponent will do anything when called.
        /// </summary>
        [Tooltip("Whether or not the RotationComponent will do anything when called.")]
        public bool Enabled = true;

        /// <summary>
        /// Encapsulates the rotation limits for the RotationComponent.
        /// </summary>
        [Serializable]
        public class RotationLimits
        {
            /// <summary>
            /// Limits for vertical rotation will be enforced.
            /// </summary>
            [Tooltip("Limits for vertical rotation will be enforced.")]
            public bool EnableVerticalLimits = true;

            /// <summary>
            /// The limit that the Camera can rotate vertically above the target. Generally this should be a positive value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate vertically above the target. Generally this should be a positive value.")]
            public float VerticalUp = 90f;

            /// <summary>
            /// The limit that the Camera can rotate vertically below the target. Generally this should be a negative value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate vertically below the target. Generally this should be a negative value.")]
            public float VerticalDown = -90f;

            /// <summary>
            /// Limits for horizontal rotation will be enforced.
            /// </summary>
            [Tooltip("Limits for horizontal rotation will be enforced.")]
            public bool EnableHorizontalLimits = false;

            /// <summary>
            /// The limit that the Camera can rotate horizontally to the left of the target. Generally this should be a positive value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate horizontally to the left of the target. Generally this should be a positive value.")]
            public float HorizontalLeft = 90f;

            /// <summary>
            /// The limit that the Camera can rotate horizontally to the right of the target. Generally this should be a negative value.
            /// </summary>
            [Tooltip("The limit that the Camera can rotate horizontally to the right of the target. Generally this should be a negative value.")]
            public float HorizontalRight = -90f;
        }
        /// <summary>
        /// The settings for rotation limits for this RotationComponent.
        /// Limits the amount you can rotate in any direction based off the orientation.
        /// </summary>
        [Tooltip("Limits the amount you can rotate in any direction based off the orientation.")]
        public RotationLimits Limits = new RotationLimits();

        /// <summary>
        /// Encapsulates information on horizontal rotation degrees events.
        /// </summary>
        [Serializable]
        public class RotationHorizontalDegreesEvent
        {
            /// <summary>
            /// Enables the rotation component to fire events every time the horizontal has rotated a given amount of degrees in either direction.
            /// </summary>
            [Tooltip("Enables the rotation component to fire events every time the horizontal has rotated a given amount of degrees in either direction.")]
            public bool Enabled = false;

            /// <summary>
            /// Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.
            /// </summary>
            [Tooltip("Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.")]
            public float DegreesTrigger = 90f;

            /// <summary>
            /// If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees.
            /// If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees.
            /// Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value.
            /// Only really useful if you have a low trigger or massive rotations.
            /// </summary>
            [Tooltip("If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees. " +
                     "If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees. " +
                     "Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value. " +
                     "Only really useful if you have a low trigger or massive rotations.")]
            public bool ResetTotalAfterEachEvent = false;

            /// <summary>
            /// A list of all the rotation listeners currently listening for rotation events.
            /// </summary>
            public IList<HorizontalDegreesListener> RotationEventListeners;
        }
        /// <summary>
        /// The settings for horizontal rotation events for this RotationComponent.
        /// The camera controller has the ability to fire an event when you rotate it on the horizontal direction.
        /// </summary>
        [Tooltip("The camera controller has the ability to fire an event when you rotate it on the horizontal direction.")]
        public RotationHorizontalDegreesEvent HorizontalDegreesEvent = new RotationHorizontalDegreesEvent();

        /// <summary>
        /// Encapsulates information on vertical rotation degrees events.
        /// </summary>
        [Serializable]
        public class RotationVerticalDegreesEvent
        {
            /// <summary>
            /// Enables the rotation component to fire events every time the vertical has rotated a given amount of degrees in either direction.
            /// </summary>
            [Tooltip("Enables the rotation component to fire events every time the vertical has rotated a given amount of degrees in either direction.")]
            public bool Enabled = false;

            /// <summary>
            /// Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.
            /// </summary>
            [Tooltip("Everytime the camera rotates the given amount in degrees, it will fire an event to all listeners registered to it with the amount it rotated.")]
            public float DegreesTrigger = 90f;

            /// <summary>
            /// If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees.
            /// If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees.
            /// Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value.
            /// Only really useful if you have a low trigger or massive rotations.
            /// </summary>
            [Tooltip("If true, everytime the event is fired, it will reset tracking during that update to having rotated 0 degrees. " +
                     "If set to false, if you rotate 135 degrees and fire an event every 90 degrees, then you will fire an event and be left with 45 degrees. " +
                     "Technically multiple events can fire in a single update if this is set to false you rotate in a multiple of your event degrees value. " +
                     "Only really useful if you have a low trigger or massive rotations.")]
            public bool ResetTotalAfterEachEvent = false;

            /// <summary>
            /// A list of all the rotation listeners currently listening for rotation events.
            /// </summary>
            public IList<VerticalDegreesListener> RotationEventListeners;
        }
        /// <summary>
        /// The settings for vertical rotation events for this RotationComponent.
        /// The camera controller has the ability to fire an event when you rotate it on the vertical direction.
        /// </summary>
        [Tooltip("The camera controller has the ability to fire an event when you rotate it on the vertical direction.")]
        public RotationVerticalDegreesEvent VerticalDegreesEvent = new RotationVerticalDegreesEvent();
        
        #endregion

        #region Public Properties

        /// <summary>
        /// The orientation of the Camera's rotation. This orientation is used to determine what is horizontal, vertical, and limits on rotation.
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                return _orientationTransform.Rotation;
            }
        }

        /// <summary>
        /// The up vector for the current orientation of the Camera's rotation.
        /// </summary>
        public Vector3 OrientationUp
        {
            get
            {
                return _orientationTransform.Up;
            }
        }

        /// <summary>
        /// The amount of degrees the camera has rotated horizontally from the nuetral position bounded by [0, 360)
        /// </summary>
        public float HorizontalRotation
        {
            get
            {
                return (_horizontalRotationLimitsTotal % 360 + 360)%360;
            }
        }

        /// <summary>
        /// The amount of degrees the camera has rotated vertically from the nuetral position bounded by [0, 360)
        /// </summary>
        public float VerticalRotation
        {
            get
            {
                return (_verticalRotationLimitsTotal % 360 + 360) % 360;
            }
        }

        /// <summary>
        /// The VirtualTransform used to represent the orientation.
        /// Be careful directly modifying this as it may have unintended side effects.
        /// Modifying rotation of the orientation directly may have uses if you want to modify it's rotation without rotating the whole system,
        /// but manual rotation will not obey limits set up in the Rotation component.
        /// I imagine the primary use of directly accessing this would be when you've limited the horizontal rotation and you want to change the
        /// orientation so the limits are in different locations.
        /// </summary>
        public VirtualTransform OrientationTransform
        {
            get
            {
                return _orientationTransform;
            }
        }

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// The first time this rotation component is initialized, this will be set to true.
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// VirtualTransform used to represent the orientation of the RotationComponent/Camera's rotation.
        /// </summary>
        private VirtualTransform _orientationTransform;

        /// <summary>
        /// Total amount of rotation on the vertical rotation for the degrees event.
        /// </summary>
        private float _verticalRotationDegreesEventTotal;

        /// <summary>
        /// Total amount of rotation on the horizontal rotation for the degrees event.
        /// </summary>
        private float _horizontalRotationDegreesEventTotal;

        /// <summary>
        /// Total rotation horizontally for enforcing limits. This number can overflow.
        /// </summary>
        private float _horizontalRotationLimitsTotal;

        /// <summary>
        /// Total rotation vertical for enforcing limits. This number can overflow.
        /// </summary>
        private float _verticalRotationLimitsTotal;

        #endregion

        public override void Initialize(CameraController cameraController)
        {
            base.Initialize(cameraController);

            HorizontalDegreesEvent.RotationEventListeners = new List<HorizontalDegreesListener>();
            VerticalDegreesEvent.RotationEventListeners = new List<VerticalDegreesListener>();

            _orientationTransform = new VirtualTransform();
            _horizontalRotationLimitsTotal = 0;
            _verticalRotationLimitsTotal = 0;
            _horizontalRotationDegreesEventTotal = 0;
            _verticalRotationDegreesEventTotal = 0;

            _isInitialized = true;
        }

        #region Orientation

        /// <summary>
        /// Adjusts the orientation of RotationComponent. 
        /// This should be used to change the up vector of the Camera's rotation, as well as where the limits of rotation are for the RotationComponent.
        /// This will preserve the Camera's current location in relation to the target by moving the Camera.
        /// </summary>
        /// <param name="eulerAngles">New orientation in euler angles.</param>
        /// <param name="target">OffsetTarget that is currently being Rotated around.</param>
        public void SetRotationOrientation(Vector3 eulerAngles, Vector3 target)
        {
            SetRotationOrientation(Quaternion.Euler(eulerAngles), target);
        }

        /// <summary>
        /// Adjusts the orientation of RotationComponent. 
        /// This should be used to change the up vector of the Camera's rotation, as well as where the limits of rotation are for the RotationComponent.
        /// This will preserve the Camera's current location in relation to the target by moving the Camera.
        /// </summary>
        /// <param name="orientation">New orientation.</param>
        /// <param name="target">Target that is currently being Rotated around.</param>
        public void SetRotationOrientation(Quaternion orientation, Vector3 target)
        {
            if (_orientationTransform.Rotation == orientation)
            {
                return;
            }

            float distance = Vector3.Distance(target, CameraTransform.Position);

            var diff = orientation * Quaternion.Inverse(Orientation);

            CameraTransform.Rotate(diff, Space.World);
            CameraTransform.Position = target - distance * CameraTransform.Forward;

            _orientationTransform.Rotation = orientation;
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotates the Camera horizontally by the given degrees.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        public void RotateHorizontally(float degrees)
        {
            if (!Enabled)
            {
                return;
            }

            if (Limits.EnableHorizontalLimits)
            {
                degrees = GetEnforcedHorizontalDegrees(degrees);
            }
            
            CameraTransform.Rotate(_orientationTransform.Up, degrees, Space.World);

            if (HorizontalDegreesEvent.Enabled)
            {
                TrackHorizontalEventRotation(degrees);
            }

            TrackHorizontalLimitsRotation(degrees);
        }

        /// <summary>
        /// Rotates the Camera vertically by the given degrees
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        public void RotateVertically(float degrees)
        {
            if (!Enabled)
            {
                return;
            }

            if (Limits.EnableVerticalLimits)
            {
                degrees = GetEnforcedVerticalDegrees(degrees);
            }

            CameraTransform.Rotate(CameraTransform.Right, degrees, Space.World);

            if (VerticalDegreesEvent.Enabled)
            {
                TrackVerticalEventRotation(degrees);
            }

            TrackVerticalLimitsRotation(degrees);
        }

        /// <summary>
        /// Rotates the Camera horizontally and then vertically at once by the given degrees.
        /// </summary>
        /// <param name="horizontalDegrees">Given degrees for horizontal.</param>
        /// <param name="verticalDegrees">Given degrees for vertical.</param>
        public void Rotate(float horizontalDegrees, float verticalDegrees)
        {
            RotateHorizontally(horizontalDegrees);
            RotateVertically(verticalDegrees);
        }

        /// <summary>
        /// Sets the rotation of the rotation component to the given settings.
        /// </summary>
        /// <param name="horizontalDegrees">Horizontal degrees of rotation to set it to.</param>
        /// <param name="verticalDegrees">Vertical degrees of rotation to set it to.</param>
        /// <param name="trackRotation">Whether or not rotation is tracked for events.</param>
        public void SetRotation(float horizontalDegrees, float verticalDegrees, bool trackRotation = false)
        {
            bool horizontalTracking = HorizontalDegreesEvent.Enabled;
            bool verticalTracking = VerticalDegreesEvent.Enabled;

            if (!trackRotation)
            {
                HorizontalDegreesEvent.Enabled = false;
                VerticalDegreesEvent.Enabled = false;
            }

            float horizontalDiff = horizontalDegrees - HorizontalRotation;
            float verticalDiff = verticalDegrees - VerticalRotation;

            RotateHorizontally(horizontalDiff);
            RotateVertically(verticalDiff);

            HorizontalDegreesEvent.Enabled = horizontalTracking;
            VerticalDegreesEvent.Enabled = verticalTracking;
        }

        /// <summary>
        /// Sets the rotation of the rotation component to the given settings.
        /// </summary>
        /// <param name="rotation">Represents the rotation we want.</param>
        /// <param name="trackRotation">Whether or not rotation is tracked for events.</param>
        public void SetRotation(Quaternion rotation,bool trackRotation = false)
        {
            Vector3 rotationEuler = rotation.eulerAngles;
            Vector3 cameraEuler = CameraTransform.EulerAngles;
            
            float horizontalDiff = (rotationEuler.y - cameraEuler.y);
            float verticalDiff = (rotationEuler.x - cameraEuler.x);

            if (horizontalDiff > 180)
            {
                horizontalDiff -= 360;
            }
            else if (horizontalDiff < -180)
            {
                horizontalDiff += 360;
            }

            if (verticalDiff > 180)
            {
                verticalDiff -= 360;
            }
            else if (verticalDiff < -180)
            {
                verticalDiff += 360;
            }

            SetRotation(horizontalDiff + HorizontalRotation, verticalDiff + VerticalRotation, trackRotation);
        }

        #endregion

        #region Limited Rotation

        /// <summary>
        /// Returns the amount of degrees you can rotate by when enforcing horizontal limits when given an amount of degrees you want to rotate by.
        /// </summary>
        /// <param name="degrees">Degrees you want to attempt to rotate by.</param>
        /// <returns>Actual degrees you're allowed to rotate by.</returns>
        public float GetEnforcedHorizontalDegrees(float degrees)
        {
            // rotating right
            if (degrees < 0)
            {
                if (_horizontalRotationLimitsTotal + degrees < Limits.HorizontalRight)
                {
                    return Limits.HorizontalRight - _horizontalRotationLimitsTotal;
                }
            }
            // rotating left
            else if (degrees >= 0)
            {
                if (_horizontalRotationLimitsTotal + degrees > Limits.HorizontalLeft)
                {
                    return Limits.HorizontalLeft - _horizontalRotationLimitsTotal;
                }
            }

            return degrees;
        }

        /// <summary>
        /// Returns the amount of degrees you can rotate by when enforcing vertical limits when given an amount of degrees you want to rotate by.
        /// </summary>
        /// <param name="degrees">Degrees you want to attempt to rotate by.</param>
        /// <returns>Actual degrees you're allowed to rotate by.</returns>
        public float GetEnforcedVerticalDegrees(float degrees)
        {
            // rotating down
            if (degrees < 0)
            {
                if (_verticalRotationLimitsTotal + degrees < Limits.VerticalDown)
                {
                    return Limits.VerticalDown - _verticalRotationLimitsTotal;
                }
            }
            // rotating up
            else if (degrees >= 0)
            {
                if (_verticalRotationLimitsTotal + degrees > Limits.VerticalUp)
                {
                    return Limits.VerticalUp - _verticalRotationLimitsTotal;
                }
            }

            return degrees;
        }

        #endregion

        #region Tracking Rotation

        /// <summary>
        /// Tracks the horizontally rotated degrees for rotation events.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackHorizontalEventRotation(float degrees)
        {
            _horizontalRotationDegreesEventTotal += degrees;
        }

        /// <summary>
        /// Tracks the vertically rotated degrees for rotation events.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackVerticalEventRotation(float degrees)
        {
            _verticalRotationDegreesEventTotal += degrees;
        }

        /// <summary>
        /// Tracks the horizontal rotation for enforcing limits.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackHorizontalLimitsRotation(float degrees)
        {
            unchecked
            {
                _horizontalRotationLimitsTotal += degrees;
            }
        }

        /// <summary>
        /// Tracks the vertical rotation for enforcing limits.
        /// </summary>
        /// <param name="degrees">Given degrees.</param>
        private void TrackVerticalLimitsRotation(float degrees)
        {
            unchecked
            {
                _verticalRotationLimitsTotal += degrees;
            }
        }

        #endregion

        #region Rotation Events
        
        /// <summary>
        /// Checks the horizontal degrees events and fires events as needed first, then checks the vertical degrees events and fires events as needed.
        /// This does not fire if the rotation component is not enabled.
        /// </summary>
        public void CheckRotationDegreesEvents()
        {
            if (!Enabled)
            {
                return;
            }

            CheckHorizontalDegreesEvents();
            CheckVerticalDegreesEvents();
        }

        #region Horizontal Rotation Event

        /// <summary>
        /// Checks the horizontal degrees events and fires events as needed.
        /// This does not fire if the rotation component is not enabled or if the horizontal degrees event is not enabled.
        /// </summary>
        public void CheckHorizontalDegreesEvents()
        {
            // Even though we don't track when disabled, we're still going to return here as well to prevent any situation were 
            if (!HorizontalDegreesEvent.Enabled || !Enabled)
            {
                return;
            }

            float total = Mathf.Abs(_horizontalRotationDegreesEventTotal);

            if (total < HorizontalDegreesEvent.DegreesTrigger)
            {
                return;
            }

            float modifier = _horizontalRotationDegreesEventTotal > 0 ? 1 : -1;
            float amount = modifier * HorizontalDegreesEvent.DegreesTrigger;

            if (HorizontalDegreesEvent.ResetTotalAfterEachEvent)
            {
                FireHorizontalDegreesEvent(amount);
                _horizontalRotationDegreesEventTotal = 0;
            }
            else
            {
                while (total > HorizontalDegreesEvent.DegreesTrigger)
                {
                    FireHorizontalDegreesEvent(amount);
                    total -= HorizontalDegreesEvent.DegreesTrigger;
                    _horizontalRotationDegreesEventTotal -= amount;
                }
            }
        }

        /// <summary>
        /// Registers a listener to listen to when the camera has rotated a set number of degrees.
        /// The value will be positive or negative based on which direction the camera rotated.
        /// You shouldn't alter the camera during this event or bad things may happen.
        /// </summary>
        /// <param name="degreesListener">The listener</param>
        public void RegisterListener(HorizontalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't register a HorizontalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (!HorizontalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                HorizontalDegreesEvent.RotationEventListeners.Add(degreesListener);
            }
        }

        /// <summary>
        /// Unregisters a listener so that it no longer listens to when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="degreesListener"></param>
        public void UnregisterListener(HorizontalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't unregister a HorizontalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (HorizontalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                HorizontalDegreesEvent.RotationEventListeners.Remove(degreesListener);
            }
        }

        /// <summary>
        /// Clears all HorizontalDegreeListeners from listening to this RotationComponent.
        /// </summary>
        public void ClearHorizontalListeners()
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't clear a HorizontalDegreesListeners until the RotationComponent has been initialized atleast once.");
            }

            HorizontalDegreesEvent.RotationEventListeners.Clear();
        }

        /// <summary>
        /// Fires the rotation listener event to alert listeners when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="amount">The amount of degrees the camera has rotated.</param>
        private void FireHorizontalDegreesEvent(float amount)
        {
            foreach (var listener in HorizontalDegreesEvent.RotationEventListeners)
            {
                listener.DegreesRotated(amount);
            }
        }

        #endregion

        #region Vertical Rotation Event

        /// <summary>
        /// Checks the vertical degrees events and fires events as needed.
        /// This does not fire if the rotation component is not enabled or if the vertical degrees event is not enabled.
        /// </summary>
        public void CheckVerticalDegreesEvents()
        {
            // Even though we don't track when disabled, we're still going to return here as well to prevent any situation were 
            if (!VerticalDegreesEvent.Enabled || !Enabled)
            {
                return;
            }

            float total = Mathf.Abs(_verticalRotationDegreesEventTotal);

            if (total < VerticalDegreesEvent.DegreesTrigger)
            {
                return;
            }

            float modifier = _verticalRotationDegreesEventTotal > 0 ? 1 : -1;
            float amount = modifier * VerticalDegreesEvent.DegreesTrigger;

            if (VerticalDegreesEvent.ResetTotalAfterEachEvent)
            {
                FireVerticalDegreesEvent(amount);
                _verticalRotationDegreesEventTotal = 0;
            }
            else
            {
                while (total > VerticalDegreesEvent.DegreesTrigger)
                {
                    FireVerticalDegreesEvent(amount);
                    total -= VerticalDegreesEvent.DegreesTrigger;
                    _verticalRotationDegreesEventTotal -= amount;
                }
            }
        }

        /// <summary>
        /// Registers a listener to listen to when the camera has rotated a set number of degrees.
        /// The value will be positive or negative based on which direction the camera rotated.
        /// You shouldn't alter the camera during this event or bad things may happen.
        /// </summary>
        /// <param name="degreesListener">The listener</param>
        public void RegisterListener(VerticalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't register a VerticalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (!VerticalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                VerticalDegreesEvent.RotationEventListeners.Add(degreesListener);
            }
        }

        /// <summary>
        /// Unregisters a listener so that it no longer listens to when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="degreesListener"></param>
        public void UnregisterListener(VerticalDegreesListener degreesListener)
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't unregister a VerticalDegreesListener until the RotationComponent has been initialized atleast once.");
            }

            if (VerticalDegreesEvent.RotationEventListeners.Contains(degreesListener))
            {
                VerticalDegreesEvent.RotationEventListeners.Remove(degreesListener);
            }
        }

        /// <summary>
        /// Clears all HorizontalDegreeListeners from listening to this RotationComponent.
        /// </summary>
        public void ClearVerticalListeners()
        {
            if (!_isInitialized)
            {
                throw new ComponentNotInitializedException("Can't clear a VerticalDegreesListeners until the RotationComponent has been initialized atleast once.");
            }

            VerticalDegreesEvent.RotationEventListeners.Clear();
        }

        /// <summary>
        /// Fires the rotation listener event to alert listeners when the camera has rotated a set number of degrees.
        /// </summary>
        /// <param name="amount">The amount of degrees the camera has rotated.</param>
        private void FireVerticalDegreesEvent(float amount)
        {
            foreach (var listener in VerticalDegreesEvent.RotationEventListeners)
            {
                listener.DegreesRotated(amount);
            }
        }

        #endregion

        #endregion
        
    }
}