namespace SupFile.Back.Core.Constants;

public static class EmailTemplateConstants
{
    public static (string Template, string Subject) VerifyEmail => ("VerifyEmail.cshtml", "Confirm your email");
    public static (string Template, string Subject) ForgotPassword => ("ForgotPassword.cshtml", "Reset your password");
    public static (string Template, string Subject) ShareInvitation => ("ShareInvitation.cshtml", "You have been invited to access a shared item");
}


