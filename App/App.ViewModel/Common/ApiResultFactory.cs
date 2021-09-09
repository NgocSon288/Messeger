using System;
using System.Collections.Generic;
using System.Text;

namespace App.ViewModel.Common
{
    public static class ApiResultFactory
    {
        /// <summary>
        /// Create ApiResult không có data
        /// </summary>
        /// <param name="isSuccessfully"></param>
        /// <param name="message"></param>
        /// <returns>ApiResult<string></returns>
        public static ApiResult<string> NoData(bool isSuccessfully, string message)
        {
            return new ApiResult<string>(isSuccessfully, message);
        }
    }
}
