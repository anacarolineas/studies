using DevAna.Business.Intefaces;
using DevAna.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevAna.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
       private readonly INotificador _notificador;

        protected MainController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object? result = null)
        {
            if (OperacaoValida()) return Ok(new ResultValidation(result));
            
            return BadRequest(new ResultValidation(_notificador.ObterNotificacoes().Select(x => x.Mensagem)));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(x => x.Errors);
            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.ErrorMessage; //cobertura tanto de erros quanto de exceptions
                NotificarErro(erroMsg);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

        protected class ResultValidation
        {
            public bool Success { get; set; }
            public object? Data { get; set; }
            public IEnumerable<string>? Erros { get; set; }

            public ResultValidation(object? data)
            {
                Success = true;
                Data = data;
            }

            public ResultValidation(IEnumerable<string>? erros)
            {
                Success = false;
                Erros = erros;
            }
        }
    }
}
