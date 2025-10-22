using ET.Client;

namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class AppStartInitFinish_CreateLoginUI: AEvent<Scene, AppStartInitFinish>
	{
		protected override async ETTask Run(Scene root, AppStartInitFinish args)
		{
			await UIHelper.Create(root, UIType.UILogin, UILayer.Mid);
			//view是表现层，在这里书写Unity相关。至于在事件的run方法中，et是事件驱动的
			Computer computer = root.GetComponent<ComputerComponent>().AddChild<Computer>(); computer.AddComponent<PCCaseComponent>();
			computer.AddComponent<MonitorComponent, int>(100);
			computer.Open();
			computer.GetComponent<MonitorComponent>().ChangeLight(50);
			await root.GetComponent<TimerComponent>().WaitAsync(5000); //单位毫秒\
			computer?.Dispose();
		}
	} 
}
