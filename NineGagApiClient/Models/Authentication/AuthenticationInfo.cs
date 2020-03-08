using NineGagApiClient.Utils;
using System;

namespace NineGagApiClient.Models
{
    public class AuthenticationInfo
    {
        #region Constructors

        public AuthenticationInfo()
        {   
            Token = RequestUtils.GetSha1(Timestamp);
            TokenWillExpireAt = DateTime.UtcNow;
            LastAuthenticationType = AuthenticationType.None;

            Timestamp = RequestUtils.GetTimestamp();
            DeviceUuid = RequestUtils.GetUuid();
            Signature = RequestUtils.GetSignature(Timestamp, AppId, DeviceUuid);
        }

        #endregion

        #region Properties

        public string UserLogin
        {
            get;
            set;
        }
        public string UserPassword
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }
        public DateTime TokenWillExpireAt
        {
            get;
            set;
        }
        public AuthenticationType LastAuthenticationType
        {
            get;
            set;
        }

        public string Timestamp
        {
            get;
            set;
        }
        public string AppId
        {
            get => "com.ninegag.android.app";
        }
        public string DeviceUuid
        {
            get;
            set;
        }
        public string Signature
        {
            get;
            set;
        }

        public bool HasTokenExpired
        {
            get => TokenWillExpireAt <= DateTime.UtcNow;
        }
        public bool IsAuthenticated
        {
            get => !(string.IsNullOrEmpty(Token) || HasTokenExpired);
        }
        public bool AreCredentialsPresent
        {
            get => !(string.IsNullOrEmpty(UserLogin) || string.IsNullOrEmpty(UserPassword));
        }

        #endregion

        #region Methods

        public void ClearToken()
        {
            Token = string.Empty;
            TokenWillExpireAt = DateTime.UtcNow;
            LastAuthenticationType = AuthenticationType.None;
        }

        #endregion
    }
}
