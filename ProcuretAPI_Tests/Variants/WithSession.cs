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
                // run tests. Once I figure out how to get VS2019 to play nicely
                // with macOS environment variables, this can be made more
                // pleasant.
                return new Session(
                    apiKey: "QpFNziCBG3l5HXV3jyUB8GP2K0ALDyVy",
                    sessionId: 36284293055983192
                );
            }
        }
    }
}
