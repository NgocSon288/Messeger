using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Web.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetUserId(this ClaimsPrincipal that)
        {
            return that.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
        public static string GetName(this ClaimsPrincipal that)
        {
            return that.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
        }
    }
}
