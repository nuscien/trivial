using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Reflection
{
    /// <summary>
    /// The exception handler instance.
    /// </summary>
    public class ExceptionHandler
    {
        /// <summary>
        /// The item of exception handlers registered.
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Initializes a new instance of the ExceptionHandler.Item class.
            /// </summary>
            /// <param name="type">The exception type.</param>
            /// <param name="handler">The catch handler.</param>
            public Item(Type type, Func<Exception, Exception> handler)
            {
                Type = type;
                Handler = handler;
            }

            /// <summary>
            /// Gets the exception type.
            /// </summary>
            public Type Type { get; private set; }

            /// <summary>
            /// Gets the catch handler.
            /// </summary>
            public Func<Exception, Exception> Handler { get; private set; }
        }

        /// <summary>
        /// The catch handler list.
        /// </summary>
        private IList<Item> list = new List<Item>();

        /// <summary>
        /// Tests if need throw an exception.
        /// </summary>
        /// <param name="ex">The exception catched.</param>
        /// <returns>The exception needed to throw.</returns>
        public Exception GetException(Exception ex)
        {
            foreach (var item in list)
            {
                var result = item.Handler(ex);
                if (result != null) return result;
            }

            return ex;
        }

        /// <summary>
        /// Adds a catch handler.
        /// </summary>
        /// <typeparam name="T">The type of exception to try to catch.</typeparam>
        /// <param name="catchHandler">The handler to return if need throw an exception.</param>
        public void Add<T>(Func<T, Exception> catchHandler) where T : Exception
        {
            var type = typeof(T);
            foreach (var item in list)
            {
                if (item.Type == type && item.Handler == catchHandler) return;
            }

            list.Add(new Item(type, ex =>
            {
                return ex is T exConverted ? catchHandler(exConverted) : ex;
            }));
        }

        /// <summary>
        /// Removes a catch handler.
        /// </summary>
        /// <typeparam name="T">The type of exception to try to catch.</typeparam>
        /// <param name="catchHandler">The handler to return if need throw an exception.</param>
        public bool Remove<T>(Func<T, Exception> catchHandler) where T : Exception
        {
            var type = typeof(T);
            var removing = new List<Item>();
            foreach (var item in list)
            {
                if (item.Type == type && item.Handler == catchHandler) removing.Add(item);
            }

            var count = removing.Count;
            foreach (var item in removing)
            {
                list.Remove(item);
            }

            return count > 0;
        }

        /// <summary>
        /// Clears all catch handlers.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }
    }
}
