using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SingularityGroup.HotReload.Editor.Localization;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor
{
	internal static class EditorWindowHelper
	{
#if UNITY_2020_1_OR_NEWER
		public static bool supportsNotifications = true;
#else
        public static bool supportsNotifications = false;
#endif

		static readonly Regex ValidEmailRegex =
			new(
				@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" +
				@"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

		public static bool IsValidEmailAddress(string email) => ValidEmailRegex.IsMatch(email);

		public static bool IsHumanControllingUs()
		{
			if (Application.isBatchMode) return false;

			var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
			return !isCI;
		}

		internal enum NotificationStatus
		{
			None,
			Patching,
			NeedsRecompile
		}

		static Dictionary<NotificationStatus, GUIContent> notificationContent => new()
		{
			{NotificationStatus.Patching, new GUIContent(Translations.Miscellaneous.NotificationPatching)},
			{NotificationStatus.NeedsRecompile, new GUIContent(Translations.Miscellaneous.NotificationNeedsRecompile)}
		};

		static Type gameViewT;
		static EditorWindow[] gameViewWindows
		{
			get
			{
				gameViewT = gameViewT ?? typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
				return Resources.FindObjectsOfTypeAll(gameViewT).Cast<EditorWindow>().ToArray();
			}
		}

		static EditorWindow[] sceneWindows => Resources.FindObjectsOfTypeAll(typeof(SceneView)).Cast<EditorWindow>().ToArray();

		static EditorWindow[] notificationWindows => gameViewWindows.Concat(sceneWindows).ToArray();

		static NotificationStatus lastNotificationStatus;
		static DateTime? latestNotificationStartedAt;
		static bool notificationShownRecently => latestNotificationStartedAt != null && DateTime.UtcNow - latestNotificationStartedAt < TimeSpan.FromSeconds(1);

		internal static void ShowNotification(NotificationStatus notificationType, float maxDuration = 3)
		{
			// Patch status goes from Unsupported changes to patching rapidly when making unsupported change
			// patching also shows right before unsupported changes sometimes 
			// so we don't override NeedsRecompile notification ever
			var willOverrideNeedsCompileNotification = notificationType != NotificationStatus.NeedsRecompile && notificationShownRecently ||
			                                           lastNotificationStatus == NotificationStatus.NeedsRecompile && notificationShownRecently;
			if (!supportsNotifications || willOverrideNeedsCompileNotification) return;

			foreach (var notificationWindow in notificationWindows)
			{
				notificationWindow.ShowNotification(notificationContent[notificationType], maxDuration);
				notificationWindow.Repaint();
			}

			latestNotificationStartedAt = DateTime.UtcNow;
			lastNotificationStatus = notificationType;
		}

		internal static void RemoveNotification()
		{
			if (!supportsNotifications) return;
			// only patching notifications should be removed after showing less than 1 second
			if (notificationShownRecently && lastNotificationStatus != NotificationStatus.Patching) return;
			foreach (var notificationWindow in notificationWindows)
			{
				notificationWindow.RemoveNotification();
				notificationWindow.Repaint();
			}

			latestNotificationStartedAt = null;
			lastNotificationStatus = NotificationStatus.None;
		}
	}
}