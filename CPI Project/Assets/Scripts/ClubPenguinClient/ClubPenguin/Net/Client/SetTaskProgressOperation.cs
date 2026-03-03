using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/task/v1")]
	[RequestQueue("Task")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class SetTaskProgressOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SignedResponse<TaskProgress> RequestBody;

		public SetTaskProgressOperation(SignedResponse<TaskProgress> task)
		{
			RequestBody = task;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SetOfflineTaskProgress(offlineDatabase, RequestBody.Data);
		}

		public static TaskProgressList GetOfflineTaskProgress(OfflineDatabase offlineDatabase)
		{
			DailyTaskProgressData value = offlineDatabase.Read<DailyTaskProgressData>();
			if (value.Progress == null)
			{
				value.Init();
			}
			ensureCurrentDay(ref value);
			offlineDatabase.Write(value);
			return value.Progress;
		}

		public static bool TryGetOfflineTaskProgress(OfflineDatabase offlineDatabase, string taskId, out TaskProgress progress)
		{
			progress = default(TaskProgress);
			if (string.IsNullOrEmpty(taskId))
			{
				return false;
			}
			TaskProgressList offlineTaskProgress = GetOfflineTaskProgress(offlineDatabase);
			for (int i = 0; i < offlineTaskProgress.Count; i++)
			{
				if (offlineTaskProgress[i].taskId == taskId)
				{
					progress = offlineTaskProgress[i];
					return true;
				}
			}
			return false;
		}

		public static void SetOfflineTaskProgress(OfflineDatabase offlineDatabase, TaskProgress progress)
		{
			if (string.IsNullOrEmpty(progress.taskId))
			{
				return;
			}
			DailyTaskProgressData value = offlineDatabase.Read<DailyTaskProgressData>();
			if (value.Progress == null)
			{
				value.Init();
			}
			ensureCurrentDay(ref value);
			bool flag = false;
			for (int i = 0; i < value.Progress.Count; i++)
			{
				if (value.Progress[i].taskId == progress.taskId)
				{
					value.Progress[i] = progress;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				value.Progress.Add(progress);
			}
			offlineDatabase.Write(value);
		}

		private static void ensureCurrentDay(ref DailyTaskProgressData value)
		{
			int dayStampUtc = getDayStampUtc();
			if (value.DayStampUtc != dayStampUtc)
			{
				value.Progress = new TaskProgressList();
				value.DayStampUtc = dayStampUtc;
			}
		}

		private static int getDayStampUtc()
		{
			DateTime utcNow = DateTime.UtcNow;
			return utcNow.Year * 10000 + utcNow.Month * 100 + utcNow.Day;
		}
	}
}
