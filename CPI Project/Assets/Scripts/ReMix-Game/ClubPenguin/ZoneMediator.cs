using ClubPenguin.DailyChallenge;
using ClubPenguin.Adventure;
using ClubPenguin.Chat;
using ClubPenguin.Core;
using ClubPenguin.Interactables.Domain;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using ClubPenguin.Net;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class ZoneMediator
	{
		private ContentSchedulerService contentSchedulerService;

		private DailyChallengeService dailyChallengeService;

		private PartyGameManager partyGameManager;

		private string currentOfflineZone = null;

		private LocomotionEventBroadcaster localPlayerLocoBroadcaster;

		private PropUser localPlayerPropUser;

		public ZoneMediator(EventDispatcher eventDispatcher, DailyChallengeService dailyChallengeService, ContentSchedulerService contentSchedulerService, PartyGameManager partyGameManager)
		{
			this.dailyChallengeService = dailyChallengeService;
			this.partyGameManager = partyGameManager;
			this.contentSchedulerService = contentSchedulerService;
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.AddListener<WorldServiceEvents.ContentDateChanged>(onContentDateChanged);
			eventDispatcher.AddListener<InteractablesEvents.InWorldItemCollected>(onItemCollected);
			eventDispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
			eventDispatcher.AddListener<SwitchEvents.SwitchChange>(onSwitchChange);
			eventDispatcher.AddListener<ChatMessageSender.SendChatMessage>(onChatMessageSent);
			eventDispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded);
			eventDispatcher.AddListener<TaskEvents.TaskCompleted>(onTaskCompleted);
		}

		private bool onContentDateChanged(WorldServiceEvents.ContentDateChanged evt)
		{
			dailyChallengeService.ReloadChallenges(evt.ContentDate);
			return false;
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			switch (evt.State)
			{
			case ZoneTransitionEvents.ZoneTransition.States.Done:
				dailyChallengeService.ReloadChallenges(contentSchedulerService.CurrentContentDate());
				currentOfflineZone = evt.ToZone;
				bindLocalPlayerOfflineSignals();
				break;
			case ZoneTransitionEvents.ZoneTransition.States.Begin:
				dailyChallengeService.ClearLoadedDailies();
				partyGameManager.Reset();
				unbindLocalPlayerOfflineSignals();
				break;
			}
			return false;
		}

		private bool onItemCollected(InteractablesEvents.InWorldItemCollected evt)
		{
			if (!isPureOffline())
			{
				return false;
			}
			int num = evt.CoinCount;
			if (num <= 0)
			{
				return false;
			}
			tryIncrementByCategory(TaskDefinition.TaskCategory.Collect, num);
			return false;
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			if (!isPureOffline())
			{
				return false;
			}
			if (evt.Action == InputEvents.Actions.Action1 || evt.Action == InputEvents.Actions.Action2 || evt.Action == InputEvents.Actions.Action3 || evt.Action == InputEvents.Actions.Jump || evt.Action == InputEvents.Actions.Snowball || evt.Action == InputEvents.Actions.Torpedo)
			{
				tryIncrementByCategory(TaskDefinition.TaskCategory.Action, 1);
			}
			return false;
		}

		private bool onChatMessageSent(ChatMessageSender.SendChatMessage evt)
		{
			if (!isPureOffline())
			{
				return false;
			}
			tryIncrementByCategory(TaskDefinition.TaskCategory.Chat, 1);
			bool flag = false;
			string message = evt.Message;
			if (!string.IsNullOrEmpty(message))
			{
				char[] array = message.ToCharArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (EmoteManager.IsEmoteCharacter(array[i]))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag || evt.SizzleClip != null)
			{
				tryIncrementByCategory(TaskDefinition.TaskCategory.Emote, 1);
				tryIncrementByCategory(TaskDefinition.TaskCategory.Express, 1);
			}
			else if (!string.IsNullOrEmpty(evt.Message) && !string.IsNullOrEmpty(evt.Message.Trim()))
			{
				tryIncrementByCategory(TaskDefinition.TaskCategory.Express, 1);
			}
			return false;
		}

		private bool onSwitchChange(SwitchEvents.SwitchChange evt)
		{
			if (!isPureOffline() || !evt.Value || evt.Owner == null)
			{
				return false;
			}
			tryIncrementMatchingSwitch(evt.Owner.name);
			return false;
		}

		private bool onTaskCompleted(TaskEvents.TaskCompleted evt)
		{
			if (!isPureOffline() || evt.Task == null || evt.Task.Definition == null)
			{
				return false;
			}
			if (evt.Task.Definition.Category == TaskDefinition.TaskCategory.TaskCompletion)
			{
				return false;
			}
			refreshTaskCompletionProgress();
			return false;
		}

		private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
		{
			if (!isPureOffline())
			{
				return false;
			}
			refreshTaskCompletionProgress();
			return false;
		}

		private bool isPureOffline()
		{
			return Service.Get<GameSettings>().OfflineMode;
		}

		private void tryIncrementByCategory(TaskDefinition.TaskCategory category, int amount)
		{
			if (amount <= 0)
			{
				return;
			}
			TaskService taskService = Service.Get<TaskService>();
			if (!taskService.HasLoadedTasks)
			{
				return;
			}
			OfflineDatabase offlineDatabase = Service.Get<OfflineDatabase>();
			foreach (ClubPenguin.Task.Task task in taskService.Tasks)
			{
				if (task == null || task.IsComplete || task.IsRewardClaimed || task.Definition.Category != category)
				{
					continue;
				}
				ZonedTaskDefinition zonedTaskDefinition = task.Definition as ZonedTaskDefinition;
				if (zonedTaskDefinition != null)
				{
					if (zonedTaskDefinition.Zone == null || string.IsNullOrEmpty(currentOfflineZone) || !string.Equals(zonedTaskDefinition.Zone.ZoneName, currentOfflineZone, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
				}
				applyOfflineTaskIncrement(task, amount, offlineDatabase);
			}
		}

		private void tryIncrementMatchingSwitch(string switchName)
		{
			if (string.IsNullOrEmpty(switchName))
			{
				return;
			}
			TaskService taskService = Service.Get<TaskService>();
			if (!taskService.HasLoadedTasks)
			{
				return;
			}
			OfflineDatabase offlineDatabase = Service.Get<OfflineDatabase>();
			foreach (ClubPenguin.Task.Task task in taskService.Tasks)
			{
				if (task == null || task.IsComplete || task.IsRewardClaimed)
				{
					continue;
				}
				if (task.Definition == null || task.Definition.Category != TaskDefinition.TaskCategory.Explore)
				{
					continue;
				}
				if (!doesSwitchMatchTask(switchName, task.Id))
				{
					continue;
				}
				applyOfflineTaskIncrement(task, 1, offlineDatabase);
			}
		}

		private static bool doesSwitchMatchTask(string switchName, string taskId)
		{
			if (string.IsNullOrEmpty(taskId))
			{
				return false;
			}
			if (string.Equals(switchName, taskId, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			string value = taskId + "_Task";
			if (string.Equals(switchName, value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return switchName.StartsWith(taskId + "_", StringComparison.OrdinalIgnoreCase);
		}

		private void applyOfflineTaskIncrement(ClubPenguin.Task.Task task, int amount, OfflineDatabase offlineDatabase)
		{
			int num = task.Counter + amount;
			if (task.Definition.CounterMax > 0)
			{
				num = Math.Min(num, task.Definition.CounterMax);
			}
			TaskProgress progress = default(TaskProgress);
			progress.taskId = task.Id;
			progress.counter = num;
			progress.claimed = task.IsRewardClaimed;
			SetTaskProgressOperation.SetOfflineTaskProgress(offlineDatabase, progress);
			Service.Get<EventDispatcher>().DispatchEvent(new TaskNetworkServiceEvents.TaskCounterChanged(task.Id, num));
		}

		private void refreshTaskCompletionProgress()
		{
			TaskService taskService = Service.Get<TaskService>();
			if (!taskService.HasLoadedTasks)
			{
				return;
			}
			ClubPenguin.Task.Task task = null;
			int num = 0;
			foreach (ClubPenguin.Task.Task task2 in taskService.Tasks)
			{
				if (task2 == null || task2.Definition == null)
				{
					continue;
				}
				if (task2.Definition.Category == TaskDefinition.TaskCategory.TaskCompletion)
				{
					task = task2;
				}
				else if (task2.IsComplete)
				{
					num++;
				}
			}
			if (task == null)
			{
				return;
			}
			if (task.Definition.CounterMax > 0)
			{
				num = Math.Min(num, task.Definition.CounterMax);
			}
			setOfflineTaskCounter(task, num, Service.Get<OfflineDatabase>());
		}

		private void setOfflineTaskCounter(ClubPenguin.Task.Task task, int counter, OfflineDatabase offlineDatabase)
		{
			if (task.Counter == counter)
			{
				return;
			}
			TaskProgress progress = default(TaskProgress);
			progress.taskId = task.Id;
			progress.counter = counter;
			progress.claimed = task.IsRewardClaimed;
			SetTaskProgressOperation.SetOfflineTaskProgress(offlineDatabase, progress);
			Service.Get<EventDispatcher>().DispatchEvent(new TaskNetworkServiceEvents.TaskCounterChanged(task.Id, counter));
		}

		private void bindLocalPlayerOfflineSignals()
		{
			if (!isPureOffline())
			{
				return;
			}
			unbindLocalPlayerOfflineSignals();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject == null)
			{
				return;
			}
			localPlayerLocoBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
			if (localPlayerLocoBroadcaster != null)
			{
				localPlayerLocoBroadcaster.OnInteractionStartedEvent += onLocalInteractionStarted;
			}
			localPlayerPropUser = localPlayerGameObject.GetComponent<PropUser>();
			if (localPlayerPropUser != null)
			{
				localPlayerPropUser.EPropRemoved += onLocalPropRemoved;
			}
		}

		private void unbindLocalPlayerOfflineSignals()
		{
			if (localPlayerLocoBroadcaster != null)
			{
				localPlayerLocoBroadcaster.OnInteractionStartedEvent -= onLocalInteractionStarted;
				localPlayerLocoBroadcaster = null;
			}
			if (localPlayerPropUser != null)
			{
				localPlayerPropUser.EPropRemoved -= onLocalPropRemoved;
				localPlayerPropUser = null;
			}
		}

		private void onLocalInteractionStarted(GameObject trigger)
		{
			if (!isPureOffline())
			{
				return;
			}
			tryIncrementByCategory(TaskDefinition.TaskCategory.Interact, 1);
		}

		private void onLocalPropRemoved(Prop prop)
		{
			if (!isPureOffline())
			{
				return;
			}
			tryIncrementByCategory(TaskDefinition.TaskCategory.Supplies, 1);
			tryIncrementByCategory(TaskDefinition.TaskCategory.Prop, 1);
		}
	}
}
