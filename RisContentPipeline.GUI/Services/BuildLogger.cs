
using Eto.Forms;

namespace RisContentPipeline.GUI.Services
{
    /// <summary>
    /// The messanger class is responsible for managing messages that are generated during the asset processing workflow. 
    /// It allows other parts of the application to subscribe to message events and receive updates when new messages are pushed.
    /// This can be used to display status updates, errors, or other relevant information to the user in real-time as assets are processed.
    /// </summary>
    internal class BuildLogger
    {
        private readonly List<string> _successLogs = new List<string>();
        private readonly List<string> _errorLogs = new List<string>();
        private readonly List<string> _infoLogs = new List<string>();

        /// <summary>
        /// When a message is pushed, this event is triggered with the message as a parameter.
        /// </summary>
        public event Action<string>? OnSuccessLog;

        /// <summary>
        /// When an error message is pushed, this event is triggered with the error message as a parameter.
        /// </summary>
        public event Action<string>? OnErrorLog;

        /// <summary>
        /// This event is triggered when an informational message is pushed to the logger.
        /// </summary>
        public event Action<string>? OnInfoLog;

        /// <summary>
        /// Pushes an informational message to the logger.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            _infoLogs.Add(message);
            OnInfoLog?.Invoke(message);
        }

        /// <summary>
        /// Pushes an informational message to the logger from an asynchronous context.
        /// </summary>
        /// <param name="message">The message.</param>
        public void InfoAsync(string message)
        {
            Application.Instance.AsyncInvoke(() =>
            {
                Info(message);
            });
        }

        /// <summary>
        /// Pushes a success message to the logger. 
        /// This method adds the message to the list of success logs and triggers the OnSuccessLog event, allowing subscribers to receive the new message.
        /// </summary>
        /// <param name="message">The sucess message.</param>
        public void Success(string message)
        {
            _successLogs.Add(message);
            OnSuccessLog?.Invoke(message);
        }

        /// <summary>
        /// Pushes a success message to the logger from an asynchronous context.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SuccessAsync(string message)
        {
            Application.Instance.AsyncInvoke(() =>
            {
                Success(message);
            });
        }

        /// <summary>
        /// Pushes an error message to the logger from an asynchronous context.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ErrorAsync(string message)
        {
            Application.Instance.AsyncInvoke(() =>
            {
                Error(message);
            });
        }

        /// <summary>
        /// Pushes an error message to the logger.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            _errorLogs.Add(message);
        }



        /// <summary>
        /// Clears all logs from the logger.
        /// This can be used to reset the logger state before starting a new build process, ensuring that old messages do not interfere with the new build's messages.
        /// </summary>
        public void Clear()
        {
            _successLogs.Clear();
            _errorLogs.Clear();
        }
    }
}
