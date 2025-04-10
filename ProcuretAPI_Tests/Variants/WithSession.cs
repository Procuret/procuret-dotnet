using System;
using ProcuretAPI;

namespace ProcuretAPI_Tests.Variants
{
    public class WithSession
    {

        public Session Session
        {
            get
            {
                // Example values shown. Input valid session credentials to
                // run tests.
                // TODO: Use environment variables / csproj settings to make
                // this more pleasant.
                return new Session(
                    apiKey: "QpFNziCBG3l5HXV3jyUB8GP2K0ALDyVy",
                    sessionId: 36284293055983192
                );
            }
        }
    }
}
