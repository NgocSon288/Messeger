using App.Data.Entities;
using App.ViewModel.Common;
using App.ViewModel.Messages;
using System.Threading.Tasks;

namespace App.Application.Interfaces
{
    public interface IMessageService
    {
        Task<ApiResult<Message>> CreateMessage(MessageCreateRequest request);
    }
}