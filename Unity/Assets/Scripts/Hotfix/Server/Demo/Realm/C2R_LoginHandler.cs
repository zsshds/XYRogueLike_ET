// using System;
// using System.Net;
// using ET.Model.Server;
// using System.Collections.Generic;
//
// namespace ET.Server
// {
//     [MessageSessionHandler(SceneType.Realm)] //这里继承的基类是MessageSessionHandler，且泛型变为两个
//     [FriendOfAttribute(typeof(ET.Model.Server.Account))]
//     public class C2R_LoginHandler : MessageSessionHandler<C2R_Login, R2C_Login>
//     {
//         //这里P1获取到客户端的session
//         protected override async ETTask Run(Session session, C2R_Login request, R2C_Login response)
//         {
//             //登录时数据为空
//             if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
//             {
//                 response.Error = ErrorCode.ERR_LoginInfoEmpty;
//                 //关闭链接
//                 CloseSession(session).Coroutine();
//                 return;
//             }
//
//             //防止多个玩家同时注册同一个账号，将Account字段用协程锁锁起来，实际上，CoroutineLockComponent内部维护一个队列，依次执行下方异步块
//             using (await session.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginAccount, request.Account.GetLongHashCode()))
//             {
//                 //Zone是一个区服信息，数据库的配置在scene表和zone表中，配套
//                 DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
//                 List<Account> accountInfoList = await dbComponent.Query<Account>(accountInfo => accountInfo.account == request.Account);
//                 //没有查询到account，说明没有注册，自动注册
//                 if(accountInfoList.Count <= 0)
//                 {
//                     //保证session上有AccountInfoComponent组件
//                     AccountInfoComponent accountInfoComponent =
//                             session.GetComponent<AccountInfoComponent>() ?? session.AddComponent<AccountInfoComponent>();
//                     Account account = accountInfoComponent.AddChild<Account>();
//                     account.account = request.Account;
//                     account.password = request.Password;
//                     //这里MongoDB直接保存的就是AccountInfo实体，DB会帮你加上两个字段，一个是_id--唯一Id，一个是_t--数据类型
//                     await dbComponent.Save(account);
//                 }
//                 else
//                 {
//                     //逻辑上只会查询到一条数据
//                     Account account = accountInfoList[0];
//                     if (account.password != request.Password)
//                     {
//                         response.Error = ErrorCode.ERR_LoginPasswordError;
//                         //关闭链接
//                         CloseSession(session).Coroutine();
//                         return;
//                     }
//                 }
//             }
//             
//             // 随机分配一个Gate
//             StartSceneConfig config = RealmGateAddressHelper.GetGate(session.Zone(), request.Account);
//             Log.Debug($"gate address: {config}");
//
//             // 向gate请求一个key,客户端可以拿着这个key连接gate
//             R2G_GetLoginKey r2GGetLoginKey = R2G_GetLoginKey.Create();
//             r2GGetLoginKey.Account = request.Account;
//             G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await session.Fiber().Root.GetComponent<MessageSender>().Call(
//                 config.ActorId, r2GGetLoginKey);
//
//             response.Address = config.InnerIPPort.ToString();
//             response.Key = g2RGetLoginKey.Key;
//             response.GateId = g2RGetLoginKey.GateId;
//
//             CloseSession(session).Coroutine();
//         }
//
//         private async ETTask CloseSession(Session session)
//         {
//             await session.Root().GetComponent<TimerComponent>().WaitAsync(1000);
//             session.Dispose();
//         }
//     }
// }
