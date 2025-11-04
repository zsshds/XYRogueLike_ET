using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ET.Model.Server;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.Model.Server.Account))]
    public class C2R_LoginAccountHandler : MessageSessionHandler<C2R_LoginAccount, R2C_LoginAccount>
    {
        //session是连接会话实体
        // ReSharper disable Unity.PerformanceAnalysis
        protected override async ETTask Run(Session session, C2R_LoginAccount request, R2C_LoginAccount response)
        {
            //先移除session的时间限制
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            //判断是否是被协程锁锁住的也就是说，服务端逻辑尚未处理完，客户端再次请求，主要是防外挂
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                session?.Disconnect().Coroutine();
                return;
            }

            //判断用户名密码是否为空
            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoEmpty;
                session?.Disconnect().Coroutine(); //用协程异步，不要卡住逻辑
                return;
            }
            //这里可以对账号密码加上正则判断

            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                //这里协程锁的的目的是防止在异步环境多个玩家同时注册同一个账号，将Account字段用协程锁锁起来，
                //实际上，CoroutineLockComponent内部维护一个队列，依次执行下方异步块
                using (await coroutineLockComponent.Wait(CoroutineLockType.LoginAccount, request.Account.GetLongHashCode()))
                {
                    //Zone是一个区服信息，数据库的配置在scene表和zone表中，配套，也就是获取当前Realm所在的区服
                    DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());

                    List<Account> accountInfoList = await dbComponent.Query<Account>(account => account.AccountName.Equals(request.Account));
                    Account curAccount = null;
                    //账号存在数据空中
                    if (accountInfoList != null && accountInfoList.Count > 0)
                    {
                        curAccount = accountInfoList[0];
                        session.AddChild(curAccount); //将account交给session管理，保证服务器端的每个实体都是可控的（在实体层级树中）
                        if (curAccount.AccountType == AccountType.BlackList)
                        {
                            response.Error = ErrorCode.ERR_RequestRepeatedly;
                            session?.Disconnect().Coroutine();
                            curAccount?.Dispose(); //手动回收一下，其实在session销毁后也会回收
                            return;
                        }

                        if (curAccount.Password != request.Password)
                        {
                            response.Error = ErrorCode.ERR_LoginPasswordError;
                            session.Disconnect().Coroutine();
                            curAccount.Dispose();
                        }
                    }
                    else
                    {
                        curAccount = session.AddChild<Account>();
                        curAccount.AccountName = request.Account;
                        curAccount.Password = request.Password;
                        curAccount.AccountType = AccountType.General;
                        curAccount.CreatTime = TimeInfo.Instance.ServerNow(); //当前游戏服务器时间
                        await dbComponent.Save(curAccount);
                    }
                    
                    //转发到登录中心处理完成实际登录，也就是客户端连接LoginCenter服务器
                    R2L_LoginAccountRequest r2LLoginAccountRequest = R2L_LoginAccountRequest.Create();
                    r2LLoginAccountRequest.AccountName = request.Account;
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    L2R_LoginAccountResponse l2RLoginAccountResponse =
                            await session.Root().GetComponent<MessageSender>().Call(loginCenterConfig.ActorId, r2LLoginAccountRequest) as
                                    L2R_LoginAccountResponse;
                    if (l2RLoginAccountResponse.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = l2RLoginAccountResponse.Error;
                        session?.Disconnect().Coroutine();
                        curAccount?.Dispose();
                        return;
                    }
                    
                    //判断是有有别处登录的情况
                    Session otherSession = session.Root().GetComponent<AccountSessionsComponent>().Get(request.Account);
                    otherSession?.Send(A2C_Disconnect.Create());
                    otherSession?.Disconnect().Coroutine();
                    
                    //对session进行管理，长时间不操作异常session，减轻服务器压力
                    session.Root().GetComponent<AccountSessionsComponent>().Add(request.Account, session);
                    session.AddComponent<AccountCheckOutTimeComponent, string>(request.Account);

                    //创建token，完成C2RLoginAccount请求
                    string Token = TimeInfo.Instance.ServerNow().ToString() + RandomGenerator.RandomNumber(int.MinValue, int.MaxValue).ToString();
                    //对token进行管理，长时间不操作，移除token
                    session.Root().GetComponent<TokenComponent>().Remove(request.Account);
                    session.Root().GetComponent<TokenComponent>().Add(request.Account, Token);
                    response.Token = Token;
                    curAccount?.Dispose();
                }
            }
        }
    }
}

