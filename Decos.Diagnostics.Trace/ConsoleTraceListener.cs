using System;
using System.Diagnostics;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents a trace listener that writes logging events and data to the
    /// system console.
    /// </summary>
    public class ConsoleTraceListener : TraceListenerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTraceListener"/>
        /// class.
        /// </summary>
        public ConsoleTraceListener()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTraceListener"/>
        /// class with the specified name.
        /// </summary>
        /// <param name="name">The name of the trace listener.</param>
        public ConsoleTraceListener(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <remarks>
        /// While <see cref="Console"/> is thread safe, writing messages in
        /// multiple steps is not and will result in garbled output.
        /// </remarks>
        public override bool IsThreadSafe => false;

        /// <summary>
        /// Writes trace information and a message to standard output.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected override void TraceInternal(TraceEventData e, string message)
        {
          try // Berend #345703: Prevent console logging from crashing the app
          {
            Console.Write("[");
            using (new ConsoleHighlight(ConsoleColor.Gray))
              Console.Write($"{e.Cache.DateTime:T}");
            Console.Write("] ");

            Console.Write("[");
            using (FormatForLogLevel(e.Type ?? TraceEventType.Information))
              Console.Write($"{e.Type ?? TraceEventType.Information}");
            Console.Write("] ");

            Console.Write(message);

            Console.Write(" [");
            using (new ConsoleHighlight(ConsoleColor.DarkCyan))
              Console.Write($"{e.Source}");
            Console.Write("]");

            Console.WriteLine();
          }
          catch { }
        }

        /// <summary>
        /// Changes the console formatting options for the specified event type
        /// and returns an object that, when disposed, resets the settings to
        /// their previous values.
        /// </summary>
        /// <param name="e">The type of event to format for.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that will return the settings to their
        /// previous values.
        /// </returns>
        protected virtual IDisposable FormatForLogLevel(TraceEventType? e)
        {
            switch (e)
            {
                case TraceEventType.Critical:
                    return new ConsoleHighlight(ConsoleColor.White, ConsoleColor.Red);

                case TraceEventType.Error:
                    return new ConsoleHighlight(ConsoleColor.Red);

                case TraceEventType.Warning:
                    return new ConsoleHighlight(ConsoleColor.Yellow);

                case TraceEventType.Information:
                    return new ConsoleHighlight(ConsoleColor.Green);

                case TraceEventType.Verbose:
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Resume:
                case TraceEventType.Transfer:
                    return new ConsoleHighlight(ConsoleColor.Gray);
            }

            return new ConsoleHighlight();
        }

        /// <summary>
        /// Provides a mechanic for temporarily changing console formatting
        /// options and resetting the affected settings to their previous values
        /// when the object is disposed.
        /// </summary>
        protected class ConsoleHighlight : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConsoleHighlight"/>
            /// class that does not change any settings.
            /// </summary>
            public ConsoleHighlight()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConsoleHighlight"/>
            /// class that changes the foreground color.
            /// </summary>
            /// <param name="foregroundColor">
            /// The new foreground color to use.
            /// </param>
            public ConsoleHighlight(ConsoleColor foregroundColor)
            {
                PreviousForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = foregroundColor;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConsoleHighlight"/>
            /// class that changes the foreground and background colors.
            /// </summary>
            /// <param name="foregroundColor">
            /// The new foreground color to use.
            /// </param>
            /// <param name="backgroundColor">
            /// The new background color to use.
            /// </param>
            public ConsoleHighlight(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
            {
                PreviousForegroundColor = Console.ForegroundColor;
                PreviousBackgroundColor = Console.BackgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
            }

            /// <summary>
            /// Gets the original foreground color, or <c>null</c> if the
            /// foreground color has not been changed.
            /// </summary>
            public ConsoleColor? PreviousForegroundColor { get; }

            /// <summary>
            /// Gets the original background color, or <c>null</c> if the
            /// background color has not been changed.
            /// </summary>
            public ConsoleColor? PreviousBackgroundColor { get; }

            /// <summary>
            /// Resets affected console formatting options to their original
            /// values.
            /// </summary>
            public void Dispose()
            {
                if (PreviousForegroundColor != null)
                    Console.ForegroundColor = PreviousForegroundColor.Value;

                if (PreviousBackgroundColor != null)
                    Console.BackgroundColor = PreviousBackgroundColor.Value;
            }
        }
    }
}
