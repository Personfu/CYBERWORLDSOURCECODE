using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ClubPenguin.Net
{
	public class ConnectionManager : MonoBehaviour
	{
		public enum NetworkConnectionState
		{
			NoConnection,
			BasicConnection
		}

		private const float pingTimeout = 2f;
		private const string pingAddress = "8.8.8.8";
		private const string webglPingUrl = "https://www.google.com"; // You can change this to your own endpoint

		public NetworkConnectionState ConnectionState
		{
			get
			{
				if (Service.Get<GameSettings>().OfflineMode)
				{
					return NetworkConnectionState.BasicConnection;
				}
				return (Application.internetReachability != 0) ? NetworkConnectionState.BasicConnection : NetworkConnectionState.NoConnection;
			}
		}

		public void DoPingCheck(Action<NetworkConnectionState> callback)
		{
			CoroutineRunner.Start(doPingCheck(callback), this, "Connection Ping Check");
		}

		private IEnumerator doPingCheck(Action<NetworkConnectionState> callback)
		{
			float pingStartTime = Time.unscaledTime;
			NetworkConnectionState connectionState = ConnectionState;
			if (Service.Get<GameSettings>().OfflineMode)
			{
				callback(NetworkConnectionState.BasicConnection);
				yield break;
			}

#if UNITY_WEBGL
			bool isDone = false;
			NetworkConnectionState resultState = NetworkConnectionState.NoConnection;
			using (UnityWebRequest request = UnityWebRequest.Head(webglPingUrl))
			{
				request.timeout = Mathf.CeilToInt(pingTimeout);
				var asyncOp = request.SendWebRequest();
				while (!asyncOp.isDone && Time.unscaledTime - pingStartTime < pingTimeout)
				{
					yield return null;
				}
				if (request.result == UnityWebRequest.Result.Success)
				{
					resultState = NetworkConnectionState.BasicConnection;
				}
				else
				{
					resultState = NetworkConnectionState.NoConnection;
				}
			}
			callback(resultState);
#else
			bool isPinging = true;
			Ping ping = new Ping(pingAddress);
			while (isPinging)
			{
				if (ping.isDone)
				{
					connectionState = ((ping.time >= 0) ? NetworkConnectionState.BasicConnection : NetworkConnectionState.NoConnection);
					isPinging = false;
				}
				else if (Time.unscaledTime - pingStartTime >= pingTimeout)
				{
					connectionState = NetworkConnectionState.NoConnection;
					isPinging = false;
				}
				yield return null;
			}
			callback(connectionState);
#endif
		}
	}
}
