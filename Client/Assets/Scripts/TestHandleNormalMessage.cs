using UnityEngine;
using zFramework.TinyRPC;
using zFramework.TinyRPC.Generated;

public class TestHandleNormalMessage : MonoBehaviour
{
    private void OnEnable() => this.AddNetworkSignal<TestMessage>(OnTestMessageReceived);
    private void OnDisable() => this.RemoveNetworkSignal<TestMessage>(OnTestMessageReceived);

    private void OnTestMessageReceived(Session session, TestMessage message)
    {
        Debug.Log($"获取到{(session.IsServerSide?"客户端":"服务器")}  {session}  的消息, message = {message}");
    }
}
