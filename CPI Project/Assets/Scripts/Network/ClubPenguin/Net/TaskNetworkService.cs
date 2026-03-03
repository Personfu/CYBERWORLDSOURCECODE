using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.http;
using UnityEngine;

namespace ClubPenguin.Net
{
	internal class TaskNetworkService : BaseNetworkService, ITaskService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.TASK_COUNT_UPDATED, onTaskCountUpdated);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.TASK_PROGRESS_UPDATED, onTaskProgressUpdated);
		}

		public void Pickup(string path, string tag, Vector3 position)
		{
			clubPenguinClient.GameServer.Pickup(path, tag, position);
		}

		public void ClaimReward(string taskId)
		{
			ICommonGameSettings commonGameSettings = Service.Get<ICommonGameSettings>();
			if (clubPenguinClient.OfflineMode && string.IsNullOrEmpty(commonGameSettings.CPAPIServicehost))
			{
				OfflineDatabase offlineDatabase = Service.Get<OfflineDatabase>();
				IOfflineDefinitionLoader offlineDefinitionLoader = Service.Get<IOfflineDefinitionLoader>();
				ClaimTaskRewardResponse responseBody;
				if (ClaimTaskRewardOperation.ClaimTaskReward(taskId, out responseBody, offlineDatabase, offlineDefinitionLoader))
				{
					ClubPenguin.Net.Offline.PlayerAssets assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
					Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(assets.Assets));
					Reward reward = null;
					if (responseBody != null && responseBody.reward != null)
					{
						reward = responseBody.reward.ToReward();
					}
					if (reward != null)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.TASK, taskId, reward));
					}
					handleCPResponse(responseBody);
				}
				return;
			}
			APICall<ClaimTaskRewardOperation> aPICall = clubPenguinClient.TaskApi.ClaimTaskReward(taskId);
			aPICall.OnResponse += delegate(ClaimTaskRewardOperation op, HttpResponse httpResponse)
			{
				Reward reward = null;
				if (op.ResponseBody != null && op.ResponseBody.reward != null)
				{
					reward = op.ResponseBody.reward.ToReward();
				}
				if (reward != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.TASK, taskId, reward));
				}
				handleCPResponse(op.ResponseBody);
			};
			aPICall.OnError += delegate(ClaimTaskRewardOperation op, HttpResponse response)
			{
				if (clubPenguinClient.OfflineMode && response != null && response.StatusCode == HttpStatusCode.Gone)
				{
					return;
				}
				handleCPResponseError(op, response);
			};
			aPICall.Execute();
		}

		private void onTaskCountUpdated(GameServerEvent gameServerEvent, object data)
		{
			TaskProgress taskProgress = (TaskProgress)data;
			Service.Get<EventDispatcher>().DispatchEvent(new TaskNetworkServiceEvents.TaskCounterChanged(taskProgress.taskId, taskProgress.counter));
		}

		private void onTaskProgressUpdated(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<TaskProgress> progress = (SignedResponse<TaskProgress>)data;
			APICall<SetTaskProgressOperation> aPICall = clubPenguinClient.TaskApi.SetProgress(progress);
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}
	}
}
