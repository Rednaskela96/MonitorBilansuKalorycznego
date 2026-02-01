namespace MonitorBilansuKalorycznego.Interfaces
{
    // interfejs dla obiektów, które potrafią się serializowac do/z JSONa.
    internal interface ISerializable
    {
        string ToJson();
        void FromJson(string json);
    }
}
