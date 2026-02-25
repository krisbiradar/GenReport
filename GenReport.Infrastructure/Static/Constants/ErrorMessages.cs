namespace GenReport.Infrastructure.Static.Constants
{
    /// <summary>
    /// Defines the <see cref="ErrorMessages" />
    /// </summary>
    public struct ErrorMessages
    {
        /// <summary>
        /// Gets the TOKEN_NOT_VALID
        /// </summary>
        public static string TOKEN_NOT_VALID => "INVALID_TOKEN";

        /// <summary>
        /// Gets the TOKEN_EXPIRED
        /// </summary>
        public static string TOKEN_EXPIRED => "TOKEN_EXPIRED";

        /// <summary>
        /// Gets the DEFAULT_ERROR_CODE
        /// </summary>
        public static string DEFAULT_ERROR_CODE => "UNKNOWN_ERROR_SOMETHING_WENT_WRONG";

        /// <summary>
        /// Gets the USER_NOT_FOUND
        /// </summary>
        public static string USER_NOT_FOUND => "USER_NOT_FOUND";
        public static string PASSWORD_DOESNT_MATCH => "PASSWORD_DOES_NOT_MATCH";
        public static string USER_ALREADY_EXISTS => "USER_ALREADY_EXISTS";
        public static string MIDDLEWARE_ERROR => "MIDDLEWARE_ERROR";
        public static string UNAUTHORIZED => "UNAUTHORIZED ACCESS USER NOT LOGGED IN!!";
        public static string INVALID_OTP => "INVALID_OTP";
        public static string OTP_EXPIRED => "OTP_EXPIRED";
        public static string PASSWORD_MISMATCH => "PASSWORD_MISMATCH";
    }
}
