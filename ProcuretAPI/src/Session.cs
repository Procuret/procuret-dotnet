using System;
using System.Net;


namespace ProcuretAPI
{
    public struct Session
    {

        private const String path = "/session";
        private const String createTemplate = @"
{
    ""email"": ""{0}"",
    ""secret"": ""{1}"",
    ""perspective"": ""1"",
    ""lifecycle"": ""2""
}";

        public readonly String apiKey;
        public readonly ulong sessionId;

        public Session(
            String apiKey,
            ulong sessionId
        )
        {
            this.apiKey = apiKey;
            this.sessionId = sessionId;

            return;
        }

        // Private pending XML API availability
        private static async void Create(
            String email,
            String plaintextSecret
        )
        {

            String data = String.Format(
                Session.createTemplate,
                email,
                plaintextSecret
            );

            String resultBody = await ApiRequest.MakeAsyncPost(
                path: Session.path,
                bodyJsonData: "something",
                session: null
            );


        }

    }

}
