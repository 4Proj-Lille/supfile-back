namespace SupFile.Back.Core.Constants;

public static class EmailTemplateConstants
{
    public static (string Template, string Subject) VerifyEmail => ("VerifyEmail.cshtml", "Confirm your email");
    public static (string Template, string Subject) ForgotPassword => ("ForgotPassword.cshtml", "Reset your password");
}


