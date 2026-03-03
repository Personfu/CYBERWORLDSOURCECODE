using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Offline
{
	public struct DailyTaskProgressData : IOfflineData
	{
		public TaskProgressList Progress;

		public int DayStampUtc;

		public void Init()
		{
			Progress = new TaskProgressList();
			DayStampUtc = 0;
		}
	}
}
