using MediatR;
using System;

namespace Sample.Fluxo.Caixa.Core.Messages
{
    public abstract class Event : Message, INotification
    {
        public DateTime Timestamp { get; set; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}
