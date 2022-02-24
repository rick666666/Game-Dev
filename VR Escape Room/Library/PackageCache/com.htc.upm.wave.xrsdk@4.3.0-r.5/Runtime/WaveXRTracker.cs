﻿// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Wave.XR.Settings;

namespace Wave.OpenXR
{
	public static class InputDeviceTracker
	{
		const string LOG_TAG = "Wave.OpenXR.InputDeviceTracker";
		#region Wave XR Interface
		public static void ActivateTracker(bool active)
		{
			WaveXRSettings settings = WaveXRSettings.GetInstance();
			if (settings != null && settings.EnableTracker != active)
			{
				settings.EnableTracker = active;
				Debug.Log(LOG_TAG + " ActivateTracker() " + (settings.EnableTracker ? "Activate." : "Deactivate."));
				SettingsHelper.SetBool(WaveXRSettings.EnableTrackerText, settings.EnableTracker);
			}
		}
		#endregion

		#region Unity XR Tracker definitions
		const string kTracker0Name = "Wave Tracker0";
		const string kTracker1Name = "Wave Tracker1";
		const string kTracker2Name = "Wave Tracker2";
		const string kTracker3Name = "Wave Tracker3";

		const string kTracker0SN = "HTC-211012-Tracker0";
		const string kTracker1SN = "HTC-211012-Tracker1";
		const string kTracker2SN = "HTC-211012-Tracker2";
		const string kTracker3SN = "HTC-211012-Tracker3";

		/// <summary> Standalone Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kAloneTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice
		);
		/// <summary> Right Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kRightTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Right
		);
		/// <summary> Left Tracker Characteristics </summary>
		public const InputDeviceCharacteristics kLeftTrackerCharacteristics = (
			InputDeviceCharacteristics.TrackedDevice |
			InputDeviceCharacteristics.Left
		);
		#endregion

		public enum TrackerId
		{
			Tracker0 = 0,
			Tracker1 = 1,
			Tracker2 = 2,
			Tracker3 = 3,
		}

		public static string Name(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return kTracker0Name; }
			if (trackerId == TrackerId.Tracker1) { return kTracker1Name; }
			if (trackerId == TrackerId.Tracker2) { return kTracker2Name; }
			if (trackerId == TrackerId.Tracker3) { return kTracker3Name; }
			return kTracker0Name;
		}
		public static string SerialNumber(this TrackerId trackerId)
		{
			if (trackerId == TrackerId.Tracker0) { return kTracker0SN; }
			if (trackerId == TrackerId.Tracker1) { return kTracker1SN; }
			if (trackerId == TrackerId.Tracker2) { return kTracker2SN; }
			if (trackerId == TrackerId.Tracker3) { return kTracker3SN; }
			return kTracker0SN;
		}

		public enum TrackerRole
		{
			Undefined = 0,
			Standalone = 1,
			Pair1_Right = 2,
			Pair1_Left = 3,
		}

		public static bool IsTrackerDevice(InputDevice input, TrackerId trackerId)
		{
			if (input.name.Equals(trackerId.Name()) && input.serialNumber.Equals(trackerId.SerialNumber()))
				return true;

			return false;
		}

		internal static List<InputDevice> s_InputDevices = new List<InputDevice>();

		public static bool IsAvailable()
		{
			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker0) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker1) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker2) ||
					IsTrackerDevice(s_InputDevices[i], TrackerId.Tracker3))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAvailable(TrackerId trackerId)
		{
			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (IsTrackerDevice(s_InputDevices[i], trackerId)) { return true; }
			}
			return false;
		}

		public static bool IsTracked(TrackerId trackerId)
		{
			InputDevices.GetDevices(s_InputDevices);
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				if (s_InputDevices[i].TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked))
				{
					return isTracked;
				}
			}
			return false;
		}

		public static bool GetPosition(TrackerId trackerId, out Vector3 position)
		{
			position = Vector3.zero;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.devicePosition, out position);
			}

			return false;
		}
		public static bool GetRotation(TrackerId trackerId, out Quaternion rotation)
		{
			rotation = Quaternion.identity;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
			}

			return false;
		}

		public static TrackerRole GetRole(TrackerId trackerId)
		{
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				if (s_InputDevices[i].characteristics.Equals(kLeftTrackerCharacteristics))
					return TrackerRole.Pair1_Left;

				if (s_InputDevices[i].characteristics.Equals(kRightTrackerCharacteristics))
					return TrackerRole.Pair1_Right;

				if (s_InputDevices[i].characteristics.Equals(kAloneTrackerCharacteristics))
					return TrackerRole.Standalone;
			}


			return TrackerRole.Undefined;
		}

		public static bool ButtonDown(TrackerId trackerId, InputFeatureUsage<bool> button, out bool down)
		{
			down = false;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(button, out down);
			}

			return false;
		}
		public static bool ButtonAxis(TrackerId trackerId, InputFeatureUsage<float> button, out float axis)
		{
			axis = 0;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(button, out axis);
			}

			return false;
		}
		public static bool ButtonAxis(TrackerId trackerId, InputFeatureUsage<Vector2> button, out Vector2 axis)
		{
			axis = Vector2.zero;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(button, out axis);
			}

			return false;
		}

		public static bool BatteryLevel(TrackerId trackerId, out float level)
		{
			level = 0;

			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }
				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				return s_InputDevices[i].TryGetFeatureValue(CommonUsages.batteryLevel, out level);
			}

			return false;
		}

		public static bool HapticPulse(TrackerId trackerId, UInt32 durationMicroSec = 500000, UInt32 frequency = 0, float amplitude = 0.5f)
		{
			for (int i = 0; i < s_InputDevices.Count; i++)
			{
				if (!s_InputDevices[i].isValid) { continue; }

				if (!IsTrackerDevice(s_InputDevices[i], trackerId)) { continue; }

				float durationSec = durationMicroSec / 1000000;
				Debug.Log(LOG_TAG + " HapticPulse() " + trackerId
					+ "[" + trackerId.Name() + "]"
					+ "[" + trackerId.SerialNumber() + "]"
					+ ": " + durationSec.ToString() + ", " + amplitude);
				return s_InputDevices[i].SendHapticImpulse(0, amplitude, durationSec);
			}

			return false;
		}
	}
}