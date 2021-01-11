using System;
using Microsoft.AspNetCore.Mvc;

namespace BasePackageModule2.Extensions
{
    public static class AlertExtensions
    {
        public static IActionResult WithSuccess(this IActionResult result, string title)
        {
            return WithSuccess(result, title, null);
        }

        public static IActionResult WithSuccess(this IActionResult result, string title, string body)
        {
            body ??= string.Empty;
            return Alert(result, "success", title, body);
        }

        public static IActionResult WithInfo(this IActionResult result, string title)
        {
            return WithInfo(result, title, null);
        }


        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return Alert(result, "info", title, body);
        }

        public static IActionResult WithWarning(this IActionResult result, string title)
        {
            return WithWarning(result, title, null);
        }

        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return Alert(result, "warn", title, body);
        }

        public static IActionResult WithError(this IActionResult result, string title)
        {
            return WithError(result, title, null);
        }

        public static IActionResult WithError(this IActionResult result, string title, string body)
        {
            return Alert(result, "error", title, body);
        }

        private static IActionResult Alert(IActionResult result, string type, string title)
        {
            return Alert(result, type, title, string.Empty);
        }


        private static IActionResult Alert(IActionResult result, string type, string title, string body)
        {
            return new AlertDecoratorResult(result, type, title, body);
        }
    }
}
