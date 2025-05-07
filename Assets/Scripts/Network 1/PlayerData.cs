using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public bool isReady;
    public Color playerColor;
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId 
            && playerName == other.playerName 
            && playerId == other.playerId && playerColor.Equals(other.playerColor);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref isReady);

        // Unity的Color不是原生支持的序列化类型，需要分量化处理
        float r = playerColor.r, g = playerColor.g, b = playerColor.b, a = playerColor.a;
        serializer.SerializeValue(ref r);
        serializer.SerializeValue(ref g);
        serializer.SerializeValue(ref b);
        serializer.SerializeValue(ref a);
        if (serializer.IsReader)
        {
            playerColor = new Color(r, g, b, a);
        }
    }
}
