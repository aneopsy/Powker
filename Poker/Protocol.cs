using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.DPSBase;
using Newtonsoft.Json;

namespace Poker
{
    [ProtoContract]
    public class HandShake
    {
        [ProtoMember(1)]
        public string IP { get; private set; }

        [ProtoMember(2)]
        public string Name { get; private set; }

        [ProtoMember(3)]
        [JsonProperty]
        string _sourceIdentifier;

        public ShortGuid SourceIdentifier { get { return new ShortGuid(_sourceIdentifier); } }

        protected HandShake() { }

        public HandShake(ShortGuid sourceIdentifier, string Name, string IP)
        {
            this._sourceIdentifier = sourceIdentifier;
            this.Name = Name;
            this.IP = IP;
        }
    }

    [ProtoContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class Protocol : IExplicitlySerialize
    {
        [ProtoMember(1)]
        [JsonProperty]
        string _sourceIdentifier;

        public ShortGuid SourceIdentifier { get { return new ShortGuid(_sourceIdentifier); } }

        [ProtoMember(2)]
        [JsonProperty]
        public string SourceName { get; private set; }

        [ProtoMember(3)]
        [JsonProperty]
        public string Message { get; private set; }

        [ProtoMember(4)]
        [JsonProperty]
        public long MessageIndex { get; private set; }

        [ProtoMember(5)]
        [JsonProperty]
        public int RelayCount { get; private set; }

        private Protocol() { }

        public Protocol(ShortGuid sourceIdentifier, string sourceName, string message, long messageIndex)
        {
            this._sourceIdentifier = sourceIdentifier;
            this.SourceName = sourceName;
            this.Message = message;
            this.MessageIndex = messageIndex;
            this.RelayCount = 0;
        }

        public void IncrementRelayCount()
        {
            RelayCount++;
        }

        public void Serialize(System.IO.Stream outputStream)
        {
            List<byte[]> data = new List<byte[]>();

            byte[] sourceIDData = Encoding.UTF8.GetBytes(_sourceIdentifier);
            byte[] sourceIDLengthData = BitConverter.GetBytes(sourceIDData.Length);

            data.Add(sourceIDLengthData); data.Add(sourceIDData);

            byte[] sourceNameData = Encoding.UTF8.GetBytes(SourceName);
            byte[] sourceNameLengthData = BitConverter.GetBytes(sourceNameData.Length);

            data.Add(sourceNameLengthData); data.Add(sourceNameData);

            byte[] messageData = Encoding.UTF8.GetBytes(Message);
            byte[] messageLengthData = BitConverter.GetBytes(messageData.Length);

            data.Add(messageLengthData); data.Add(messageData);

            byte[] messageIdxData = BitConverter.GetBytes(MessageIndex);

            data.Add(messageIdxData);

            byte[] relayCountData = BitConverter.GetBytes(RelayCount);

            data.Add(relayCountData);

            foreach (byte[] datum in data)
                outputStream.Write(datum, 0, datum.Length);
        }

        public void Deserialize(System.IO.Stream inputStream)
        {
            byte[] sourceIDLengthData = new byte[sizeof(int)]; inputStream.Read(sourceIDLengthData, 0, sizeof(int));
            byte[] sourceIDData = new byte[BitConverter.ToInt32(sourceIDLengthData, 0)]; inputStream.Read(sourceIDData, 0, sourceIDData.Length);
            _sourceIdentifier = new String(Encoding.UTF8.GetChars(sourceIDData));

            byte[] sourceNameLengthData = new byte[sizeof(int)]; inputStream.Read(sourceNameLengthData, 0, sizeof(int));
            byte[] sourceNameData = new byte[BitConverter.ToInt32(sourceNameLengthData, 0)]; inputStream.Read(sourceNameData, 0, sourceNameData.Length);
            SourceName = new String(Encoding.UTF8.GetChars(sourceNameData));

            byte[] messageLengthData = new byte[sizeof(int)]; inputStream.Read(messageLengthData, 0, sizeof(int));
            byte[] messageData = new byte[BitConverter.ToInt32(messageLengthData, 0)]; inputStream.Read(messageData, 0, messageData.Length);
            Message = new String(Encoding.UTF8.GetChars(messageData));

            byte[] messageIdxData = new byte[sizeof(long)]; inputStream.Read(messageIdxData, 0, sizeof(long));
            MessageIndex = BitConverter.ToInt64(messageIdxData, 0);

            byte[] relayCountData = new byte[sizeof(int)]; inputStream.Read(relayCountData, 0, sizeof(int));
            RelayCount = BitConverter.ToInt32(relayCountData, 0);
        }

        public static void Deserialize(System.IO.Stream inputStream, out Protocol result)
        {
            result = new Protocol();
            result.Deserialize(inputStream);
        }
    }
}
