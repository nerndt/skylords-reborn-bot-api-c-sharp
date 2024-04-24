namespace SkylordsRebornAPI.Cardbase
{
    public class APIWrap<T> //T = Return Type of API Call
    {
        public ExceptionData Exception { get; set; }
        public T[] Result { get; set; }
        public bool Success { get; set; }
    }
}