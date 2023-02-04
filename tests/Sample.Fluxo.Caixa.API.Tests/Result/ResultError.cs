namespace Sample.Fluxo.Caixa.API.Tests.Result
{
    public class ResultError
    {
        public string title { get; set; }
        public int status { get; set; }
        public Errors errors { get; set; }
    }

    public class Errors
    {
        public string[] Mensagens { get; set; }
    }

}
