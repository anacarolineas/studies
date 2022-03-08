using Microsoft.AspNetCore.Identity;

namespace DevAna.Api.Extensions
{
    public class IdentityMessagesCustom : IdentityErrorDescriber
    {
        // override nos métodos de mensagem error do identity

        public override IdentityError InvalidEmail(string email) =>
            new IdentityError { Code = nameof(InvalidEmail), Description = "Insira um e-mail válido."};

    }
}
