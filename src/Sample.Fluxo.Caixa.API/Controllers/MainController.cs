using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Sample.Fluxo.Caixa.Core.Messages.CommonMessages.Notifications;

namespace Sample.Fluxo.Caixa.API.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;

        public MainController(INotificationHandler<DomainNotification> notifications)
        {
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", _notifications.ObterNotificacoes().Select(c => c.Value).ToArray() }
            }));
        }

        protected bool OperacaoValida()
        {
            return !_notifications.TemNotificacao();
        }
    }
}
