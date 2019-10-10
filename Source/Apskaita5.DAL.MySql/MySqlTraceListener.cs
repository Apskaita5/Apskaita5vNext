using System;
using Microsoft.Extensions.Logging;

namespace Apskaita5.DAL.MySql
{
    internal class MySqlTraceListener : System.Diagnostics.TraceListener
    {

        private readonly ILogger _logger;
        private readonly MySqlAgent _agent;


        public MySqlTraceListener(ILogger logger, MySqlAgent agent)
        {
            _logger = logger;
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
        }


        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            if (_logger != null && message != null)
            {
                if (message.IndexOf("warning", StringComparison.OrdinalIgnoreCase) < 0) _logger.LogError(message);
                else _logger.LogWarning(message);
            }
        }

        public bool BelongsTo(MySqlAgent agent) => object.ReferenceEquals(_agent, agent ?? throw new ArgumentNullException(nameof(agent)));

    }
}
