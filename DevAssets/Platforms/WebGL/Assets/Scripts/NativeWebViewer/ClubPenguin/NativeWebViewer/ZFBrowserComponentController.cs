#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
using LitJson;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.NativeWebViewer
{
	public class ZFBrowserComponentController : AbstractWebViewComponentController
	{
		private RawImage rawImage;

		private GameObject webViewContainer;

		public override bool SupportsAddJavascript
		{
			get
			{
				return false;
			}
		}

		public ZFBrowserComponentController(GameObject gameObject)
		{
		}

		public static void SetupBrowser(GameObject webViewPanel, GameObject webViewContainer)
		{
			Canvas componentInParent = webViewPanel.GetComponentInParent<Canvas>();
			RectTransform component = webViewContainer.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			float scaleFactor = componentInParent.scaleFactor;
			float num = (float)Screen.currentResolution.height / (float)Screen.height;
			scaleFactor *= num;
			float num2 = 1f / scaleFactor;
			component.localScale *= num2;
			RectTransform rectTransform = webViewPanel.transform as RectTransform;
			Vector2 b = rectTransform.rect.size * scaleFactor;
			Vector2 a = rectTransform.rect.size - b;
			component.sizeDelta = -a;
			LayoutElement component2 = webViewContainer.GetComponent<LayoutElement>();
			component2.ignoreLayout = true;
			webViewContainer.transform.SetParent(webViewPanel.transform, false);
		}

		public override void SetUp(GameObject webViewPanel, bool isDownsampled, bool allowPopups, bool openPopupInNewBrowser, float zoomLevel, float newBrowserZoomLevel)
		{
			webViewContainer = new GameObject("WebViewContainer", typeof(RectTransform), typeof(LayoutElement));
			SetupBrowser(webViewPanel, webViewContainer);
			rawImage = webViewContainer.GetComponent<RawImage>();
			if (rawImage != null)
			{
				rawImage.enabled = false;
			}
			raiseOnSetupComplete();
		}

		public override void Load()
		{
			// No browser to load
		}

		public override void Show()
		{
			if (rawImage != null)
			{
				rawImage.enabled = true;
			}
		}

		public override void CleanCache()
		{
		}

		public override void Close()
		{
			UnityEngine.Object.Destroy(webViewContainer);
		}

		public override void EvaluateJavaScript(string javaScript)
		{
			// No browser to evaluate JS
		}

		public override void CallFunction(string name, params JsonData[] arguments)
		{
			// No browser to call function
		}

		public override void RegisterJSFunction(string jsFunctionName)
		{
			// No browser to register JS function
		}

		public override void AddJavaScript(string javaScript)
		{
			throw new NotImplementedException();
		}
	}
}
#else
using LitJson;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.NativeWebViewer
{
	public class ZFBrowserComponentController : AbstractWebViewComponentController
	{
		private RawImage rawImage;

		private GameObject webViewContainer;

		public override bool SupportsAddJavascript
		{
			get
			{
				return false;
			}
		}

		public ZFBrowserComponentController(GameObject gameObject)
		{
		}

		public static void SetupBrowser(GameObject webViewPanel, GameObject webViewContainer)
		{
			Canvas componentInParent = webViewPanel.GetComponentInParent<Canvas>();
			RectTransform component = webViewContainer.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			float scaleFactor = componentInParent.scaleFactor;
			float num = (float)Screen.currentResolution.height / (float)Screen.height;
			scaleFactor *= num;
			float num2 = 1f / scaleFactor;
			component.localScale *= num2;
			RectTransform rectTransform = webViewPanel.transform as RectTransform;
			Vector2 b = rectTransform.rect.size * scaleFactor;
			Vector2 a = rectTransform.rect.size - b;
			component.sizeDelta = -a;
			LayoutElement component2 = webViewContainer.GetComponent<LayoutElement>();
			component2.ignoreLayout = true;
			webViewContainer.transform.SetParent(webViewPanel.transform, false);
		}

		public override void SetUp(GameObject webViewPanel, bool isDownsampled, bool allowPopups, bool openPopupInNewBrowser, float zoomLevel, float newBrowserZoomLevel)
		{
			webViewContainer = new GameObject("WebViewContainer", typeof(RectTransform), typeof(LayoutElement));
			SetupBrowser(webViewPanel, webViewContainer);
			rawImage = webViewContainer.GetComponent<RawImage>();
			if (rawImage != null)
			{
				rawImage.enabled = false;
			}
			raiseOnSetupComplete();
		}

		public override void Load()
		{
			// No browser to load
		}

		public override void Show()
		{
			if (rawImage != null)
			{
				rawImage.enabled = true;
			}
		}

		public override void CleanCache()
		{
		}

		public override void Close()
		{
			UnityEngine.Object.Destroy(webViewContainer);
		}

		public override void EvaluateJavaScript(string javaScript)
		{
			// No browser to evaluate JS
		}

		public override void CallFunction(string name, params JsonData[] arguments)
		{
			// No browser to call function
		}

		public override void RegisterJSFunction(string jsFunctionName)
		{
			// No browser to register JS function
		}

		public override void AddJavaScript(string javaScript)
		{
			throw new NotImplementedException();
		}
	}
}
#endif