using System;
using System.Net;
using System.Net.Sockets;
using CommandLine;

namespace ET.Client
{
    //命名规范--消息名 + Handler 表示对这样一条网络消息进行处理，继承自MessageHandler基类，需要注意泛型
    [MessageHandler(SceneType.NetClient)] //使用MessageHandler特性，传入scene实体的类型
    public class Main2NetClient_LoginHandler: MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            string password = request.Password;
            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent =
                    root.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
            await routerAddressComponent.Init();
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(routerAddressComponent.RouterManagerIPAddress.AddressFamily, NetworkProtocol.UDP);
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            NetComponent netComponent = root.GetComponent<NetComponent>();
            
            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

            C2R_LoginAccount c2RLoginAccount = C2R_LoginAccount.Create();
            //R2C_Login r2CLogin;
            R2C_LoginAccount r2CLoginAccount;
            Session session = await netComponent.CreateRouterSession(realmAddress, account, password);
            session.AddComponent<ClientSessionErrorComponent>();
            //这里有大坑，如果使用using包裹session，执行完成后session消耗，在调用clientSenderComponent.Call是拿不到session的 操！！！
            //using(Session session = await netComponent.CreateRouterSession(realmAddress, account, password))
            //{
                // C2R_Login c2RLogin = C2R_Login.Create();
                // c2RLogin.Account = account;
                // c2RLogin.Password = password;
                // //请求中这个这个Call方法所带的参数是服务端返回的网络数据结构
                // r2CLogin = (R2C_Login)await session.Call(c2RLogin);
                // if (r2CLogin.Error != ErrorCode.ERR_Success)
                // {
                //     response.Error = r2CLogin.Error;
                //     return;
                // }
                c2RLoginAccount.Account = account;
                c2RLoginAccount.Password = password;
                r2CLoginAccount = (R2C_LoginAccount)await session.Call(c2RLoginAccount);
                if (r2CLoginAccount.Error == ErrorCode.ERR_Success)
                {
                    root.AddComponent<SessionComponent>().Session = session;
                }
                else
                {
                    session?.Dispose();
                }

                response.ToKen = r2CLoginAccount.Token;
                response.Message = r2CLoginAccount.Message;
                response.Error = r2CLoginAccount.Error;
            //}

            // // 创建一个gate Session,并且保存到SessionComponent中
            // Session gateSession = await netComponent.CreateRouterSession(NetworkHelper.ToIPEndPoint(r2CLogin.Address), account, password);
            // gateSession.AddComponent<ClientSessionErrorComponent>();
            // root.AddComponent<SessionComponent>().Session = gateSession;
            // //请求gate
            // C2G_LoginGate c2GLoginGate = C2G_LoginGate.Create();
            // c2GLoginGate.Key = r2CLogin.Key;
            // c2GLoginGate.GateId = r2CLogin.GateId;
            // G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(c2GLoginGate);
            //
            // Log.Debug("登陆gate成功!");
            //
            // response.PlayerId = g2CLoginGate.PlayerId;
        }
    }
}