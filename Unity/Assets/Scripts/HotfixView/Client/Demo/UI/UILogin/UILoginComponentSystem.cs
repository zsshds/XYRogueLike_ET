// using UnityEngine;
// using UnityEngine.UI;
//
// namespace ET.Client
// {
// 	[EntitySystemOf(typeof(UILoginComponent))]
// 	[FriendOf(typeof(UILoginComponent))]
// 	public static partial class UILoginComponentSystem
// 	{
// 		[EntitySystem]
// 		private static void Awake(this UILoginComponent self)
// 		{
// 			//通过引用组件获取UI游戏物体上挂载的子游戏物体
// 			ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
// 			self.loginBtn = rc.Get<GameObject>("LoginBtn");
// 			
// 			self.loginBtn.GetComponent<Button>().onClick.AddListener(()=> { self.OnLogin(); });
// 			self.account = rc.Get<GameObject>("Account");
// 			self.password = rc.Get<GameObject>("Password");
// 		}
//
// 		
// 		public static void OnLogin(this UILoginComponent self)
// 		{
// 			LoginHelper.Login(
// 				self.Root(), 
// 				self.account.GetComponent<InputField>().text, 
// 				self.password.GetComponent<InputField>().text).Coroutine();
// 		}
// 	}
// }
