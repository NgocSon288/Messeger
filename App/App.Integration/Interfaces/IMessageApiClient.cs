using App.Data.Entities;
using App.ViewModel.Common;
using App.ViewModel.Messages;
using System.Threading.Tasks;

namespace App.Integration.Interfaces
{
    public interface IMessageApiClient
    {
        Task<ApiResult<Message>> CreateMessage(MessageCreateRequest request);
    }
}