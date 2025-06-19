namespace BackendP2P.Models.Request
{
    public class ResponseModel
    {
        public string Message { get; set; }

        public ResponseModel(string message) { this.Message = message; }
    }
}
