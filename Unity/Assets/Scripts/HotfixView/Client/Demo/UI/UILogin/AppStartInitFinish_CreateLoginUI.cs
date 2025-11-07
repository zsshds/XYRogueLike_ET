// using ET.Client;
//
// namespace ET.Client
// {
// 	[Event(SceneType.Demo)]
// 	public class AppStartInitFinish_CreateLoginUI: AEvent<Scene, AppStartInitFinish>
// 	{
// 		protected override async ETTask Run(Scene root, AppStartInitFinish args)
// 		{
// 			//await UIHelper.Create(root, UIType.UILogin, UILayer.Mid);
// 			//await root.GetComponent<FUIComponent>().ShowPanelAsync(PanelId.LoginPanel);
// 			//view是表现层，在这里书写Unity相关。至于在事件的run方法中，et是事件驱动的
// 			Log.Debug("开始异步测试");
// 			//await TestAsynk(root);
// 			TestAsynk(root).Coroutine(); //以协程的方式调用的
// 			int retVal = await TaskAsynk2(root);
// 			Log.Debug("retVal = " + retVal);
// 			Log.Debug("异步测试完成");
// 		}
// 		
// 	} 
// }
