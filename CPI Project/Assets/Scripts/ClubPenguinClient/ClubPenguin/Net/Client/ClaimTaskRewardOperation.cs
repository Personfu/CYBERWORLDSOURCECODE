using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using hg.ApiWebKit.mappers;
using System.Collections;
using System.Reflection;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpContentType("text/plain")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[RequestQueue("Task")]
	[HttpPath("cp-api-base-uri", "/task/v1/reward")]
	public class ClaimTaskRewardOperation : CPAPIHttpOperation
	{
		[HttpRequestTextBody]
		public string RequestBody;

		[HttpResponseJsonBody]
		public ClaimTaskRewardResponse ResponseBody;

		private bool claimed;

		public ClaimTaskRewardOperation(string taskId)
		{
			RequestBody = taskId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			claimed = ClaimTaskReward(RequestBody, out ResponseBody, offlineDatabase, offlineDefinitions);
		}

		protected override HttpResponse CreateOfflineResponse()
		{
			HttpResponse httpResponse = base.CreateOfflineResponse();
			if (!claimed)
			{
				httpResponse.StatusCode = HttpStatusCode.Gone;
			}
			return httpResponse;
		}

		public static bool ClaimTaskReward(string taskId, out ClaimTaskRewardResponse responseBody, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			responseBody = new ClaimTaskRewardResponse();
			if (string.IsNullOrEmpty(taskId))
			{
				return false;
			}
			TaskProgress progress;
			if (SetTaskProgressOperation.TryGetOfflineTaskProgress(offlineDatabase, taskId, out progress) && progress.claimed)
			{
				return false;
			}
			object task = findLoadedTask(taskId);
			object definition = null;
			bool isComplete = false;
			int counter = progress.counter;
			if (task != null)
			{
				isComplete = getBoolProperty(task, "IsComplete");
				counter = getIntProperty(task, "Counter", counter);
				definition = getFieldOrProperty(task, "Definition");
			}
			else
			{
				definition = findKnownTaskDefinition(taskId);
				if (definition != null)
				{
					isComplete = evaluateCompletion(definition, counter);
				}
			}
			if (definition == null)
			{
				definition = findKnownTaskDefinition(taskId);
				if (definition != null)
				{
					isComplete = evaluateCompletion(definition, counter);
				}
			}
			if (!isComplete || definition == null)
			{
				return false;
			}
			Reward reward = new Reward();
			object rewardDefinition = getFieldOrProperty(definition, "Reward");
			if (rewardDefinition != null)
			{
				MethodInfo method = rewardDefinition.GetType().GetMethod("ToReward", BindingFlags.Public | BindingFlags.Instance);
				if (method != null)
				{
					Reward reward2 = method.Invoke(rewardDefinition, null) as Reward;
					if (reward2 != null)
					{
						reward = reward2;
						offlineDefinitions.AddReward(reward, responseBody);
					}
				}
			}
			progress.taskId = taskId;
			progress.counter = counter;
			progress.claimed = true;
			SetTaskProgressOperation.SetOfflineTaskProgress(offlineDatabase, progress);
			JsonService jsonService = Service.Get<JsonService>();
			responseBody.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward)));
			return true;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClaimTaskRewardResponse responseBody = new ClaimTaskRewardResponse();
			ClaimTaskReward(RequestBody, out responseBody, offlineDatabase, offlineDefinitions);
		}

		private static object findLoadedTask(string taskId)
		{
			object obj = findTaskService();
			if (obj == null)
			{
				return null;
			}
			PropertyInfo property = obj.GetType().GetProperty("Tasks", BindingFlags.Public | BindingFlags.Instance);
			if (property == null)
			{
				return null;
			}
			IEnumerable enumerable = property.GetValue(obj, null) as IEnumerable;
			if (enumerable == null)
			{
				return null;
			}
			foreach (object item in enumerable)
			{
				if (getStringProperty(item, "Id") == taskId)
				{
					return item;
				}
			}
			return null;
		}

		private static object findKnownTaskDefinition(string taskId)
		{
			object obj = findTaskService();
			if (obj == null)
			{
				return null;
			}
			FieldInfo field = obj.GetType().GetField("knownTasks", BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
			{
				return null;
			}
			object value = field.GetValue(obj);
			if (value == null)
			{
				return null;
			}
			MethodInfo method = value.GetType().GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance);
			if (method == null)
			{
				return null;
			}
			object[] parameters = new object[2]
			{
				taskId,
				null
			};
			object obj2 = method.Invoke(value, parameters);
			if (!(obj2 is bool) || !(bool)obj2)
			{
				return null;
			}
			return parameters[1];
		}

		private static object findTaskService()
		{
			foreach (object allService in Service.AllServices)
			{
				if (allService != null && allService.GetType().FullName == "ClubPenguin.Task.TaskService")
				{
					return allService;
				}
			}
			return null;
		}

		private static object getRewardDefinition(object task)
		{
			object obj = getProperty(task, "Definition");
			if (obj == null)
			{
				return null;
			}
			return getFieldOrProperty(obj, "Reward");
		}

		private static bool evaluateCompletion(object definition, int counter)
		{
			int intFieldOrProperty = getIntFieldOrProperty(definition, "Threshold", 0);
			int intFieldOrProperty2 = getIntFieldOrProperty(definition, "Comparison", 1);
			switch (intFieldOrProperty2)
			{
			case 0:
				return counter < intFieldOrProperty;
			case 1:
				return counter == intFieldOrProperty;
			case 2:
				return counter > intFieldOrProperty;
			default:
				return false;
			}
		}

		private static int getIntFieldOrProperty(object instance, string name, int fallback)
		{
			object fieldOrProperty = getFieldOrProperty(instance, name);
			if (fieldOrProperty is int)
			{
				return (int)fieldOrProperty;
			}
			return fallback;
		}

		private static object getFieldOrProperty(object instance, string name)
		{
			if (instance == null)
			{
				return null;
			}
			object value = getProperty(instance, name);
			if (value != null)
			{
				return value;
			}
			FieldInfo field = instance.GetType().GetField(name, BindingFlags.Public | BindingFlags.Instance);
			if (field == null)
			{
				return null;
			}
			return field.GetValue(instance);
		}

		private static object getProperty(object instance, string name)
		{
			if (instance == null)
			{
				return null;
			}
			PropertyInfo property = instance.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
			if (property == null)
			{
				return null;
			}
			return property.GetValue(instance, null);
		}

		private static string getStringProperty(object instance, string name)
		{
			object property = getProperty(instance, name);
			return property as string;
		}

		private static bool getBoolProperty(object instance, string name)
		{
			object property = getProperty(instance, name);
			return property is bool && (bool)property;
		}

		private static int getIntProperty(object instance, string name, int fallback)
		{
			object property = getProperty(instance, name);
			if (property is int)
			{
				return (int)property;
			}
			return fallback;
		}
	}
}
