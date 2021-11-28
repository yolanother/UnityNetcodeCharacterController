using Unity.Collections;
using Unity.Netcode;

namespace DoubTech.Networking.Data
{
    public struct NetworkString : INetworkSerializable
    {
        private FixedString32Bytes stringData;


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref stringData);
        }

        public override string ToString()
        {
            return stringData.ToString();
        }

        public static implicit operator string(NetworkString s) => s.ToString();

        public static implicit operator NetworkString(string s) =>
            new NetworkString() {stringData = s};
    }
}
