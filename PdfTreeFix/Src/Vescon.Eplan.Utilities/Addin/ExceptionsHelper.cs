using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using Eplan.EplApi.Base;

namespace Vescon.Eplan.Utilities.Addin
{
    public static class ExceptionsHelper
    {
        public static void HandleException(this Exception ex, string title)
        {
            var msg = ex.Message;

            MessageBox.Show(
                msg,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            var exceptions = new List<string>();
            while (ex != null)
            {
                exceptions.Add(ex.Message);
                ex = ex.InnerException;
            }

            BaseException innerExeption = null;

            for (var i = exceptions.Count - 1; i >= 0; --i)
                innerExeption = new BaseException(exceptions[i], MessageLevel.Error, innerExeption);

            new BaseException(
                $"{title} - Exception occurred.",
                MessageLevel.Error,
                innerExeption)
                .FixMessage();
        }

        public static void HandleException(this ExceptionDetail ex, string title)
        {
            var msg = ex.Message;

            MessageBox.Show(
                msg,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            var exceptions = new List<string>();
            while (ex != null)
            {
                exceptions.Add(ex.Message);
                ex = ex.InnerException;
            }

            BaseException innerExeption = null;

            for (var i = exceptions.Count - 1; i >= 0; --i)
                innerExeption = new BaseException(exceptions[i], MessageLevel.Error, innerExeption);

            new BaseException(
                $"{title} - Exception occurred.",
                MessageLevel.Error,
                innerExeption)
                .FixMessage();
        }
    }
}
